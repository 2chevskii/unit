using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

interface IRestore : IHazSlnFiles
{
    Target Restore =>
        _ =>
            _.Executes(
                () =>
                    DotNetTasks.DotNetRestore(settings =>
                        settings.SetProcessWorkingDirectory(Sln.Directory)
                    )
            );
}
