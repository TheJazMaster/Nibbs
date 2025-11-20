using System;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.Nibbs;

internal interface IRegisterableCard
{
	private static Spr RegisterSpriteOrDefault(string path, Spr defaultSprite, IModHelper helper, IPluginPackage<IModManifest> package) {
		var file = package.PackageRoot.GetRelativeFile(path);
		if (file.Exists)
			return helper.Content.Sprites.RegisterSprite(file).Sprite;
		return defaultSprite;
	}

	static ICardEntry Register(Type type, Deck deck, string charname, Rarity rarity, IModHelper helper, IPluginPackage<IModManifest> package, out string name, bool dontOffer = false) {
		name = type.Name[..^4];
		return Register(type, deck, charname, rarity, dontOffer, name, RegisterSpriteOrDefault($"Sprites/Cards/{name}.png", StableSpr.cards_colorless, helper, package), helper, package);
	}
	static ICardEntry Register(Type type, Deck deck, string charname, Rarity rarity, IModHelper helper, IPluginPackage<IModManifest> package, out string name, out Spr unflippedSprite, out Spr flippedSprite, bool dontOffer = false) {
		name = type.Name[..^4];
		unflippedSprite = RegisterSpriteOrDefault($"Sprites/Cards/{charname}/{name}Unflipped.png", StableSpr.cards_colorless, helper, package);
		flippedSprite = RegisterSpriteOrDefault($"Sprites/Cards/{charname}/{name}Flipped.png", unflippedSprite, helper, package);
		return Register(type, deck, charname, rarity, dontOffer, name, unflippedSprite, helper, package);
	}
	static ICardEntry Register(Type type, Deck deck, string charname, Rarity rarity, IModHelper helper, IPluginPackage<IModManifest> package, out string name, out Spr normalSprite, out Spr unflippedSprite, out Spr flippedSprite, bool dontOffer = false) {
		name = type.Name[..^4];
		normalSprite = RegisterSpriteOrDefault($"Sprites/Cards/{charname}/{name}.png", StableSpr.cards_colorless, helper, package);
		unflippedSprite = RegisterSpriteOrDefault($"Sprites/Cards/{charname}/{name}Unflipped.png", normalSprite, helper, package);
		flippedSprite = RegisterSpriteOrDefault($"Sprites/Cards/{charname}/{name}Flipped.png", normalSprite, helper, package);
		return Register(type, deck, charname, rarity, dontOffer, name, normalSprite, helper, package);
	}

	static abstract void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package);

	private static ICardEntry Register(Type type, Deck deck, string charname, Rarity rarity, bool dontOffer, string name, Spr sprite, IModHelper helper, IPluginPackage<IModManifest> package) {
		return helper.Content.Cards.RegisterCard(name, new()
		{
			CardType = type,
			Meta = new()
			{
				deck = deck,
				rarity = rarity,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = dontOffer
			},
			Art = sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", charname, name, "name"]).Localize
		});
	}
}

internal interface IRegisterableArtifact
{
	private static Spr RegisterSpriteOrDefault(string path, Spr defaultSprite, IModHelper helper, IPluginPackage<IModManifest> package) {
		var file = package.PackageRoot.GetRelativeFile(path);
		if (file.Exists)
			return helper.Content.Sprites.RegisterSprite(file).Sprite;
		return defaultSprite;
	}

	static IArtifactEntry Register(Type type, Deck deck, string charname, ArtifactPool[] pools, IModHelper helper, IPluginPackage<IModManifest> package, out string name, bool unremovable = false) {
		name = type.Name[..^8];
		return Register(type, deck, charname, pools, unremovable, name, RegisterSpriteOrDefault($"Sprites/Artifacts/{charname}/{name}.png", StableSpr.artifacts_Crosslink, helper, package), helper, package);
	}
	static IArtifactEntry Register(Type type, Deck deck, string charname, ArtifactPool[] pools, IModHelper helper, IPluginPackage<IModManifest> package, out string name, out Spr activeSpr, out Spr inactiveSpr, bool unremovable = false) {
		name = type.Name[..^8];
		activeSpr = RegisterSpriteOrDefault($"Sprites/Artifacts/{charname}/{name}.png", StableSpr.artifacts_Crosslink, helper, package);
		inactiveSpr = RegisterSpriteOrDefault($"Sprites/Artifacts/{charname}/{name}Disabled.png", activeSpr, helper, package);
		return Register(type, deck, charname, pools, unremovable, name, activeSpr, helper, package);
	}
	static abstract void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package);

	private static IArtifactEntry Register(Type type, Deck deck, string charname, ArtifactPool[] pools, bool unremovable, string name, Spr sprite, IModHelper helper, IPluginPackage<IModManifest> package) {
		return helper.Content.Artifacts.RegisterArtifact(name, new()
		{
			ArtifactType = type,
			Meta = new()
			{
				owner = deck,
				pools = pools,
				unremovable = unremovable
			},
			Sprite = sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", charname, name, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", charname, name, "description"]).Localize
		});
	}
}