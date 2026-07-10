using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using TheJazMaster.Nibbs.Artifacts;

namespace TheJazMaster.Nibbs.Features;

internal class StoryVarsManager
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;
    static IModData ModData => Instance.Helper.ModData;
	
	internal static readonly string JustBacktrackedKey = "JustBacktracked";
	internal static readonly string JustGrazedKey = "JustGrazed";
	internal static readonly string JustReturnedFromMissingKey = "JustReturnedFromMissing";
	internal static readonly string SavedStatusWithTimestopKey = "SavedStatusWithTimestop";


	public StoryVarsManager() {
		Instance.Helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnEnemyDodgePlayerAttackByOneTile), (State state, Combat combat) => {
			state.storyVars.ApplyModData(JustGrazedKey, true);
		}, 0);

        Harmony.TryPatch(
            logger: Instance.Logger,
            original: AccessTools.DeclaredMethod(typeof(StoryNode), nameof(StoryNode.Filter)),
            postfix: new HarmonyMethod(GetType(), nameof(StoryNode_Filter_Postfix))
        );
		Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(StoryVars), nameof(StoryVars.ResetAfterCombatLine)),
			postfix: new HarmonyMethod(GetType(), nameof(StoryVars_ResetAfterCombatLine_Postfix))
		);
		Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(StoryVars), nameof(StoryVars.ResetAfterEndTurn)),
			postfix: new HarmonyMethod(GetType(), nameof(StoryVars_ResetAfterEndTurn_Postfix))
		);
		Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(StoryVars), nameof(StoryVars.ResetAfterCombat)),
			postfix: new HarmonyMethod(GetType(), nameof(StoryVars_ResetAfterCombat_Postfix))
		);
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin)),
            prefix: new HarmonyMethod(GetType(), nameof(AStatus_Begin_Prefix)),
			postfix: new HarmonyMethod(GetType(), nameof(AStatus_Begin_Postfix))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(ScriptCtx), nameof(ScriptCtx.Advance)),
			transpiler: new HarmonyMethod(GetType(), nameof(ScriptCtx_Advance_Transpiler))
		);
	}

    private static void RedoGreedySwitch(Instruction instr, ScriptCtx ctx) {
        if (instr is GreedySwitch gs) {
			if (!gs.isExhausted) {
            	ctx.idx--;
			} else {
				gs.isExhausted = false;
				gs.banned.Clear();
			}
		}
    }
	private static void StoryNode_Filter_Postfix(ref bool __result, string key, StoryNode n, State s, StorySearch ctx) {
		if (!__result) return;

		{
			if (ModData.TryGetModData<bool>(n, JustBacktrackedKey, out var required) &&
				(!ModData.TryGetModData(s.storyVars, JustBacktrackedKey, out bool present) || required != present)) {
				__result = false;
				return;
			}
		} {
			if (ModData.TryGetModData<bool>(n, JustGrazedKey, out var required) &&
				(!ModData.TryGetModData(s.storyVars, JustGrazedKey, out bool present) || required != present)) {
				__result = false;
				return;
			}
		} {
			if (ModData.TryGetModData<bool>(n, JustReturnedFromMissingKey, out var required) &&
				(!ModData.TryGetModData(s.storyVars, JustReturnedFromMissingKey, out bool present) || required != present)) {
				__result = false;
				return;
			}
		} {
			if (ModData.TryGetModData<string>(n, SavedStatusWithTimestopKey, out var required) &&
				(!ModData.TryGetModData(s.storyVars, SavedStatusWithTimestopKey, out string? present) || required != present)) {
				__result = false;
				return;
			}
		}
	}

	private static void StoryVars_ResetAfterCombatLine_Postfix(StoryVars __instance) {
		__instance.RemoveModData(JustBacktrackedKey);
		__instance.RemoveModData(JustGrazedKey);
		__instance.RemoveModData(JustReturnedFromMissingKey);
	}

	private static void StoryVars_ResetAfterCombat_Postfix(StoryVars __instance) {
		StoryVars_ResetAfterCombatLine_Postfix(__instance);
	}

	private static void StoryVars_ResetAfterEndTurn_Postfix(StoryVars __instance) {
		__instance.RemoveModData(SavedStatusWithTimestopKey);
	}
    private static void AStatus_Begin_Prefix(AStatus __instance, State s, ref int __state)
		=> __state = __instance.targetPlayer ? s.ship.Get(__instance.status) : 0;


	private static void AStatus_Begin_Postfix(AStatus __instance, State s, Combat c, ref int __state)
	{
		if (!__instance.targetPlayer)
			return;

		if (__instance.status == Instance.NibbsManager.NibbsCharacter.MissingStatus.Status && __state > 0 && s.ship.Get(Instance.NibbsManager.NibbsCharacter.MissingStatus.Status) <= 0)
			s.storyVars.ApplyModData(JustReturnedFromMissingKey, true);
		
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
	}

    private static IEnumerable<CodeInstruction> ScriptCtx_Advance_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldloc<Instruction>(originalMethod).CreateLdlocInstruction(out var ldLoc),
                ILMatches.Isinst(typeof(Jump))
            )
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                ldLoc,
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(RedoGreedySwitch)))
            })
            .AllElements();
    }
}