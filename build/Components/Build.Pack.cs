using Nuke.Common;
using Nuke.Common.Tools.DotNet;

partial class Build
{
    Target Pack =>
        _ =>
            _.DependsOn(Compile)
                .Executes(() =>
                {
                    DotNetTasks.DotNetPack(settings =>
                        settings
                            .EnableNoRestore()
                            .EnableNoBuild()
                            .EnableNoDependencies()
                            .SetVerbosity(DotNetVerbosity.normal)
                            .SetOutputDirectory(PackagesDirectory)
                            .SetConfiguration(Configuration)
                            .SetVersion(Version.FullSemVer)
                    );
                });
}
