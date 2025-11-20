using FSPRO;
using TheJazMaster.Nibbs.Features;

namespace TheJazMaster.Nibbs.Actions;

public class AFluxen : CardAction
{
    public int worldX;
    public bool targetPlayer;
    public bool justTheActiveOverride;

    public override void Begin(G g, State s, Combat c)
    {
        timer *= 0.5;
        Part? partAtWorldX = (targetPlayer ? s.ship : c.otherShip).GetPartAtWorldX(worldX);
        if (partAtWorldX != null) {
            if (justTheActiveOverride)
            {
                partAtWorldX.damageModifierOverrideWhileActive = FluxManager.FluxDamageModifier;
            }
            else
            {
                partAtWorldX.damageModifier = FluxManager.FluxDamageModifier;
            }
            Audio.Play(Event.Status_PowerDown);
        }
        else {
            timer = 0;
        }
    }
}