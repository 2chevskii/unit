using System;
using System.Diagnostics.CodeAnalysis;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.ReportGenerator;

[Requires<CoverletTasks>(Version = "6.0.2")]
[Requires<ReportGeneratorTasks>(Version = "5.3.6")]
partial class Build
{
    [Parameter, Secret]
    readonly string CodecovToken;

    [Parameter]
    readonly bool CoverageHtmlReport;

    [Parameter]
    readonly bool CodecovReport;

    Target Coverage => _ => _.DependsOn(CoverageCollect, CoverageReport);

    Target CoverageCollect =>
        _ =>
            _.Unlisted()
                .DependsOn(Compile)
                .Executes(
                    () =>
                        CoverletTasks.Coverlet(settings =>
                            settings
                                .Apply(CoverletSettingsBase)
                                .CombineWith(
                                    SrcProjects,
                                    (settings, srcProject) =>
                                        settings.Apply(CoverletSettingsProject(srcProject))
                                )
                        )
                );

    Target CoverageReport =>
        _ =>
            _.DependsOn(CoverageCollect)
                .Executes(() =>
                {
                    if (CoverageHtmlReport)
                    {
                        ReportGeneratorTasks.ReportGenerator(settings =>
                            settings.Apply(ReportGeneratorSettingsBase)
                        );
                    }

                    /*
                     * Disabled, need to implement support for new codecov tool
                     */
                    /*if (CodecovReport)
                    {
                        if (string.IsNullOrEmpty(CodecovToken))
                        {
                            throw new Exception("Codecov token not found");
                        }

                        CodecovTasks.Codecov(settings =>
                            settings
                                .SetFiles(
                                    CoverageDirectory.GlobFiles("*.xml").Select(x => x.ToString())
                                )
                                .SetBuild(Version.FullSemVer)
                                .SetBranch(GitRepository.Branch)
                                .SetSha(GitRepository.Commit)
                                .SetToken(CodecovToken)
                        );
                    }*/
                });

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
                .SetReportTypes(ReportTypes.HtmlInline)
                .SetTargetDirectory(CoverageReportsDirectory);
}
