using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Utilities;
using Octokit;
using Serilog;
using FileMode = System.IO.FileMode;

interface INugetPush : ICreateGitHubRelease, IControlNuGetSources
{
    string NugetApiKey => EnvironmentInfo.GetVariable("NUGET_API_KEY");

    Target NugetPush =>
        _ =>
            _.Requires(() => NugetApiKey)
                .Requires(() => NugetFeed)
                .DependsOn(EnsureHasNugetFeed)
                .Executes(
                    () =>
                        DotNetTasks.DotNetNuGetPush(settings =>
                            settings
                                .SetSource(NugetSourceName)
                                .SetApiKey(NugetApiKey)
                                .CombineWith(
                                    ArtifactPaths.Packages.GlobFiles("*.nupkg"),
                                    (settings, package) => settings.SetTargetPath(package)
                                )
                        )
                );

    Target DownloadReleaseAssets =>
        _ =>
            _.Requires(() => !string.IsNullOrEmpty(GitHubActions.Instance.Token))
                .Executes(async () =>
                {
                    Release release = await GitHubTasks.GitHubClient.Repository.Release.Get(
                        await GetRepositoryId(),
                        TagName
                    );

                    Log.Debug("Downloading assets from release {ReleaseName}", release.Name);

                    IEnumerable<Task> downloadTasks =
                        from asset in release.Assets
                        let name = asset.Name
                        where name.EndsWith("nupkg")
                        let path = ArtifactPaths.Packages / name
                        select HttpTasks
                            .HttpDownloadFileAsync(
                                asset.BrowserDownloadUrl,
                                path,
                                FileMode.CreateNew
                            )
                            .ContinueWith(_ =>
                                Log.Debug("Downloaded asset {AssetName} to {Path}", name, path)
                            );

                    await Task.WhenAll(downloadTasks);
                });
}
