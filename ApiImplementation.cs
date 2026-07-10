using System.Linq;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Features.Ix;
using TheJazMaster.Nibbs.Features.Nibbs;

namespace TheJazMaster.Nibbs;

public class ApiImplementation : INibbsApi
{

	public Deck NibbsDeck => ModEntry.Instance.NibbsManager.NibbsDeck;
	public Status BacktrackLeftStatus => ModEntry.Instance.NibbsManager.BacktrackLeftStatus;
	public Status BacktrackRightStatus => ModEntry.Instance.NibbsManager.BacktrackRightStatus;
	public Status ReversibleAutotodgeLeftStatus => ModEntry.Instance.NibbsManager.BacktrackAutododgeLeftStatus;
	public Status ReversibleAutotodgeRightStatus => ModEntry.Instance.NibbsManager.BacktrackAutododgeRightStatus;
	public Status BlurStatus => ModEntry.Instance.NibbsManager.SmokescreenStatus;
	public Status BackflipStatus => ModEntry.Instance.NibbsManager.BackflipStatus;
	public CardAction MakeReversibleMove(int dir, bool isRandom = false) => new ABacktrackMove {
		dir = dir,
		targetPlayer = true,
		isRandom = isRandom
	};
	public bool IsBacktrackMovement(AMove move) => ModEntry.Instance.Helper.ModData.TryGetModData(move, BacktrackManager.NoStrafeKey, out bool noStrafe) && noStrafe;
	public bool ShouldBacktrackTriggerStrafe(State s) => !s.EnumerateAllArtifacts().Any(item => item is FledgelingOrbArtifact);

    public AAttack? GetAttackContext() => AffectDamageDoneManager.AttackContext;
}
