
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using TheJazMaster.Nibbs.Cards;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Features;
using TheJazMaster.Nibbs.Features.Nibbs;
using TheJazMaster.Nibbs.Story;

namespace TheJazMaster.Nibbs.Characters;

public class NibbsManager
{
    internal IPlayableCharacterEntryV2 NibbsCharacter { get; }
    internal Deck NibbsDeck { get; }

	internal Status BackflipStatus { get; }
	internal Status SmokescreenStatus { get; }
	internal Status BacktrackLeftStatus { get; }
    internal Status BacktrackRightStatus { get; }
	internal Status BacktrackAutododgeLeftStatus { get; }
	internal Status BacktrackAutododgeRightStatus { get; }

    internal Spr NibbsFrame { get; }
    internal Spr NibbsCardBorder { get; }

    internal Spr BacktrackLeftIcon { get; }
	internal Spr BacktrackRightIcon { get; }
    internal Spr BacktrackMoveLeftIcon { get; }
	internal Spr BacktrackMoveRightIcon { get; }
	internal Spr BacktrackMoveRandomIcon { get; }

	

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

	public NibbsManager(ModEntry caller, IPluginPackage<IModManifest> package, IModHelper helper)
	{
		NibbsFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Characters/Nibbs/Panel.png")).Sprite;
        NibbsCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Characters/Nibbs/CardBorder.png")).Sprite;

		BacktrackLeftIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackLeft.png")).Sprite;
		BacktrackRightIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackRight.png")).Sprite;

        BacktrackLeftStatus = helper.Content.Statuses.RegisterStatus("BacktrackLeft", new()
		{
			Definition = new()
			{
				icon = BacktrackLeftIcon,
				color = new("a051ff"),
				affectedByTimestop = true,
				isGood = true
			},
			Name = caller.AnyLocalizations.Bind(["status", "BacktrackLeft", "name"]).Localize,
			Description = caller.AnyLocalizations.Bind(["status", "BacktrackLeft", "simple"]).Localize
		}).Status;

        BacktrackRightStatus = helper.Content.Statuses.RegisterStatus("BacktrackRight", new()
		{
			Definition = new()
			{
				icon = BacktrackRightIcon,
				color = new("a051ff"),
				affectedByTimestop = true,
				isGood = true
			},
			Name = caller.AnyLocalizations.Bind(["status", "BacktrackRight", "name"]).Localize,
			Description = caller.AnyLocalizations.Bind(["status", "BacktrackRight", "simple"]).Localize
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
			Name = caller.AnyLocalizations.Bind(["status", "BacktrackAutododgeLeft", "name"]).Localize,
			Description = caller.AnyLocalizations.Bind(["status", "BacktrackAutododgeLeft", "description"]).Localize,
			ShouldFlash = (s, c, ship, status) =>
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
			Name = caller.AnyLocalizations.Bind(["status", "BacktrackAutododgeRight", "name"]).Localize,
			Description = caller.AnyLocalizations.Bind(["status", "BacktrackAutododgeRight", "description"]).Localize,
			ShouldFlash = (s, c, ship, status) =>
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
			Name = caller.AnyLocalizations.Bind(["status", "Smokescreen", "name"]).Localize,
			Description = caller.AnyLocalizations.Bind(["status", "Smokescreen", "description"]).Localize
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
			Name = caller.AnyLocalizations.Bind(["status", "Backflip", "name"]).Localize,
			Description = caller.AnyLocalizations.Bind(["status", "Backflip", "description"]).Localize
		}).Status;
		
		BacktrackMoveLeftIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackMoveLeft.png")).Sprite;
		BacktrackMoveRightIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackMoveRight.png")).Sprite;
		BacktrackMoveRandomIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/BacktrackMoveRandom.png")).Sprite;
		
		_ = new NibbsStatusManager();
		_ = new AutododgeManager();
		_ = new BacktrackManager();
		_ = new BackflipManager();
		_ = new OverheatPreventionManager();

		helper.Events.OnModLoadPhaseFinished += (_, phase) => {
			if (phase == ModLoadPhase.AfterDbInit) {
				new CombatDialogue().Inject();
				new EventDialogue().Inject();
				new SwitchInjections().Inject();
			}
		};
		
        NibbsCharacter = caller.RegisterCharacter("Nibbs", new Color("c74b9b"), NibbsCardTypes, NibbsArtifactTypes, 
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

		caller.RegisterAnimation(NibbsDeck, "Nibbs", "squint");
		caller.RegisterAnimation(NibbsDeck, "Nibbs", "gameover");
		caller.RegisterAnimation(NibbsDeck, "Nibbs", "cheeky");
		caller.RegisterAnimation(NibbsDeck, "Nibbs", "happy");
		caller.RegisterAnimation(NibbsDeck, "Nibbs", "wowza");
		caller.RegisterAnimation(NibbsDeck, "Nibbs", "serious");
	}
}