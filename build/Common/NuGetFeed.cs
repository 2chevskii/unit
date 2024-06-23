using System;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common.Utilities;

class NuGetFeed
{
    public string Name;
    public Uri Uri;
    public bool IsEnabled;

    public override string ToString()
    {
        return $@"[{Name}]({Uri}):{(IsEnabled ? "Enabled" : "Disabled")}";
    }

    public static NuGetFeed FromMatch(Match match)
    {
        return new NuGetFeed
        {
            Name = match.Groups[1].Value,
            IsEnabled = ParseIsEnabled(match.Groups[2].Value),
            Uri = new Uri(match.Groups[3].Value)
        };
    }

    public static NuGetFeed FromUri(string strUri)
    {
        Uri uri = new Uri(strUri);

        return new NuGetFeed { Name = uri.Host.Split('.').TakeLast(2).JoinDot(), Uri = uri };
    }

    static bool ParseIsEnabled(string strEnabled)
    {
        return strEnabled switch
        {
            "Enabled" or "E" => true,
            "Disabled" or "D" => false,
            var _ => throw new FormatException()
        };
    }
}
