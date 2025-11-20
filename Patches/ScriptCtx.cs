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

namespace TheJazMaster.Nibbs.Patches;

public class ScriptCtxPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(ScriptCtx), nameof(ScriptCtx.Advance)),
			transpiler: new HarmonyMethod(typeof(ScriptCtxPatches), nameof(ScriptCtx_Advance_Transpiler))
		);
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
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(ScriptCtxPatches), nameof(RedoGreedySwitch)))
            })
            .AllElements();
    }

    private static void RedoGreedySwitch(Instruction instr, ScriptCtx ctx) {
        if (instr is GreedySwitch gs && !gs.isExhausted) {
            ctx.idx--;
        }
    }
}
