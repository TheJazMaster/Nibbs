namespace TheJazMaster.Nibbs;

public interface INibbsApi
{

	Deck NibbsDeck { get; }
	Status BacktrackLeftStatus { get; }
	Status BacktrackRightStatus { get; }
	Status ReversibleAutotodgeLeftStatus { get; }
	Status ReversibleAutotodgeRightStatus { get; }
	Status BlurStatus { get; }
	Status BackflipStatus { get; }
	CardAction MakeReversibleMove(int dir, bool isRandom);
}
