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
		    original: AccessTools.DeclaredMethod(typeof(AMissileHit), nameof(AMissileHit.Begin)),
			transpiler: new HarmonyMethod(typeof(AMissileHitPatches), nameof(AMissileHit_Begin_Transpiler))
		);
    }

	private static IEnumerable<CodeInstruction> AMissileHit_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
		var label = il.DefineLabel();
		new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldloc<Ship>(originalMethod).CreateLdlocInstruction(out var ldLoc)
            );

		new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
				ILMatches.LdcI4(Status.tempPayback),
                ILMatches.Call("Get"),
				ILMatches.LdcI4(0),
				ILMatches.AnyBranch
            )
			.PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Element().operand = label;

        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Stfld("fast"),
                ILMatches.Call("QueueImmediate")
            )
            .PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Advance(1)
			.ExtractLabels(out var labels)
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
                new CodeInstruction(ldLoc).WithLabels(label),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(AAttackPatches), nameof(AAttackPatches.DoPerseveranceEffect))),
            ])
            .AllElements();
    }
}
