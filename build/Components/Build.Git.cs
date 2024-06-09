using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Octokit;

[Requires<GitVersionTasks>(Version = "6.0.0-rc.1")]
partial class Build
{
    [GitRepository]
    GitRepository GitRepository;

    long GitHubRepositoryId;

    Target FetchGitHubRepositoryId =>
        _ =>
            _.OnlyWhenDynamic(() => GitHubRepositoryId == default)
                .Executes(async () =>
                {
                    string gitHubName = GitRepository.GetGitHubName();
                    string gitHubOwner = GitRepository.GetGitHubOwner();

                    Repository repository = await GitHubTasks.GitHubClient.Repository.Get(
                        gitHubOwner,
                        gitHubName
                    );

                    GitHubRepositoryId = repository.Id;
                });
}
