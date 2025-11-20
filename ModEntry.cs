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

namespace TheJazMaster.Nibbs;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

    internal Harmony Harmony { get; }
	// internal readonly HookManager<INibbsApi.IHook> HookManager;

	internal INibbsApi NibbsApi { get; }
	internal IKokoroApi.IV2 KokoroApi { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi { get; }
	internal IJohnsonApi? JohnsonApi { get; }
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
	internal Spr OmniPrismIcon { get; }
	internal Spr FlipIcon { get; }
	internal Spr FlipLeftIcon { get; }
	internal Spr FlipRightIcon { get; }

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

		typeof(EqualityCard),

		typeof(RighteousShotCard),
		typeof(GreenEnergyCard),
		typeof(MakePeaceCard),
		typeof(NaturesShieldCard),

		typeof(InnerPeaceCard),
		typeof(HardenCard),
		typeof(SabotageCard),
		typeof(FocusFireCard),
		typeof(WeighPerspectivesCard),

		typeof(MartyrCard),
		typeof(BalanceCard),
		typeof(SabotageCard),
		typeof(VindicateCard),
		typeof(EyeForAnEyeCard),
		typeof(FaultlessCard),
		typeof(WeighPerspectivesCard),

		typeof(MartyrCard),
		typeof(BalanceCard),
		typeof(CrystalizeCard),
		typeof(DemonstrateCard),
		typeof(CrackdownCard),
		typeof(HardAsDiamondCard),
		typeof(RetaliateCard),

		typeof(ChakraAlignerArtifact),

		typeof(CorrectiveLensesArtifact),
		typeof(HealingCrystalsArtifact),

		typeof(CorrectiveLensesArtifact),
		typeof(HealingCrystalsArtifact),
		// typeof(ConservationEffortArtifact),
		// typeof(SolarTreeArtifact),
		// typeof(GemOrbitArtifact),
		// typeof(ManifestoArtifact),
		// typeof(MultiFacetedArtifact),

		// typeof(ThoriteArtifact),
		// typeof(FilterArtifact),

		// typeof(DiamondCubicArtifact),
		typeof(ConservationEffortArtifact)

		// typeof(DiamondCubicArtifact),
		typeof(ConservationEffortArtifact)
		// typeof(ConservationEffortArtifact),
		// typeof(SolarTreeArtifact),
		// typeof(GemOrbitArtifact),
		// typeof(ManifestoArtifact),
		// typeof(MultiFacetedArtifact),

		// typeof(ThoriteArtifact),
		// typeof(FilterArtifact),
	internal static IReadOnlyList<Type> IxArtifactTypes { get; } = [
		typeof(DiamondCubicArtifact),

		typeof(ConservationEffortArtifact),
		typeof(GemOrbitArtifact),
		typeof(ManifestoArtifact),
		typeof(MultiFacetedArtifact),

		typeof(ThoriteArtifact),
		typeof(FilterArtifact),
		// typeof(ManifestoOldArtifact),
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

		NibbsFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Characters/Nibbs/Panel.png")).Sprite;
        NibbsCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Characters/Nibbs/CardBorder.png")).Sprite;

        BacktrackLeftStatus = helper.Content.Statuses.RegisterStatus("BacktrackLeft", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackLeft.png")).Sprite,
				color = new("a051ff"),
				affectedByTimestop = true,
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "BacktrackLeft", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "BacktrackLeft", "description"]).Localize
		}).Status;

        BacktrackRightStatus = helper.Content.Statuses.RegisterStatus("BacktrackRight", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackRight.png")).Sprite,
				color = new("a051ff"),
				affectedByTimestop = true,
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "BacktrackRight", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "BacktrackRight", "description"]).Localize
		}).Status;

        BacktrackAutododgeLeftStatus = helper.Content.Statuses.RegisterStatus("BacktrackAutododgeLeft", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackAutododgeLeft.png")).Sprite,
				color = new("c744ff"),
				affectedByTimestop = true,
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "BacktrackAutododgeLeft", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "BacktrackAutododgeLeft", "description"]).Localize,
			ShouldFlash = (State s, Combat c, Ship ship, Status status) =>
			{
				return true;
			}
		}).Status;

        BacktrackAutododgeRightStatus = helper.Content.Statuses.RegisterStatus("BacktrackAutododgeRight", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackAutododgeRight.png")).Sprite,
				color = new("c744ff"),
				affectedByTimestop = true,
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "BacktrackAutododgeRight", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "BacktrackAutododgeRight", "description"]).Localize,
			ShouldFlash = (State s, Combat c, Ship ship, Status status) =>
			{
				return true;
			}
		}).Status;

        SmokescreenStatus = helper.Content.Statuses.RegisterStatus("Smokescreen", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Smokescreen.png")).Sprite,
				color = new("c4c4c4"),
				affectedByTimestop = true,
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "Smokescreen", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Smokescreen", "description"]).Localize
		}).Status;

        BackflipStatus = helper.Content.Statuses.RegisterStatus("Backflip", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Backflip.png")).Sprite,
				color = new("ffc646"),
				affectedByTimestop = true,
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "Backflip", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Backflip", "description"]).Localize
		}).Status;



        MidShieldStatus = helper.Content.Statuses.RegisterStatus("MidShield", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/MidShield.png")).Sprite,
				color = new("21CE39"),
				isGood = true,
				affectedByTimestop = true
			},
			Name = AnyLocalizations.Bind(["status", "MidShield", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "MidShield", "description"]).Localize,
		}).Status;

        // FractureStatus = helper.Content.Statuses.RegisterStatus("Fracture", new()
		// {
		// 	Definition = new()
		// 	{
		// 		icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Fracture.png")).Sprite,
		// 		color = new("6A9B73"),
		// 		isGood = false,
		// 		affectedByTimestop = true
		// 	},
		// 	Name = AnyLocalizations.Bind(["status", "Fracture", "name"]).Localize,
		// 	Description = AnyLocalizations.Bind(["status", "Fracture", "description"]).Localize,
		// }).Status;

        PerseveranceStatus = helper.Content.Statuses.RegisterStatus("Perseverance", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Perseverance.png")).Sprite,
				color = new("6A9B73"),
				isGood = true,
				affectedByTimestop = true
			},
			Name = AnyLocalizations.Bind(["status", "Perseverance", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Perseverance", "description"]).Localize,
		}).Status;

		BacktrackMoveLeftIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackMoveLeft.png")).Sprite;
		BacktrackMoveRightIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackMoveRight.png")).Sprite;
		BacktrackMoveRandomIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackMoveRandom.png")).Sprite;
		PrismIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Prism.png")).Sprite;
		PerfectPrismIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/PerfectPrism.png")).Sprite;
		MirrorIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Mirror.png")).Sprite;
		FlipIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Flip.png")).Sprite;
		FlipLeftIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/FlipLeft.png")).Sprite;
		FlipRightIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/FlipRight.png")).Sprite;

		// ChippingSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Chipping.png")).Sprite;
		
		PrismSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Prism.png")).Sprite;
		PerfectPrismSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/PerfectPrism.png")).Sprite;
		MirrorSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Mirror.png")).Sprite;

		_ = new StatusManager();
		_ = new SafetyShieldManager();
		_ = new BacktrackManager();
		// _ = new FractureManager();
		// _ = new ChippingManager();
		_ = new AffectDamageDoneManager();
		_ = new PrismManager();
		_ = new AngledAttacksManager();
		_ = new FluxManager();
		// _ = new ToughManager();
		CardPatches.Apply();
		AMovePatches.Apply();
		AMissileHitPatches.Apply();
		AStunPatches.Apply();
		AAttackPatches.Apply();
		CharacterPatches.Apply();
		AStatusPatches.Apply();
		ShipPatches.Apply();
		CombatPatches.Apply();
		StoryVarsPatches.Apply();
		StoryNodePatches.Apply();
		ScriptCtxPatches.Apply();

		Helper.Events.OnModLoadPhaseFinished += (_, phase) => {
			if (phase == ModLoadPhase.AfterDbInit) {
				new CombatDialogue().Inject();
				new EventDialogue().Inject();
				new SwitchInjections().Inject();
			}
		};
		Helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnEnemyDodgePlayerAttackByOneTile), (State state, Combat combat) => {
			state.storyVars.ApplyModData(StoryVarsPatches.JustGrazedKey, true);
		}, 0);
		
        NibbsCharacter = RegisterCharacter("Nibbs", new Color("c74b9b"), NibbsCardTypes, NibbsArtifactTypes, 
			new StarterDeck {
				cards = [
					new SmeltCard(),
					new TrailblazerCard()
				],
				artifacts = [
					new FledgelingOrbArtifact()
				]
			},
			new StarterDeck {
				cards = {
					new WingsOfFireCard(),
					new WormholeSurfingCard()
				},
				artifacts = {
					new FledgelingOrbArtifact()
				}
			}
		);
		NibbsDeck = NibbsCharacter.Configuration.Deck;
		
        IxCharacter = RegisterCharacter("Ix", new Color("22cf3a"), IxCardTypes, IxArtifactTypes, 
			new StarterDeck {
				cards = [
					new PlatitudesCard(),
					new SpacePrismCard()
				]
			},
			new StarterDeck {
				cards = {
					new MakePeaceCard(),
					new SpaceMirrorCard()
				}
			}
		);
		IxDeck = IxCharacter.Configuration.Deck;

		RegisterAnimation(NibbsDeck, "Nibbs", "squint");
		RegisterAnimation(NibbsDeck, "Nibbs", "gameover");
		RegisterAnimation(NibbsDeck, "Nibbs", "cheeky");
		RegisterAnimation(NibbsDeck, "Nibbs", "happy");
		RegisterAnimation(NibbsDeck, "Nibbs", "wowza");
		RegisterAnimation(NibbsDeck, "Nibbs", "serious");

        Harmony.PatchAll();
    }


	public IPlayableCharacterEntryV2 RegisterCharacter(string name, Color color, IReadOnlyList<Type> cardTypes, IReadOnlyList<Type> artifactTypes, StarterDeck starters, StarterDeck altStarters) {
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

	private CharacterAnimationConfigurationV2 RegisterAnimation(Deck deck, string charname, string name) =>
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
		return NibbsApi;
	}
}