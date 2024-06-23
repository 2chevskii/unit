using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common.ProjectModel;

partial class Build
{
    [Solution]
    readonly Solution Sln;

    SolutionFolder SrcFolder => Sln.GetSolutionFolder("src");
    SolutionFolder TestFolder => Sln.GetSolutionFolder("test");

    List<Project> SrcProjects => SrcFolder.Projects.ToList();
    List<Project> TestProjects => TestFolder.Projects.ToList();
    Project BuildProject => Sln.GetProject("_build");

    Project GetTestProjectForSrcProject(Project srcProject)
    {
        return TestProjects.FirstOrDefault(x => x.Name == $"{srcProject.Name}.Tests");
    }
}
