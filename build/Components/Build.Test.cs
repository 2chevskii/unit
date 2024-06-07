using System;
using Extensions;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

partial class Build
{
    [Parameter]
    readonly bool HtmlTestResults;

    Target Test =>
        _ =>
            _.DependsOn(Compile)
                .Executes(
                    () =>
                        DotNetTasks.DotNetTest(settings =>
                            settings
                                .Apply(TestSettingsBase)
                                .CombineWith(
                                    TestProjects,
                                    (settings, project) =>
                                        settings
                                            .Apply(TestSettingsLoggers(project))
                                            .SetProjectFile(project)
                                )
                        )
                );

    Configure<DotNetTestSettings> TestSettingsBase =>
        settings => settings.EnableNoBuild().SetConfiguration(Configuration);

    Func<Project, Configure<DotNetTestSettings>> TestSettingsLoggers =>
        project =>
            settings =>
                settings
                    .AddLoggers("console;verbosity=detailed")
                    .When(
                        HtmlTestResults,
                        settings => settings.AddLoggers($"html;logfilename={project.Name}.html")
                    )
                    .When(
                        Host.IsGitHubActions(),
                        settings =>
                            settings.AddLoggers(
                                "GitHubActions;summary.includePassedTests=true;summary.includeSkippedTests=true"
                            )
                    );
}
