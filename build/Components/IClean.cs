using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;

interface IClean : IHazArtifacts, IHazSlnFiles, IHazConfiguration, ITest
{
    Target CleanProjects =>
        _ =>
            _.Executes(
                () =>
                    DotNetTasks.DotNetClean(settings =>
                        settings
                            .SetVerbosity(DotNetVerbosity.minimal)
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
            _.Executes(
                () =>
                    new[] { PackagesDirectory, LibrariesDirectory, TestResultsDirectory }.ForEach(
                        x => x.CreateOrCleanDirectory()
                    )
            );

    Target Clean => _ => _.DependsOn(CleanProjects, CleanArtifacts);
}
