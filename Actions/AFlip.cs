using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FSPRO;
using Microsoft.Extensions.Logging;
using Nickel;

namespace TheJazMaster.Nibbs.Actions;

public class AFlip : CardAction
{
	public int dir = 0;

	public override void Begin(G g, State s, Combat c)
	{
		foreach (StuffBase item in GetAffectedMidrow(s, c)) {
			item.targetPlayer = !item.targetPlayer;
		}
		Audio.Play(Event.Status_PowerUp);
	}

	private IEnumerable<StuffBase> GetAffectedMidrow(State s, Combat c) {
		int intervalFloor = -999; int intervalCeil = 999; bool equals = true;
		if (dir != 0) {
			int count = s.ship.parts.Where(p => p.type == PType.missiles && p.active).Count();
			if (count > 1) {
				var indices = s.ship.parts.Where(p => p.type == PType.missiles && p.active).Select((p, x) => s.ship.x + x);
				intervalFloor = indices.Min();
				intervalCeil = indices.Max();
				if (dir < 0) equals = false;
			}
			else if (count == 1) {
				int bayX = s.ship.x + s.ship.parts.Select((p, x) => new {
					part = p,
					pos = x
				}).Where(p => p.part.type == PType.missiles && p.part.active).First().pos;		
				if (dir < 0) {
					intervalCeil = bayX;
				} else {
					intervalFloor = bayX;
				}
			}
			else {
				if (dir < 0) {
					intervalCeil = s.ship.x + (int)Math.Ceiling(s.ship.parts.Count / 2.0);
				} else {
					intervalFloor = s.ship.x + s.ship.parts.Count / 2;
				}
			}
		}
		
		foreach ((int x, StuffBase item) in c.stuff) {
			if ((x > intervalFloor && x < intervalCeil) == equals)
				yield return item;
		}
	}

	private string TooltipKey() {
		if (dir < 0) return "left";
		if (dir > 0) return "right";
		return "all";
	} 

	public override Icon? GetIcon(State s) => 
		new Icon(dir switch {
			var x when x < 0 => ModEntry.Instance.FlipLeftIcon,
			var x when x > 0 => ModEntry.Instance.FlipRightIcon,
			var _ => ModEntry.Instance.FlipIcon
		}, null, Colors.textMain);

	public override List<Tooltip> GetTooltips(State s) {
		if (s.route is Combat combat) {
			foreach (StuffBase value in GetAffectedMidrow(s, combat)) {
				value.hilight = 2;
			}
		}
		return [
			new GlossaryTooltip(dir == 0 ? "action.flip" : "action.flipDirectional") {
				Icon = ModEntry.Instance.FlipIcon,
				TitleColor = Colors.action,
				Title = ModEntry.Instance.Localizations.Localize(["action", "flip", "name", TooltipKey()]),
				Description = ModEntry.Instance.Localizations.Localize(["action", "flip", "description", dir == 0 ? "all" : "directional"],
					dir == 0 ? null :
					new { Dir = dir < 0 ? ModEntry.Instance.Localizations.Localize(["action", "flip", "left"]) : ModEntry.Instance.Localizations.Localize(["action", "flip", "right"]) }
				)
			}
		];
	}
}
