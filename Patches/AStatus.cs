using System.Collections.Generic;
using HarmonyLib;
using TheJazMaster.Nibbs.Artifacts;
using static TheJazMaster.Nibbs.Patches.StoryVarsPatches;

namespace TheJazMaster.Nibbs.Patches;

public class AStatusPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin)),
            prefix: new HarmonyMethod(typeof(AStatusPatches), nameof(AStatus_Begin_Prefix)),
			postfix: new HarmonyMethod(typeof(AStatusPatches), nameof(AStatus_Begin_Postfix))
		);
    }

    private static void AStatus_Begin_Prefix(AStatus __instance, State s, ref int __state)
		=> __state = __instance.targetPlayer ? s.ship.Get(__instance.status) : 0;


	private static void AStatus_Begin_Postfix(AStatus __instance, State s, Combat c, ref int __state)
	{
		if (!__instance.targetPlayer)
			return;

		if (__instance.status == ModEntry.Instance.NibbsCharacter.MissingStatus.Status && __state > 0 && s.ship.Get(ModEntry.Instance.NibbsCharacter.MissingStatus.Status) <= 0)
			s.storyVars.ApplyModData(JustReturnedFromMissingKey, true);
			// c.QueueImmediate(new ADummyAction { dialogueSelector = $".{ModEntry.Instance.Package.Manifest.UniqueName}::ReturningFromMissing" });
		
		if (__instance.status != Status.timeStop || __instance.statusAmount <= 0 || __state > 0) return;


		if (s.ship.Get(Status.heat) >= s.ship.heatTrigger - FledgelingOrbArtifact.aLot)
			s.storyVars.ApplyModData(SavedStatusWithTimestopKey, Status.heat.Key());

		foreach (Status status in new List<Status> {
			Status.overdrive, Status.temporaryCheap, Status.stunCharge, Status.autopilot, Status.perfectShield
		}) {
			if (s.ship.Get(status) > 0) {
				s.storyVars.ApplyModData(SavedStatusWithTimestopKey, status.Key());
				break;
			}
		}
		// if (s.ship.Get(Status.overdrive) > 0) {
		// 	c.QueueImmediate(new ADummyAction {
		// 		dialogueSelector = $".{ModEntry.Instance.Package.Manifest.UniqueName}::SavedOverdriveWithTimestop",
		// 		whoDidThis = __instance.whoDidThis
		// 	});
		// }
		// else if (s.ship.Get(Status.temporaryCheap) > 0) {
		// 	c.QueueImmediate(new ADummyAction { 
		// 		dialogueSelector = $".{ModEntry.Instance.Package.Manifest.UniqueName}::SavedTempCheapWithTimestop",
		// 		whoDidThis = __instance.whoDidThis
		// 	});
		// }
		// else if (s.ship.Get(Status.stunCharge) > 0) {
		// 	c.QueueImmediate(new ADummyAction { 
		// 		dialogueSelector = $".{ModEntry.Instance.Package.Manifest.UniqueName}::SavedStunChargeWithTimestop",
		// 		whoDidThis = __instance.whoDidThis
		// 	});
		// }
		// else if (s.ship.Get(Status.autopilot) > 0) {
		// 	c.QueueImmediate(new ADummyAction { 
		// 		dialogueSelector = $".{ModEntry.Instance.Package.Manifest.UniqueName}::SavedAutopilotWithTimestop",
		// 		whoDidThis = __instance.whoDidThis
		// 	});
		// }
		// else if (s.ship.Get(Status.perfectShield) > 0) {
		// 	c.QueueImmediate(new ADummyAction { 
		// 		dialogueSelector = $".{ModEntry.Instance.Package.Manifest.UniqueName}::SavedAutopilotWithPerfectShield",
		// 		whoDidThis = __instance.whoDidThis
		// 	});
		// }
		// else {
		// 	foreach (Artifact item in s.EnumerateAllArtifacts()) {
		// 		if (item is FledgelingOrbArtifact && s.ship.Get(Status.heat) >= s.ship.heatTrigger - FledgelingOrbArtifact.aLot) {
		// 			c.QueueImmediate(new ADummyAction {
		// 				dialogueSelector = $".{ModEntry.Instance.Package.Manifest.UniqueName}::PreventedOverheatWithTimestop",
		// 				whoDidThis = __instance.whoDidThis
		// 			});
		// 		}
		// 	}
		// }
	}
}
