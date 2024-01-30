using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tools.GitHub;

interface IHazGitRepository : INukeBuild
{
    [Required]
    [GitRepository]
    GitRepository GitRepository => TryGetValue(() => GitRepository);

    Task<long> GetRepositoryId() =>
        GitHubTasks
            .GitHubClient.Repository.Get(
                GitRepository.GetGitHubOwner(),
                GitRepository.GetGitHubName()
            )
            .ContinueWith(response => response.Result.Id);
}
