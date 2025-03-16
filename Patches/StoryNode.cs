using System.Reflection;
using HarmonyLib;
using Nickel;
using static TheJazMaster.Nibbs.Patches.StoryVarsPatches;

namespace TheJazMaster.Nibbs.Patches;

internal class StoryNodePatches
{
	static ModEntry Instance => ModEntry.Instance;
	static IModData ModData => Instance.Helper.ModData;
    static Harmony Harmony => Instance.Harmony;


	public static void Apply() {
        Harmony.TryPatch(
            logger: Instance.Logger,
            original: AccessTools.DeclaredMethod(typeof(StoryNode), nameof(StoryNode.Filter)),
            postfix: new HarmonyMethod(typeof(StoryNodePatches), nameof(StoryNode_Filter_Postfix))
        );
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


}