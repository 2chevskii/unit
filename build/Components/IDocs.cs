using System.Formats.Tar;
using System.IO;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DocFX;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace Components;

interface IDocs : IHazSlnFiles, IHazArtifacts, IRestore
{
    AbsolutePath DocsDirectory => RootDirectory / "docs";
    AbsolutePath DocfxConfig => DocsDirectory / "docfx.json";
    AbsolutePath DocsOutputDirectory => DocsDirectory / "dist";
    AbsolutePath DocsArtifactPath => ArtifactsDirectory / "docs/github-pages.tar";

    Target Docs => _ => _.DependsOn(DocsCompile, DocsGzip);

    Target DocsPreview =>
        _ =>
            _.DependsOn(DocsCompile)
                .Executes(() => DotNetTasks.DotNet($"docfx serve {DocsOutputDirectory}"))
                .ProceedAfterFailure();

    Target DocsClean =>
        _ =>
            _.Executes(
                () => DocsOutputDirectory.DeleteDirectory(),
                () => (DocsDirectory / "api").DeleteDirectory()
            );

    Target DocsCompile =>
        _ =>
            _.DependsOn(RestoreTools)
                .Executes(() =>
                {
                    Log.Information("Generating documentation website...");
                    DotNetTasks.DotNet($"docfx build {DocfxConfig}");
                });

    Target DocsGzip =>
        _ =>
            _.DependsOn(DocsCompile)
                .Executes(() =>
                {
                    Log.Information(
                        "Compressing documentation website files to {ArtifactPath}",
                        DocsArtifactPath
                    );

                    DocsArtifactPath.Parent.CreateOrCleanDirectory();
                    using Stream tarfileStream = File.OpenWrite(DocsArtifactPath);
                    TarFile.CreateFromDirectory(DocsOutputDirectory, tarfileStream, false);
                });
}
