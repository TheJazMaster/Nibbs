namespace TheJazMaster.Nibbs.Features;
#nullable enable

public class StatusManager : IStatusLogicHook
{
    private static ModEntry Instance => ModEntry.Instance;

    public StatusManager()
    {
        ModEntry.Instance.KokoroApi.RegisterStatusLogicHook(this, 0);
    }

    private static bool TickDownGeneric(Ship ship, Status status, ref int amount)
    {
        if (ship.Get(Status.timeStop) > 0) return false;
        if ((status == Instance.BackflipStatus.Status ||
            status == Instance.SmokescreenStatus.Status) && amount > 0) {
                amount -= 1;
            }
        else if (status == Instance.BacktrackLeftStatus.Status ||
            status == Instance.BacktrackRightStatus.Status) {
                amount = 0;
        }
        return false;
    }

    private static bool TickDownAutododge(Ship ship, Status status, ref int amount)
    {
        if (ship.Get(Status.timeStop) > 0) return false;
        if (status == Instance.BacktrackAutododgeLeftStatus.Status || status == Instance.BacktrackAutododgeRightStatus.Status) {
            amount = 0;
        }
        return false;
    }

    public bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
	{
        return (timing == StatusTurnTriggerTiming.TurnEnd && TickDownGeneric(ship, status, ref amount))
            || (timing == StatusTurnTriggerTiming.TurnStart && TickDownAutododge(ship, status, ref amount));
	}
}