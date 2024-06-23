using System.Linq;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

partial class Build
{
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "packages";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test_results";
    AbsolutePath CoverageReportsDirectory => ArtifactsDirectory / "coverage" / "reports";
    AbsolutePath CoverageSummaryDirectory => ArtifactsDirectory / "coverage" / "summary";
    AbsolutePath ReleaseAssetsDirectory => ArtifactsDirectory / "release_assets";

    AbsolutePath GetProjectOutputAssemblyPath(Project project) =>
        project.Directory
        / "bin"
        / Configuration
        / project.GetTargetFrameworks()!.First()
        / $"{project.Name}.dll";
}
