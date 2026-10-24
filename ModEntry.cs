using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheJazMaster.Nibbs.Features;
using Shockah.Kokoro;
using Shockah.Dyna;
using TheJazMaster.Nibbs.Characters;

namespace TheJazMaster.Nibbs;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

    internal Harmony Harmony { get; }
	// internal readonly HookManager<INibbsApi.IHook> HookManager;

	internal NibbsManager NibbsManager { get; }
	internal IxManager IxManager { get; }

	internal INibbsApi Api { get; }
	internal IKokoroApi.IV2 KokoroApi { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi { get; }
	internal IJohnsonApi? JohnsonApi { get; }
	internal IDynaApi? DynaApi { get; }
	internal IEddieApi? EddieApi { get; }


	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		// HookManager = new(package.Manifest.UniqueName);

		Api = new ApiImplementation();
		MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties")!;
		DynaApi = helper.ModRegistry.GetApi<IDynaApi>("Shockah.Dyna");
		JohnsonApi = helper.ModRegistry.GetApi<IJohnsonApi>("Shockah.Johnson");
		EddieApi = helper.ModRegistry.GetApi<IEddieApi>("TheJazMaster.Eddie");
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

		NibbsManager = new NibbsManager(this, package, helper);
		IxManager = new IxManager(this, package, helper);
		_ = new StoryVarsManager();
		_ = new OnStunManager();

        Harmony.PatchAll();
    }


	internal IPlayableCharacterEntryV2 RegisterCharacter(string name, Color color, IReadOnlyList<Type> cardTypes, IReadOnlyList<Type> artifactTypes, StarterDeck starters, StarterDeck altStarters) {
		var borderSprite = Helper.Content.Sprites.RegisterSprite(Package.PackageRoot.GetRelativeFile($"Sprites/Characters/{name}/{name}CardBorder.png")).Sprite;
		var frameSprite = Helper.Content.Sprites.RegisterSprite(Package.PackageRoot.GetRelativeFile($"Sprites/Characters/{name}/{name}Frame.png")).Sprite;

		var deck = Helper.Content.Decks.RegisterDeck(name, new()
		{
			Definition = new() {
				color = color,
				titleColor = Colors.black
			},
			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = borderSprite,
			Name = AnyLocalizations.Bind(["character", name, "name"]).Localize
		}).Deck;

        foreach (var cardType in cardTypes)
			AccessTools.DeclaredMethod(cardType, nameof(IRegisterableCard.Register))?.Invoke(null, [deck, name, Helper, Package]);
		foreach (var artifactType in artifactTypes)
			AccessTools.DeclaredMethod(artifactType, nameof(IRegisterableCard.Register))?.Invoke(null, [deck, name, Helper, Package]);

		MoreDifficultiesApi?.RegisterAltStarters(deck, altStarters);

        return Helper.Content.Characters.V2.RegisterPlayableCharacter(name, new()
		{
			Deck = deck,
			Description = AnyLocalizations.Bind(["character", name, "description"]).Localize,
			BorderSprite = frameSprite,
			Starters = starters,
			NeutralAnimation = RegisterAnimation(deck, name, "neutral"),
			MiniAnimation = RegisterAnimation(deck, name, "mini"),
		});
	}

	internal CharacterAnimationConfigurationV2 RegisterAnimation(Deck deck, string charname, string name) =>
		Helper.Content.Characters.V2.RegisterCharacterAnimation(charname + "_" + name, new()
		{
			CharacterType = deck.Key(),
			LoopTag = name,
			Frames = Enumerable.Range(1, 100)
				.Select(i => Package.PackageRoot.GetRelativeFile($"Sprites/Characters/{charname}/{name}/{charname}_{name}_{i}.png"))
				.TakeWhile(f => f.Exists)
				.Select(f => Helper.Content.Sprites.RegisterSprite(f).Sprite)
				.ToList()
        }).Configuration;

	public override object? GetApi(IModManifest requestingMod)
	{
		return Api;
	}
}







































