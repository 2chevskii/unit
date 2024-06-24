using NuGet.Versioning;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.GitVersion;

partial class Build
{
    [GitVersion]
    readonly GitVersion Version;

    [LatestGitHubRelease("2chevskii/unit", TrimPrefix = true)]
    readonly string LatestGitHubReleaseTag;
}
