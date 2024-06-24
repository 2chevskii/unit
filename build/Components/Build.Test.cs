using System;
using Extensions;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    [Parameter]
    readonly bool EnableHtmlTestResults;

    Target Test =>
        _ =>
            _.DependsOn(Compile)
                .Executes(
                    () =>
                        DotNetTest(settings =>
                            settings
                                .Apply(TestSettingsBase)
                                .CombineWith(
                                    TestProjects,
                                    (settings, project) =>
                                        settings
                                            .Apply(TestSettingsLogging(project))
                                            .SetProjectFile(project)
                                )
                        )
                );

    Configure<DotNetTestSettings> TestSettingsBase =>
        settings =>
            settings
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetResultsDirectory(TestResultsDirectory);

    Func<Project, Configure<DotNetTestSettings>> TestSettingsLogging =>
        project =>
            settings =>
                settings
                    .AddLoggers("console;verbosity=detailed;")
                    .When(
                        EnableHtmlTestResults,
                        settings => settings.AddLoggers($"html;logfilename={project.Name}.html;")
                    )
                    .When(
                        Host.IsGitHubActions(),
                        settings =>
                            settings.AddLoggers(
                                "GitHubActions;summary.includePassedTests=true;summary.includeSkippedTests=true;"
                            )
                    );
}
