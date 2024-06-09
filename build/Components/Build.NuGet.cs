using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;

partial class Build
{
    [Parameter]
    readonly string NugetFeed;

    Target EnsureNugetFeedEnabled => _ => _.Requires(() => NugetFeed).Executes(() => { });

    NuGetFeed[] GetNuGetFeeds()
    {
        var output = DotNetTasks.DotNet("nuget list source");

        if (output.Any(x => x.Type == OutputType.Err))
        {
            throw new Exception(
                "Error while executing 'dotnet nuget list source': "
                    + output.Where(x => x.Type == OutputType.Err).Select(x => x.Text).JoinNewLine()
            );
        }

        throw new NotImplementedException();
    }
}

class NuGetFeed
{
    public string Name;
    public Uri Uri;
    public bool IsEnabled;
}
