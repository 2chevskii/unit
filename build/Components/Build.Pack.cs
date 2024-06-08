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
                            .SetOutputDirectory(PackagesDirectory)
                            .SetConfiguration(Configuration)
                            .EnableNoBuild()
                            .EnableNoRestore()
                            .EnableNoDependencies()
                            .SetVersion(Version.FullSemVer)
                            .SetVerbosity(DotNetVerbosity.normal)
                    );
                });
}
