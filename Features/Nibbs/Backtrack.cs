using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using Shockah.Kokoro;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Characters;
using static Shockah.Kokoro.IKokoroApi.IV2.IEvadeHookApi;

namespace TheJazMaster.Nibbs.Features.Nibbs;

public class BacktrackManager : IKokoroApi.IV2.IStatusRenderingApi.IHook
{
	private static ModEntry Instance => ModEntry.Instance;
	private static NibbsManager NibbsManager => Instance.NibbsManager;
	private static Harmony Harmony => Instance.Harmony;

	internal static readonly string NoStrafeKey = "NoStrafe";

	public BacktrackManager() : base()
	{
		Instance.KokoroApi.StatusRendering.RegisterHook(this, Priority.VeryHigh);

		var action = Instance.KokoroApi.EvadeHook.RegisterAction(new BacktrackEvadeAction(), 10);
		action.RegisterPaymentOption(new BacktrackPaymentOption());
		action.InheritPreconditions(Instance.KokoroApi.EvadeHook.DefaultAction);
		action.InheritPostconditions(Instance.KokoroApi.EvadeHook.DefaultAction);

        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.RenderMoveButtons)),
			transpiler: new HarmonyMethod(GetType(), nameof(Combat_RenderMoveButtons_Transpiler))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.RenderStatusRow)),
			transpiler: new HarmonyMethod(GetType(), nameof(Ship_RenderStatusRow_Transpiler))
		);
        Harmony.TryPatch(
			logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AMove), nameof(AMove.Begin)),
            prefix: new HarmonyMethod(GetType(), nameof(AMove_Begin_Prefix)),
			postfix: new HarmonyMethod(GetType(), nameof(AMove_Begin_Postfix)),
			transpiler: new HarmonyMethod(GetType(), nameof(AMove_Begin_Transpiler))
		);
	}

	private static bool AMove_Begin_Prefix(AMove __instance, G g, State s, Combat c, ref int __state)
    {
        __state = s.ship.x;
        return true;
    }

    private static void AMove_Begin_Postfix(AMove __instance, G g, State s, Combat c, int __state)
    {
        Status smokescreen = NibbsManager.SmokescreenStatus;
        Ship ship = __instance.targetPlayer ? s.ship : c.otherShip;
        if (ship.Get(smokescreen) > 0) {
            c.QueueImmediate(new AStatus {
                status = Status.tempShield,
                statusAmount = ship.Get(smokescreen),
                targetPlayer = __instance.targetPlayer
            });
        }

        if (ModEntry.Instance.Api.IsBacktrackMovement(__instance))
            s.storyVars.ApplyModData(StoryVarsManager.JustBacktrackedKey, true);

        if (__instance is not ABacktrackMove && __instance.targetPlayer && __instance.dir != 0) {
            foreach (Artifact item in s.EnumerateAllArtifacts()) {
                if (item is EyeOfCobaArtifact artifact && artifact.active) {
                    ABacktrackMove.AddBacktrack(s, c, __state, s.ship.x, __instance);
                    artifact.active = false;
                    artifact.Pulse();
                    break;
                }
            }
        }
    }

    internal static int hoveredButton = 0;
    private static IEnumerable<CodeInstruction> Combat_RenderMoveButtons_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldarg(0),
                ILMatches.LdcI4(2),
                ILMatches.Stfld("isHoveringMove")
            )
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new(OpCodes.Ldc_I4, -1),
                new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(SetHoveredButton)))
            })
            .Find(SequenceBlockMatcherFindOccurence.First, SequenceMatcherRelativeBounds.After,
                ILMatches.Ldarg(0),
                ILMatches.LdcI4(2),
                ILMatches.Stfld("isHoveringMove")
            )
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new(OpCodes.Ldc_I4, 1),
                new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(SetHoveredButton)))
            })
            .AllElements();
    }

    private static void SetHoveredButton(int dir)
    {
        hoveredButton = dir;   
    }

    private static IEnumerable<CodeInstruction> Ship_RenderStatusRow_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
		return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Ldloca<KeyValuePair<Status, int>>(originalMethod).CreateLdlocaInstruction(out var ldLoc1),
                ILMatches.Call("get_Key"),
                ILMatches.LdcI4(Status.evade),
                ILMatches.Beq.GetBranchTarget(out var label)
            )
            .PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
                ldLoc1,
                new(OpCodes.Call, typeof(KeyValuePair<Status, int>).GetMethod("get_Key")),
                new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(ShouldHilight))),
                new(OpCodes.Brtrue, label.Value)
            ])
			.AllElements();
	}

    private static bool ShouldHilight(Status status)
    {
        return status == NibbsManager.BacktrackLeftStatus && hoveredButton == -1 || status == NibbsManager.BacktrackRightStatus && hoveredButton == 1;
    }

    private static IEnumerable<CodeInstruction> AMove_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldloc<Ship>(originalMethod).CreateLdlocInstruction(out var ldLoc),
                ILMatches.LdcI4(Status.strafe),
                ILMatches.Call("Get"),
                ILMatches.LdcI4(0),
                ILMatches.Ble.GetBranchTarget(out var label)
            )
            .PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
                ldLoc,
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(CheckIfStrafeAllowed))),
                new(OpCodes.Brfalse, label.Value)
            ])
            .AllElements();
    }

    private static bool CheckIfStrafeAllowed(Ship ship, State state, Combat combat, AMove move)
    {
        if (!ModEntry.Instance.Api.IsBacktrackMovement(move)) return true;
        
        return ModEntry.Instance.Api.ShouldBacktrackTriggerStrafe(state);
    }   

	public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args) {
		if (args.Status == NibbsManager.BacktrackAutododgeLeftStatus) {
			return [
				.. args.Tooltips,
				new GlossaryTooltip($"status{NibbsManager.BacktrackRightStatus.Key()}") {
					Title = Instance.Localizations.Localize(["status", "BacktrackRight", "name"]),
					TitleColor = Colors.status,
					Icon = NibbsManager.BacktrackRightIcon,
					Description = Instance.Localizations.Localize(["status", "BacktrackRight", "simple"])
				}
			];
		}
		else if (args.Status == NibbsManager.BacktrackAutododgeRightStatus) {
			return [
				.. args.Tooltips,
				new GlossaryTooltip($"status{NibbsManager.BacktrackLeftStatus.Key()}") {
					Title = Instance.Localizations.Localize(["status", "BacktrackLeft", "name"]),
					TitleColor = Colors.status,
					Icon = NibbsManager.BacktrackLeftIcon,
					Description = Instance.Localizations.Localize(["status", "BacktrackLeft", "simple"])
				}
			];
		}
		else if (args.Tooltips[0] is TTGlossary tt && MG.inst.g.state.route is Combat c && c.IsVisible()) {
			if (args.Status == NibbsManager.BacktrackLeftStatus) {
				(string, string)? leftRightTooltipKeys = tt.GetLeftRightTooltipKeys("status.evade");
				string key = $"status.{args.Status.Key()}";
				return [
					new GlossaryTooltip(key) {
						Title = Instance.Localizations.Localize(["status", "BacktrackLeft", "name"]),
						TitleColor = Colors.status,
						Icon = NibbsManager.BacktrackLeftIcon,
						Description = Instance.Localizations.Localize(["status", "BacktrackLeft", "description"], new {Key = leftRightTooltipKeys!.Value.Item1})
					}
				];
			}
			else if (args.Status == NibbsManager.BacktrackRightStatus) {
				(string, string)? leftRightTooltipKeys = tt.GetLeftRightTooltipKeys("status.evade");
				string key = $"status.{args.Status.Key()}";
				return [
					new GlossaryTooltip(key) {
						Title = Instance.Localizations.Localize(["status", "BacktrackRight", "name"]),
						TitleColor = Colors.status,
						Icon = NibbsManager.BacktrackRightIcon,
						Description = Instance.Localizations.Localize(["status", "BacktrackRight", "description"], new {Key = leftRightTooltipKeys!.Value.Item2})
					}
				];
			}
		}
		return args.Tooltips;
	}
}

