using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using TheJazMaster.Nibbs.Characters;

namespace TheJazMaster.Nibbs.Features.Nibbs;

public class BackflipManager
{
	internal static ModEntry Instance => ModEntry.Instance;
	internal static NibbsManager NibbsManager => Instance.NibbsManager;
	internal static Harmony Harmony => ModEntry.Instance.Harmony;


	public BackflipManager() : base()
	{
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetDataWithOverrides)),
			transpiler: new HarmonyMethod(GetType(), nameof(Card_GetDataWithOverrides_Transpiler))
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
			.Replace([
                new(OpCodes.Cgt),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(HasBackflip))),
                new(OpCodes.Or),
                new(OpCodes.Brfalse, branch.Value)
            ])
            .AllElements();
    }

    private static bool HasBackflip(Card card, State state) =>
        state.ship.Get(NibbsManager.BackflipStatus) > 0;

}