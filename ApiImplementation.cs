using TheJazMaster.Nibbs.Actions;

namespace TheJazMaster.Nibbs;

public class ApiImplementation : INibbsApi
{

	public Deck NibbsDeck => ModEntry.Instance.NibbsDeck.Deck;
	public Status BacktrackLeftStatus => ModEntry.Instance.BacktrackLeftStatus.Status;
	public Status BacktrackRightStatus => ModEntry.Instance.BacktrackRightStatus.Status;
	public Status ReversibleAutotodgeLeftStatus => ModEntry.Instance.BacktrackAutododgeLeftStatus.Status;
	public Status ReversibleAutotodgeRightStatus => ModEntry.Instance.BacktrackAutododgeRightStatus.Status;
	public Status BlurStatus => ModEntry.Instance.SmokescreenStatus.Status;
	public Status BackflipStatus => ModEntry.Instance.BackflipStatus.Status;
	public CardAction MakeReversibleMove(int dir, bool isRandom = false) => new ABacktrackMove {
		dir = dir,
		targetPlayer = true,
		isRandom = isRandom
	};
}
