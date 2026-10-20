using Shockah.Kokoro;
using TheJazMaster.Nibbs.Characters;
using static Shockah.Kokoro.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace TheJazMaster.Nibbs.Features.Ix;

public class IxStatusManager : IKokoroApi.IV2.IStatusLogicApi.IHook
{
    private static ModEntry Instance => ModEntry.Instance;
    private static IxManager IxManager => Instance.IxManager;

    public IxStatusManager()
    {
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(this, 0);
    }

    public bool HandleStatusTurnAutoStep(IHandleStatusTurnAutoStepArgs args)
	{
        if (args.Amount > 0 && args.Status == IxManager.PerseveranceStatus && args.Timing == IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
            args.Amount--;
		return false;
	}
}