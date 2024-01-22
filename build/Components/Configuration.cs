using System;
using System.ComponentModel;
using System.Linq;
using Nuke.Common.Tooling;

[TypeConverter(typeof(TypeConverter<Configuration>))]
public class Configuration : Enumeration
{
    public static readonly Configuration Debug = new Configuration { Value = nameof(Debug) };
    public static readonly Configuration Release = new Configuration { Value = nameof(Release) };
    public static readonly Configuration[] All = [Debug, Release];

    public static implicit operator string(Configuration configuration)
    {
        return configuration.Value;
    }
}
