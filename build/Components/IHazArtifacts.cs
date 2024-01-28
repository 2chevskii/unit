using Nuke.Common;
using Nuke.Common.IO;

interface IHazArtifacts : INukeBuild
{
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "pkg";
    AbsolutePath LibrariesDirectory => ArtifactsDirectory / "lib";
    AbsolutePath DocsArtifactsDirectory => ArtifactsDirectory / "docs";

    void InitializeArtifactsDirectories()
    {
        ArtifactsDirectory.CreateDirectory();
        PackagesDirectory.CreateDirectory();
        LibrariesDirectory.CreateDirectory();
        DocsArtifactsDirectory.CreateDirectory();
    }
}
