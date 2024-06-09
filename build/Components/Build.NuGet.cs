using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using Serilog;

partial class Build
{
    static readonly Regex NugetFeedRegex = GetNugetFeedRegex();

    [Parameter]
    readonly string NugetFeed;

    Target EnsureNugetFeedEnabled =>
        _ =>
            _.Requires(() => NugetFeed)
                .Executes(() =>
                {
                    NuGetFeed requiredFeed = NuGetFeed.FromUri(NugetFeed);
                    requiredFeed.IsEnabled = true;
                    Log.Information("Required NuGet feed: {RequiredFeed}", requiredFeed);
                    List<NuGetFeed> feeds = GetNuGetFeeds();

                    NuGetFeed matchedFeed = feeds.Find(x => x.Uri == requiredFeed.Uri);

                    if (matchedFeed == null)
                    {
                        Log.Warning("Feed not found, adding");
                        DotNetTasks.DotNet(
                            $"nuget add source \"{requiredFeed.Uri}\" --name \"{requiredFeed.Name}\""
                        );
                    }
                    else if (!matchedFeed.IsEnabled)
                    {
                        Log.Warning("Feed was found, but not enabled, enabling");
                        DotNetTasks.DotNet($"nuget enable source \"{requiredFeed.Name}\"");
                    }
                    else
                    {
                        Log.Information("Required feed found and enabled");
                    }
                });

    [GeneratedRegex(@"\s+\d+\.\s+([^\s]+)\s+\[([^\]]+?)\]\r?\n\s*([^\s]+)")]
    private static partial Regex GetNugetFeedRegex();

    List<NuGetFeed> GetNuGetFeeds()
    {
        string outputString = GetNugetListSourceOutput();
        MatchCollection matches = GetNugetSourceListMatches(outputString);

        return matches.Select(NuGetFeed.FromMatch).ToList();
    }

    string GetNugetListSourceOutput()
    {
        IReadOnlyCollection<Output> output = DotNetTasks.DotNet(
            "nuget list source --format Detailed"
        );

        if (output.Any(x => x.Type == OutputType.Err))
        {
            throw new Exception(
                "Error while executing 'dotnet nuget list source': "
                    + output.Where(x => x.Type == OutputType.Err).Select(x => x.Text).JoinNewLine()
            );
        }

        string outputString = output
            .Where(x => x.Type == OutputType.Std)
            .Select(x => x.Text)
            .JoinNewLine();

        return outputString;
    }

    MatchCollection GetNugetSourceListMatches(string outputString)
    {
        MatchCollection matches = NugetFeedRegex.Matches(outputString);
        if (matches.Count == 0)
        {
            Log.Warning("No matched sources");
        }

        return matches;
    }
}
