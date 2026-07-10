using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheJazMaster.Nibbs.Cards;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Features;
using TheJazMaster.Nibbs.Patches;
using Shockah.Kokoro;
using Shockah.Dyna;

namespace TheJazMaster.Nibbs;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

    internal Harmony Harmony { get; }
	// internal readonly HookManager<INibbsApi.IHook> HookManager;

	internal INibbsApi NibbsApi { get; }
	internal IKokoroApi.IV2 KokoroApi { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi { get; }
	internal IJohnsonApi? JohnsonApi { get; }
	internal IDynaApi? DynaApi { get; }
	internal IEddieApi? EddieApi { get; }


	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal IPlayableCharacterEntryV2 NibbsCharacter { get; }
    internal IPlayableCharacterEntryV2 IxCharacter { get; }

    internal Deck NibbsDeck { get; }

    internal Deck IxDeck { get; }

	internal Status BackflipStatus { get; }
	internal Status SmokescreenStatus { get; }
	internal Status BacktrackLeftStatus { get; }
    internal Status BacktrackRightStatus { get; }
	internal Status BacktrackAutododgeLeftStatus { get; }
	internal Status BacktrackAutododgeRightStatus { get; }
	internal Status MidShieldStatus { get; }
	// internal Status FractureStatus { get; }
	internal Status PerseveranceStatus { get; }

    internal Spr NibbsFrame { get; }
    internal Spr NibbsCardBorder { get; }

    internal Spr BacktrackMoveLeftIcon { get; }
	internal Spr BacktrackMoveRightIcon { get; }
	internal Spr BacktrackMoveRandomIcon { get; }
	internal Spr PrismIcon { get; }
	internal Spr PerfectPrismIcon { get; }
	internal Spr MirrorIcon { get; }
	internal Spr FlipIcon { get; }
	internal Spr FlipLeftIcon { get; }
	internal Spr FlipRightIcon { get; }

	internal Spr ChippingSprite { get; }
	internal Spr PrismSprite { get; }
	internal Spr PerfectPrismSprite { get; }
	internal Spr MirrorSprite { get; }
	

	internal static IReadOnlyList<Type> NibbsCardTypes { get; } = [
		typeof(SmeltCard),
		typeof(WingsOfFireCard),
		typeof(SpaceHoppingCard),
		typeof(TrailblazerCard),
		typeof(WormholeSurfingCard),
		typeof(OverHereCard),
		typeof(HotPursuitCard),
		typeof(BackflipCard),
        typeof(SmokescreenCard),
		
		typeof(QuantumTurbulenceCard),
		typeof(HydraulicsCard),
		typeof(FlapFlapCard),
		typeof(FluxCompressorCard),
		typeof(HoldOnCard),
		typeof(DimensionalJauntCard),
		typeof(TauntCard),

		typeof(DragonFrenzyCard),
		typeof(SuperpositionCard),
		typeof(QuantumCollapseCard),
		typeof(BlurCard),
		typeof(NovaCard),
		
		typeof(HopCard),
		typeof(SkipCard),
	];

	internal static IReadOnlyList<Type> ExeCardTypes = [
		typeof(NibbsExeCard),
	];


    internal static IReadOnlyList<Type> NibbsArtifactTypes { get; } = [
		typeof(DragonfireCandleArtifact),
		typeof(EyeOfCobaArtifact),
		typeof(GalacticNewsCoverageArtifact),
		typeof(EternalFlameArtifact),
		
		typeof(QuantumEnginesArtifact),
		typeof(SugarRushArtifact),
		
		typeof(FledgelingOrbArtifact),
	];



	internal static IReadOnlyList<Type> IxCardTypes { get; } = [
		typeof(PlatitudesCard),
		typeof(SpacePrismCard),
		
		typeof(SpaceMirrorCard),
		typeof(MakePeaceCard),

		typeof(PrismArrayCard),
		typeof(EqualityCard),
		typeof(RighteousShotCard),
		typeof(GreenEnergyCard),
		typeof(NaturesShieldCard),

		typeof(InnerPeaceCard),
		typeof(HardenCard),
		typeof(SabotageCard),
		typeof(VindicateCard),
		typeof(EyeForAnEyeCard),
		typeof(FaultlessCard),
		typeof(WeighPerspectivesCard),

		typeof(MartyrCard),
		typeof(BalanceCard),
		typeof(CrystalizeCard),
		typeof(FocusFireCard),
		typeof(RetaliateCard),
	];

	internal static IReadOnlyList<Type> IxArtifactTypes { get; } = [
		typeof(ChakraAlignerArtifact),
		typeof(ManifestoArtifact),
		typeof(CorrectiveLensesArtifact),
		typeof(HealingCrystalsArtifact),

		typeof(PeaceSignArtifact),
		typeof(ConservationEffortArtifact)

		// typeof(DiamondCubicArtifact),

		// typeof(ConservationEffortArtifact),
		// typeof(SolarTreeArtifact),
		// typeof(GemOrbitArtifact),
		// typeof(ManifestoArtifact),
		// typeof(MultiFacetedArtifact),

		// typeof(ThoriteArtifact),
		// typeof(FilterArtifact),
	];

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		// HookManager = new(package.Manifest.UniqueName);

		NibbsApi = new ApiImplementation();
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