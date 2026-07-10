using System;
using System.Collections.Generic;
using Nickel;
using TheJazMaster.Nibbs.Characters;

namespace TheJazMaster.Nibbs.Actions;

public class ABacktrackMove : AMove
{
	private static ModEntry Instance => ModEntry.Instance;
	private static NibbsManager NibbsManager => Instance.NibbsManager;

	public bool directionlessTooltip;

	public override void Begin(G g, State s, Combat c)
	{
		Ship ship = targetPlayer ? s.ship : c.otherShip;
		int x = ship.x;
		base.Begin(g, s, c);
		if (AddBacktrack(s, c, x, ship.x, this) != 0) timer = 0;
		
	}

	internal static int AddBacktrack(State s, Combat c, int oldX, int newX, AMove move) {
		int diff = oldX - newX;
		if (diff == 0) return 0;
		bool right = diff <= 0;
		c.QueueImmediate(new AStatus {
			status = right ? NibbsManager.BacktrackLeftStatus : NibbsManager.BacktrackRightStatus,
			statusAmount = Math.Abs(diff),
			targetPlayer =  move.targetPlayer
		});
		return diff;
	}

	public override Icon? GetIcon(State s) {
		int amount = GetDisplayAmount(s);
		if (isRandom) {
			return new Icon(NibbsManager.BacktrackMoveRandomIcon, amount, Colors.textMain);
		} else if (dir < 0) {
			return new Icon(NibbsManager.BacktrackMoveLeftIcon, amount, Colors.textMain);
		} else {
			return new Icon(NibbsManager.BacktrackMoveRightIcon, amount, Colors.textMain);
		}
	}

	public int GetDisplayAmount(State s)
	{
		Ship ship = s.ship;
		return Math.Abs(dir) + (ignoreHermes ? 0 : ship.Get(Status.hermes));
	}		

	public override List<Tooltip> GetTooltips(State s) {
		if (directionlessTooltip || dir == 0) {
			return [
				new GlossaryTooltip("action.backtrackMove") {
					TitleColor = Colors.action,
					Icon = NibbsManager.BacktrackMoveRightIcon,
					Title = Instance.Localizations.Localize(["action", "backtrackMove", "name"]),
					Description = Instance.Localizations.Localize(["action", "backtrackMove", "description"], new { Amount = GetDisplayAmount(s) }),
				},
				.. StatusMeta.GetTooltips(NibbsManager.BacktrackRightStatus, 1)
			];
		} if (isRandom) {
			return [
				new GlossaryTooltip("action.backtrackMoveRandom") {
					TitleColor = Colors.action,
					Icon = NibbsManager.BacktrackMoveRandomIcon,
					Title = Instance.Localizations.Localize(["action", "backtrackMoveRandom", "name"]),
					Description = Instance.Localizations.Localize(["action", "backtrackMoveRandom", "description"], new { Amount = GetDisplayAmount(s) }),
				},
				.. StatusMeta.GetTooltips(NibbsManager.BacktrackRightStatus, Math.Abs(dir))
			];
		} else if (dir < 0) {
			return [
				new GlossaryTooltip("action.backtrackMoveLeft") {
					TitleColor = Colors.action,
					Icon = NibbsManager.BacktrackMoveLeftIcon,
					Title = Instance.Localizations.Localize(["action", "backtrackMoveLeft", "name"]),
					Description = Instance.Localizations.Localize(["action", "backtrackMoveLeft", "description"], new { Amount = GetDisplayAmount(s) }),
				},
				.. StatusMeta.GetTooltips(NibbsManager.BacktrackRightStatus, -dir)
			];
		} else if (dir > 0) {
			return [
				new GlossaryTooltip("action.backtrackMoveRight") {
					TitleColor = Colors.action,
					Icon = NibbsManager.BacktrackMoveRightIcon,
					Title = Instance.Localizations.Localize(["action", "backtrackMoveRight", "name"]),
					Description = Instance.Localizations.Localize(["action", "backtrackMoveRight", "description"], new { Amount = GetDisplayAmount(s) }),
				},
				.. StatusMeta.GetTooltips(NibbsManager.BacktrackLeftStatus, dir)
			];
		} else {
			return [];
		}
	}
}
