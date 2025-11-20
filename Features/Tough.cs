using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nickel;
namespace TheJazMaster.Nibbs.Features;

public class ToughManager
{
	internal const PDamMod ToughDamageModifier = (PDamMod)59813;

	internal static ISpriteEntry ToughModifierIcon { get; private set; } = null!;


	public ToughManager()
	{
		ToughModifierIcon = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Icons/Tough.png"));

		ModEntry.Instance.Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.RenderPartUI)),
			postfix: new HarmonyMethod(GetType(), nameof(Ship_RenderPartUI_Postfix))
		);
		ModEntry.Instance.Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.ModifyDamageDueToParts)),
			prefix: new HarmonyMethod(GetType(), nameof(Ship_ModifyDamageDueToParts_Prefix))
		);
	}

	private static bool Ship_ModifyDamageDueToParts_Prefix(Ship __instance, ref int __result, State s, Combat c, int incomingDamage, Part part, bool piercing = false) {
		if (part.damageModifier == ToughDamageModifier)
		{
			if (!piercing && __result > 1)
				__result = 1;
			else
				__result = incomingDamage;
			return false;
		}
		return true;
	}


	internal static IEnumerable<Tooltip> MakeToughPartModTooltips()
	{
		return [new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::PartDamModifier::Tough")
			{
				Icon = ToughModifierIcon.Sprite,
				TitleColor = Colors.parttrait,
				Title = ModEntry.Instance.Localizations.Localize(["partModifier", "tough", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["partModifier", "tough", "description"])
			}
		];
	}

	private static void Ship_RenderPartUI_Postfix(Ship __instance, G g, Part part, int localX, string keyPrefix, bool isPreview)
	{
		if (part.invincible || (part.damageModifier != ToughDamageModifier))
			return;
		if (g.boxes.FirstOrDefault(b => b.key == new UIKey(StableUK.part, localX, keyPrefix)) is not { } box)
			return;
		

		var offset = isPreview ? 25 : 34;
		var v = box.rect.xy + new Vec(0, __instance.isPlayerShip ? (offset - 16) : 8);

		var color = new Color(1, 1, 1, 0.8 + Math.Sin(g.state.time * 4.0) * 0.3);
		Draw.Sprite(ToughModifierIcon.Sprite, v.x, v.y, color: color);

		if (!box.IsHover())
			return;
		g.tooltips.Add(g.tooltips.pos, MakeToughPartModTooltips());
	}
}