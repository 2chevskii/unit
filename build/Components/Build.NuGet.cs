using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NuGet.Packaging;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Utilities;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    static readonly Regex NugetFeedRegex = new Regex(
        @"\s+\d+\.\s+([^\s]+)\s+\[([^\]]+?)\]\r?\n\s*([^\s]+)"
    );

    [Parameter]
    readonly string NugetFeed;

    [Parameter]
    readonly string PackageId;

    [Parameter, Secret]
    readonly string NugetApiKey;

    Target NugetPush =>
        _ =>
            _.Requires(() => NugetFeed, () => NugetApiKey, () => PackageId)
                .DependsOn(NugetEnsureFeedEnabled)
                .Executes(() =>
                {
                    NuGetFeed nugetFeed = NuGetFeed.FromUri(NugetFeed);
                    AbsolutePath packagePath = PackagesDirectory
                        .GlobFiles($"{PackageId}.*.nupkg")
                        .FirstOrDefault();

                    if (packagePath == null)
                    {
                        throw new Exception(
                            $"Could not find package with ID {PackageId} at {PackagesDirectory} directory"
                        );
                    }

                    Log.Debug(
                        "Nuget feed: {NugetFeed}, Package ID: {PackageId}, Package path: {PackagePath}",
                        nugetFeed,
                        PackageId,
                        packagePath
                    );

                    using PackageArchiveReader packageArchiveReader = new PackageArchiveReader(
                        packagePath
                    );

                    NuGetVersion packageVersion = packageArchiveReader.NuspecReader.GetVersion();

                    Log.Information(
                        "Uploading {PackageId} v{Version} to {NugetFeed}",
                        PackageId,
                        packageVersion,
                        nugetFeed.Name
                    );

                    DotNetNuGetPush(settings =>
                        settings
                            .SetSource(nugetFeed.Name)
                            .SetApiKey(NugetApiKey)
                            .SetTargetPath(packagePath)
                    );

                    string deploymentUrl = GetPackageDeploymentUrl(
                        nugetFeed,
                        packageVersion.ToString()
                    );

                    if (deploymentUrl == null)
                    {
                        Log.Warning("Failed to determine package deployment URL");
                    }
                    else
                    {
                        WriteActionsOutput("package_url", deploymentUrl);
                    }
                });

    Target NugetEnsureFeedEnabled =>
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
                        DotNet(
                            $"nuget add source \"{requiredFeed.Uri}\" --name \"{requiredFeed.Name}\""
                        );
                    }
                    else if (!matchedFeed.IsEnabled)
                    {
                        Log.Warning("Feed was found, but not enabled, enabling");
                        DotNet($"nuget enable source \"{requiredFeed.Name}\"");
                    }
                    else
                    {
                        Log.Information("Required feed found and enabled");
                    }
                });

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

    string GetPackageDeploymentUrl(NuGetFeed nugetFeed, string packageVersion)
    {
        return nugetFeed.Name switch
        {
            "nuget.org" => $"https://nuget.org/packages/{PackageId}/{packageVersion}",
            "github.com"
                => $"https://github.com/{GitRepository.GetGitHubOwner()}/{GitRepository.GetGitHubName()}/pkgs/nuget/{PackageId}",
            var _ => null
        };
    }

    void WriteActionsOutput(string variableName, string format, params object[] args)
    {
        Log.Debug(
            "Writing actions output: {VariableName} {Format} {@Args}",
            variableName,
            format,
            args
        );

        AbsolutePath outputPath = AbsolutePath.Create(
            EnvironmentInfo.GetVariable<string>("GITHUB_OUTPUT")
        );

        Log.Debug("Output path is {OutputPath}", outputPath);

        string content = $"{variableName}={string.Format(format, args)}\n";

        Log.Debug("Content is {Content}", content);

        outputPath.AppendAllText(content);
    }
}
