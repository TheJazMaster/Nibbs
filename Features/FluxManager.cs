using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using TheJazMaster.Nibbs.Actions;

namespace TheJazMaster.Nibbs.Features;

public class FluxManager
{
	private static Harmony Harmony => ModEntry.Instance.Harmony;
	private static IModData ModData => ModEntry.Instance.Helper.ModData;

	private static ISpriteEntry FluxTraitIcon = null!;

    public static readonly string FluxenKey = "Fluxen";

    public static PDamMod FluxDamageModifier;

    public FluxManager() {
		ModEntry.Instance.Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
			transpiler: new HarmonyMethod(GetType(), nameof(AAttack_Begin_Transpiler))
		);
		ModEntry.Instance.Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.GetTooltips)),
			postfix: new HarmonyMethod(GetType(), nameof(AAttack_GetTooltips_Postfix))
		);
		ModEntry.Instance.Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.RenderAction)),
			prefix: new HarmonyMethod(GetType(), nameof(Card_RenderAction_Prefix))
		);
		FluxTraitIcon = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Icons/Flux.png"));

		if (ModEntry.Instance.DynaApi != null) {
            FluxDamageModifier = ModEntry.Instance.DynaApi.FluxDamageModifier;
            return;
        }

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

		ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnPlayerTakeNormalDamage), (State state, Combat combat, Part? part) =>
		{
			if (AffectDamageDoneManager.AttackContext is null)
				return;
			TriggerFluxIfNeeded(state, combat, part, targetPlayer: true);
		});

		ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnEnemyGetHit), (State state, Combat combat, Part? part) =>
		{
			if (AffectDamageDoneManager.AttackContext is null)
				return;
			TriggerFluxIfNeeded(state, combat, part, targetPlayer: false);
		});
    }

	private static void TriggerFluxIfNeeded(State state, Combat combat, Part? part, bool targetPlayer)
	{
		if (part is null || part.invincible || part.GetDamageModifier() != FluxDamageModifier)
			return;

		combat.QueueImmediate(new AStatus
		{
			targetPlayer = !targetPlayer,
			status = Status.tempShield,
			statusAmount = 1
		});
	}

	internal static IEnumerable<Tooltip> MakeFluxPartModTooltips()
		=> [
			new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::PartDamageModifier::Flux")
			{
				Icon = FluxTraitIcon.Sprite,
				TitleColor = Colors.parttrait,
				Title = ModEntry.Instance.Localizations.Localize(["partModifier", "flux", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["partModifier", "flux", "description"])
			},
			..StatusMeta.GetTooltips(Status.tempShield, 1)
		];


	private static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod) {
        new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find([
                ILMatches.Ldloc<RaycastResult>(originalMethod).CreateLdlocInstruction(out var ldLoc)
            ]);
        return new SequenceBlockMatcher<CodeInstruction>(instructions)
			.Find([
				ILMatches.Ldarg(0),
				ILMatches.Ldfld("weaken"),
				ILMatches.Brfalse.GetBranchTarget(out var branch),
			])
			.PointerMatcher(branch.Value).ExtractLabels(out var labels)
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
				new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
				new(OpCodes.Ldarg_1),
				new(OpCodes.Ldarg_2),
				new(OpCodes.Ldarg_3),
				ldLoc,
				new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(FluxManager), nameof(DoFluxenEffect))),
			])
			.AllElements();
	}
	private static void DoFluxenEffect(AAttack attack, G g, State s, Combat c, RaycastResult result) {
		if (ModData.GetModDataOrDefault(attack, FluxenKey, false)) {
			c.QueueImmediate(new AFluxen
			{
				worldX = result.worldX,
				targetPlayer = attack.targetPlayer
			});
		}
	}

	private static void AAttack_GetTooltips_Postfix(State s, AAttack __instance, List<Tooltip> __result)
	{
		if (!ModData.GetModDataOrDefault(__instance, FluxenKey, false))
			return;

		__result.AddRange(MakeFluxPartModTooltips());
	}

    private static bool ignoreFluxen = false;
    private static bool Card_RenderAction_Prefix(G g, State state, CardAction action, bool dontDraw, int shardAvailable, int stunChargeAvailable, int bubbleJuiceAvailable, ref int __result)
	{
		if (ignoreFluxen || action is not AAttack attack || !ModData.GetModDataOrDefault(attack, FluxenKey, false))
			return true;

		ignoreFluxen = true;

		var position = g.Push(rect: new()).rect.xy;
		int initialX = (int)position.x;

		position.x += Card.RenderAction(g, state, attack, dontDraw, shardAvailable, stunChargeAvailable, bubbleJuiceAvailable);
		g.Pop();

		__result = (int)position.x - initialX;
		__result += 2;

		if (!dontDraw)
		{
			Draw.Sprite(FluxTraitIcon.Sprite, initialX + __result, position.y, color: action.disabled ? Colors.disabledIconTint : Colors.white);
		}
		__result += 10;

		ignoreFluxen = false;
		return false;
	}

	private static void Ship_RenderPartUI_Postfix(Ship __instance, G g, Part part, int localX, string keyPrefix, bool isPreview)
	{
		if (part.invincible || part.GetDamageModifier() != FluxDamageModifier)
			return;
		if (g.boxes.FirstOrDefault(b => b.key == new UIKey(StableUK.part, localX, keyPrefix)) is not { } box)
			return;

		var offset = isPreview ? 25 : 34;
		var v = box.rect.xy + new Vec(0, __instance.isPlayerShip ? (offset - 16) : 8);

		var color = new Color(1, 1, 1, 0.8 + Math.Sin(g.state.time * 4.0) * 0.3);
		Draw.Sprite(FluxTraitIcon.Sprite, v.x, v.y, color: color);

		if (!box.IsHover())
			return;
		g.tooltips.Add(g.tooltips.pos, MakeFluxPartModTooltips());
	}

	private static bool Ship_ModifyDamageDueToParts_Prefix(int incomingDamage, Part part, ref int __result)
	{
		if (part.GetDamageModifier() != FluxDamageModifier)
			return true;

		part.brittleIsHidden = false;
		__result = part.invincible ? 0 : incomingDamage;
		return false;
	}

}