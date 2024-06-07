using Nuke.Common;
using Nuke.Common.Tools.DotNet;

partial class Build
{
    Target Restore =>
        _ => _.Executes(() => DotNetTasks.DotNetRestore(settings => settings.SetProjectFile(Sln)));
}
