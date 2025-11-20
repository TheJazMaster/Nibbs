using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using HarmonyLib;
using Nickel;
using Shockah.Kokoro;
using TheJazMaster.Nibbs.Artifacts;
using static Shockah.Kokoro.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace TheJazMaster.Nibbs.Features;

public class ChippingManager
{
	private static Harmony Harmony => ModEntry.Instance.Harmony;
	private static IModData ModData => ModEntry.Instance.Helper.ModData;

    public static readonly string ChippingKey = "Chipping";

    public ChippingManager() {
		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.NormalDamage)),
			postfix: new HarmonyMethod(GetType(), nameof(Ship_NormalDamage_Postfix))
		);

		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.GetIcon)),
			postfix: new HarmonyMethod(GetType(), nameof(AAttack_GetIcon_Postfix))
		);
		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.GetTooltips)),
			postfix: new HarmonyMethod(GetType(), nameof(AAttack_GetTooltips_Postfix))
		);
    }

    private static void Ship_NormalDamage_Postfix(State s, int incomingDamage) {
        if (AffectDamageDoneManager.AttackContext != null && ModData.GetModDataOrDefault(AffectDamageDoneManager.AttackContext, ChippingKey, false)) {
			s.ship.Add(Status.tempShield, AffectDamageDoneManager.AttackContext.damage);
		}
    }

    private static void AAttack_GetIcon_Postfix(AAttack __instance, ref Icon? __result) {
        if (ModData.GetModDataOrDefault(__instance, ChippingKey, false) && __result.HasValue) {
			__result = new Icon(ModEntry.Instance.ChippingSprite, __result.Value.number, __result.Value.color, __result.Value.flipY);
		}
    }

    private static void AAttack_GetTooltips_Postfix(AAttack __instance, ref List<Tooltip> __result, State s) {
        if (ModData.GetModDataOrDefault(__instance, ChippingKey, false) && __result != null) {
			__result.InsertRange(0, StatusMeta.GetTooltips(Status.tempShield, __instance.damage));
			__result.Insert(0, new GlossaryTooltip("action.attackChipping") {
				TitleColor = Colors.action,
				Icon = ModEntry.Instance.ChippingSprite,
				Title = ModEntry.Instance.Localizations.Localize(["action", "chipping", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["action", "chipping", "description"], new { Amount = __instance.damage }),
			});
		}
    }
}