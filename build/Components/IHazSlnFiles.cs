using System.Linq;
using Nuke.Common;
using Nuke.Common.ProjectModel;

interface IHazSlnFiles : INukeBuild
{
    // Solution Sln { get; }
    [Solution]
    [Required]
    Solution Sln => TryGetValue(() => Sln);
    Project MainProject => Sln.GetAllProjects("Dvchevskii.Unit").First();
    Project TestsProject => Sln.GetAllProjects("Dvchevskii.Unit.Tests").First();
}
