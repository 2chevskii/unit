using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tools.GitHub;
using Octokit;
using Serilog;
using ContentType = Azure.Core.ContentType;
using FileMode = System.IO.FileMode;

interface ICreateGitHubRelease : IHazGitRepository
{
    string ReleaseName => /*Version.MajorMinorPatch*/
        string.Empty;
    string TagName => $"v{ReleaseName}";

    Target CreateGitHubRelease =>
        _ =>
            _.Requires(() => !string.IsNullOrEmpty(GitHubActions.Instance.Token))
                .Requires(() => GitHubActions.Instance.Ref.StartsWith("refs/tags/"))
                .Executes(/*async () =>
                {
                    long repositoryId = await GetRepositoryId();
                    Log.Information(
                        "Creating GitHub release (repository id={RepositoryId}, release name={ReleaseName}, tag name={TagName})",
                        repositoryId,
                        ReleaseName,
                        TagName
                    );

                    Release release = await GitHubTasks.GitHubClient.Repository.Release.Create(
                        repositoryId,
                        new NewRelease(TagName)
                        {
                            Name = ReleaseName,
                            Draft = true,
                            Prerelease = !string.IsNullOrEmpty( /*Version.PreReleaseLabel#1#
                                string.Empty
                            ),
                            Body = "# TODO: Write release notes here"
                        }
                    );

                    IEnumerable<Task> uploadTasks =
                        from path in GetPackageAssets().Concat(PrepareAndGetLibraryAssets())
                        let filename = path.Name
                        let stream = File.OpenRead(path)
                        let assetUpload = new ReleaseAssetUpload
                        {
                            FileName = filename,
                            ContentType = ContentType.ApplicationOctetStream.ToString(),
                            RawData = stream
                        }
                        select GitHubTasks
                            .GitHubClient.Repository.Release.UploadAsset(release, assetUpload)
                            .ContinueWith(task =>
                            {
                                Log.Debug("Uploaded release artifact {Filename}", filename);
                                stream.Dispose();
                            });

                    await Task.WhenAll(uploadTasks);
                }*/);

    /*IEnumerable<AbsolutePath> GetPackageAssets() =>
        ArtifactPaths.Packages.GetFiles("*.*nupkg").ToArray();

    IEnumerable<AbsolutePath> PrepareAndGetLibraryAssets() =>
        from dir in ArtifactPaths.Libraries.GetDirectories().ToArray()
        let tarGzPath = ArtifactPaths.Libraries / dir.Name + ".tar.gz"
        select new Func<AbsolutePath>(() =>
        {
            Log.Debug("Compressing archive directory {DirPath} to {TarballPath}", dir, tarGzPath);
            dir.TarGZipTo(tarGzPath, fileMode: FileMode.Create);
            return tarGzPath;
        })();*/
}
