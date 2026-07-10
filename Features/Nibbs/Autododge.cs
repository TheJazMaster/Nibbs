using System.Linq;
using HarmonyLib;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Characters;

namespace TheJazMaster.Nibbs.Features.Nibbs;

public class AutododgeManager
{
	internal static ModEntry Instance => ModEntry.Instance;
	internal static NibbsManager NibbsManager => Instance.NibbsManager;
	internal static Harmony Harmony => ModEntry.Instance.Harmony;


	public AutododgeManager() : base()
	{
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.ApplyAutododge)),
			prefix: new HarmonyMethod(GetType(), nameof(AAttack_ApplyAutododge_Prefix)),
			postfix: new HarmonyMethod(GetType(), nameof(AAttack_ApplyAutododge_Postfix))
		);
	}
    
    private static bool AAttack_ApplyAutododge_Prefix(AAttack __instance, ref bool __result, Combat c, Ship target, RaycastResult ray)
    {
        if (ray.hitShip && !__instance.isBeam && !__result)
		{
            Status autododgeLeft = NibbsManager.BacktrackAutododgeLeftStatus;
            Status autododgeRight = NibbsManager.BacktrackAutododgeRightStatus;
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