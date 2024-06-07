using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;

partial class Build : NukeBuild, IPack, INugetPush, IDocs
{
    public static int Main() => Execute<Build>(x => x.Compile);

    protected override void OnBuildInitialized()
    {
        CreateArtifactDirectories();
    }
}

partial class Build
{
    ArtifactPathCollection ArtifactPaths => new();

    void CreateArtifactDirectories()
    {
        ArtifactPaths.All.ForEach(x => x.CreateDirectory());
    }
}
