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

public class CardPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetDataWithOverrides)),
			transpiler: new HarmonyMethod(typeof(CardPatches), nameof(Card_GetDataWithOverrides_Transpiler))
		);
    }

    private static IEnumerable<CodeInstruction> Card_GetDataWithOverrides_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldarg(1),
                ILMatches.Ldfld("ship"),
                ILMatches.LdcI4((int)Status.tableFlip),
                ILMatches.Call("Get"),
                ILMatches.LdcI4(0),
                ILMatches.Ble.TryGetBranchTarget(out var branch)
            )
            .PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Replace(new List<CodeInstruction> {
                new(OpCodes.Cgt),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CardPatches), nameof(HasBackflip))),
                new(OpCodes.Or),
                new(OpCodes.Brfalse, branch.Value)
            })
            .AllElements();
    }

    private static bool HasBackflip(Card card, State state) =>
        state.ship.Get(ModEntry.Instance.BackflipStatus.Status) > 0;
}
