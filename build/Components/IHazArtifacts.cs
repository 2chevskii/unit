using Nuke.Common;
using Nuke.Common.IO;

interface IHazArtifacts : INukeBuild
{
    /*ArtifactPathCollection ArtifactPaths =>
        new ArtifactPathCollection { Root = RootDirectory / "artifacts" };

    void InitializeArtifactsDirectories()
    {
        ArtifactPaths.Root.CreateDirectory();
        ArtifactPaths.Packages.CreateDirectory();
        ArtifactPaths.Libraries.CreateDirectory();
        ArtifactPaths.TestResults.CreateDirectory();
        ArtifactPaths.Coverage.CreateDirectory();
        ArtifactPaths.Docs.CreateDirectory();
    }

    readonly struct ArtifactPathCollection
    {
        public AbsolutePath Root { get; init; }
        public AbsolutePath Packages => Root / "pkg";
        public AbsolutePath Libraries => Root / "lib";
        public AbsolutePath TestResults => Root / "test_results";
        public AbsolutePath Coverage => TestResults / "coverage";
        public AbsolutePath Docs => Root / "docs";
        public AbsolutePath[] All => [Root, Packages, Libraries, TestResults, Coverage, Docs];
    }*/
}
