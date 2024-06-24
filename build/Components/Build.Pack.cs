using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    Target Pack =>
        _ =>
            _.DependsOn(Compile)
                .Executes(
                    () =>
                        DotNetPack(settings =>
                            settings
                                .EnableNoRestore()
                                .EnableNoBuild()
                                .EnableNoDependencies()
                                .SetVerbosity(DotNetVerbosity.normal)
                                .SetOutputDirectory(PackagesDirectory)
                                .SetConfiguration(Configuration)
                                .SetVersion(Version.FullSemVer)
                        )
                );
}
