using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NuGet.Packaging;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.NuGet;
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

    [Parameter, Secret]
    readonly string NugetApiKey;

    Target NugetPush =>
        _ =>
            _.Requires(() => NugetFeed)
                .Requires(() => NugetApiKey)
                .DependsOn(NugetEnsureFeedEnabled)
                .Executes(() =>
                {
                    IReadOnlyCollection<AbsolutePath> packagesToUpload =
                        PackagesDirectory.GlobFiles("*.nupkg");

                    Log.Information(
                        "Found {PackageCount} packages pending upload to feed {NugetFeed}",
                        packagesToUpload.Count,
                        NugetFeed
                    );

                    NuGetFeed feed = NuGetFeed.FromUri(NugetFeed);

                    Log.Information("Nuget feed is {NugetFeed}", feed);

                    List<(NuGetFeed, string, string)> pushedPackages = [];

                    foreach (AbsolutePath packagePath in packagesToUpload)
                    {
                        Log.Information("Uploading package {PackagePath}", packagePath);

                        using PackageArchiveReader packageArchiveReader = new PackageArchiveReader(
                            packagePath
                        );

                        string packageId = packageArchiveReader.NuspecReader.GetId();
                        NuGetVersion packageVersion =
                            packageArchiveReader.NuspecReader.GetVersion();

                        Log.Information(
                            "Package metadata: {Id}.{Version}",
                            packageId,
                            packageVersion
                        );

                        DotNetNuGetPush(settings =>
                            settings
                                .SetSource(feed.Name)
                                .SetApiKey(NugetApiKey)
                                .SetTargetPath(packagePath)
                        );

                        pushedPackages.Add((feed, packageId, packageVersion.ToString()));
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

    Target TestDeploymentUrls =>
        _ =>
            _.Executes(() =>
            {
                var outputPath = EnvironmentInfo.GetVariable<AbsolutePath>("GITHUB_OUTPUT");

                outputPath.AppendAllLines(
                    [
                        $"package_url=https://example.com/pkg/nuget/testpackage/v1.0.0-test.deployments"
                    ]
                );
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

    void WritePushedPackageUrlsToGithubOutput(
        IReadOnlyCollection<(
            NuGetFeed feed,
            string packageId,
            string packageVersion
        )> pushedPackages
    )
    {
        const string variableName = "packages_urls";

        var packageUrls = pushedPackages
            .Select(pushedPackage =>
                pushedPackage.feed.Name switch
                {
                    "nuget.org"
                        => GetNugetOrgPackageUrl(
                            pushedPackage.packageId,
                            pushedPackage.packageVersion
                        ),
                    "github.com" => GetGithubPackageUrl(pushedPackage.packageId),
                    var _ => null
                }
            )
            .Where(x => x != null);
    }

    string GetNugetOrgPackageUrl(string packageId, string packageVersion)
    {
        return $"https://nuget.org/packages/{packageId}/{packageVersion}";
    }

    string GetGithubPackageUrl(string packageId)
    {
        return $"https://github.com/{GitRepository.GetGitHubOwner()}/{GitRepository.GetGitHubName()}/pkgs/nuget/{packageId}";
    }

    void WriteActionsOutput(string variableName, string format, params object[] args)
    {
        Log.Debug(
            "Writing actions output: {VariableName} {Format} {@Args}",
            variableName,
            format,
            args
        );

        var outputPath = AbsolutePath.Create(EnvironmentInfo.GetVariable<string>("GITHUB_OUTPUT"));

        Log.Debug("Output path is {OutputPath}", outputPath);

        var content = $"{variableName}={string.Format(format, args)}\n";

        Log.Debug("Content is {Content}", content);

        outputPath.AppendAllText(content);
    }
}
