using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace TheJazMaster.Nibbs;

public partial interface IKokoroApi
{
	void RegisterStatusLogicHook(IStatusLogicHook hook, double priority);
	void UnregisterStatusLogicHook(IStatusLogicHook hook);

	void RegisterStatusRenderHook(IStatusRenderHook hook, double priority);
	void UnregisterStatusRenderHook(IStatusRenderHook hook);

	Color DefaultActiveStatusBarColor { get; }
	Color DefaultInactiveStatusBarColor { get; }

	IActionApi Actions { get; }

	public interface IActionApi
	{
		CardAction MakeExhaustEntireHandImmediate();
		CardAction MakePlaySpecificCardFromAnywhere(int cardId, bool showTheCardIfNotInHand = true);
		CardAction MakePlayRandomCardsFromAnywhere(IEnumerable<int> cardIds, int amount = 1, bool showTheCardIfNotInHand = true);

		CardAction MakeContinue(out Guid id);
		CardAction MakeContinued(Guid id, CardAction action);
		IEnumerable<CardAction> MakeContinued(Guid id, IEnumerable<CardAction> action);
		CardAction MakeStop(out Guid id);
		CardAction MakeStopped(Guid id, CardAction action);
		IEnumerable<CardAction> MakeStopped(Guid id, IEnumerable<CardAction> action);

		CardAction MakeHidden(CardAction action, bool showTooltips = false);
		AVariableHint SetTargetPlayer(AVariableHint action, bool targetPlayer);
		AVariableHint MakeEnergyX(AVariableHint? action = null, bool energy = true, int? tooltipOverride = null);
		AStatus MakeEnergy(AStatus action, bool energy = true);

		List<CardAction> GetWrappedCardActions(CardAction action);
		List<CardAction> GetWrappedCardActionsRecursively(CardAction action);
		List<CardAction> GetWrappedCardActionsRecursively(CardAction action, bool includingWrapperActions);

		void RegisterWrappedActionHook(IWrappedActionHook hook, double priority);
		void UnregisterWrappedActionHook(IWrappedActionHook hook);
	}
}

public partial interface IKokoroApi
{
	IEvadeHook VanillaEvadeHook { get; }
	IEvadeHook VanillaDebugEvadeHook { get; }
	void RegisterEvadeHook(IEvadeHook hook, double priority);
	void UnregisterEvadeHook(IEvadeHook hook);

	bool IsEvadePossible(State state, Combat combat, EvadeHookContext context);
	IEvadeHook? GetEvadeHandlingHook(State state, Combat combat, EvadeHookContext context);
	void AfterEvade(State state, Combat combat, int direction, IEvadeHook hook);

	IDroneShiftHook VanillaDroneShiftHook { get; }
	IDroneShiftHook VanillaDebugDroneShiftHook { get; }
	void RegisterDroneShiftHook(IDroneShiftHook hook, double priority);
	void UnregisterDroneShiftHook(IDroneShiftHook hook);

	bool IsDroneShiftPossible(State state, Combat combat, DroneShiftHookContext context);
	IDroneShiftHook? GetDroneShiftHandlingHook(State state, Combat combat, DroneShiftHookContext context);
	void AfterDroneShift(State state, Combat combat, int direction, IDroneShiftHook hook);
}

public enum DroneShiftHookContext
{
	Rendering, Action
}

public interface IDroneShiftHook
{
	bool? IsDroneShiftPossible(State state, Combat combat, int direction, DroneShiftHookContext context) => IsDroneShiftPossible(state, combat, context);
	bool? IsDroneShiftPossible(State state, Combat combat, DroneShiftHookContext context) => null;
	void PayForDroneShift(State state, Combat combat, int direction) { }
	void AfterDroneShift(State state, Combat combat, int direction, IDroneShiftHook hook) { }
	List<CardAction>? ProvideDroneShiftActions(State state, Combat combat, int direction) => null;
}

public enum EvadeHookContext
{
	Rendering, Action
}

public interface IEvadeHook
{
	bool? IsEvadePossible(State state, Combat combat, int direction, EvadeHookContext context) => null;
	void PayForEvade(State state, Combat combat, int direction) { }
	void AfterEvade(State state, Combat combat, int direction, IEvadeHook hook) { }
	List<CardAction>? ProvideEvadeActions(State state, Combat combat, int direction) => null;
}

public interface IWrappedActionHook
{
	List<CardAction>? GetWrappedCardActions(CardAction action);
}

public interface IStatusRenderHook
{
	IEnumerable<(Status Status, double Priority)> GetExtraStatusesToShow(State state, Combat combat, Ship ship) => Enumerable.Empty<(Status Status, double Priority)>();
	bool? ShouldShowStatus(State state, Combat combat, Ship ship, Status status, int amount) => null;
	bool? ShouldOverrideStatusRenderingAsBars(State state, Combat combat, Ship ship, Status status, int amount) => null;
	(IReadOnlyList<Color> Colors, int? BarTickWidth) OverrideStatusRendering(State state, Combat combat, Ship ship, Status status, int amount) => new();
	List<Tooltip> OverrideStatusTooltips(Status status, int amount, bool isForShipStatus, List<Tooltip> tooltips) => tooltips;
	List<Tooltip> OverrideStatusTooltips(Status status, int amount, Ship? ship, List<Tooltip> tooltips) => tooltips;
}

public interface IStatusLogicHook
{
	int ModifyStatusChange(State state, Combat combat, Ship ship, Status status, int oldAmount, int newAmount) => newAmount;
	bool? IsAffectedByBoost(State state, Combat combat, Ship ship, Status status) => null;
	void OnStatusTurnTrigger(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, int oldAmount, int newAmount) { }
	bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy) => false;
}

public enum StatusTurnTriggerTiming
{
	TurnStart, TurnEnd
}

public enum StatusTurnAutoStepSetStrategy
{
	Direct, QueueSet, QueueAdd, QueueImmediateSet, QueueImmediateAdd
}

internal interface IHookPriority
{
	double HookPriority { get; }
}
