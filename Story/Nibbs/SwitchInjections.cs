namespace TheJazMaster.Nibbs.Story.Nibbs;

internal class NibbsSwitchInjections() : BaseSwitchInjections(
    ModEntry.Instance.NibbsManager.NibbsCharacter.CharacterType,
    ModEntry.Instance.Package.PackageRoot.GetRelativeDirectory("I18n/en/Nibbs").GetRelativeFile("inject.json")
) {}