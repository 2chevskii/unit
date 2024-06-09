using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tools.GitHub;

partial class Build
{
    [Parameter]
    readonly string ReleaseTag;

    Target DownloadReleaseAssets => _ => _.Requires(() => GitHubActions.Instance.Token)
        .Requires(() => ReleaseTag)
        .DependsOn(FetchGitHubRepositoryId)
        .Executes( async () =>
        {
            var release = await GitHubTasks.GitHubClient.Repository.Release.Get(GitHubRepositoryId, ReleaseTag);


        });
}
