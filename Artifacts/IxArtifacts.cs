using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.Nibbs.Artifacts;


internal sealed class ManifestoArtifact : Artifact, IRegisterableArtifact
{
	public int count = 0;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnPlayerTakeNormalDamage(State state, Combat combat, int rawAmount, Part? part)
	{
		count++;
		if (count == 3) {
			count = 0;
			combat.Queue(new AStatus
			{
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override int? GetDisplayNumber(State s)
	{
		return count;
	}
	
	public override List<Tooltip>? GetExtraTooltips() => StatusMeta.GetTooltips(Status.droneShift, 1);
}


internal sealed class HealingCrystalsArtifact : Artifact, IRegisterableArtifact
{
    public bool active = true;

    private static Spr ActiveSpr;
	private static Spr InactiveSpr;
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _, out ActiveSpr, out InactiveSpr);
	}

    public override void OnCombatEnd(State state)
    {
        active = true;
    }

    public override Spr GetSprite() => active ? ActiveSpr : InactiveSpr;
}


internal sealed class TelescopeArtifact : Artifact, IRegisterableArtifact
{
	public int counter = 0;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

    public override void OnPlayerSpawnSomething(State state, Combat combat, StuffBase thing)
    {
        if (++counter >= 2) {
			combat.QueueImmediate(new AStatus
			{
				status = ModEntry.Instance.IxManager.MidShieldStatus,
				statusAmount = 1,
				targetPlayer = true,
				artifactPulse = Key()
			});
			counter = 0;
		}
    }

    public override int? GetDisplayNumber(State s) => counter;
}


internal sealed class RayleighScatteringArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}
}




internal sealed class MoralHighGroundArtifact : Artifact, IRegisterableArtifact
{
	private static IModData ModData => ModEntry.Instance.Helper.ModData;
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		foreach (Part part in combat.otherShip.parts) {
			if (part.type == PType.empty) continue;

			ModData.SetModData(part, Key(), part.damageModifier);
			part.damageModifier = PDamMod.brittle;
			part.brittleIsHidden = false;
		}
		Pulse();
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		foreach (Part part in combat.otherShip.parts) {
			if (part.type == PType.empty || part.damageModifier != PDamMod.weak) continue;

			var mod = ModData.GetOptionalModData<PDamMod>(part, Key());
			if (mod.HasValue) part.damageModifier = mod.Value;
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("parttrait.weak")
	];
}



internal sealed class ConservationEffortArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
	}

	public override void OnReceiveArtifact(State state)
	{
		(state.GetDialogue()?.actionQueue ?? state.GetCurrentQueue()).Queue(new AShipUpgrades {
			actions = [
				new AInsertPart {
					targetPlayer = true,
					x = state.rngActions.NextInt() % (state.ship.parts.Count - 1) + 1,
					part = new Part {
						type = PType.special,
						skin = "crystal_1",
						invincible = true
						// damageModifier = ToughManager.ToughDamageModifier
					}
				}
			]
		});
	}

    public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("parttrait.invincible")
    ];
    // public override List<Tooltip>? GetExtraTooltips() => ToughManager.MakeToughPartModTooltips().ToList();
}


internal sealed class PeaceSignArtifact : Artifact, IRegisterableArtifact
{
    public bool active = true;

    public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
	}

    public override void OnReceiveArtifact(State state) {
        state.ship.baseEnergy++;
    }

    public override void OnRemoveArtifact(State state) {
        state.ship.baseEnergy--;
    }

    public override void OnTurnStart(State state, Combat combat) {
        active = true;
    }

    public override void OnPlayerAttack(State state, Combat combat) {
        if (active) {
            Pulse();
            active = false;
        }
    }

    public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
    {
        return (fromPlayer && active) ? -9999 : 0;
    }
}

