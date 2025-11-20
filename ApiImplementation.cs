using System.Linq;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Features;

namespace TheJazMaster.Nibbs;

public class ApiImplementation : INibbsApi
{

	public Deck NibbsDeck => ModEntry.Instance.NibbsDeck;
	public Status BacktrackLeftStatus => ModEntry.Instance.BacktrackLeftStatus;
	public Status BacktrackRightStatus => ModEntry.Instance.BacktrackRightStatus;
	public Status ReversibleAutotodgeLeftStatus => ModEntry.Instance.BacktrackAutododgeLeftStatus;
	public Status ReversibleAutotodgeRightStatus => ModEntry.Instance.BacktrackAutododgeRightStatus;
	public Status BlurStatus => ModEntry.Instance.SmokescreenStatus;
	public Status BackflipStatus => ModEntry.Instance.BackflipStatus;
	public CardAction MakeReversibleMove(int dir, bool isRandom = false) => new ABacktrackMove {
		dir = dir,
		targetPlayer = true,
		isRandom = isRandom
	};
	public bool IsBacktrackMovement(AMove move) => ModEntry.Instance.Helper.ModData.TryGetModData(move, BacktrackManager.NoStrafeKey, out bool noStrafe) && noStrafe;
	public bool ShouldBacktrackTriggerStrafe(State s) => !s.EnumerateAllArtifacts().Any(item => item is FledgelingOrbArtifact);

    public AAttack? GetAttackContext() => AffectDamageDoneManager.AttackContext;
}
