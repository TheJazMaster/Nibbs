using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nickel;
using TheJazMaster.Nibbs.Actions;

#nullable enable
namespace TheJazMaster.Nibbs.Artifacts;

internal sealed class ChocolateSurpriseEggArtifact : Artifact, INibbsArtifact
{
	bool active = false;

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


internal sealed class EyeOfCobaArtifact : Artifact, INibbsArtifact
{
	public bool active = true;

	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("EyeOfCoba", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/EyeOfCoba.png")).Sprite,
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
}


internal sealed class GalacticNewsCoverageArtifact : Artifact, INibbsArtifact
{
	bool active = false;
	private static readonly int threshold = 6;
	static Spr ActiveSprite;
	static Spr InactiveSprite;

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
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "GalacticNewsCoverage", "description"]).Localize
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
			if (state.ship.Get(backflip) >= 5) {
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
	public bool active = true;
	static Spr ActiveSprite;
	static Spr InactiveSprite;

	public static void Register(IModHelper helper)
	{
		ActiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/EternalFlame.png")).Sprite;
		InactiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/EternalFlameInactive.png")).Sprite;
		helper.Content.Artifacts.RegisterArtifact("EternalFlame", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.NibbsDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = ActiveSprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "EternalFlame", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "EternalFlame", "description"]).Localize
		});
	}

	public override Spr GetSprite()
	{
		return active ? ActiveSprite : InactiveSprite;
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		[.. StatusMeta.GetTooltips(Status.heat, 3), .. StatusMeta.GetTooltips(Status.timeStop, 1)];


	public override void OnCombatEnd(State state)
	{
		active = true;
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		if (active && state.ship.Get(Status.timeStop) == 0 && state.ship.Get(Status.heat) >= state.ship.heatTrigger) {
			active = false;
			Pulse();
			state.ship.Add(Status.timeStop, 1);
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
		if (statusAmount <= 0) return;
		Status backtrackLeft = ModEntry.Instance.BacktrackLeftStatus.Status;
		Status backtrackRight = ModEntry.Instance.BacktrackRightStatus.Status;
		if (status == backtrackLeft) {
			state.ship.Add(backtrackLeft, 1);
			Pulse();
		}
		if (status == backtrackRight) {
			state.ship.Add(backtrackRight, 1);
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
	}

	public override void OnRemoveArtifact(State state)
	{
		state.ship.baseEnergy -= 1;
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		combat.Queue(new ABacktrackMove {
			dir = 1,
			isRandom = true,
			targetPlayer = true
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

	public override void OnReceiveArtifact(State state)
	{
		Update(state.ship, state.ship.Get(Status.timeStop));
	}

	public override void OnRemoveArtifact(State state)
	{
		if (active) {
			active = false;
			state.ship.heatTrigger -= aLot;
		}
	}

	private static readonly int aLot = 999999;
	public void Update(Ship ship, int statusAmount)
	{
		if (!active && statusAmount > 0) {
			active = true;
			ship.heatTrigger += aLot;
		}
		else if (active && statusAmount <= 0) {
			active = false;
			ship.heatTrigger -= aLot;
		}
	}
	public override List<Tooltip>? GetExtraTooltips() =>
		StatusMeta.GetTooltips(Status.timeStop, 1).Concat(StatusMeta.GetTooltips(Status.strafe, 1)).ToList();
}