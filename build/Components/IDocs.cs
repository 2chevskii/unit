using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DocFX;
using Nuke.Common.Tools.DotNet;

namespace Components;

interface IDocs : IHazSlnFiles
{
    AbsolutePath DocsDirectory => RootDirectory / "docs";
    AbsolutePath DocfxConfig => DocsDirectory / "docfx.json";
    AbsolutePath DocsOutputDirectory => DocsDirectory / "dist";

    Target Docs =>
        _ =>
            _.Executes(
                () =>
                    /*DocFXTasks.DocFXBuild(settings =>
                        settings
                            .SetConfigFile(DocfxConfig)
                            .SetProcessWorkingDirectory(DocsDirectory)
                            .SetProcessToolPath("dotnet-docfx")
                    )*/
                DotNetTasks.DotNet("docfx build docs/docfx.json")
            );

    Target DocsPreview =>
        _ =>
            _.DependsOn(Docs)
                .Executes(
                    () =>
                        /*DocFXTasks.DocFXServe(settings =>
                            settings.SetFolder(DocsDirectory / "dist").SetProcessToolPath("dotnet-docfx")
                        )*/
                    DotNetTasks.DotNet("docfx --serve docs/docfx.json")
                );

    Target DocsClean =>
        _ =>
            _.Executes(
                () => DocsOutputDirectory.DeleteDirectory(),
                () => (DocsDirectory / "api").DeleteDirectory()
            );
}
