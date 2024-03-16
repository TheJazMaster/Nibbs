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
			prefix: new HarmonyMethod(typeof(AAttackPatches), nameof(AAttack_ApplyAutododge_Prefix))
		);
    }

    private static bool AAttack_ApplyAutododge_Prefix(AAttack __instance, ref bool __result, Combat c, Ship target, RaycastResult ray)
    {
        if (ray.hitShip && !__instance.isBeam)
		{
            Status autododgeLeft = Instance.BacktrackAutododgeLeftStatus.Status;
            Status autododgeRight = Instance.BacktrackAutododgeRightStatus.Status;
			if (target.Get(autododgeRight) > 0)
			{
				target.Add(autododgeRight, -1);
				int dir = ray.worldX - target.x + 1;
				c.QueueImmediate(new List<CardAction>
				{
					new ABacktrackMove
					{
						targetPlayer = __instance.targetPlayer,
						dir = dir
					},
					__instance
				});
				__instance.timer = 0.0;
                __result = true;
                return false;
			}
			if (target.Get(autododgeLeft) > 0)
			{
				target.Add(autododgeLeft, -1);
				int dir2 = ray.worldX - target.x - target.parts.Count;
				c.QueueImmediate(new List<CardAction>
				{
					new ABacktrackMove
					{
						targetPlayer = __instance.targetPlayer,
						dir = dir2
					},
					__instance
				});
				__instance.timer = 0.0;
				__result = true;
                return false;
			}
		}
		return true;
    }

    private static IEnumerable<CodeInstruction> Ship_RenderStatusRow_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldloca<KeyValuePair<Status, int>>(originalMethod).CreateLdlocaInstruction(out var ldLoc1).CreateLdlocaInstruction(out var ldLoc2),
                ILMatches.Call("get_Key"),
                ILMatches.LdcI4((int)Enum.Parse<Status>("evade")),
                ILMatches.Beq.GetBranchTarget(out var label)
            )
            .PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                ldLoc1,
                new(OpCodes.Call, typeof(KeyValuePair<Status, int>).GetMethod("get_Key")),
                new(OpCodes.Ldc_I4, (int)Instance.BacktrackLeftStatus.Status),
                new(OpCodes.Beq, label.Value),
                ldLoc2,
                new(OpCodes.Call, typeof(KeyValuePair<Status, int>).GetMethod("get_Key")),
                new(OpCodes.Ldc_I4, (int)Instance.BacktrackRightStatus.Status),
                new(OpCodes.Beq, label.Value)
            })
            .AllElements();
    }
}
