using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    Target Clean =>
        _ =>
            _.Executes(
                () =>
                    DotNetClean(settings =>
                        settings
                            .SetProject(Sln)
                            .CombineWith(
                                Configuration.All,
                                (settings, configuration) =>
                                    settings.SetConfiguration(configuration)
                            )
                    )
            );

    Target CleanArtifacts =>
        _ =>
            _.Executes(() =>
            {
                Log.Information("Cleaning artifact directories");
                AbsolutePath[] directoriesToClean =
                [
                    ArtifactsDirectory,
                    PackagesDirectory,
                    TestResultsDirectory,
                    CoverageDirectory,
                    CoverageHtmlDirectory,
                ];
                foreach (AbsolutePath directory in directoriesToClean)
                {
                    if (!directory.DirectoryExists())
                    {
                        continue;
                    }
                    Log.Debug("Cleaning {Directory}", RootDirectory.GetRelativePathTo(directory));
                    directory.CreateOrCleanDirectory();
                }
            });
}
