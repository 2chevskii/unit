using Nuke.Common;

partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);
}
