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
using Shockah.Kokoro;
using Shockah.Dyna;
using TheJazMaster.Nibbs.Features.Ix;
using TheJazMaster.Nibbs.Characters;

namespace TheJazMaster.Nibbs.Characters;

public class IxManager
{

    internal IPlayableCharacterEntryV2 IxCharacter { get; }


    internal Deck IxDeck { get; }

	internal Spr PrismIcon { get; }
	internal Spr PerfectPrismIcon { get; }
	internal Spr MirrorIcon { get; }
	internal Spr MirrorIconFlipped { get; }

	internal Spr DestroyPrismsIcon { get; }
	
	internal Status MidShieldStatus { get; }
	internal Status PerseveranceStatus { get; }

	internal Spr PrismSprite { get; }
	internal Spr PerfectPrismSprite { get; }
	internal Spr MirrorSprite { get; }



	internal static IReadOnlyList<Type> IxCardTypes { get; } = [
		typeof(PlatitudesCard),
		typeof(MovingSentimentCard),
		typeof(SpaceMirrorCard),
		typeof(MakePeaceCard),
		typeof(EqualityCard),
		typeof(HardenCard),
		typeof(GreenEnergyCard),
		typeof(NaturesShieldCard),
		typeof(RighteousShotCard),

		typeof(PrismArrayCard),
		typeof(InnerPeaceCard),
		typeof(SabotageCard),
		typeof(VindicateCard),
		typeof(EyeForAnEyeCard),
		typeof(FaultlessCard),
		typeof(DetoxCard),

		typeof(MartyrCard),
		typeof(BalanceCard),
		typeof(WeighPerspectivesCard),
		typeof(FocusFireCard),
		typeof(RetaliateCard),

		typeof(SpacePrismCard),
	];

	internal static IReadOnlyList<Type> IxArtifactTypes { get; } = [
		typeof(ManifestoArtifact),
		typeof(CorrectiveLensesArtifact),
		typeof(RayleighScatteringArtifact),
		typeof(HealingCrystalsArtifact),

		typeof(PeaceSignArtifact),
		typeof(ConservationEffortArtifact)
	];

	public IxManager(ModEntry caller, IPluginPackage<IModManifest> package, IModHelper helper)
	{

        MidShieldStatus = helper.Content.Statuses.RegisterStatus("MidShield", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/MidShield.png")).Sprite,
				color = new("21CE39"),
				isGood = true,
				affectedByTimestop = true
			},
			Name = caller.AnyLocalizations.Bind(["status", "MidShield", "name"]).Localize,
			Description = caller.AnyLocalizations.Bind(["status", "MidShield", "description"]).Localize,
		}).Status;

        PerseveranceStatus = helper.Content.Statuses.RegisterStatus("Perseverance", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Perseverance.png")).Sprite,
				color = new("6A9B73"),
				isGood = true,
				affectedByTimestop = true
			},
			Name = caller.AnyLocalizations.Bind(["status", "Perseverance", "name"]).Localize,
			Description = caller.AnyLocalizations.Bind(["status", "Perseverance", "description"]).Localize,
		}).Status;

		
		
		PrismIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Prism.png")).Sprite;
		PerfectPrismIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/PerfectPrism.png")).Sprite;
		MirrorIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Mirror.png")).Sprite;
		MirrorIconFlipped = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/MirrorFlipped.png")).Sprite;
		
		DestroyPrismsIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/DestroyPrisms.png")).Sprite;
		
		PrismSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Prism.png")).Sprite;
		PerfectPrismSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/PerfectPrism.png")).Sprite;
		MirrorSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Mirror.png")).Sprite;

		_ = new IxStatusManager();
		_ = new AffectDamageDoneManager();
		_ = new AngledAttacksManager();
		_ = new GlowingEyesManager();
		_ = new FluxManager();
		_ = new MidShieldManager();
		_ = new PerseveranceManager();
		_ = new PrismManager();


        IxCharacter = caller.RegisterCharacter("Ix", new Color("22cf3a"), IxCardTypes, IxArtifactTypes, 
			new StarterDeck {
				cards = [
					new MovingSentimentCard(),
					new PlatitudesCard(),
					new SpacePrismCard()
				]
			},
			new StarterDeck {
				cards = {
					new MakePeaceCard(),
					new SpaceMirrorCard(),
				}
			}
		);
		IxDeck = IxCharacter.Configuration.Deck;
	}
}