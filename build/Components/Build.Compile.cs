using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    Target Compile =>
        _ =>
            _.DependsOn(Restore)
                .Executes(() => DotNetBuild(settings => settings.Apply(BuildSettingsBase)));

    Configure<DotNetBuildSettings> BuildSettingsBase =>
        settings =>
            settings
                .EnableNoRestore()
                .SetVerbosity(DotNetVerbosity.normal)
                .SetConfiguration(Configuration)
                .SetVersion(Version.FullSemVer)
                .SetProjectFile(Sln);
}
