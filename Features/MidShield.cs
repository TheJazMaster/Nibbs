using System;
using System.Linq;
using System.Net;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nickel;
using Shockah.Kokoro;
using TheJazMaster.Nibbs.Artifacts;
using static Shockah.Kokoro.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace TheJazMaster.Nibbs.Features;

public class MidShieldManager : IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
	private static Harmony Harmony => ModEntry.Instance.Harmony;
	private static IModData ModData => ModEntry.Instance.Helper.ModData;

	public static readonly string MidShieldKey = "MidShield";
	public static readonly string GhostMidShieldKey = "GhostMidShield";


    public MidShieldManager() {
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(this, 1);
        ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this, 0);

		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.RenderHealthBar)),
			postfix: new HarmonyMethod(GetType(), nameof(Ship_RenderHealthBar_Postfix))
		);

		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.NormalDamage)),
			prefix: new HarmonyMethod(AccessTools.DeclaredMethod(GetType(), nameof(Ship_NormalDamage_Prefix)), Priority.Last),
			postfix: new HarmonyMethod(AccessTools.DeclaredMethod(GetType(), nameof(Ship_NormalDamage_Postfix)), Priority.First)
		);
		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.Update)),
			postfix: new HarmonyMethod(GetType(), nameof(Ship_Update_Postfix))
		);
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
			prefix: new HarmonyMethod(GetType(), nameof(AAttack_Begin_Prefix)),
			finalizer: new HarmonyMethod(GetType(), nameof(AAttack_Begin_Finalizer))
		);
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(ASpaceMineAttack), nameof(ASpaceMineAttack.Begin)),
			prefix: new HarmonyMethod(GetType(), nameof(ASpaceMineAttack_Begin_Prefix)),
			finalizer: new HarmonyMethod(GetType(), nameof(ASpaceMineAttack_Begin_Finalizer))
		);
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AMissileHit), nameof(AMissileHit.Update)),
			prefix: new HarmonyMethod(GetType(), nameof(ASpaceMineAttack_Begin_Prefix)),
			finalizer: new HarmonyMethod(GetType(), nameof(ASpaceMineAttack_Begin_Finalizer))
		);
    }

	private static bool isFromMidrow;
	private static void AAttack_Begin_Prefix(AAttack __instance) {
		isFromMidrow = __instance.fromDroneX.HasValue;
	}
	private static void AAttack_Begin_Finalizer() {
		isFromMidrow = false;
	}
	private static void ASpaceMineAttack_Begin_Prefix() {
		isFromMidrow = true;
	}
	private static void ASpaceMineAttack_Begin_Finalizer() {
		isFromMidrow = false;
	}

	struct OldTempShieldInfo {
		public int oldTempShield;
		public int newTempShield;
	}
	private static OldTempShieldInfo info = new();

	private static void Ship_NormalDamage_Prefix(Ship __instance, State s, Combat c, int incomingDamage, int? maybeWorldGridX, bool piercing)
	{
		if (!isFromMidrow)
		{
			return;
		}

		info = new OldTempShieldInfo {
			oldTempShield = __instance.Get(Status.tempShield),
			newTempShield = __instance.Get(Status.tempShield) + __instance.Get(ModEntry.Instance.MidShieldStatus)
		};
		__instance._Set(Status.tempShield, info.newTempShield);
	}

	private static void Ship_NormalDamage_Postfix(Ship __instance, State s, Combat c, int incomingDamage, int? maybeWorldGridX, bool piercing = false){
		
		if (!isFromMidrow) {
			return;
		}

		int diff = info.newTempShield - __instance.Get(Status.tempShield);
		if (__instance.Get(ModEntry.Instance.MidShieldStatus) > 0	) {
			int midShieldDamage = Math.Max(0, diff - info.oldTempShield);
			__instance._Set(ModEntry.Instance.MidShieldStatus, __instance.Get(ModEntry.Instance.MidShieldStatus) - midShieldDamage);
			__instance._Set(Status.tempShield, info.oldTempShield - diff + midShieldDamage);

			ModData.SetModData(__instance, GhostMidShieldKey, ModData.GetModDataOrDefault(__instance, GhostMidShieldKey, 0) + midShieldDamage);
			__instance.ghostTempShield -= midShieldDamage;
		}
		// int diff = info.newTempShield - __instance.Get(Status.tempShield);
		// if (diff > 0) {
		// 	int overflow = Math.Min(__instance.Get(ModEntry.Instance.MidShieldStatus), diff);
		// 	diff -= overflow;
		// 	__instance._Set(ModEntry.Instance.MidShieldStatus, __instance.Get(ModEntry.Instance.MidShieldStatus) - overflow);
		// 	__instance._Set(Status.tempShield, info.oldTempShield - diff);

		// 	ModData.SetModData(__instance, GhostMidShieldKey, ModData.GetModDataOrDefault(__instance, GhostMidShieldKey, 0) + overflow);
		// 	__instance.ghostTempShield -= overflow;
		// }
	}

	private static void Ship_Update_Postfix(Ship __instance) {
		if (__instance.ghostHpTimer == 0) ModData.RemoveModData(__instance, GhostMidShieldKey);
	}

	internal static Color MidShieldColor = new("64e38a");
	private static void Ship_RenderHealthBar_Postfix(Ship __instance, G g, bool isPreview, string keyPrefix) {
		Box? box = g.boxes.Find(b => b.key == new UIKey(StableUK.healthBar, 0, keyPrefix));
        if (box == null) return;

        int MidShieldAmount = __instance.Get(ModEntry.Instance.MidShieldStatus);
		int ghostMidShieldAmount = ModData.GetModDataOrDefault(__instance, GhostMidShieldKey, 0);
		int maxShield = __instance.GetMaxShield();
        if (MidShieldAmount + ghostMidShieldAmount == 0 || maxShield == 0) return;
        int tempShieldAmount = __instance.Get(Status.tempShield);
		int num3 = __instance.hullMax + maxShield;
		int num5 = isPreview ? __instance.parts.Count : (__instance.parts.Count + 2);
		int num6 = 16 * num5;
		int hullHeight = 5;
		int shieldHeight = 3;
		int chunkWidth = Mutil.Clamp(num6 / num3, 2, 4) - 1;
		int chunkMargin = 1;

		Vec v = box.rect.xy;

		Draw.Rect(v.x + __instance.hullMax * (chunkWidth + chunkMargin) + 1, v.y + shieldHeight + 3, maxShield * (chunkWidth + chunkMargin) + 2, shieldHeight + 2, Colors.black.fadeAlpha(0.75));
		Draw.Rect(v.x + __instance.hullMax * (chunkWidth + chunkMargin) - 2, v.y + hullHeight + 3, 3, 3, Colors.black.fadeAlpha(0.75));

		Draw.Rect(v.x + __instance.hullMax * (chunkWidth + chunkMargin) + 1, v.y + shieldHeight + 3, maxShield * (chunkWidth + chunkMargin) + 1, shieldHeight + 1, Colors.healthBarBorder);
		Draw.Rect(v.x + __instance.hullMax * (chunkWidth + chunkMargin) - 1, v.y + hullHeight + 3, 2, 2, Colors.healthBarBorder);

		Draw.Rect(v.x + __instance.hullMax * (chunkWidth + chunkMargin), v.y + shieldHeight + 2, maxShield * (chunkWidth + chunkMargin) + 1, shieldHeight + 1, Colors.healthBarBbg);
		Draw.Rect(v.x + __instance.hullMax * (chunkWidth + chunkMargin), v.y + shieldHeight + 2, 1, 1, Colors.healthBarBbg);

		for (int l = 0; l < maxShield; l++)
		{
			Color color = l < MidShieldAmount ? MidShieldColor : (l < MidShieldAmount + ghostMidShieldAmount) ? Colors.healthBarGhost : MidShieldColor.fadeAlpha(0.25);
			DrawChunk(__instance.hullMax + l, 3, color, l < MidShieldAmount - 1 && l % 5 != 4);
		}

		void DrawChunk(int i, int height, Color color, bool rightMargin)
		{
			double num9 = v.x + 1 + (i * (chunkWidth + chunkMargin));
			double y = v.y + 2 + shieldHeight;
			Draw.Rect(num9, y, chunkWidth, height, color);
			if (rightMargin)
			{
				Draw.Rect(num9 + chunkWidth, y, chunkMargin, height, MidShieldColor.fadeAlpha(0.6));
			}
		}
    }

    public bool? ShouldShowStatus(IKokoroApi.IV2.IStatusRenderingApi.IHook.IShouldShowStatusArgs args)
		=> args.Status == ModEntry.Instance.MidShieldStatus ? false : null;
	
	public int ModifyStatusChange(IModifyStatusChangeArgs args) {
		if (args.Status == ModEntry.Instance.MidShieldStatus) {
			return Math.Min(args.NewAmount, args.Ship.GetMaxShield());
		}
		return args.NewAmount;
	}
}