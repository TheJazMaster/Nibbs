using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Artifacts;

namespace TheJazMaster.Nibbs.Patches;

public class ShipPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.Set)),
			postfix: new HarmonyMethod(typeof(ShipPatches), nameof(Ship_Set_Postfix))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.RenderStatusRow)),
			transpiler: new HarmonyMethod(typeof(ShipPatches), nameof(Ship_RenderStatusRow_Transpiler))
		);
    }

    private static void Ship_Set_Postfix(Ship __instance, Status status, int n)
    {
        if (status == Status.timeStop) {
            foreach (Artifact item in MG.inst.g.state.EnumerateAllArtifacts()) {
                if (item is FledgelingOrbArtifact artifact) {
                    artifact.Update(__instance, n);
                }
            }
        }
    }

    private static IEnumerable<CodeInstruction> Ship_RenderStatusRow_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldloca<KeyValuePair<Status, int>>(originalMethod).CreateLdlocaInstruction(out var ldLoc1),
                ILMatches.Call("get_Key"),
                ILMatches.LdcI4((int)Enum.Parse<Status>("evade")),
                ILMatches.Beq.GetBranchTarget(out var label)
            )
            .PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                ldLoc1,
                new(OpCodes.Call, typeof(KeyValuePair<Status, int>).GetMethod("get_Key")),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(ShipPatches), nameof(ShouldHilight))),
                new(OpCodes.Brtrue, label.Value)
            })
            .Find(SequenceBlockMatcherFindOccurence.First, SequenceMatcherRelativeBounds.Before,
                ILMatches.LdcI4(3),
                ILMatches.Br,
                ILMatches.Ldarg(1),
                ILMatches.Ldfld("state"),
                ILMatches.Ldfld("ship"),
                ILMatches.Ldfld("heatTrigger")
            )
            .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(ShipPatches), nameof(HeatTriggerDisplayModifier))),
                new(OpCodes.Add)
            })
            .AllElements();
    }

    private static bool ShouldHilight(Status status)
    {
        return status == Instance.BacktrackLeftStatus.Status && CombatPatches.hoveredButton == -1 || status == Instance.BacktrackRightStatus.Status && CombatPatches.hoveredButton == 1;
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