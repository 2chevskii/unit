using System.Collections.Generic;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities;
using Serilog;

interface IDocs : IHazArtifacts, IRestore, IHazVersion, IHazGitHubRelease
{
    AbsolutePath DocsDirectory => RootDirectory / "docs";
    AbsolutePath DocsOutputDirectory => DocsDirectory / ".vitepress" / "dist";
    AbsolutePath DocsArtifact => DocsArtifactsDirectory / "github-pages.tar";
    AbsolutePath PackageJson => DocsDirectory / "package.json";

    Target Docs => _ => _.DependsOn(DocsCompile, DocsGzip);

    Target DocsRestore => _ => _.Executes(() => NpmTasks.NpmInstall(SetDocsWorkingDirectory));

    Target DocsClean => _ => _.Executes(() => DocsOutputDirectory.DeleteDirectory());

    Target PatchPackageVersion =>
        _ =>
            _.After(DocsRestore)
                .Executes(() =>
                {
                    Log.Information(
                        "Patching version in {PackageJsonPath}",
                        RootDirectory.GetRelativePathTo(PackageJson)
                    );
                    string version = Version.SemVer;

                    Dictionary<string, object> props = PackageJson
                        .ReadAllText()
                        .GetJson<Dictionary<string, object>>();
                    props["version"] = version;
                    props["latestNugetVersion"] = LatestGitHubReleaseTag.OriginalVersion;

                    Log.Debug("Version property set to {Version}", version);
                    Log.Debug(
                        "Latest nuget version property set to {LatestNugetVersion}",
                        LatestGitHubReleaseTag.OriginalVersion
                    );
                    PackageJson.WriteJson(props);
                })
                .Unlisted();

    Target DocsDev =>
        _ =>
            _.DependsOn(DocsRestore, PatchPackageVersion)
                .Executes(
                    () =>
                        NpmTasks.NpmRun(settings =>
                            SetDocsWorkingDirectory(settings).SetCommand("docs:dev")
                        )
                );
    Target DocsCompile =>
        _ =>
            _.DependsOn(DocsRestore, PatchPackageVersion)
                .Executes(
                    () =>
                        NpmTasks.NpmRun(settings =>
                            SetDocsWorkingDirectory(settings).SetCommand("docs:build")
                        )
                );

    Target DocsPreview =>
        _ =>
            _.DependsOn(DocsCompile)
                .Executes(
                    () =>
                        NpmTasks.NpmRun(settings =>
                            SetDocsWorkingDirectory(settings).SetCommand("docs:preview")
                        )
                );

    Target DocsGzip =>
        _ =>
            _.DependsOn(DocsCompile)
                .Executes(() =>
                {
                    Log.Information(
                        "Compressing documentation website files to {ArtifactPath}",
                        DocsArtifact
                    );

                    DocsArtifact.Parent.CreateOrCleanDirectory();

                    ProcessTasks.StartShell(
                        $"tar --dereference --directory {DocsOutputDirectory} -cvf {DocsArtifact} ."
                    );
                });

    T SetDocsWorkingDirectory<T>(T settings)
        where T : ToolSettings
    {
        return settings.SetProcessWorkingDirectory(DocsDirectory);
    }
}
