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

namespace TheJazMaster.Nibbs;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

    internal Harmony Harmony { get; }
	internal IKokoroApi KokoroApi { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi { get; }
	internal IJohnsonApi? JohnsonApi { get; }
	internal IEddieApi? EddieApi { get; }


	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal IPlayableCharacterEntryV2 NibbsCharacter { get; }

    internal IDeckEntry NibbsDeck { get; }

	internal IStatusEntry BackflipStatus { get; }
	internal IStatusEntry SmokescreenStatus { get; }
	internal IStatusEntry BacktrackLeftStatus { get; }
    internal IStatusEntry BacktrackRightStatus { get; }
	internal IStatusEntry BacktrackAutododgeLeftStatus { get; }
	internal IStatusEntry BacktrackAutododgeRightStatus { get; }

    internal ISpriteEntry NibbsPortrait { get; }
    internal ISpriteEntry NibbsPortraitMini { get; }
    internal ISpriteEntry NibbsFrame { get; }
    internal ISpriteEntry NibbsCardBorder { get; }

    internal ISpriteEntry BacktrackMoveLeftIcon { get; }
	internal ISpriteEntry BacktrackMoveRightIcon { get; }
	internal ISpriteEntry BacktrackMoveRandomIcon { get; }

	// internal static IReadOnlyList<Type> CommonCardTypes { get; } = [
	// 	typeof(FireballCard),
	// 	typeof(BlazingPathCard),
	// 	typeof(SpaceHoppingCard),
	// 	typeof(WingsOfFireCard),
	// 	typeof(WormholeSurfingCard),
	// 	typeof(OverHereCard),
	// 	typeof(HotPursuitCard),
	// 	typeof(BackflipCard),
    //     typeof(SteamCoverCard),
	// ];

	// internal static IReadOnlyList<Type> UncommonCardTypes { get; } = [
	// 	typeof(QuantumTurbulenceCard),
	// 	typeof(FireBreathCard),
	// 	typeof(FlapFlapCard),
	// 	typeof(FluxCompressorCard),
	// 	typeof(HoldOnCard),
	// 	typeof(DimensionalJauntCard),
	// 	typeof(TauntCard),
	// ];

	// internal static IReadOnlyList<Type> RareCardTypes { get; } = [
	// 	typeof(DragonFrenzyCard),
	// 	typeof(SuperpositionCard),
	// 	typeof(CoolantPumpCard),
	// 	typeof(SmokescreenCard),
	// 	typeof(NovaCard),
	// ];

	internal static IReadOnlyList<Type> CommonCardTypes { get; } = [
		typeof(SmeltCard),
		typeof(WingsOfFireCard),
		typeof(SpaceHoppingCard),
		typeof(TrailblazerCard),
		typeof(WormholeSurfingCard),
		typeof(OverHereCard),
		typeof(HotPursuitCard),
		typeof(BackflipCard),
        typeof(SmokescreenCard),
	];

	internal static IReadOnlyList<Type> UncommonCardTypes { get; } = [
		typeof(QuantumTurbulenceCard),
		typeof(HydraulicsCard),
		typeof(FlapFlapCard),
		typeof(FluxCompressorCard),
		typeof(HoldOnCard),
		typeof(DimensionalJauntCard),
		typeof(TauntCard),
	];

	internal static IReadOnlyList<Type> RareCardTypes { get; } = [
		typeof(DragonFrenzyCard),
		typeof(SuperpositionCard),
		typeof(QuantumCollapseCard),
		typeof(BlurCard),
		typeof(NovaCard),
	];


	internal static IReadOnlyList<Type> SecretCardTypes { get; } = [
		typeof(HopCard),
		typeof(SkipCard),
		typeof(NibbsExeCard),
	];

    internal static IEnumerable<Type> AllCardTypes
		=> CommonCardTypes
			.Concat(UncommonCardTypes)
			.Concat(RareCardTypes)
			.Concat(SecretCardTypes);

    internal static IReadOnlyList<Type> CommonArtifacts { get; } = [
		typeof(DragonfireCandleArtifact),
		typeof(EyeOfCobaArtifact),
		typeof(GalacticNewsCoverageArtifact),
		typeof(EternalFlameArtifact),
	];

	internal static IReadOnlyList<Type> BossArtifacts { get; } = [
		typeof(QuantumEnginesArtifact),
		typeof(SugarRushArtifact),
	];

	internal static IReadOnlyList<Type> StarterArtifacts { get; } = [
		typeof(FledgelingOrbArtifact),
	];

	internal static IEnumerable<Type> AllArtifactTypes
		=> CommonArtifacts.Concat(BossArtifacts).Concat(StarterArtifacts);

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties")!;
		JohnsonApi = helper.ModRegistry.GetApi<IJohnsonApi>("Shockah.Johnson")!;
		EddieApi = helper.ModRegistry.GetApi<IEddieApi>("TheJazMaster.Eddie")!;
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

        NibbsPortrait = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Nibbs_neutral_0.png"));
        NibbsPortraitMini = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Nibbs_mini.png"));
		NibbsFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Panel.png"));
        NibbsCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/CardBorder.png"));

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
		});

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
		});

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
		});

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
		});

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
		});

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
		});

		BacktrackMoveLeftIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackMoveLeft.png"));
		BacktrackMoveRightIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackMoveRight.png"));
		BacktrackMoveRandomIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackMoveRandom.png"));

		NibbsDeck = helper.Content.Decks.RegisterDeck("Nibbs", new()
		{
			Definition = new() { color = new Color("c74b9b"), titleColor = Colors.black },
			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = NibbsCardBorder.Sprite,
			Name = AnyLocalizations.Bind(["character", "name"]).Localize
		});

        foreach (var cardType in AllCardTypes)
			AccessTools.DeclaredMethod(cardType, nameof(INibbsCard.Register))?.Invoke(null, [helper]);
		foreach (var artifactType in AllArtifactTypes)
			AccessTools.DeclaredMethod(artifactType, nameof(INibbsArtifact.Register))?.Invoke(null, [helper]);

		MoreDifficultiesApi?.RegisterAltStarters(NibbsDeck.Deck, new StarterDeck {
            cards = {
				new WingsOfFireCard(),
				new WormholeSurfingCard()
            },
			artifacts = {
				new FledgelingOrbArtifact()
			}
        });
		
		_ = new StatusManager();
		_ = new BacktrackManager();
		CustomTTGlossary.ApplyPatches(Harmony);
		CardPatches.Apply();
		AMovePatches.Apply();
		AStunPatches.Apply();
		AAttackPatches.Apply();
		AStatusPatches.Apply();
		ShipPatches.Apply();
		CombatPatches.Apply();
		// ScriptCtxPatches.Apply();

		Helper.Events.OnModLoadPhaseFinished += (_, phase) => {
			if (phase == ModLoadPhase.AfterDbInit) {
				new CombatDialogue().Inject();
				new EventDialogue().Inject();
				new SwitchInjections().Inject();
			}
		};
		
        NibbsCharacter = helper.Content.Characters.V2.RegisterPlayableCharacter("Nibbs", new()
		{
			Deck = NibbsDeck.Deck,
			Description = AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = NibbsFrame.Sprite,
			Starters = new StarterDeck {
				// cards = [ new FireballCard(), new BlazingPathCard() ],
				cards = [ new SmeltCard(), new TrailblazerCard() ],
				artifacts = [ new FledgelingOrbArtifact() ]
			},
			ExeCardType = typeof(NibbsExeCard),
			NeutralAnimation = new()
			{
				CharacterType = NibbsDeck.Deck.Key(),
				LoopTag = "neutral",
				Frames = [
					NibbsPortrait.Sprite
				]
			},
			MiniAnimation = new()
			{
				CharacterType = NibbsDeck.Deck.Key(),
				LoopTag = "mini",
				Frames = [
					NibbsPortraitMini.Sprite
				]
			}
		});

		RegisterAnimation(helper, "Neutral");
		RegisterAnimation(helper, "Squint");
		RegisterAnimation(helper, "Gameover");
		RegisterAnimation(helper, "Mini");
		RegisterAnimation(helper, "Cheeky");
		RegisterAnimation(helper, "Happy");
		RegisterAnimation(helper, "Wowza");
		RegisterAnimation(helper, "Serious");
    }

	private void RegisterAnimation(IModHelper helper, string name)
    {
        var files = Instance.Package.PackageRoot.GetRelative($"Sprites/Character/{name}").AsDirectory?.GetFilesRecursively().Where(f => f.Name.EndsWith(".png"));
		List<Spr> sprites = [];
		if (files != null) {
			foreach (IFileInfo file in files) {
				sprites.Add(Instance.Helper.Content.Sprites.RegisterSprite(file).Sprite);
			}
		}
		
		helper.Content.Characters.V2.RegisterCharacterAnimation(name, new()
		{
			CharacterType = NibbsDeck.Deck.Key(),
			LoopTag = name.ToLower(),
			Frames = sprites
		});
    }
}