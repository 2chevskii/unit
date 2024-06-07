using System.Collections.Generic;
using Nuke.Common.ProjectModel;

partial class Build
{
    [Solution]
    readonly Solution Sln;

    IReadOnlyCollection<Project> SrcProjects => Sln.GetSolutionFolder("src")!.Projects;
    IReadOnlyCollection<Project> TestProjects => Sln.GetSolutionFolder("test")!.Projects;
}
