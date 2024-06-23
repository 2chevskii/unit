using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tools.GitHub;
using Serilog;

partial class Build
{
    [Parameter]
    readonly string ReleaseTag;

    Target DownloadReleaseAssets =>
        _ =>
            _.Requires(() => GitHubActions.Instance.Token)
                .Requires(() => ReleaseTag)
                .DependsOn(FetchGitHubRepositoryId)
                .Executes(async () =>
                {
                    var release = await GitHubTasks.GitHubClient.Repository.Release.Get(
                        GitHubRepositoryId,
                        ReleaseTag
                    );
                });

    Target CreateGithubReleaseDraft =>
        _ =>
            _.Executes(async () =>
            {
                var assetFiles = PackagesDirectory.GlobFiles("*.{nupkg,snupkg}");

                if (assetFiles.Count == 0)
                {
                    Log.Warning(
                        "No asset files were found in the {PackagesDirectory}",
                        PackagesDirectory
                    );
                }

                var tagName = $"v{Version.SemVer}";


            });
}
