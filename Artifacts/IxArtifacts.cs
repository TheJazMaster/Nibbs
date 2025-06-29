using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Cards;
using TheJazMaster.Nibbs.Features;

namespace TheJazMaster.Nibbs.Artifacts;


internal sealed class DiamondCubicArtifact : Artifact, IRegisterableArtifact, INibbsApi.IHook
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.EventOnly], helper, package, out _, true);
	}

	public void AffectDamageDone(INibbsApi.IHook.IAffectDamageDoneArgs args) {
		if (args.AttackContext != null) {
			if (!args.Piercing && !args.Ship.isPlayerShip && args.AttackContext.paybackCounter > 0) {
				args.Piercing = true;
				Pulse();
				return;
			}
		}

		if (args.Ship.isPlayerShip) {
			if (args.Piercing) {
				args.Piercing = false;
				Pulse();
			}
			if (args.AttackContext != null) {
				args.AttackContext.weaken = false;
				args.AttackContext.brittle = false;
				args.AttackContext.armorize = false;
			}
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("action.attackPiercing"),
		.. StatusMeta.GetTooltips(Status.payback, 1)
	];
}


internal sealed class MultiFacetedArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		combat.Queue(new AStatus {
			status = ModEntry.Instance.PerseveranceStatus,
			statusAmount = 1,
			targetPlayer = true,
			artifactPulse = Key()
		});
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		combat.Queue(new AStatus {
			status = ModEntry.Instance.PerseveranceStatus,
			statusAmount = -1,
			targetPlayer = true
		});
	}

	public override List<Tooltip>? GetExtraTooltips() => StatusMeta.GetTooltips(ModEntry.Instance.PerseveranceStatus, 1);
}


internal sealed class ConservationEffortArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnReceiveArtifact(State state)
	{
		(state.GetDialogue()?.actionQueue ?? state.GetCurrentQueue()).Queue(new AInsertPart {
			targetPlayer = true,
			x = state.rngActions.NextInt() % (state.ship.parts.Count - 1) + 1,
			part = new Part {
				type = PType.special,
				skin = "crystal_1",
				invincible = true
			}
		});
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("parttrait.invincible")
	];
}


internal sealed class GemOrbitArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		List<int> list = [];
		for (int i = state.ship.x - 1; i < state.ship.x + state.ship.parts.Count + 1; i++)
		{
			if (!combat.stuff.ContainsKey(i))
			{
				list.Add(i);
			}
		}
		List<int> positions = list.Shuffle(state.rngActions).Take(3).ToList();
		List<int> prisms = new List<int> {1, 2, 3}.Shuffle(state.rngActions).ToList();
		for (int i = 0; i < positions.Count; i++)
		{
			int x = positions[i];
			StuffBase item = prisms[i] switch {
				1 => new PrismManager.RegularPrism {
					targetPlayer = true
				},
				2 => new PrismManager.RegularPrism {
					targetPlayer = false
				},
				_ => new PrismManager.OmniPrism()
			};
			item.x = x;
			combat.stuff.Add(x, item);
		}
		if (positions.Count > 0)
		{
			Pulse();
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. new PrismManager.RegularPrism().GetTooltips(),
		.. new PrismManager.OmniPrism().GetTooltips(),
	];
}


internal sealed class ManifestoArtifact : Artifact, IRegisterableArtifact, INibbsApi.IHook
{
	private static IModData ModData => ModEntry.Instance.Helper.ModData;
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

	public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer) =>
		(fromPlayer && state.ship.Get(ModEntry.Instance.FractureStatus) >= 3) ? 1 : 0;
	
	public override List<Tooltip>? GetExtraTooltips() => StatusMeta.GetTooltips(ModEntry.Instance.FractureStatus, 3);
}


internal sealed class ThoriteArtifact : Artifact, IRegisterableArtifact, INibbsApi.IHook
{
	private static IModData ModData => ModEntry.Instance.Helper.ModData;
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		foreach (Part part in combat.otherShip.parts) {
			if (part.type == PType.empty) continue;

			ModData.SetModData(part, Key(), part.damageModifier);
			ModData.SetModData(part, Key() + "Hidden", part.brittleIsHidden);
			part.damageModifier = PDamMod.brittle;
			part.brittleIsHidden = false;
		}
		Pulse();
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		foreach (Part part in combat.otherShip.parts) {
			if (part.type == PType.empty) continue;

			var mod = ModData.GetOptionalModData<PDamMod>(part, Key());
			if (mod.HasValue) part.damageModifier = mod.Value;
			var hid = ModData.GetOptionalModData<bool>(part, Key() + "Hidden");
			if (hid.HasValue) part.brittleIsHidden = hid.Value;
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("parttrait.brittle")
	];
}


