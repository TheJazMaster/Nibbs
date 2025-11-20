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

public class CombatPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.RenderMoveButtons)),
			transpiler: new HarmonyMethod(typeof(CombatPatches), nameof(Combat_RenderMoveButtons_Transpiler))
		);
    }


    internal static int hoveredButton = 0;
    private static IEnumerable<CodeInstruction> Combat_RenderMoveButtons_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldarg(0),
                ILMatches.LdcI4(2),
                ILMatches.Stfld("isHoveringMove")
            )
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new(OpCodes.Ldc_I4, -1),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CombatPatches), nameof(SetHoveredButton)))
            })
            .Find(SequenceBlockMatcherFindOccurence.First, SequenceMatcherRelativeBounds.After,
                ILMatches.Ldarg(0),
                ILMatches.LdcI4(2),
                ILMatches.Stfld("isHoveringMove")
            )
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new(OpCodes.Ldc_I4, 1),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CombatPatches), nameof(SetHoveredButton)))
            })
            .AllElements();
    }

    private static void SetHoveredButton(int dir)
    {
        hoveredButton = dir;   
    }
}