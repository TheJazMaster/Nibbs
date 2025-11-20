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

public class AMissileHitPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AMissileHit), nameof(AMissileHit.Update)),
			transpiler: new HarmonyMethod(typeof(AMissileHitPatches), nameof(AMissileHit_Update_Transpiler))
		);
    }

	private static IEnumerable<CodeInstruction> AMissileHit_Update_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
		return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
				ILMatches.Ldloc<Ship>(originalMethod).CreateLdlocInstruction(out var ldLoc).ExtractLabels(out var labels),
				ILMatches.LdcI4(Status.payback),
				ILMatches.Call("Get"),
				ILMatches.LdcI4(0),
				ILMatches.Bgt
			)
			.PointerMatcher(SequenceMatcherRelativeElement.First)
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
				new CodeInstruction(ldLoc).WithLabels(labels),
				new(OpCodes.Ldarg_2),
				new(OpCodes.Ldarg_3),
				new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(AAttackPatches), nameof(AAttackPatches.DoPerseveranceEffect))),
            ])
            .AllElements();
    }
}
