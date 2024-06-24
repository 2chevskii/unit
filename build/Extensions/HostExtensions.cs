using Nuke.Common;
using Nuke.Common.CI.GitHubActions;

namespace Extensions;

public static class HostExtensions
{
    public static bool IsGitHubActions(this Host self) => self is GitHubActions;
}
