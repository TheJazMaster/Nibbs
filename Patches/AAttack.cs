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
						dir = dir,
						dialogueSelector = "UsedAutododge"
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
						dir = dir2,
						dialogueSelector = "UsedAutododge"
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

	private static void AAttack_ApplyAutododge_Postfix(AAttack __instance, ref bool __result, Combat c, Ship target, RaycastResult ray)
    {
		if (__result == true) {
			if (c.cardActions.First() is AMove move && move.dialogueSelector == null) {
				move.dialogueSelector = "UsedAutododge";
			}
		}
	}
}
