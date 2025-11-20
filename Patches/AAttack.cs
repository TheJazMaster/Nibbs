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

public class AAttackPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.ApplyAutododge)),
			prefix: new HarmonyMethod(typeof(AAttackPatches), nameof(AAttack_ApplyAutododge_Prefix)),
			postfix: new HarmonyMethod(typeof(AAttackPatches), nameof(AAttack_ApplyAutododge_Postfix))
		);

        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
			transpiler: new HarmonyMethod(typeof(AAttackPatches), nameof(AAttack_Begin_Transpiler))
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
                // new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(AAttackPatches), nameof(DoPerseveranceEffect))),
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

	internal static void DoPerseveranceEffect(Ship target, State s, Combat c) {
		int status = target.Get(ModEntry.Instance.PerseveranceStatus);
		if (status > 0) {
			c.QueueImmediate(new AStatus {
				status = Status.shield,
				statusAmount = status,
				targetPlayer = target.isPlayerShip,
				statusPulse = ModEntry.Instance.PerseveranceStatus
			});
		}
	}


    private static bool AAttack_ApplyAutododge_Prefix(AAttack __instance, ref bool __result, Combat c, Ship target, RaycastResult ray)
    {
        if (ray.hitShip && !__instance.isBeam && !__result)
		{
            Status autododgeLeft = Instance.BacktrackAutododgeLeftStatus;
            Status autododgeRight = Instance.BacktrackAutododgeRightStatus;
			if (target.Get(autododgeRight) > 0)
			{
				target.Add(autododgeRight, -1);
				int dir = ray.worldX - target.x + 1;
				c.QueueImmediate([
					new ABacktrackMove
					{
						targetPlayer = __instance.targetPlayer,
						dir = dir,
						dialogueSelector = "UsedAutododge"
					},
					__instance
				]);
				__instance.timer = 0.0;
                __result = true;
                return false;
			}
			if (target.Get(autododgeLeft) > 0)
			{
				target.Add(autododgeLeft, -1);
				int dir2 = ray.worldX - target.x - target.parts.Count;
				c.QueueImmediate([
					new ABacktrackMove
					{
						targetPlayer = __instance.targetPlayer,
						dir = dir2,
						dialogueSelector = "UsedAutododge"
					},
					__instance
				]);
				__instance.timer = 0.0;
				__result = true;
                return false;
			}
		}

		return true;
    }

	private static void AAttack_ApplyAutododge_Postfix(AAttack __instance, ref bool __result, Combat c, Ship target, RaycastResult ray)
    {
		if (__result) {
			if (c.cardActions.Count != 0 && c.cardActions.FirstOrDefault() is AMove move && move.dialogueSelector == null) {
				move.dialogueSelector = "UsedAutododge";
			}
		}
	}
}
