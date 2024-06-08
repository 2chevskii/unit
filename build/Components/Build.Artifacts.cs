using System.Linq;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

partial class Build
{
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "pkg";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test_results";
    AbsolutePath CoverageDirectory => TestResultsDirectory / "coverage";
    AbsolutePath CoverageReportsDirectory => CoverageDirectory / "reports";

    AbsolutePath GetProjectOutputAssemblyPath(Project project) =>
        project.Directory
        / "bin"
        / Configuration
        / project.GetTargetFrameworks()!.First()
        / $"{project.Name}.dll";
}
