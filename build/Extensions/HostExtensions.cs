using Nuke.Common;

namespace Extensions;

public static class HostExtensions
{
    public static bool IsGitHubActions(this Host self) =>
        EnvironmentInfo.HasVariable("GITHUB_ACTIONS");
}
