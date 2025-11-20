using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nickel;

namespace TheJazMaster.Nibbs.Actions;

public class ABacktrackMove : AMove
{
	public bool directionlessTooltip;

	public override void Begin(G g, State s, Combat c)
	{
		Ship ship = targetPlayer ? s.ship : c.otherShip;
		int x = ship.x;
		base.Begin(g, s, c);
		AddBacktrack(s, c, x, ship.x, this);
	}

	internal static void AddBacktrack(State s, Combat c, int oldX, int newX, AMove move) {
		int diff = oldX - newX;
		if (diff == 0) return;
		bool right = diff <= 0;
		c.QueueImmediate(new AStatus {
			status = right ? ModEntry.Instance.BacktrackLeftStatus : ModEntry.Instance.BacktrackRightStatus,
			statusAmount = Math.Abs(diff),
			targetPlayer =  move.targetPlayer
		});
	}

	public override Icon? GetIcon(State s) {
		int amount = GetDisplayAmount(s);
		if (isRandom) {
			return new Icon(ModEntry.Instance.BacktrackMoveRandomIcon, amount, Colors.textMain);
		} else if (dir < 0) {
			return new Icon(ModEntry.Instance.BacktrackMoveLeftIcon, amount, Colors.textMain);
		} else {
			return new Icon(ModEntry.Instance.BacktrackMoveRightIcon, amount, Colors.textMain);
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
					Icon = ModEntry.Instance.BacktrackMoveRightIcon,
					Title = ModEntry.Instance.Localizations.Localize(["action", "backtrackMove", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["action", "backtrackMove", "description"], new { Amount = GetDisplayAmount(s) }),
				}
			];
		} if (isRandom) {
			return [
				new GlossaryTooltip("action.backtrackMoveRandom") {
					TitleColor = Colors.action,
					Icon = ModEntry.Instance.BacktrackMoveRandomIcon,
					Title = ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveRandom", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveRandom", "description"], new { Amount = GetDisplayAmount(s) }),
				}
			];
		} else if (dir < 0) {
			return [
				new GlossaryTooltip("action.backtrackMoveLeft") {
					TitleColor = Colors.action,
					Icon = ModEntry.Instance.BacktrackMoveLeftIcon,
					Title = ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveLeft", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveLeft", "description"], new { Amount = GetDisplayAmount(s) }),
				}
			];
		} else if (dir > 0) {
			return [
				new GlossaryTooltip("action.backtrackMoveRight") {
					TitleColor = Colors.action,
					Icon = ModEntry.Instance.BacktrackMoveRightIcon,
					Title = ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveRight", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveRight", "description"], new { Amount = GetDisplayAmount(s) }),
					
				}
			];
		} else {
			return [];
		}
	}
}
