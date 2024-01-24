using System.IO;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DocFX;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace Components;

interface IDocs : IHazSlnFiles, IHazArtifacts
{
    AbsolutePath DocsDirectory => RootDirectory / "docs";
    AbsolutePath DocfxConfig => DocsDirectory / "docfx.json";
    AbsolutePath DocsOutputDirectory => DocsDirectory / "dist";
    AbsolutePath DocsArtifactPath => ArtifactsDirectory / "docs/github-pages.tar.gz";

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
            _.Executes(() =>
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
                    DocsOutputDirectory.TarGZipTo(DocsArtifactPath, fileMode: FileMode.Create);
                });
}
