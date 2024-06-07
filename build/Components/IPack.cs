using Nuke.Common;
using Nuke.Common.Tools.DotNet;

interface IPack
{
    Target Pack =>
        _ =>
            _ /*.DependsOn(CompileMain)*/
            .Executes(
                () =>
                    DotNetTasks.DotNetPack(settings =>
                        settings
                            .EnableNoBuild()
                            .SetVersion( /*Version.SemVer*/
                                ""
                            )
                    // .SetOutputDirectory(/*ArtifactPaths.Packages*/)
                    // .SetConfiguration(Configuration)
                    )
            );
}
