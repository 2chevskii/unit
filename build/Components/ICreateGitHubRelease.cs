using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.GitHub;
using Octokit;
using Serilog;
using ContentType = Azure.Core.ContentType;

interface ICreateGitHubRelease : IHazVersion, IHazGitRepository, IHazArtifacts
{
    string ReleaseName => Version.MajorMinorPatch;
    string TagName => $"v{ReleaseName}";
    string GithubToken => GitHubActions.Instance.Token;

    Target CreateGitHubRelease =>
        _ =>
            _.Requires(() => !string.IsNullOrEmpty(GithubToken))
                .Requires(() => GitHubActions.Instance.Ref.StartsWith("refs/tags/"))
                .Executes(async () =>
                {
                    GitHubTasks.GitHubClient.Credentials = new Credentials(GithubToken);
                    Release release = await GetOrCreateRelease();

                    IEnumerable<Task> uploadTasks =
                        from path in AssetPaths
                        let filename = path.Name
                        where release.Assets.All(asset => asset.Name != filename)
                        let stream = File.OpenRead(path)
                        let assetUpload = new ReleaseAssetUpload
                        {
                            FileName = filename,
                            ContentType = ContentType.ApplicationOctetStream.ToString(),
                            RawData = stream
                        }
                        select GitHubTasks
                            .GitHubClient.Repository.Release.UploadAsset(release, assetUpload)
                            .ContinueWith(response => stream.Dispose());

                    await Task.WhenAll(uploadTasks);
                });

    async Task<Release> GetOrCreateRelease()
    {
        var repositoryId = await GetRepositoryId();

        Log.Information(
            "Creating GitHub release (repository id={RepositoryId}, release name={ReleaseName}, tag name={TagName})",
            repositoryId,
            ReleaseName,
            TagName
        );

        Release release;

        try
        {
            release = await GitHubTasks.GitHubClient.Repository.Release.Create(
                repositoryId,
                new NewRelease(TagName)
                {
                    Name = ReleaseName,
                    Draft = true,
                    Prerelease = !string.IsNullOrEmpty(Version.PreReleaseLabel),
                }
            );
        }
        catch
        {
            release = await GitHubTasks.GitHubClient.Repository.Release.Get(repositoryId, TagName);
        }

        return release;
    }

    IEnumerable<AbsolutePath> AssetPaths => PackagesDirectory.GetFiles();
}

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
