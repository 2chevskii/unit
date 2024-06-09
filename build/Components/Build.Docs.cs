using System;
using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities;
using Serilog;
using static Nuke.Common.Tools.Npm.NpmTasks;

partial class Build
{
    string docsPackageJsonBackup;

    AbsolutePath DocsDirectory => RootDirectory / "docs";
    AbsolutePath DocsPackageJson => DocsDirectory / "package.json";

    Target Docs => _ => _.DependsOn(DocsCompile);

    Target DocsRestore =>
        _ =>
            _.Unlisted()
                .Executes(
                    () => NpmInstall(settings => settings.SetProcessWorkingDirectory(DocsDirectory))
                );

    Target DocsCompile =>
        _ =>
            _.Unlisted()
                .DependsOn(DocsRestore)
                .DependsOn(DocsPatchPackageJson)
                .Executes(
                    () =>
                        NpmRun(settings =>
                            settings
                                .SetProcessWorkingDirectory(DocsDirectory)
                                .SetCommand("docs:build")
                        )
                );

    Target DocsDev =>
        _ =>
            _.DependsOn(DocsRestore)
                .DependsOn(DocsPatchPackageJson)
                .Executes(
                    () =>
                        NpmRun(settings =>
                            settings
                                .SetProcessWorkingDirectory(DocsDirectory)
                                .SetCommand("docs:dev")
                        )
                );

    Target DocsPatchPackageJson =>
        _ =>
            _.Before(DocsRestore)
                .DependsOn(DocsSavePackageJson)
                .Executes(() =>
                {
                    Log.Information(
                        "Patching {PackageJsonPath}",
                        RootDirectory.GetRelativePathTo(DocsPackageJson)
                    );
                    Dictionary<string, object> packageJsonProps = DocsPackageJson.ReadJson<
                        Dictionary<string, object>
                    >();

                    packageJsonProps["version"] = Version.SemVer;
                    Log.Information("Set version to {Version}", Version.SemVer);
                    packageJsonProps["latestReleaseVersion"] = LatestGitHubReleaseTag;
                    Log.Information(
                        "Set latest release version to {LatestReleaseVersion}",
                        LatestGitHubReleaseTag
                    );

                    DocsPackageJson.WriteJson(packageJsonProps);
                });

    Target DocsSavePackageJson =>
        _ =>
            _.Executes(() =>
            {
                Log.Information(
                    "Saving {PackageJsonPath}",
                    RootDirectory.GetRelativePathTo(DocsPackageJson)
                );
                docsPackageJsonBackup = DocsPackageJson.ReadAllText();
            });
    Target DocsRestorePackageJson =>
        _ =>
            _.TriggeredBy(DocsPatchPackageJson)
                .After(DocsDev, DocsCompile, DocsRestore)
                .OnlyWhenDynamic(() => docsPackageJsonBackup != null)
                .Executes(() =>
                {
                    Log.Information(
                        "Restoring {PackageJsonPath} to pre-build state",
                        RootDirectory.GetRelativePathTo(DocsPackageJson)
                    );
                    DocsPackageJson.WriteAllText(docsPackageJsonBackup);
                });
}
