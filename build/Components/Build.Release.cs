using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Utilities;
using Octokit;
using Serilog;

partial class Build
{
    string TagName => $"v{Version.SemVer}";

    Target DownloadGithubReleaseAssets =>
        _ =>
            _.Requires(() => GitHubActions.Instance.Token)
                .DependsOn(FetchGitHubRepositoryId)
                .Executes(async () =>
                {
                    Log.Information(
                        "Downloading assets from the {RefName} release",
                        GitHubActions.Instance.RefName
                    );

                    Release release = await GitHubTasks.GitHubClient.Repository.Release.Get(
                        GitHubRepositoryId,
                        GitHubActions.Instance.RefName
                    );

                    foreach (
                        ReleaseAsset releaseAsset in release.Assets.Where(asset =>
                            asset.Name.EndsWithAny(".nupkg", ".snupkg")
                        )
                    )
                    {
                        Log.Information("Downloading asset {Name}", releaseAsset.Name);

                        await HttpTasks.HttpDownloadFileAsync(
                            releaseAsset.BrowserDownloadUrl,
                            PackagesDirectory / releaseAsset.Name
                        );
                    }
                });

    Target CreateGithubReleaseDraft =>
        _ =>
            _.DependsOn(FetchGitHubRepositoryId)
                .Executes(async () =>
                {
                    Log.Information("Creating GitHub release draft with tag {TagName}", TagName);

                    IReadOnlyCollection<AbsolutePath> releaseAssets =
                        ReleaseAssetsDirectory.GlobFiles("*.{nupkg,snupkg}");

                    if (releaseAssets.Count == 0)
                    {
                        Log.Warning("No release assets were found");
                    }
                    else
                    {
                        Log.Information(
                            "Discovered {AssetCount} release assets",
                            releaseAssets.Count
                        );
                    }

                    NewRelease newRelease = new NewRelease(TagName)
                    {
                        Draft = true,
                        Name = Version.SemVer,
                        Prerelease = !string.IsNullOrEmpty(Version.PreReleaseLabel),
                        TargetCommitish = Version.Sha,
                    };

                    Release release = await GitHubTasks.GitHubClient.Repository.Release.Create(
                        GitHubRepositoryId,
                        newRelease
                    );

                    Log.Information("Created release with ID: {Id}", release.Id);

                    foreach (AbsolutePath asset in releaseAssets)
                    {
                        await using FileStream assetFs = File.OpenRead(asset);
                        Log.Information(
                            "Uploading release asset: {AssetName} at {AssetPath} ({AssetLength} bytes)",
                            asset.Name,
                            asset.ToString(),
                            assetFs.Length
                        );

                        ReleaseAssetUpload releaseAssetUpload = new ReleaseAssetUpload(
                            asset.Name,
                            "application/octet-stream",
                            assetFs,
                            null
                        );
                        ReleaseAsset releaseAsset =
                            await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(
                                release,
                                releaseAssetUpload
                            );

                        Log.Information("Uploaded asset ID: {Id}", releaseAsset.Id);
                    }
                });
}
