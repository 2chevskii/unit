using System.Linq;
using Extensions;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ReportGenerator;

interface ITest
{
    /*AbsolutePath CoverageXmlPath => ArtifactPaths.Coverage / $"coverage.{MainProject.Name}.xml";
    AbsolutePath CoverageHtmlDirectory => ArtifactPaths.Coverage / $"report.{MainProject.Name}";

    Target Test =>
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

    Target Coverage => _ => _.DependsOn(CoverageCollect, CoverageReport);

    Target CoverageCollect =>
        _ =>
            _.DependsOn(CompileTests)
                .Executes(
                    () =>
                        CoverletTasks.Coverlet(settings =>
                            settings
                                .SetFormat("cobertura")
                                .SetTarget("dotnet")
                                .SetTargetArgs(
                                    $"test {TestsProject} --no-build --configuration {Configuration}"
                                )
                                .SetAssembly(
                                    TestsProject.Directory
                                        / "bin"
                                        / Configuration
                                        / "net8.0"
                                        / $"{TestsProject.Name}.dll"
                                )
                                .SetInclude($"[{MainProject.Name}]*")
                                .SetOutput(CoverageXmlPath)
                        )
                );

    Target CoverageReport =>
        _ =>
            _.DependsOn(CoverageCollect)
                .Executes(
                    () =>
                        ReportGeneratorTasks.ReportGenerator(settings =>
                            settings
                                .SetReports(CoverageXmlPath)
                                .SetTargetDirectory(CoverageHtmlDirectory)
                        )
                );

    sealed Configure<DotNetTestSettings> TestSettingsBase =>
        settings => settings.EnableNoBuild().SetConfiguration(Configuration);

    sealed Configure<DotNetTestSettings> TestSettingsLoggers =>
        settings =>
            settings
                .AddLoggers("console;verbosity=detailed")
                .When(HtmlTestResults, s => s.AddLoggers($"html;logfilename=test-results.{MainProject.Name}.html"))
                .When(
                    Host.IsGitHubActions(),
                    s =>
                        s.AddLoggers(
                            "GitHubActions;summary.includePassedTests=true;summary.includeSkippedTests=true"
                        )
                )
                .SetResultsDirectory(ArtifactPaths.TestResults);

    [Parameter]
    bool HtmlTestResults => TryGetValue<bool?>(() => HtmlTestResults).GetValueOrDefault();*/
}
