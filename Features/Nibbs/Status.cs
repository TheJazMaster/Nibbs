using Shockah.Kokoro;
using TheJazMaster.Nibbs.Characters;
using static Shockah.Kokoro.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace TheJazMaster.Nibbs.Features.Nibbs;

public class NibbsStatusManager : IKokoroApi.IV2.IStatusLogicApi.IHook
{
    private static ModEntry Instance => ModEntry.Instance;
    private static NibbsManager NibbsManager => Instance.NibbsManager;

    public NibbsStatusManager()
    {
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(this, 0);
    }

    private static bool TickDownEnd(Ship ship, Status status, ref int amount)
    {
        if ((status == NibbsManager.BackflipStatus ||
            status == NibbsManager.SmokescreenStatus) && amount > 0) {
                amount -= 1;
            }
        else if (status == NibbsManager.BacktrackLeftStatus ||
            status == NibbsManager.BacktrackRightStatus) {
                amount = 0;
        }
        return false;
    }

    private static bool TickDownStart(Ship ship, Status status, ref int amount)
    {
        if (status == NibbsManager.BacktrackAutododgeLeftStatus || status == NibbsManager.BacktrackAutododgeRightStatus) {
            amount = 0;
        }
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