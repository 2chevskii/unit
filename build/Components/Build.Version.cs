using Nuke.Common.Tools.GitVersion;

partial class Build
{
    [GitVersion]
    readonly GitVersion Version;
}
