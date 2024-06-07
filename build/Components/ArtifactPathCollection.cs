using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.IO;

class ArtifactPathCollection
{
    public AbsolutePath RootDirectory => NukeBuild.RootDirectory / "build" / "artifacts";

    public AbsolutePath PackagesDirectory => RootDirectory / "pkg";

    public AbsolutePath LibrariesDirectory => RootDirectory / "lib";

    public AbsolutePath TestResults => RootDirectory / "test_results";

    public AbsolutePath Coverage => RootDirectory / "coverage";

    public AbsolutePath CoverageHtml => Coverage / "html";

    public AbsolutePath DocsDirectory => RootDirectory / "docs";

    public IReadOnlyList<AbsolutePath> All =>
        [
            RootDirectory,
            PackagesDirectory,
            LibrariesDirectory,
            TestResults,
            Coverage,
            CoverageHtml,
            DocsDirectory
        ];

    /*
     * public AbsolutePath Root { get; init; }
        public AbsolutePath Pkg => Root / "pkg";
        public AbsolutePath Lib => Root / "lib";
        public AbsolutePath TestResults => Root / "test_results";
        public AbsolutePath Coverage => Root / "coverage";
        public AbsolutePath Docs => Root / "docs";
        public List<AbsolutePath> All => [Root, Pkg, Lib, TestResults, Coverage, Docs];
     */
}
