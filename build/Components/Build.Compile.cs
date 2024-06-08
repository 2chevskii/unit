using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

partial class Build
{
    Target Compile =>
        _ =>
            _.Executes(
                () => DotNetTasks.DotNetBuild(settings => settings.Apply(BuildSettingsBase))
            );

    Configure<DotNetBuildSettings> BuildSettingsBase =>
        settings =>
            settings
                .EnableNoRestore()
                .SetVerbosity(DotNetVerbosity.normal)
                .SetConfiguration(Configuration)
                .SetVersion(Version.FullSemVer)
                .SetProjectFile(Sln);
}
