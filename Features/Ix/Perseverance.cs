using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using TheJazMaster.Nibbs.Characters;

namespace TheJazMaster.Nibbs.Features.Ix;

public class PerseveranceManager
{
    private static ModEntry Instance => ModEntry.Instance;
    private static IxManager IxManager => Instance.IxManager;
	private static Harmony Harmony => ModEntry.Instance.Harmony;


    public PerseveranceManager() {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
			transpiler: new HarmonyMethod(GetType(), nameof(AAttack_Begin_Transpiler))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AMissileHit), nameof(AMissileHit.Update)),
			transpiler: new HarmonyMethod(GetType(), nameof(AMissileHit_Update_Transpiler))
		);
    }

	private static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
		var label = il.DefineLabel();
		new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldloc<Ship>(originalMethod).CreateLdlocInstruction(out var ldLoc)
            );

        var h = new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Stfld("storyFromPayback"),
                ILMatches.Call("QueueImmediate")
            )
            .PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Advance(1)
			.ExtractLabels(out var labels)
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
                new CodeInstruction(ldLoc).WithLabels(labels),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(DoPerseveranceEffect))),
            ])
            .AllElements();
		try {
			return new SequenceBlockMatcher<CodeInstruction>(h)
				.Find([
					ILMatches.Ldarg(0),
					ILMatches.Ldflda("givesEnergy"),
					ILMatches.Call("get_HasValue"),
				])
				.Find([
					ILMatches.LdcI4(1),
					ILMatches.Stfld("changeAmount"),
				])
				.PointerMatcher(SequenceMatcherRelativeElement.First)
				.Replace(
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldflda, typeof(AAttack).GetField("givesEnergy")!),
					new CodeInstruction(OpCodes.Call, typeof(int?).GetProperty("Value")!.GetAccessors().First(m => m.ReturnType == typeof(int)))
				)
				.AllElements();
		} catch (Exception) {
			return h;
		}
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
				new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(DoPerseveranceEffectMissile))),
            ])
            .AllElements();
    }

	internal static void DoPerseveranceEffectMissile(Ship target, State s, Combat c) {
		int status = target.Get(IxManager.PerseveranceStatus);
		if (status > 0) {
			c.QueueImmediate(new AStatus {
				status = Status.shield,
				statusAmount = status,
				targetPlayer = target.isPlayerShip,
				statusPulse = IxManager.PerseveranceStatus
			});
		}
	}

	internal static void DoPerseveranceEffect(Ship target, State s, Combat c, AAttack attack) {
		int status = target.Get(IxManager.PerseveranceStatus);
		if (status > 0 && !attack.isBeam) {
			c.QueueImmediate(new AStatus {
				status = Status.shield,
				statusAmount = status,
				targetPlayer = target.isPlayerShip,
				statusPulse = IxManager.PerseveranceStatus
			});
		}
	}
}