using System.Collections.Generic;
using System.Linq;
using Nickel;
using Shockah.Kokoro;
using TheJazMaster.Nibbs.Artifacts;
using static Shockah.Kokoro.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace TheJazMaster.Nibbs.Features;

public class FractureManager : IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public FractureManager() {
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(this, 0);
        ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this, 0);
    }

    public bool HandleStatusTurnAutoStep(IHandleStatusTurnAutoStepArgs args)
	{
        if (args.Status != ModEntry.Instance.FractureStatus) return false;

        if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart) return false;

        args.Combat.Queue(new ATriggerFracture {
            targetPlayer = args.Ship.isPlayerShip
        });
        return false;
	}

    internal class ATriggerFracture : CardAction {
        public int amount = 9999;
        public bool targetPlayer = false;

		public override void Begin(G g, State s, Combat c)
		{
			Ship ship = targetPlayer ? s.ship : c.otherShip;
            int amt = ship.Get(ModEntry.Instance.FractureStatus);
            timer = 0;
            if (amt == 0) {
                return;
            }
            if (amount > amt) amount = amt;
            ship.PulseStatus(ModEntry.Instance.FractureStatus);
            ship.Add(ModEntry.Instance.FractureStatus, -amount);

            c.QueueImmediate([
                new AStatus {
                    status = ModEntry.Instance.SafetyShieldStatus,
                    statusAmount = amount,
                    targetPlayer = !targetPlayer
                },
            ]);
		}
    }

    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
		=> args.Status == ModEntry.Instance.FractureStatus ? [
			.. args.Tooltips,
			.. StatusMeta.GetTooltips(ModEntry.Instance.SafetyShieldStatus, 1),
		] : args.Tooltips;
}