using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Shockah.Kokoro;
using static Shockah.Kokoro.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace TheJazMaster.Nibbs.Features;

public class StatusManager : IKokoroApi.IV2.IStatusLogicApi.IHook
{
    private static ModEntry Instance => ModEntry.Instance;

    public StatusManager()
    {
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(this, 0);
    }

    private static bool TickDownEnd(Ship ship, Status status, ref int amount)
    {
        if ((status == Instance.BackflipStatus ||
            status == Instance.SmokescreenStatus) && amount > 0) {
                amount -= 1;
            }
        else if (status == Instance.BacktrackLeftStatus ||
            status == Instance.BacktrackRightStatus) {
                amount = 0;
        }
        return false;
    }

    private static bool TickDownStart(Ship ship, Status status, ref int amount)
    {
        if (status == Instance.BacktrackAutododgeLeftStatus || status == Instance.BacktrackAutododgeRightStatus) {
            amount = 0;
        }
        if (amount > 0 && status == Instance.PerseveranceStatus) amount--;
		return false;
    }

    public bool HandleStatusTurnAutoStep(IHandleStatusTurnAutoStepArgs args)
	{
        int amt = args.Amount;
        if (args.Timing == IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnEnd) TickDownEnd(args.Ship, args.Status, ref amt);
        else if (args.Timing == IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart) TickDownStart(args.Ship, args.Status, ref amt);
        args.Amount = amt;
        return false;
	}
}