internal sealed class FilterArtifact : Artifact, IRegisterableArtifact, INibbsApi.IHook
{
	private static IModData ModData => ModEntry.Instance.Helper.ModData;
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
	}

	public void AffectDamageDone(INibbsApi.IHook.IAffectDamageDoneArgs args) {
		if (!args.Ship.isPlayerShip || args.AttackContext == null) return;

		if (ModData.GetModDataOrDefault(args.AttackContext, PrismManager.SidewaysAttackKey, PrismManager.Sideways.NONE) != PrismManager.Sideways.NONE) return;

		int refractions = ModData.GetModDataOrDefault(args.AttackContext, PrismManager.RefractKey, 0);
		if (refractions > 0) {
			args.Damage = Math.Max(0, args.Damage - 1);
			Pulse();
		}
	}
}







internal sealed class ManifestoOldArtifact : Artifact, IRegisterableArtifact, INibbsApi.IHook
{
	const int AMOUNT = 6;
	public int counter = AMOUNT;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
	}

	public void AffectDamageDone(INibbsApi.IHook.IAffectDamageDoneArgs args) {
		if (!args.Ship.isPlayerShip || !args.Combat.isPlayerTurn) return;
		
		int prev = Math.Min(counter, args.Damage);
		if (prev > 0) {
			args.Damage -= prev;
			counter -= prev;
			Pulse();
		}
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		counter = AMOUNT;
	}

	public override int? GetDisplayNumber(State s) => s.route is Combat ? counter : null;
}


internal sealed class SolarTreeOldArtifact : Artifact, IRegisterableArtifact
{
	public bool activateNextTurn;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		if (combat.energy > 0)
		{
			activateNextTurn = true;
		}
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (activateNextTurn)
		{
			activateNextTurn = false;
			combat.Queue(new AStatus
			{
				status = ModEntry.Instance.SafetyShieldStatus,
				statusAmount = 2,
				targetPlayer = true,
				dialogueSelector = ".solarTreeTrigger",
				artifactPulse = Key()
			});
		}
	}

	public override void OnCombatEnd(State state)
	{
		activateNextTurn = false;
	}

	public override List<Tooltip>? GetExtraTooltips() => StatusMeta.GetTooltips(ModEntry.Instance.FractureStatus, 1);
}


internal sealed class HealingCrystalsOldArtifact : Artifact, IRegisterableArtifact, INibbsApi.IHook
{
	public bool active = false;

	public static Spr activeSprite;
	public static Spr inactiveSprite;

	private static IModData ModData => ModEntry.Instance.Helper.ModData;
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _, out activeSprite, out inactiveSprite);
	}

	public void AffectDamageDone(INibbsApi.IHook.IAffectDamageDoneArgs args) {
		if (!active || !args.Ship.isPlayerShip || args.AttackContext == null) return;

		int refractions = ModData.GetModDataOrDefault(args.AttackContext, PrismManager.RefractKey, 0);
		if (refractions >= 2) {
			active = false;
			args.Combat.QueueImmediate(new AHeal {
				healAmount = 2,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override void OnCombatStart(State state, Combat combat) {
		active = true;
	}

	public override void OnCombatEnd(State state) {
		active = true;
	}

	public override Spr GetSprite()
	{
		return active ? activeSprite : inactiveSprite;
	}
}


// internal sealed class MultiFacetedOldArtifact : Artifact, IRegisterableArtifact
// {
// 	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
// 		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
// 	}

// 	public override void OnTurnStart(State state, Combat combat)
// 	{
//         if (state.ship.Get(ModEntry.Instance.FractureStatus) > 0)
// 			combat.Queue(new AAddCard {
// 				card = new DarkSideCard(),
// 				destination = CardDestination.Hand,
// 				artifactPulse = Key()
// 			});
// 	}

// 	public override List<Tooltip>? GetExtraTooltips() => [
// 		.. StatusMeta.GetTooltips(ModEntry.Instance.FractureStatus, 1),
// 		new TTCard {
// 			card = new DarkSideCard()
// 		}
// 	];
// }