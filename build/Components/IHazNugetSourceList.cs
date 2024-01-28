using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;

interface IHazNugetSourceList
{
    IEnumerable<NugetSource> NugetSources => NugetSource.GetSources();

    bool HasNugetSource(Uri uri) => HasNugetSource(uri.Host);
    bool HasNugetSource(string name) => NugetSources.Any(source => source.Name == name);

    public struct NugetSource
    {
        [CanBeNull]
        static IEnumerable<NugetSource> _sources;

        static readonly Regex NameEnabledRegex = new Regex(
            @"\d+\.\s*([a-zA-Z0-9_\.-]+)\s+\[(Enabled|Disabled)\]"
        );

        public string Name;
        public string Uri;
        public bool Enabled;

        public static IEnumerable<NugetSource> GetSources()
        {
            if (_sources == null)
                _sources = ParseAll(
                    DotNetTasks.DotNet("nuget list source --format detailed").Skip(1).ToList()
                );

            return _sources;
        }

        public static NugetSource Parse(IEnumerable<Output> lines)
        {
            string firstLine = lines.First().Text;
            Match nameEnabledMatch = NameEnabledRegex.Match(firstLine);
            string name = nameEnabledMatch.Groups[1].Value;

            string enabledString = nameEnabledMatch.Groups[2].Value;

            Log.Information("Enabled string: {Str}", enabledString);

            bool isEnabled = ParseEnabled(enabledString);
            string uri = lines.Skip(1).First().Text.Trim();

            return new NugetSource
            {
                Name = name,
                Enabled = isEnabled,
                Uri = uri
            };
        }

        public static IEnumerable<NugetSource> ParseAll(IReadOnlyList<Output> lines)
        {
            for (int i = 0; i < lines.Count; i += 2)
            {
                Output thisLine = lines[i];
                Output nextLine = lines[i + 1];
                NugetSource source = Parse([thisLine, nextLine]);
                yield return source;
            }
        }

        static bool ParseEnabled(string str)
        {
            return str switch
            {
                "Enabled" or "E" => true,
                "Disabled" or "D" => false
            };
        }
    }
}
