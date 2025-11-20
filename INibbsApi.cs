using System.Collections.Generic;
using TheJazMaster.Nibbs.Features;
using static TheJazMaster.Nibbs.Features.PrismManager;

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
	bool IsBacktrackMovement(AMove move);
	bool ShouldBacktrackTriggerStrafe(State s);

    AAttack? GetAttackContext();

    // public interface IHook
    // {
    // void AffectDamageDone(IAffectDamageDoneArgs args) {}

    // public interface IAffectDamageDoneArgs
    // {
    // 	State State { get; }
    // 	Combat Combat { get; }
    // 	Ship Ship { get; }
    // 	AAttack? AttackContext { get; }
    // 	int? MaybeWorldX { get; }
    // 	bool Piercing { get; set; }
    // 	int Damage { get; set; }
    // }
    // }
}
