using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Nanoray.PluginManager;
using Nickel;
using TheJazMaster.Nibbs.Actions;

namespace TheJazMaster.Nibbs.Artifacts;

internal sealed class ChocolateSurpriseEggArtifact : Artifact, IRegisterableArtifact
{
	public bool active = false;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.EventOnly], helper, package, out _);
	}

	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		if (status == Status.heat && statusAmount > 0) {
			active = true;
			Pulse();
		}
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		int amt = state.ship.Get(Status.heat);
		if (active && amt > 0)
			combat.QueueImmediate(new AStatus {
				status = Status.tempShield,
				statusAmount = amt,
				targetPlayer = true,
				artifactPulse = Key()
			});
		active = false;
	}
}


internal sealed class DragonfireCandleArtifact : Artifact, IRegisterableArtifact, IOnStunArtifact
{

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		[new TTGlossary("action.stun"), new TTGlossary("action.stunShip")];


	public void OnStun(State state, Combat combat, IOnStunArtifact.StunType type, Intent? intent = null)
	{
		combat.QueueImmediate(new AHurt {
			targetPlayer = false,
			hurtAmount = 1,
			artifactPulse = Key(),
			dialogueSelector = ".OnDragonfireActivation"
		});
	}
}


internal sealed class EyeOfCobaArtifact : Artifact, IRegisterableArtifact
{
	public bool active = true;

	public static Spr activeSprite;
	public static Spr inactiveSprite;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _, out activeSprite, out inactiveSprite);
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		new ABacktrackMove{ dir = 1, directionlessTooltip = true }.GetTooltips(DB.fakeState);

	public override void OnTurnStart(State state, Combat combat)
	{
		active = true;
	}

	public override void OnCombatEnd(State state)
	{
		active = true;
	}

	public override Spr GetSprite()
	{
		return active ? activeSprite : inactiveSprite;
	}
}


internal sealed class GalacticNewsCoverageArtifact : Artifact, IRegisterableArtifact
{
	public bool active = false;
	private static readonly int threshold = 5;
	public static Spr activeSprite;
	public static Spr inactiveSprite;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _, out activeSprite, out inactiveSprite);
	}

	public override Spr GetSprite()
	{
		return !active ? activeSprite : inactiveSprite;
	}

	public override void OnCombatEnd(State state)
	{
		active = false;
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		StatusMeta.GetTooltips(ModEntry.Instance.BackflipStatus, threshold);

	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		Status backflip = ModEntry.Instance.BackflipStatus;
		if (status != backflip) return;
		if (!active) {
			if (state.ship.Get(backflip) >= threshold) {
				active = true;
				combat.QueueImmediate(new AStatus {
					status = Status.tempShield,
					statusAmount = state.ship.Get(backflip) - threshold,
					targetPlayer = true
				});
				combat.QueueImmediate(new AStatus {
					status = backflip,
					statusAmount = 0,
					mode = AStatusMode.Set,
					targetPlayer = true
				});
				combat.QueueImmediate(new AStatus {
					status = Status.tableFlip,
					statusAmount = 1,
					targetPlayer = true
				});
				combat.QueueImmediate(new AStatus {
					status = backflip,
					statusAmount = -threshold,
					targetPlayer = true,
					artifactPulse = Key()
				});
			}
		} else {
		}
	}

	public override void OnQueueEmptyDuringPlayerTurn(State state, Combat combat)
	{
		Status backflip = ModEntry.Instance.BackflipStatus;
		if (active && state.ship.Get(backflip) > 0) {
			combat.QueueImmediate(new AStatus {
				status = Status.tempShield,
				statusAmount = state.ship.Get(backflip),
				targetPlayer = true,
			});
			combat.QueueImmediate(new AStatus {
				status = backflip,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true,
				artifactPulse = Key()
			});
			combat.QueueImmediate(new ADelay());
		}
	}
}


internal sealed class EternalFlameArtifact : Artifact, IRegisterableArtifact
{
	public int count = 0;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

	public override int? GetDisplayNumber(State s)
	{
		return count;
	}

	public override List<Tooltip>? GetExtraTooltips() => StatusMeta.GetTooltips(Status.timeStop, 1);


	public override void OnTurnStart(State state, Combat combat)
	{
		count++;
		if (count == 5)
		{
			count = 0;
			combat.QueueImmediate(new AStatus
			{
				targetPlayer = true,
				status = Status.timeStop,
				statusAmount = 1,
				whoDidThis = ModEntry.Instance.NibbsDeck,
				artifactPulse = Key()
			});
		}
	}
}


internal sealed class QuantumEnginesArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
	}

	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		if (statusAmount <= 0 || mode != AStatusMode.Add) return;

		if (status == ModEntry.Instance.BacktrackLeftStatus ||
			status == ModEntry.Instance.BacktrackRightStatus ||
			status == ModEntry.Instance.BacktrackAutododgeLeftStatus ||
			status == ModEntry.Instance.BacktrackAutododgeRightStatus ||
			status == Status.autododgeLeft ||
			status == Status.autododgeRight)
		{
			state.ship.Add(status, 1);
			Pulse();
		}
	}
}


internal sealed class SugarRushArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
	}

	public override void OnReceiveArtifact(State state)
	{
		state.ship.baseEnergy += 1;
		state.ship.baseDraw += 1;
	}

	public override void OnRemoveArtifact(State state)
	{
		state.ship.baseEnergy -= 1;
		state.ship.baseDraw -= 1;
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		combat.Queue(new ABacktrackMove {
			dir = 1,
			isRandom = true,
			targetPlayer = true,
			artifactPulse = Key()
		});
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		new ABacktrackMove{ isRandom = true, dir = 1 }.GetTooltips(DB.fakeState);
}


internal sealed class FledgelingOrbArtifact : Artifact, IHeatTriggerAffectorArtifact, IRegisterableArtifact
{
	public bool active = false;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [], helper, package, out _, true);
	}

	public int ModifyHeatTriggerTooltip(G g, Ship ship)
	{
		return (active && ship.isPlayerShip) ? -aLot : 0;
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		Update(state.ship);
	}

	public override void OnReceiveArtifact(State state)
	{
		Update(state.ship);
	}

	public override void OnRemoveArtifact(State state)
	{
		if (active) {
			active = false;
			state.ship.heatTrigger -= aLot;
		}
	}

	internal static readonly int aLot = 999999;
	public void Update(Ship ship)
	{
		int statusAmount = ship.Get(Status.timeStop);
		if (ship.heatTrigger < aLot/2 && statusAmount > 0) {
			active = true;
			ship.heatTrigger += aLot;
		}
		else if (ship.heatTrigger > aLot/2 && statusAmount <= 0) {
			active = false;
			ship.heatTrigger -= aLot;
		}

		if (active && ship.heatTrigger < aLot/2) ship.heatTrigger += aLot;
		if (!active && ship.heatTrigger > aLot*1.5) ship.heatTrigger -= aLot;
	}
	public override List<Tooltip>? GetExtraTooltips() =>
		[
			.. StatusMeta.GetTooltips(Status.timeStop, 1),
			.. StatusMeta.GetTooltips(Status.heat, 3),
			.. StatusMeta.GetTooltips(Status.strafe, 1),
		];
}