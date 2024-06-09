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
                .Executes(() =>
                {
                    Dictionary<string, object> packageJsonProps = DocsPackageJson.ReadJson<
                        Dictionary<string, object>
                    >();

                    packageJsonProps["version"] = Version.SemVer;
                    packageJsonProps["latestReleaseVersion"] = LatestGitHubReleaseTag;
                    DocsPackageJson.WriteJson(packageJsonProps);
                });

    void SaveDocsPackageJson()
    {
        Log.Information("Saving docs/package.json");
        docsPackageJsonBackup = DocsPackageJson.ReadAllText();
    }

    void RestoreDocsPackageJson()
    {
        if (docsPackageJsonBackup != null)
        {
            Log.Information("Restoring docs/package.json to pre-build state");
            DocsPackageJson.WriteAllText(docsPackageJsonBackup);
        }
    }
}
