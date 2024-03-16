// AMove
using System;
using System.Collections.Generic;
using static TheJazMaster.Nibbs.CustomTTGlossary;

namespace TheJazMaster.Nibbs.Actions;

public class ABacktrackMove : AMove
{
	public bool directionlessTooltip;

	public ABacktrackMove() : base()
	{
		isTeleport = true;
	}

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
			status = right ? ModEntry.Instance.BacktrackLeftStatus.Status : ModEntry.Instance.BacktrackRightStatus.Status,
			statusAmount = Math.Abs(diff),
			targetPlayer =  move.targetPlayer
		});
	}

	public override Icon? GetIcon(State s) {
		int amount = GetDisplayAmount(s);
		if (isRandom) {
			return new Icon(ModEntry.Instance.BacktrackMoveRandomIcon.Sprite, amount, Colors.textMain);
		} else if (dir < 0) {
			return new Icon(ModEntry.Instance.BacktrackMoveLeftIcon.Sprite, amount, Colors.textMain);
		} else {
			return new Icon(ModEntry.Instance.BacktrackMoveRightIcon.Sprite, amount, Colors.textMain);
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
				new CustomTTGlossary(
					GlossaryType.action,
					() => ModEntry.Instance.BacktrackMoveRightIcon.Sprite,
					() => ModEntry.Instance.Localizations.Localize(["action", "backtrackMove", "name"]),
					() => ModEntry.Instance.Localizations.Localize(["action", "backtrackMove", "description"], new { Amount = GetDisplayAmount(s) }),
					key: "action.backtrackMove"
				)
			];
		} if (isRandom) {
			return [
				new CustomTTGlossary(
					GlossaryType.action,
					() => ModEntry.Instance.BacktrackMoveRandomIcon.Sprite,
					() => ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveRandom", "name"]),
					() => ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveRandom", "description"], new { Amount = GetDisplayAmount(s) }),
					key: "action.backtrackMoveRandom"
				)
			];
		} else if (dir < 0) {
			return [
				new CustomTTGlossary(
					GlossaryType.action,
					() => ModEntry.Instance.BacktrackMoveLeftIcon.Sprite,
					() => ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveLeft", "name"]),
					() => ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveLeft", "description"], new { Amount = GetDisplayAmount(s) }),
					key: "action.backtrackMoveLeft"
				)
			];
		} else if (dir > 0) {
			return [
				new CustomTTGlossary(
					GlossaryType.action,
					() => ModEntry.Instance.BacktrackMoveRightIcon.Sprite,
					() => ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveRight", "name"]),
					() => ModEntry.Instance.Localizations.Localize(["action", "backtrackMoveRight", "description"], new { Amount = GetDisplayAmount(s) }),
					key: "action.backtrackMoveRight"
				)
			];
		} else {
			return [];
		}
	}
}
