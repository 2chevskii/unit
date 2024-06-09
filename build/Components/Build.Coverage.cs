using System;
using Nuke.Common;
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
    [Parameter]
    readonly bool CoverageHtmlReport;

    Target Coverage => _ => _.DependsOn(CoverageCollect, CoverageReportHtml);

    Target CoverageCollect =>
        _ =>
            _.Unlisted()
                .DependsOn(Compile)
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

    Target CoverageReportHtml =>
        _ =>
            _.OnlyWhenStatic(() => CoverageHtmlReport)
                .DependsOn(Compile)
                .DependsOn(CoverageCollect)
                .Executes(
                    () =>
                        ReportGenerator(settings =>
                            settings.Apply(ReportGeneratorSettingsBase)
                        )
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
                    .SetOutput(CoverageDirectory / $"{srcProject.Name}.xml");
        };

    Configure<ReportGeneratorSettings> ReportGeneratorSettingsBase =>
        settings =>
            settings
                .SetReports(CoverageDirectory / "*.xml")
                .SetReportTypes(ReportTypes.Html_Dark)
                .SetTargetDirectory(CoverageHtmlDirectory);
}
