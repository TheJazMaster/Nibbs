using HarmonyLib;
using Nickel;

namespace TheJazMaster.Nibbs.Features;

[HarmonyPatch]
public class AffectDamageDoneManager
{
	private static Harmony Harmony => ModEntry.Instance.Harmony;
	private static IModData ModData => ModEntry.Instance.Helper.ModData;


	internal static AAttack? AttackContext = null;

	// private static readonly Pool<AffectDamageDoneArgs> AffectDamageDonePool = new(() => new());

	public AffectDamageDoneManager() {
		// Harmony.TryPatch(
		// 	logger: ModEntry.Instance.Logger,
		// 	original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
		// 	prefix: new HarmonyMethod(GetType(), nameof(AAttack_Begin_Prefix)),
		// 	finalizer: new HarmonyMethod(GetType(), nameof(AAttack_Begin_Finalizer))
		// );

		// ModEntry.Instance.Harmony.TryPatch(
		// 	logger: ModEntry.Instance.Logger,
		// 	original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.NormalDamage)),
		// 	prefix: new HarmonyMethod(GetType(), nameof(Ship_NormalDamage_Prefix))
		// );
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
	private static void AAttack_Begin_Prefix(AAttack __instance) {
		AttackContext = __instance;
	}

	[HarmonyFinalizer]
	[HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
	private static void AAttack_Begin_Finalizer() {
		AttackContext = null;
	}

	// private static void Ship_NormalDamage_Prefix(State s, Combat c, ref int incomingDamage, int? maybeWorldGridX, Ship __instance, ref bool piercing) {
	// 	int damage = incomingDamage; bool isPiercing = piercing;
	// 	AffectDamageDonePool.Do(args => {
	// 		args.State = s;
	// 		args.Ship = __instance;
	// 		args.Combat = c;
	// 		args.AttackContext = AttackContext;
	// 		args.Damage = damage;
	// 		args.Piercing = isPiercing;
	// 		args.MaybeWorldX = maybeWorldGridX;

	// 		foreach (INibbsApi.IHook hook in ModEntry.Instance.HookManager.GetHooksWithProxies(ModEntry.Instance.Helper.Utilities.ProxyManager, s.EnumerateAllArtifacts())) {
	// 			hook.AffectDamageDone(args);
	// 		}

	// 		isPiercing = args.Piercing;
	// 		damage = args.Damage;
	// 	});
	// 	piercing = isPiercing;
	// 	incomingDamage = damage;
	// }

	


	// private sealed class AffectDamageDoneArgs : INibbsApi.IHook.IAffectDamageDoneArgs
	// {
	// 	public State State { get; set; } = null!;
	// 	public Combat Combat { get; set; } = null!;
	// 	public Ship Ship { get; set; } = null!;

	// 	public AAttack? AttackContext { get; set; } = null!;
	// 	public int? MaybeWorldX { get; set; } = null!;

	// 	public bool Piercing { get; set; }

	// 	public int Damage { get; set; }
	// }
}