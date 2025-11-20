namespace TheJazMaster.Nibbs;

public interface IMoreDifficultiesApi
{
	void RegisterAltStarters(Deck deck, StarterDeck starterDeck);
    bool HasAltStarters(Deck deck);
    bool AreAltStartersEnabled(State state, Deck deck);
}
