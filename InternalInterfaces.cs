using Nickel;

namespace TheJazMaster.Nibbs;

internal interface INibbsCard
{
	static abstract void Register(IModHelper helper);
}

internal interface INibbsArtifact
{
	static abstract void Register(IModHelper helper);
}