using System;
using Extensions;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

interface ITest : ICompile
{
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test_results";

    Target UnitTest =>
        _ =>
            _.DependsOn(Compile)
                .Executes(
                    () =>
                        DotNetTasks.DotNetTest(settings =>
                            settings
                                .Apply(TestSettingsBase)
                                .Apply(TestSettingsLoggers)
                                .SetProjectFile(TestsProject)
                        )
                );

    sealed Configure<DotNetTestSettings> TestSettingsBase =>
        settings => settings.EnableNoBuild().SetConfiguration(Configuration);

    sealed Configure<DotNetTestSettings> TestSettingsLoggers =>
        settings =>
            settings
                .AddLoggers("console;verbosity=detailed")
                .When(HtmlTestResults, s => s.AddLoggers("html;logfilename=test-results.html"))
                .When(
                    Host.IsGitHubActions(),
                    s =>
                        s.AddLoggers(
                            "GitHubActions;summary.includePassedTests=true;summary.includeSkippedTests=true"
                        )
                )
                .SetResultsDirectory(TestResultsDirectory);

    [Parameter]
    bool HtmlTestResults => TryGetValue<bool?>(() => HtmlTestResults).GetValueOrDefault();
}
