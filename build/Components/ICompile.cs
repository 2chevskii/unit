using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

interface ICompile : IHazArtifacts, IRestore, IHazConfiguration, IHazVersion
{
    Target CompileMain =>
        _ =>
            _.DependsOn(Restore)
                .Executes(
                    () =>
                        DotNetTasks.DotNetBuild(settings =>
                            settings.Apply(BuildSettingsBase).SetProjectFile(MainProject)
                        )
                );
    Target CompileTests =>
        _ =>
            _.DependsOn(Restore, CompileMain)
                .Executes(
                    () =>
                        DotNetTasks.DotNetBuild(settings =>
                            settings.Apply(BuildSettingsBase).SetProjectFile(TestsProject)
                        )
                );

    Target Compile => _ => _.DependsOn(CompileMain, CompileTests);

    Target CopyLibsOutput =>
        _ =>
            _.Unlisted()
                .After(CompileMain)
                .OnlyWhenStatic(() => CopyLibs)
                .TriggeredBy(CompileMain)
                .Executes(
                    () =>
                        FileSystemTasks.CopyDirectoryRecursively(
                            MainProject.Directory / "bin/Release/netstandard2.0",
                            ArtifactPaths.Libraries / "netstandard2.0",
                            DirectoryExistsPolicy.Merge,
                            FileExistsPolicy.Overwrite
                        )
                );

    sealed Configure<DotNetBuildSettings> BuildSettingsBase =>
        settings =>
            settings
                .EnableNoRestore()
                .EnableNoDependencies()
                .SetConfiguration(Configuration)
                .SetVersion(Version.SemVer);

    [Parameter]
    bool CopyLibs => TryGetValue<bool?>(() => CopyLibs).GetValueOrDefault();
}
