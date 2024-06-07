using Nuke.Common;
using Nuke.Common.Tools.DotNet;

partial class Build
{
    Target Compile =>
        _ =>
            _.Executes(
                () =>
                    DotNetTasks.DotNetBuild(settings =>
                        settings
                            .EnableNoRestore()
                            .SetVersion(Version.SemVer)
                            .SetConfiguration(Configuration)
                            .SetProjectFile(Sln)
                    )
            );
}
