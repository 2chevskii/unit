using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities;
using Serilog;

interface IDocs : IHazArtifacts, IRestore, IHazVersion, IHazGitHubRelease
{
    AbsolutePath DocsDirectory => RootDirectory / "docs";
    AbsolutePath DocsDistDirectory => DocsDirectory / ".vitepress" / "dist";
    AbsolutePath DocsPackageJson => DocsDirectory / "package.json";

    Target Docs => _ => _.DependsOn(DocsCompile, DocsCopy);

    Target DocsRestore =>
        _ =>
            _.Executes(
                () =>
                    NpmTasks.NpmInstall(settings =>
                        settings.SetProcessWorkingDirectory(DocsDirectory)
                    )
            );

    Target DocsClean => _ => _.Executes(() => DocsDistDirectory.DeleteDirectory());

    Target DocsCompile =>
        _ =>
            _.DependsOn(DocsRestore, PatchPackageVersion)
                .Executes(
                    () =>
                        NpmTasks.NpmRun(settings =>
                            settings
                                .SetProcessWorkingDirectory(DocsDirectory)
                                .SetCommand("docs:build")
                        )
                );

    Target DocsPreview =>
        _ =>
            _.DependsOn(DocsCompile)
                .Executes(
                    () =>
                        NpmTasks.NpmRun(settings =>
                            settings
                                .SetProcessWorkingDirectory(DocsDirectory)
                                .SetCommand("docs:preview")
                        )
                );

    Target DocsDev =>
        _ =>
            _.DependsOn(DocsRestore, PatchPackageVersion)
                .Executes(
                    () =>
                        NpmTasks.NpmRun(settings =>
                            settings
                                .SetProcessWorkingDirectory(DocsDirectory)
                                .SetCommand("docs:dev")
                        )
                );

    Target PatchPackageVersion =>
        _ =>
            _.After(DocsRestore)
                .Executes(() =>
                {
                    Dictionary<string, object> props = DocsPackageJson.ReadJson<
                        Dictionary<string, object>
                    >();
                    props["version"] = Version.SemVer;
                    Log.Debug("Docs property 'version' set to {Version}", Version.SemVer);
                    props["latestReleaseVersion"] = LatestGitHubReleaseTag.OriginalVersion;
                    Log.Debug(
                        "Docs property 'latestReleaseVersion' set to {LatestReleaseVersion}",
                        LatestGitHubReleaseTag.OriginalVersion
                    );
                    DocsPackageJson.WriteJson(props);
                })
                .Unlisted();

    Target DocsCopy =>
        _ =>
            _.DependsOn(DocsCompile)
                .Executes(
                    () =>
                        FileSystemTasks.CopyDirectoryRecursively(
                            DocsDistDirectory,
                            ArtifactPaths.Docs / "dist",
                            DirectoryExistsPolicy.Merge,
                            FileExistsPolicy.Overwrite
                        )
                )
                .Unlisted();
}
