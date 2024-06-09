using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common.ProjectModel;

partial class Build
{
    [Solution]
    readonly Solution Sln;

    SolutionFolder SrcFolder;
    SolutionFolder TestFolder;

    List<Project> SrcProjects;
    List<Project> TestProjects;
    Project BuildProject;

    void LoadProjectModel()
    {
        SrcFolder =
            Sln.GetSolutionFolder("src")
            ?? throw new Exception("Failed to find 'src' solution folder");
        TestFolder =
            Sln.GetSolutionFolder("test")
            ?? throw new Exception("Failed to find 'test' solution folder");
        SrcProjects = SrcFolder.Projects.ToList();
        TestProjects = TestFolder.Projects.ToList();
        BuildProject =
            Sln.GetProject("_build") ?? throw new Exception("Failed to find '_build' project");
    }

    Project GetTestProjectForSrcProject(Project srcProject)
    {
        return TestProjects.First(x => x.Name == $"{srcProject.Name}.Tests");
    }

    Project GetSrcProjectForTestProject(Project testProject)
    {
        return SrcProjects.First(x =>
            x.Name
            == testProject.Name[..testProject.Name.LastIndexOf(".Tests", StringComparison.Ordinal)]
        );
    }
}
