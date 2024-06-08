using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.ProjectModel;

partial class Build
{
    [Solution]
    readonly Solution Sln;

    [Parameter]
    readonly Dictionary<string, string> TestProjectsMap;

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
        if (!TestProjectsMap.TryGetValue(srcProject.Name, out string name))
        {
            return null;
        }

        return TestProjects.First(x => x.Name == name);
    }

    Project GetSrcProjectForTestProject(Project testProject)
    {
        var reverseTestProjectsMap = TestProjectsMap.ToDictionary(k => k.Value, v => v.Key);

        if (!reverseTestProjectsMap.TryGetValue(testProject.Name, out string name))
        {
            return null;
        }

        return SrcProjects.First(x => x.Name == name);
    }
}
