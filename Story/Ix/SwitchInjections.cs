namespace TheJazMaster.Nibbs.Story.Ix;

internal class IxSwitchInjections() : BaseSwitchInjections(
    ModEntry.Instance.IxManager.IxCharacter.CharacterType,
    ModEntry.Instance.Package.PackageRoot.GetRelativeDirectory("I18n/en/Ix").GetRelativeFile("inject.json")
) {}