class BacktrackEvadeAction : IEvadeAction
{
	public bool CanDoEvadeAction(IEvadeAction.ICanDoEvadeArgs args) => true;

	public IReadOnlyList<CardAction> ProvideEvadeActions(IEvadeAction.IProvideEvadeActionsArgs args) => [
		new AMove {
			dir = args.Direction == Direction.Left ? -1 : 1,
			targetPlayer = true,
			ignoreHermes = true,
			isTeleport = true,
			// dialogueSelector = ".UsedBacktrack"
		}.ApplyModData(BacktrackManager.NoStrafeKey, true)
	];
}

class BacktrackPaymentOption : IEvadePaymentOption
{
	private static ModEntry Instance => ModEntry.Instance;
	private static NibbsManager NibbsManager => Instance.NibbsManager;

	public bool CanPayForEvade(IEvadePaymentOption.ICanPayForEvadeArgs args)
	{
		if (args.Direction == Direction.Left && args.State.ship.Get(NibbsManager.BacktrackLeftStatus) > 0) return true;
		else if (args.Direction == Direction.Right && args.State.ship.Get(NibbsManager.BacktrackRightStatus) > 0) return true;
		return false;
	}

	public IReadOnlyList<CardAction> ProvideEvadePaymentActions(IEvadePaymentOption.IProvideEvadePaymentActionsArgs args)
	{
		if (args.Direction == Direction.Left) args.State.ship.Add(NibbsManager.BacktrackLeftStatus, -1);
		else args.State.ship.Add(NibbsManager.BacktrackRightStatus, -1);
		return [];
	}

	public void EvadeButtonHovered(IEvadePaymentOption.IEvadeButtonHoveredArgs args)
	{
		if (args.Direction == Direction.Left) args.State.ship.statusEffectPulses[NibbsManager.BacktrackLeftStatus] = 0.05;
		else args.State.ship.statusEffectPulses[NibbsManager.BacktrackRightStatus] = 0.05;
	}
}