using Nuke.Common;

interface IHazConfiguration : INukeBuild
{
    [Parameter]
    Configuration Configuration => TryGetValue(() => Configuration) ?? Configuration.Debug;
}
