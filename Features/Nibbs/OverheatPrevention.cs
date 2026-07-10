using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Characters;

namespace TheJazMaster.Nibbs.Features.Nibbs;

public class OverheatPreventionManager
{
	static ModEntry Instance => ModEntry.Instance;
	static NibbsManager NibbsManager => Instance.NibbsManager;
    static Harmony Harmony => Instance.Harmony;

    public OverheatPreventionManager()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.Set)),
			postfix: new HarmonyMethod(GetType(), nameof(Ship_Set_Postfix))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.RenderStatusRow)),
			transpiler: new HarmonyMethod(GetType(), nameof(Ship_RenderStatusRow_Transpiler))
		);
    }

    private static void Ship_Set_Postfix(Ship __instance, Status status, int n)
    {
        if (status == Status.timeStop) {
            foreach (Artifact item in MG.inst.g.state.EnumerateAllArtifacts()) {
                if (item is FledgelingOrbArtifact artifact) {
                    artifact.Update(__instance);
                }
            }
        }
    }

    private static IEnumerable<CodeInstruction> Ship_RenderStatusRow_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.LdcI4(3),
                ILMatches.Br,
                ILMatches.Ldarg(1),
                ILMatches.Ldfld("state"),
                ILMatches.Ldfld("ship"),
                ILMatches.Ldfld("heatTrigger")
            )
            .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(HeatTriggerDisplayModifier))),
                new(OpCodes.Add)
            ])
            .AllElements();
    }

    private static int HeatTriggerDisplayModifier(G g, Ship ship)
    {
        int result = 0;
        foreach (Artifact item in g.state.EnumerateAllArtifacts()) {
            if (item is IHeatTriggerAffectorArtifact artifact) {
                result += artifact.ModifyHeatTriggerTooltip(g, ship);
            }
        }
        return result;
    }
}