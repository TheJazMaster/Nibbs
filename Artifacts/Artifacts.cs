using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Nickel;
using TheJazMaster.Nibbs.Actions;

namespace TheJazMaster.Nibbs.Artifacts;

internal sealed class ChocolateSurpriseEggArtifact : Artifact, INibbsArtifact
{
	public bool active = false;

	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("ChocolateSurpriseEgg", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/ChocolateSurpriseEgg.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ChocolateSurpriseEgg", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ChocolateSurpriseEgg", "description"]).Localize
		});
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


internal sealed class DragonfireCandleArtifact : Artifact, INibbsArtifact, IOnStunArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("DragonfireCandle", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/DragonfireCandle.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "DragonfireCandle", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "DragonfireCandle", "description"]).Localize
		});
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


internal sealed class EyeOfCobaArtifact : Artifact, INibbsArtifact
{
	public bool active = true;

	public static Spr ActiveSprite;
	public static Spr InactiveSprite;

	public static void Register(IModHelper helper)
	{
		ActiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/EyeOfCoba.png")).Sprite;
		InactiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/EyeOfCobaInactive.png")).Sprite;
		helper.Content.Artifacts.RegisterArtifact("EyeOfCoba", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = ActiveSprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "EyeOfCoba", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "EyeOfCoba", "description"]).Localize
		});
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
		return active ? ActiveSprite : InactiveSprite;
	}
}


internal sealed class GalacticNewsCoverageArtifact : Artifact, INibbsArtifact
{
	public bool active = false;
	private static readonly int threshold = 5;
	public static Spr ActiveSprite;
	public static Spr InactiveSprite;

	public static void Register(IModHelper helper)
	{
		ActiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/GalacticNewsCoverage.png")).Sprite;
		InactiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/GalacticNewsCoverageInactive.png")).Sprite;
		helper.Content.Artifacts.RegisterArtifact("GalacticNewsCoverage", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = ActiveSprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "GalacticNewsCoverage", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "GalacticNewsCoverage", "description"], new { Amount = threshold }).Localize
		});
	}

	public override Spr GetSprite()
	{
		return !active ? ActiveSprite : InactiveSprite;
	}

	public override void OnCombatEnd(State state)
	{
		active = false;
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		StatusMeta.GetTooltips(ModEntry.Instance.BackflipStatus.Status, threshold);

	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		Status backflip = ModEntry.Instance.BackflipStatus.Status;
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
		Status backflip = ModEntry.Instance.BackflipStatus.Status;
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


internal sealed class EternalFlameArtifact : Artifact, INibbsArtifact
{
	public int count = 0;

	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("EternalFlame", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/EternalFlame.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "EternalFlame", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "EternalFlame", "description"]).Localize
		});
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
				whoDidThis = ModEntry.Instance.NibbsDeck.Deck,
				artifactPulse = Key()
			});
		}
	}
}


internal sealed class QuantumEnginesArtifact : Artifact, INibbsArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("QuantumEngines", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [ArtifactPool.Boss]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/QuantumEngines.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "QuantumEngines", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "QuantumEngines", "description"]).Localize
		});
	}

	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		if (statusAmount <= 0 || mode != AStatusMode.Add) return;

		if (status == ModEntry.Instance.BacktrackLeftStatus.Status ||
			status == ModEntry.Instance.BacktrackRightStatus.Status ||
			status == ModEntry.Instance.BacktrackAutododgeLeftStatus.Status ||
			status == ModEntry.Instance.BacktrackAutododgeRightStatus.Status ||
			status == Status.autododgeLeft ||
			status == Status.autododgeRight)
		{
			state.ship.Add(status, 1);
			Pulse();
		}
	}
}


internal sealed class SugarRushArtifact : Artifact, INibbsArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("SugarRush", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [ArtifactPool.Boss]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/SugarRush.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "SugarRush", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "SugarRush", "description"]).Localize
		});
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


internal sealed class FledgelingOrbArtifact : Artifact, IHeatTriggerAffectorArtifact, INibbsArtifact
{
	public bool active = false;

	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("FledgelingOrb", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [],
				unremovable = true
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/FledgelingOrb.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "FledgelingOrb", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "FledgelingOrb", "description"]).Localize
		});
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