using System.Collections.Generic;
using System.Linq;

namespace TheJazMaster.Nibbs.Actions;

public class ARefractedAttack : CardAction
{
	public required List<CardAction> attacks;
    public StuffBase? destroyAfter;
    public bool wasPlayer;

    public override void Begin(G g, State s, Combat c)
	{
		for (int i = 0; i < c.cardActions.Count; i++) {
			if (c.cardActions[i] is ARefractedAttack refAttack) {
				c.cardActions.RemoveAt(i--);
				attacks.AddRange(refAttack.attacks);
			}
		}
		foreach (AAttack attack in attacks.OfType<AAttack>()) {
			attack.Begin(g, s, c);
		}
        c.cardActions.Sort((e1, e2) => e1 is AStunPart ? -1 : (e2 is AStunPart ? 1 : 0));
        if (destroyAfter != null) {
            c.DestroyDroneAt(s, destroyAfter.x, wasPlayer);
        }

	}
}
