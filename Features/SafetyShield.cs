using System;
using System.Linq;
using System.Net;
using HarmonyLib;
using Nickel;
using Shockah.Kokoro;
using TheJazMaster.Nibbs.Artifacts;
using static Shockah.Kokoro.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace TheJazMaster.Nibbs.Features;

public class SafetyShieldManager : IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
	private static Harmony Harmony => ModEntry.Instance.Harmony;
	private static IModData ModData => ModEntry.Instance.Helper.ModData;

	public static readonly string SafetyShieldKey = "SafetyShield";


    public SafetyShieldManager() {
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(this, 1);
        ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this, 0);

		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.Set)),
			prefix: new HarmonyMethod(GetType(), nameof(Ship_Set_Prefix))
		);
		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.RenderHealthBar)),
			postfix: new HarmonyMethod(GetType(), nameof(Ship_RenderHealthBar_Postfix))
		);
    }

    private static void Ship_RenderHealthBar_Postfix(Ship __instance, G g, bool isPreview, string keyPrefix) {

		Box? box = g.boxes.Find(b => b.key == new UIKey(StableUK.healthBar, 0, keyPrefix));
        if (box == null) return;

        int safetyShieldAmount = __instance.Get(ModEntry.Instance.SafetyShieldStatus);
        if (safetyShieldAmount == 0) return;
        int tempShieldAmount = __instance.Get(Status.tempShield);
		int maxShield = __instance.GetMaxShield();
		int num3 = __instance.hullMax + maxShield;
		int num5 = (isPreview ? __instance.parts.Count : (__instance.parts.Count + 2));
		int num6 = 16 * num5;
		int chunkWidth = Mutil.Clamp(num6 / num3, 2, 4) - 1;
		int chunkMargin = 1;

		Vec v = box.rect.xy;

		for (int l = tempShieldAmount - safetyShieldAmount; l < tempShieldAmount; l++)
		{
			DrawChunk(__instance.hullMax + maxShield + l, 3, l < tempShieldAmount - 1 && l % 5 != 4);
		}

		void DrawChunk(int i, int height, bool rightMargin)
		{
			double num9 = v.x + 1.0 + (double)(i * (chunkWidth + chunkMargin));
			double y = v.y + 1.0;
			Draw.Rect(num9, y, chunkWidth, height, new Color("64e38a"));
			if (rightMargin)
			{
				Draw.Rect(num9 + (double)chunkWidth, y, chunkMargin, height, new Color("3c8852"));
			}
		}
    }

    private static void Ship_Set_Prefix(Ship __instance, Status status, int n) {
        if (status == Status.tempShield) {
            int diff = n - __instance.Get(status);
            int safety = __instance.Get(ModEntry.Instance.SafetyShieldStatus);
            if (diff < 0 && safety > 0) {
                __instance._Set(ModEntry.Instance.SafetyShieldStatus, Math.Max(0, safety + diff));
            }
        }
        else if (status == ModEntry.Instance.SafetyShieldStatus) {
            int diff = n - __instance.Get(status);
            if (diff > 0) {
                __instance.Add(Status.tempShield, diff);
            }
        }
    }

    public bool HandleStatusTurnAutoStep(IHandleStatusTurnAutoStepArgs args)
	{
		return HandleSafetyShield(args) || HandleTempShield(args);
	}

    public bool HandleTempShield(IHandleStatusTurnAutoStepArgs args) {
        if (args.Status != Status.tempShield) return false;

        if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart) return false;

		int safetyShield = args.Ship.Get(ModEntry.Instance.SafetyShieldStatus);
        args.Amount = Math.Max(args.Amount, safetyShield);
		int diff = args.Ship.Get(Status.tempShield) - args.Amount;
		if (diff > 0) args.Ship._Set(ModEntry.Instance.SafetyShieldStatus, safetyShield + diff);
        return true;
    }

    public bool HandleSafetyShield(IHandleStatusTurnAutoStepArgs args) {
        if (args.Status != ModEntry.Instance.SafetyShieldStatus) return false;

        if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnEnd) return false;

        args.Ship.Add(Status.tempShield, -Math.Min(args.Ship.Get(Status.tempShield), args.Amount));
        args.Amount = 0;
        return false;
    }

    public bool? ShouldShowStatus(IKokoroApi.IV2.IStatusRenderingApi.IHook.IShouldShowStatusArgs args)
		=> args.Status == ModEntry.Instance.SafetyShieldStatus ? false : null;
}