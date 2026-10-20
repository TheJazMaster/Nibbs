using System;
using System.Collections.Generic;
using System.Linq;
using Nickel;
using TheJazMaster.Nibbs.Characters;
using TheJazMaster.Nibbs.Features.Ix;

namespace TheJazMaster.Nibbs.Actions;

public class AKillPrisms : CardAction
{
	private static ModEntry Instance => ModEntry.Instance;
	private static IxManager IxManager => Instance.IxManager;

    public override void Begin(G g, State s, Combat c)
	{
		timer = 0;
		int amt = 0;
		foreach (var (x, thing) in c.stuff) {
			if (thing is PrismManager.RefractorObject ro && ro.refractor.GetType() == typeof(PrismManager.Prism)) {
				c.DestroyDroneAt(s, x, true);
				amt++;
			}
		}
		if (amt > 0) {
			c.QueueImmediate(new AStatus {
				status = Status.droneShift,
				statusAmount = amt,
				targetPlayer = true
			});
		}
	}

    public override Icon? GetIcon(State s) => new Icon(IxManager.DestroyPrismsIcon, null, Colors.white);

    public override List<Tooltip> GetTooltips(State s) => [
		new GlossaryTooltip("action.destroyPrisms") {
			TitleColor = Colors.action,
			Icon = IxManager.DestroyPrismsIcon,
			Title = Instance.Localizations.Localize(["action", "destroyPrisms", "name"]),
			Description = Instance.Localizations.Localize(["action", "destroyPrisms", "description"]),
		},
		.. StatusMeta.GetTooltips(Status.droneShift, 1)
	];
}
