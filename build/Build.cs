using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;

class Build
    : NukeBuild,
        IPack,
        IClean,
        ICreateGitHubRelease
{
    public static int Main() => Execute<Build>(x => x.From<ICompile>().CompileMain);

    protected override void OnBuildInitialized()
    {
        From<IHazArtifacts>().InitializeArtifactsDirectories();
    }

    T From<T>()
        where T : class => this as T;
}
