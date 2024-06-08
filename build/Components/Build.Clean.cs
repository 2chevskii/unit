using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

partial class Build
{
    Target Clean =>
        _ =>
            _.Executes(
                () =>
                    DotNetTasks.DotNetClean(settings =>
                        settings
                            .SetProject(Sln)
                            .CombineWith(
                                Configuration.All,
                                (settings, configuration) =>
                                    settings.SetConfiguration(configuration)
                            )
                    )
            );
}
