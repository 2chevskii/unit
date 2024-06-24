using System;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.ReportGenerator;
using static Nuke.Common.Tools.Coverlet.CoverletTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

[Requires<CoverletTasks>(Version = "6.0.2")]
[Requires<ReportGeneratorTasks>(Version = "5.3.6")]
partial class Build
{
    AbsolutePath CoverageReportsGlob => CoverageReportsDirectory / "*.xml";

    Target CoverageCollect =>
        _ =>
            _.DependsOn(Compile)
                .Executes(
                    () =>
                        Coverlet(settings =>
                            settings
                                .Apply(CoverletSettingsBase)
                                .CombineWith(
                                    SrcProjects,
                                    (settings, srcProject) =>
                                        settings.Apply(CoverletSettingsProject(srcProject))
                                )
                        )
                );

    Target CoverageCreateSummary =>
        _ =>
            _.DependsOn(Compile, CoverageCollect)
                .Executes(
                    () => ReportGenerator(settings => settings.Apply(ReportGeneratorSettingsBase))
                );

    Configure<CoverletSettings> CoverletSettingsBase =>
        settings => settings.SetFormat("cobertura").SetTarget("dotnet");

    Func<Project, Configure<CoverletSettings>> CoverletSettingsProject =>
        srcProject =>
        {
            Project testProject = GetTestProjectForSrcProject(srcProject);
            return settings =>
                settings
                    .SetTargetArgs($"test {testProject} --no-build --configuration {Configuration}")
                    .SetAssembly(GetProjectOutputAssemblyPath(testProject))
                    .SetInclude($"[{srcProject.Name}]*")
                    .SetOutput(CoverageReportsDirectory / $"{srcProject.Name}.xml");
        };

    Configure<ReportGeneratorSettings> ReportGeneratorSettingsBase =>
        settings =>
            settings
                .SetReports(CoverageReportsGlob)
                .SetReportTypes(ReportTypes.Html_Dark)
                .SetTargetDirectory(CoverageSummaryDirectory);
}
