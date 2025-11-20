using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using TheJazMaster.Nibbs.Features;

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


internal sealed class ChakraAlignerArtifact : Artifact, IRegisterableArtifact
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
			ModData.SetModData(part, Key() + "Hidden", part.brittleIsHidden);
			part.damageModifier = PDamMod.weak;
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


internal sealed class CorrectiveLensesArtifact : Artifact, IRegisterableArtifact
{
	private static IModData ModData => ModEntry.Instance.Helper.ModData;
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Common], helper, package, out _);
	}
}




internal sealed class ConservationEffortArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
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
				// damageModifier = ToughManager.ToughDamageModifier
			}
		});
	}

    public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("parttrait.invincible")
    ];
    // public override List<Tooltip>? GetExtraTooltips() => ToughManager.MakeToughPartModTooltips().ToList();
}


internal sealed class PeaceSignArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
        int n = 0;
        foreach (Part part in state.ship.parts)
		{
			if (part.type == PType.cockpit)
			{
				// Remove the hardmode brittling
				if (combat.cardActions.Where(action => action is ABrittle brittle && brittle.worldX == state.ship.x + n).FirstOrDefault() is { } action)
                    combat.cardActions.Remove(action);

                combat.QueueImmediate(new AWeaken
				{
					targetPlayer = true,
					worldX = state.ship.x + n
				});
			}
			n++;
		}
	}

    public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
    {
        return -1;
    }
}

