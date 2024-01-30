using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using Serilog;

interface IControlNuGetSources : INukeBuild
{
    [Parameter]
    Uri NugetFeed => TryGetValue(() => NugetFeed);

    string NugetSourceName => NugetFeed.Host.Split('.').TakeLast(2).JoinDot();

    Regex NuGetSourceRegex =>
        new Regex(
            @"^\s*\d+\.\s+([^\s]+)\s\[(Enabled|Disabled)\]\n\s+([^\s]+)$",
            RegexOptions.Multiline
        );

    Target ReadNugetSources =>
        _ => _.Unlisted().Executes(() => NugetSource.Sources = ReadSources().ToArray());

    Target EnsureHasNugetFeed =>
        _ =>
            _.Unlisted()
                .Requires(() => NugetFeed)
                .DependsOn(ReadNugetSources)
                .OnlyWhenDynamic(() => NugetSource.Sources.All(x => x.Uri != NugetFeed))
                .Executes(
                    () =>
                        DotNetTasks.DotNetNuGetAddSource(settings =>
                            settings.SetName(NugetSourceName).SetSource(NugetFeed.ToString())
                        ),
                    () =>
                        NugetSource.Sources = NugetSource
                            .Sources.Append(
                                new NugetSource
                                {
                                    IsEnabled = true,
                                    Name = NugetSourceName,
                                    Uri = NugetFeed
                                }
                            )
                            .ToArray()
                );

    IEnumerable<NugetSource> ReadSources()
    {
        IReadOnlyCollection<Output> output = DotNetTasks.DotNet(
            "nuget list source --format detailed"
        );

        return ParseSources(output.Skip(1).ToArray());
    }

    IEnumerable<NugetSource> ParseSources(Output[] outputLines)
    {
        Log.Debug("Line count: {LineCount}", outputLines.Length);
        if (outputLines.Length % 2 != 0)
        {
            throw new Exception(
                "Even number of output lines should be given (No '--format detailed' option?)"
            );
        }

        string fullOutputText = outputLines
            .Select(line => line.Text.TrimEnd())
            .JoinNewLine(PlatformFamily.Linux);

        MatchCollection matches = NuGetSourceRegex.Matches(fullOutputText);

        return from match in matches
            let name = match.Groups[1].Value
            let strIsEnabled = match.Groups[2].Value
            let strSourceUri = match.Groups[3].Value
            let isEnabled = strIsEnabled switch
            {
                "Enabled" => true,
                "Disabled" => false,
                _ => throw new FormatException("Invalid 'is enabled' string: " + strIsEnabled)
            }
            let sourceUri = new Uri(strSourceUri)
            select new NugetSource
            {
                Name = name,
                Uri = sourceUri,
                IsEnabled = isEnabled
            };
    }
}

public readonly struct NugetSource
{
    public static NugetSource[] Sources;

    public string Name { get; init; }
    public Uri Uri { get; init; }
    public bool IsEnabled { get; init; }
}
