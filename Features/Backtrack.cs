using System.Collections.Generic;
using Shockah.Kokoro;
using static Shockah.Kokoro.IKokoroApi.IV2.IEvadeHookApi;

namespace TheJazMaster.Nibbs.Features;

public class BacktrackManager
{
	internal static ModEntry Instance => ModEntry.Instance;

	internal static readonly string NoStrafeKey = "NoStrafe";

	public BacktrackManager() : base()
	{
		var action = Instance.KokoroApi.EvadeHook.RegisterAction(new BacktrackEvadeAction(), 10);
		action.RegisterPaymentOption(new BacktrackPaymentOption());
		action.InheritPreconditions(Instance.KokoroApi.EvadeHook.DefaultAction);
		action.InheritPostconditions(Instance.KokoroApi.EvadeHook.DefaultAction);
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
	public bool CanPayForEvade(IEvadePaymentOption.ICanPayForEvadeArgs args)
	{
		if (args.Direction == Direction.Left && args.State.ship.Get(ModEntry.Instance.BacktrackLeftStatus) > 0) return true;
		else if (args.Direction == Direction.Right && args.State.ship.Get(ModEntry.Instance.BacktrackRightStatus) > 0) return true;
		return false;
	}

	public IReadOnlyList<CardAction> ProvideEvadePaymentActions(IEvadePaymentOption.IProvideEvadePaymentActionsArgs args)
	{
		if (args.Direction == Direction.Left) args.State.ship.Add(ModEntry.Instance.BacktrackLeftStatus, -1);
		else args.State.ship.Add(ModEntry.Instance.BacktrackRightStatus, -1);
		return [];
	}

	public void EvadeButtonHovered(IEvadePaymentOption.IEvadeButtonHoveredArgs args)
	{
		if (args.Direction == Direction.Left) args.State.ship.statusEffectPulses[ModEntry.Instance.BacktrackLeftStatus] = 0.05;
		else args.State.ship.statusEffectPulses[ModEntry.Instance.BacktrackRightStatus] = 0.05;
	}
}