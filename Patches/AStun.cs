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
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Artifacts;

namespace TheJazMaster.Nibbs.Patches;

public class AStunPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AStunPart), nameof(AStunPart.Begin)),
			prefix: new HarmonyMethod(typeof(AStunPatches), nameof(AStunPart_Begin_Prefix)),
			postfix: new HarmonyMethod(typeof(AStunPatches), nameof(AStunPart_Begin_Postfix))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AStunShip), nameof(AStunShip.Begin)),
			prefix: new HarmonyMethod(typeof(AStunPatches), nameof(AStunShip_Begin_Postfix))
		);
    }

	private static void AStunPart_Begin_Prefix(G g, State s, Combat c, AStunPart __instance, out Intent? __state) {
		Part? partAtWorldX = c.otherShip.GetPartAtWorldX(__instance.worldX);
		__state = partAtWorldX?.intent ?? null;
	}

	private static void AStunPart_Begin_Postfix(G g, State s, Combat c, AStunPart __instance, Intent? __state) {
		if (__state == null) return;

		foreach (Artifact item in s.EnumerateAllArtifacts()) {
			if (item is IOnStunArtifact artifact) {
				artifact.OnStun(s, c, IOnStunArtifact.StunType.Part, __state);
			}
		}
	}

	private static void AStunShip_Begin_Postfix(G g, State s, Combat c) {
		foreach (Artifact item in s.EnumerateAllArtifacts()) {
			if (item is IOnStunArtifact artifact) {
				artifact.OnStun(s, c, IOnStunArtifact.StunType.Total);
			}
		}
	}
}
