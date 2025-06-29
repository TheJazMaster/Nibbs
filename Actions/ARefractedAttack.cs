using System.Collections.Generic;

namespace TheJazMaster.Nibbs.Actions;

public class ARefractedAttack : CardAction
{
	public required List<CardAction> attacks;

	public override void Begin(G g, State s, Combat c)
	{
		for (int i = 0; i < c.cardActions.Count; i++) {
			if (c.cardActions[i] is ARefractedAttack refAttack) {
				c.cardActions.RemoveAt(i--);
				attacks.AddRange(refAttack.attacks);
			}
		}
		foreach (AAttack attack in attacks) {
			attack.Begin(g, s, c);
		}
	}
}
