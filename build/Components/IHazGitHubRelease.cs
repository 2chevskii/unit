using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.Tooling;

interface IHazGitHubRelease : INukeBuild
{
    [LatestGitHubRelease("2chevskii/unit", TrimPrefix = true)]
    NuGetVersion LatestGitHubReleaseTag =>
        NuGetVersion.Parse(TryGetValue<string>(() => LatestGitHubReleaseTag)!);
}
