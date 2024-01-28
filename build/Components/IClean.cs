using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Serilog;

interface IClean : ITest
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
                    ArtifactPaths.All.ForEach(x =>
                    {
                        Log.Debug("Cleaning directory: {Directory}", x);
                        x.CreateOrCleanDirectory();
                    })
            );

    Target Clean => _ => _.DependsOn(CleanProjects, CleanArtifacts);
}
