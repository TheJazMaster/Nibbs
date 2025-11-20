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

namespace TheJazMaster.Nibbs.Patches;

public class CharacterPatches
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;

    public static void Apply()
    {
        // Harmony.TryPatch(
		//     logger: Instance.Logger,
		//     original: AccessTools.DeclaredMethod(typeof(Character), nameof(Character.DrawFace)),
		// 	postfix: new HarmonyMethod(typeof(CharacterPatches), nameof(Character_DrawFace_Postfix))
		// );
    }

    private static readonly List<IxEye> eyePositions = [
        new(new(32, 42)), new(new(44, 42)), new(new(32, 46), true), new(new(44, 46), true)
    ];

    private static void Character_DrawFace_Postfix(Character __instance, G g, double x, double y, bool flipX, string animTag, double animationFrame, bool mini, bool? isSelected, bool renderLocked, bool hideFace) {
        if (!mini && __instance.type == ModEntry.Instance.IxCharacter.CharacterType) {
            double change = Math.Sin(animationFrame)*3;
            foreach (IxEye eye in eyePositions) {
                Glow.Draw(new Vec(x, y) + eye.position, (eye.isSmall ? 8 : 20) + change, new Color(0.2, 0.4, 0.3));
            }
        }
    }

    private struct IxEye(Vec position, bool isSmall = false) {
        public Vec position = position;
        public bool isSmall = isSmall;
    }
}
