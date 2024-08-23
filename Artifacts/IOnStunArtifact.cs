namespace TheJazMaster.Nibbs.Artifacts;

public interface IOnStunArtifact
{
    enum StunType {
        Part,
        Total
    }

    public void OnStun(State state, Combat combat, StunType type, Intent? intent = null);
}