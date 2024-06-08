using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tools.GitVersion;

[Requires<GitVersionTasks>(Version = "6.0.0-rc.1")]
partial class Build
{
    [GitRepository]
    GitRepository GitRepository;

}
