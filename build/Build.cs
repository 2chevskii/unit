using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Debug;

    [Parameter]
    readonly bool NoCompile;

    readonly IEnumerable<AbsolutePath> SourceProjects = (RootDirectory / "src").GlobFiles(
        "**/*.csproj"
    );

    readonly IEnumerable<AbsolutePath> TestProjects = (RootDirectory / "test").GlobFiles(
        "**/*.csproj"
    );

    [Solution("Dvchevskii.Unit.sln")]
    readonly Solution Sln;

    Target Restore =>
        _ =>
            _.Executes(
                () => DotNetTasks.DotNetRestore(settings => settings.SetProjectFile(Sln.Path))
            );

    Target CompileMain =>
        _ =>
            _.DependsOn(Restore)
                .Executes(
                    () =>
                        SourceProjects.ForEach(project =>
                            DotNetTasks.DotNetBuild(settings =>
                                settings
                                    .EnableNoRestore()
                                    .SetConfiguration(Configuration)
                                    .SetProjectFile(project)
                            )
                        )
                );

    Target CompileTests =>
        _ =>
            _.DependsOn(Restore, CompileMain)
                .Executes(
                    () =>
                        TestProjects.ForEach(project =>
                            DotNetTasks.DotNetBuild(settings =>
                                settings
                                    .EnableNoRestore()
                                    .EnableNoDependencies()
                                    .SetConfiguration(Configuration)
                                    .SetProjectFile(project)
                            )
                        )
                );

    Target Compile =>
        _ => _.DependsOn(CompileMain, CompileTests).OnlyWhenStatic(() => NoCompile == false);

    Target UnitTest =>
        _ =>
            _.DependsOn(Compile)
                .Executes(
                    () =>
                        Sln.GetAllProjects("*.Tests")
                            .ForEach(project =>
                                DotNetTasks.DotNetTest(settings =>
                                    settings
                                        .EnableNoBuild()
                                        .EnableNoRestore()
                                        .SetLoggers(
                                            "console;verbosity=detailed",
                                            "html;logfilename=test-results.html"
                                        )
                                        .SetProjectFile(project.Path)
                                )
                            )
                );
}
