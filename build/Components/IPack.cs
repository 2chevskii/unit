using Nuke.Common;
using Nuke.Common.Tools.DotNet;

interface IPack : ICompile
{
    Target Pack =>
        _ =>
            _.DependsOn(CompileMain)
                .Executes(
                    () =>
                        DotNetTasks.DotNetPack(settings =>
                            settings
                                .EnableNoBuild()
                                .SetVersion(Version.NuGetVersionV2)
                                .SetOutputDirectory(PackagesDirectory)
                                .SetConfiguration(Configuration)
                        )
                );
}
