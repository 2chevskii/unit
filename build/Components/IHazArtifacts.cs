using Nuke.Common;
using Nuke.Common.IO;

public interface IHazArtifacts : INukeBuild
{
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "pkg";
    AbsolutePath LibrariesDirectory => ArtifactsDirectory / "lib";

    void InitializeArtifactsDirectories()
    {
        ArtifactsDirectory.CreateDirectory();
        PackagesDirectory.CreateDirectory();
        LibrariesDirectory.CreateDirectory();
    }
}
