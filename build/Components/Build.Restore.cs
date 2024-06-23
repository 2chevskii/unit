using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    Target Restore =>
        _ => _.Executes(() => DotNetRestore(settings => settings.Apply(DotNetRestoreSettingsBase)));

    Configure<DotNetRestoreSettings> DotNetRestoreSettingsBase =>
        settings => settings.SetVerbosity(DotNetVerbosity.normal).SetProjectFile(Sln);
}
