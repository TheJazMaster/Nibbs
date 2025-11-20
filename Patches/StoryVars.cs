using HarmonyLib;

namespace TheJazMaster.Nibbs.Patches;

internal class StoryVarsPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;
	
	internal static readonly string JustBacktrackedKey = "JustBacktracked";
	internal static readonly string JustGrazedKey = "JustGrazed";
	internal static readonly string JustReturnedFromMissingKey = "JustReturnedFromMissing";
	internal static readonly string SavedStatusWithTimestopKey = "SavedStatusWithTimestop";


	public static void Apply() {
		Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(StoryVars), nameof(StoryVars.ResetAfterCombatLine)),
			postfix: new HarmonyMethod(typeof(StoryVarsPatches), nameof(StoryVars_ResetAfterCombatLine_Postfix))
		);
		Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(StoryVars), nameof(StoryVars.ResetAfterEndTurn)),
			postfix: new HarmonyMethod(typeof(StoryVarsPatches), nameof(StoryVars_ResetAfterEndTurn_Postfix))
		);
		Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(StoryVars), nameof(StoryVars.ResetAfterCombat)),
			postfix: new HarmonyMethod(typeof(StoryVarsPatches), nameof(StoryVars_ResetAfterCombat_Postfix))
		);
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
}