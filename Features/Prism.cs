
using System;
using System.Buffers;
using System.Collections.Generic;
using FMOD;
using FSPRO;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nickel;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Patches;

namespace TheJazMaster.Nibbs.Features;

public class PrismManager
{
	private static Harmony Harmony => ModEntry.Instance.Harmony;
	private static IModData ModData => ModEntry.Instance.Helper.ModData;

	public static readonly string RefractKey = "RefractTimes";
	public static readonly string SidewaysAttackKey = "SidewaysDir";

	public enum Sideways {
		NONE, LEFT, RIGHT
	}

	public PrismManager() {
		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(CombatUtils), nameof(CombatUtils.RaycastGlobal)),
			prefix: new HarmonyMethod(GetType(), nameof(CombatUtils_RaycastGlobal_Prefix))
		);
		Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(EffectSpawner), nameof(EffectSpawner.Cannon)),
			prefix: new HarmonyMethod(GetType(), nameof(EffectSpawner_Cannon_Prefix))
		);
	}

	private static bool CombatUtils_RaycastGlobal_Prefix(Combat c, Ship target, bool fromDrone, int worldX, ref RaycastResult __result) {
		if (AffectDamageDoneManager.AttackContext == null || !fromDrone) return true;

		var side = ModData.GetModDataOrDefault(AffectDamageDoneManager.AttackContext, SidewaysAttackKey, Sideways.NONE);

		if (side == Sideways.NONE) return true;

		int mul = side == Sideways.LEFT ? -1 : 1;
		for (int i = mul; i*mul < 50; i += mul) {
			if (c.stuff.ContainsKey(worldX + i)) {
				__result = new RaycastResult {
					hitShip = false,
					hitDrone = true,
					worldX = worldX + i,
					fromDrone = fromDrone
				};
				return false;
			}
		}
		__result = new RaycastResult {
			hitShip = false,
			hitDrone = false,
			worldX = worldX + 50*mul,
			fromDrone = fromDrone
		};
		return false;
	}

	private static void SidewaysCannonTrail(Rect rect)
	{
		double num = Math.Min(1000, rect.w);
		double num2 = Mutil.NextRand();
		for (int i = 0; i < num; i++)
		{
			Vec vel = Mutil.RandVel() * 5;
			Vec pos = new Vec(rect.x, rect.y) + Mutil.RandBox01() * new Vec(rect.w, rect.h);
			PFX.combatAdd.Add(new Particle
			{
				pos = pos,
				size = 1 + Mutil.NextRand(),
				vel = vel,
				color = new Color(0.3, 0.3, 0.5),
				dragCoef = Mutil.NextRand(),
				dragVel = new Vec(Math.Sin(num2 * 6.28 + pos.x * 0.3)) * 7,
				lifetime = 0.3 + 1.4 * Mutil.NextRand(),
				gravity = 0
			});
		}
	}

	private static bool EffectSpawner_Cannon_Prefix(G g, bool targetPlayer, RaycastResult ray, DamageDone dmg, bool isBeam) {
		if (AffectDamageDoneManager.AttackContext == null || AffectDamageDoneManager.AttackContext.fromDroneX == null || g.state.route is not Combat combat) return true;

		var side = ModData.GetModDataOrDefault(AffectDamageDoneManager.AttackContext, SidewaysAttackKey, Sideways.NONE);

		if (side == Sideways.NONE) return true;


		Rect rect = Rect.FromPoints(
			FxPositions.Drone(AffectDamageDoneManager.AttackContext.fromDroneX.Value) + new Vec(side == Sideways.LEFT ? -3 : 3, 0),
			FxPositions.Drone(ray.worldX) + new Vec(side == Sideways.LEFT ? 3 : -3, 0)
		);
		combat.fx.Add(new CannonBeam
		{
			rect = rect
		});
		SidewaysCannonTrail(rect);
		GUID? gUID = null;
		if (isBeam) {
			gUID = (ray.hitDrone || ray.hitShip) ? Event.Hits_BeamHit : Event.Hits_BeamMiss;
		}
		else
		{
			if (ray.hitShip)
			{
				Vec hitPos = new(rect.x, targetPlayer ? rect.y2 : rect.y);
				ParticleBursts.HullImpact(g, hitPos, targetPlayer, !ray.hitDrone, ray.fromDrone);
			}
			if (dmg.hitShield && !dmg.hitHull)
			{
				combat.fx.Add(new ShieldHit
				{
					pos = FxPositions.Shield(ray.worldX, targetPlayer)
				});
				ParticleBursts.ShieldImpact(g, FxPositions.Shield(ray.worldX, targetPlayer), targetPlayer);
			}
			if (dmg.poppedShield)
			{
				combat.fx.Add(new ShieldPop
				{
					pos = FxPositions.Shield(ray.worldX, targetPlayer)
				});
			}
			if (dmg.poppedShield)
			{
				gUID = Event.Hits_ShieldPop;
			}
			else if (dmg.hitShield)
			{
				gUID = Event.Hits_ShieldHit;
			}
			if (!ray.hitDrone && !ray.hitShip)
			{
				gUID = Event.Hits_Miss;
			}
			else if (dmg.hitHull)
			{
				gUID = (!targetPlayer) ? new GUID?(Event.Hits_OutgoingHit) : new GUID?(Event.Hits_HitHurt);
			}
			else if (ray.hitDrone)
			{
				gUID = Event.Hits_HitDrone;
			}
		}
		if (gUID.HasValue)
		{
			Audio.Play(gUID.Value);
		}
		return false;
	}

	public abstract class Prism : StuffBase {
		public override List<CardAction>? GetActionsOnShotWhileInvincible(State s, Combat c, bool wasPlayer, int damage)
		{
			if (AffectDamageDoneManager.AttackContext == null) {
				c.DestroyDroneAt(s, x, wasPlayer);
				return null;
			}
			var attack = Mutil.DeepCopy(AffectDamageDoneManager.AttackContext);
			attack.fromDroneX = x;
			ModData.SetModData(attack, RefractKey, ModData.GetModDataOrDefault(attack, RefractKey, 0) + 1);
			var ret = Refract(s, c, attack);
			if (ret != null) return [new ARefractedAttack {
				attacks = ret
			}];
			else return null;
		}

		public virtual List<CardAction>? Refract(State s, Combat c, in AAttack attack) => null;
	}

	public class RegularPrism : Prism
	{
		public override List<CardAction>? Refract(State s, Combat c, in AAttack attack) {
			if (ModData.GetModDataOrDefault(attack, SidewaysAttackKey, Sideways.NONE) != Sideways.NONE || attack.targetPlayer == this.targetPlayer) {
				if (attack.paybackCounter > 0) {
					return [attack];
				} else {
					Sideways dir = ModData.GetModDataOrDefault(attack, SidewaysAttackKey, Sideways.NONE);
					return dir switch {
						Sideways.LEFT or Sideways.RIGHT => [
							Aim(Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.NONE))
						],
						_ => [
							Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.LEFT),
							Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.RIGHT),
							Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.NONE)
						]
					};
				}
			} else {
				c.DestroyDroneAt(s, x, !attack.targetPlayer);
				return [];
			}

			AAttack Aim(AAttack attack) {
				attack.targetPlayer = !targetPlayer;
				return attack;
			}
		}

		public override bool Invincible() => AffectDamageDoneManager.AttackContext != null && (ModData.GetModDataOrDefault(AffectDamageDoneManager.AttackContext, SidewaysAttackKey, Sideways.NONE) != Sideways.NONE 
				|| AffectDamageDoneManager.AttackContext.targetPlayer == this.targetPlayer);

		public override Spr? GetIcon() => ModEntry.Instance.PrismIcon;

		public override void Render(G g, Vec v)
		{
			DrawWithHilight(g, ModEntry.Instance.PrismSprite, v + GetOffset(g), Mutil.Rand(x + 0.1) > 0.5, targetPlayer);
		}

		public override List<Tooltip> GetTooltips() => [
			new GlossaryTooltip("midrow.prism") {
				TitleColor = Colors.midrow,
				Icon = ModEntry.Instance.PrismIcon,
				Title = ModEntry.Instance.Localizations.Localize(["midrow", "prism", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["midrow", "prism", "description"]),
			}
		];
	}

	public class OmniPrism : Prism
	{
		public override List<CardAction>? Refract(State s, Combat c, in AAttack attack)
		{
			Sideways dir = ModData.GetModDataOrDefault(attack, SidewaysAttackKey, Sideways.NONE);
			if (attack.paybackCounter > 0) {
				return [attack];
			} else {
				return dir switch {
					Sideways.LEFT => [
						Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.LEFT),
						Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.NONE),
						Invert(Mutil.DeepCopy(attack)).ApplyModData(SidewaysAttackKey, Sideways.NONE),
					],
					Sideways.RIGHT => [
						Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.RIGHT),
						Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.NONE),
						Invert(Mutil.DeepCopy(attack)).ApplyModData(SidewaysAttackKey, Sideways.NONE),
					],
					_ => [
						Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.LEFT),
						Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.RIGHT),
						Mutil.DeepCopy(attack).ApplyModData(SidewaysAttackKey, Sideways.NONE),
					]
				};
			}

			static AAttack Invert(AAttack attack) {
				attack.targetPlayer = !attack.targetPlayer;
				return attack;
			}
		}

		public override bool Invincible() => AffectDamageDoneManager.AttackContext != null;

		public override Spr? GetIcon() => ModEntry.Instance.OmniPrismIcon;

		public override void Render(G g, Vec v)
		{
			DrawWithHilight(g, ModEntry.Instance.OmniPrismSprite, v + GetOffset(g), false, targetPlayer);
		}

		public override List<Tooltip> GetTooltips() => [
			new GlossaryTooltip("midrow.omniprism") {
				TitleColor = Colors.midrow,
				Icon = ModEntry.Instance.OmniPrismIcon,
				Title = ModEntry.Instance.Localizations.Localize(["midrow", "omniprism", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["midrow", "omniprism", "description"]),
			}
		];
	}
}