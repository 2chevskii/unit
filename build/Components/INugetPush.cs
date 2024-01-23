using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities;
using Octokit;
using FileMode = System.IO.FileMode;

interface INugetPush : INukeBuild, ICreateGitHubRelease, IHazNugetSourceList
{
    string NugetApiKey => EnvironmentInfo.GetVariable("NUGET_API_KEY");

    [Parameter]
    Uri NugetFeed => TryGetValue(() => NugetFeed);

    string NugetSourceName => NugetFeed.Host.Split('.').TakeLast(2).Join('.');

    Target EnsureHasNugetSource =>
        _ =>
            _.OnlyWhenDynamic(() => !HasNugetSource(NugetSourceName))
                .Executes(
                    () =>
                        DotNetTasks.DotNetNuGetAddSource(settings =>
                            settings.SetName(NugetSourceName).SetSource(NugetFeed.ToString())
                        )
                );

    Target NugetPush =>
        _ =>
            _.Requires(() => !string.IsNullOrEmpty(NugetApiKey))
                .Requires(() => NugetFeed)
                .DependsOn(EnsureHasNugetSource)
                .Executes(
                    () =>
                        DotNetTasks.DotNetNuGetPush(settings =>
                            settings
                                .SetSource(NugetSourceName)
                                .SetApiKey(NugetApiKey)
                                .CombineWith(
                                    PackagesDirectory.GlobFiles("*.nupkg"),
                                    (settings, package) => settings.SetTargetPath(package)
                                )
                        )
                );

    Target DownloadReleaseAssets =>
        _ =>
            _.Executes(async () =>
            {
                Release release = await GetOrCreateRelease();

                IEnumerable<Task> downloadTasks =
                    from asset in release.Assets
                    let name = asset.Name
                    where name.EndsWith("nupkg")
                    let path = PackagesDirectory / name
                    select HttpTasks.HttpDownloadFileAsync(
                        asset.BrowserDownloadUrl,
                        path,
                        FileMode.CreateNew
                    );

                await Task.WhenAll(downloadTasks);
            });
}
