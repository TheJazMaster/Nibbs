using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using TheJazMaster.Nibbs.Features;

namespace TheJazMaster.Nibbs.Artifacts;

internal sealed class DiplomaticImmunityArtifact : Artifact, IRegisterableArtifact
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableArtifact.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, [ArtifactPool.Boss], helper, package, out _);
	}

	// public override void OnTurnStart(State state, Combat combat)
	// {
    //     int n = 0;
    //     foreach (Part part in state.ship.parts)
	// 	{
	// 		if (part.type == PType.cockpit)
	// 		{
	// 			// Remove the hardmode brittling
	// 			if (combat.cardActions.Where(action => action is ABrittle brittle && brittle.worldX == state.ship.x + n).FirstOrDefault() is { } action)
    //                 combat.cardActions.Remove(action);

    //             combat.QueueImmediate(new AWeaken
	// 			{
	// 				targetPlayer = true,
	// 				worldX = state.ship.x + n
	// 			});
	// 		}
	// 		n++;
	// 	}
	// }

    public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
    {
        return -1;
    }
}

