using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Features;

namespace TheJazMaster.Nibbs.Patches;

public class AMovePatches
{
	static ModEntry Instance => ModEntry.Instance;
    static IModData ModData => Instance.Helper.ModData;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AMove), nameof(AMove.Begin)),
            prefix: new HarmonyMethod(typeof(AMovePatches), nameof(AMove_Begin_Prefix)),
			postfix: new HarmonyMethod(typeof(AMovePatches), nameof(AMove_Begin_Postfix)),
			transpiler: new HarmonyMethod(typeof(AMovePatches), nameof(AMove_Begin_Transpiler))
		);
    }

    private static bool AMove_Begin_Prefix(AMove __instance, G g, State s, Combat c, ref int __state)
    {
        __state = s.ship.x;
        return true;
    }

    private static void AMove_Begin_Postfix(AMove __instance, G g, State s, Combat c, int __state)
    {
        Status smokescreen = Instance.SmokescreenStatus.Status;
        Ship ship = __instance.targetPlayer ? s.ship : c.otherShip;
        if (ship.Get(smokescreen) > 0) {
            c.QueueImmediate(new AStatus {
                status = Status.tempShield,
                statusAmount = ship.Get(smokescreen),
                targetPlayer = __instance.targetPlayer
            });
        }

        if (ModData.TryGetModData(__instance, BacktrackManager.NoStrafeKey, out bool noStrafe) && noStrafe)
            s.storyVars.ApplyModData(StoryVarsPatches.JustBacktrackedKey, true);

        if (__instance is not ABacktrackMove && __instance.targetPlayer && __instance.dir != 0) {
            foreach (Artifact item in s.EnumerateAllArtifacts()) {
                if (item is EyeOfCobaArtifact artifact && artifact.active) {
                    ABacktrackMove.AddBacktrack(s, c, __state, s.ship.x, __instance);
                    artifact.active = false;
                    artifact.Pulse();
                    break;
                }
            }
        }
    }

    private static IEnumerable<CodeInstruction> AMove_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldloc<Ship>(originalMethod).CreateLdlocInstruction(out var ldLoc),
                ILMatches.LdcI4((int)Enum.Parse<Status>("strafe")),
                ILMatches.Call("Get"),
                ILMatches.LdcI4(0),
                ILMatches.Ble.GetBranchTarget(out var label)
            )
            .PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                ldLoc,
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(AMovePatches), nameof(CheckIfStrafeAllowed))),
                new(OpCodes.Brfalse, label.Value)
            })
            .AllElements();
    }

    private static bool CheckIfStrafeAllowed(Ship ship, State state, Combat combat, AMove move)
    {
        if (ModData.TryGetModData(move, BacktrackManager.NoStrafeKey, out bool value) || !value) return true;
        foreach (Artifact item in state.EnumerateAllArtifacts())
        {
            if (item is FledgelingOrbArtifact artifact) {
                return false;
            }
        }
        return true;
    }   
}
