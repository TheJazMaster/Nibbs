using Nickel;

namespace TheJazMaster.Nibbs;

public interface IJohnsonApi
{
	IDeckEntry JohnsonDeck { get; }
	IStatusEntry CrunchTimeStatus { get; }
}