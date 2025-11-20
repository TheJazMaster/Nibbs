
using System;
using System.Linq;
using System.Net;
using FMOD;
using FSPRO;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Nickel;
using TheJazMaster.Nibbs.Artifacts;

namespace TheJazMaster.Nibbs.Features;

[HarmonyPatch]
public class AngledAttacksManager
{
	private static Harmony Harmony => ModEntry.Instance.Harmony;
	private static IModData ModData => ModEntry.Instance.Helper.ModData;

	public static readonly string RefractKey = "RefractTimes";
	public static readonly string SidewaysAttackKey = "SidewaysDir";
	public static readonly string SidewaysHitKey = "SidewaysHit"; // On RaycastResult


	public enum Sideways {
		NONE, LEFT, RIGHT
	}

	public AngledAttacksManager() {}

    private static bool ignoreAutoaim = false;

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CombatUtils), nameof(CombatUtils.RaycastGlobal))]
	private static void CombatUtils_RaycastGlobal_Prefix(Combat c, Ship target, bool fromDrone, ref int worldX) {
		if (ignoreAutoaim || AffectDamageDoneManager.AttackContext == null || !fromDrone) return;

        var side = ModData.GetModDataOrDefault(AffectDamageDoneManager.AttackContext, SidewaysAttackKey, Sideways.NONE);

		if (side == Sideways.NONE) return;

		worldX += side == Sideways.LEFT ? -2 : 2;
	}

    [HarmonyPostfix]
	[HarmonyPatch(typeof(CombatUtils), nameof(CombatUtils.RaycastGlobal))]
	private static void CombatUtils_RaycastGlobal_Postfix(Combat c, Ship target, bool fromDrone, int worldX, ref RaycastResult __result) {
		if (ignoreAutoaim || __result.hitShip || AffectDamageDoneManager.AttackContext == null || !fromDrone) return;

        var side = ModData.GetModDataOrDefault(AffectDamageDoneManager.AttackContext, SidewaysAttackKey, Sideways.NONE);

		if (side == Sideways.NONE) return;

        ignoreAutoaim = true;
        int sideAdjustment = side == Sideways.LEFT ? -1 : 1;
        
		var oneFurther = CombatUtils.RaycastGlobal(c, target, fromDrone, worldX + sideAdjustment).ApplyModData(SidewaysHitKey, sideAdjustment);
        if (oneFurther.hitShip) {
            __result = oneFurther;
			ignoreAutoaim = false;
			return;
        }

        if (MG.inst.g.state.EnumerateAllArtifacts().OfType<CorrectiveLensesArtifact>().FirstOrDefault() is { } artifact) {
            var oneCloser = CombatUtils.RaycastGlobal(c, target, fromDrone, worldX - sideAdjustment);
            if (oneCloser.hitShip)
            {
                artifact.Pulse();
                __result = oneCloser;
                ignoreAutoaim = false;
                return;
            }
        }
		ignoreAutoaim = false;
    }

	private static void SidewaysCannonTrail(Vec start, Vec end, bool left)
	{
        Vec dist = end - start;
        double num = Math.Min(1000, Math.Sqrt(dist.x*dist.x + dist.y*dist.y));
		double num2 = Mutil.NextRand();
		for (int i = 0; i < num; i++)
		{
			Vec vel = Mutil.RandVel() * 5;
            Vec randVec = Mutil.RandBox01();
            Vec pos = new Vec(start.x, start.y) + randVec.x * dist;
            pos.y += randVec.y * 4 - 2;
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

	[HarmonyPrefix]
	[HarmonyPatch(typeof(EffectSpawner), nameof(EffectSpawner.Cannon))]
	private static bool EffectSpawner_Cannon_Prefix(G g, bool targetPlayer, RaycastResult ray, DamageDone dmg, bool isBeam) {
		if (AffectDamageDoneManager.AttackContext == null || AffectDamageDoneManager.AttackContext.fromDroneX == null || g.state.route is not Combat combat) return true;

		var side = ModData.GetModDataOrDefault(AffectDamageDoneManager.AttackContext, SidewaysAttackKey, Sideways.NONE);

        if (side == Sideways.NONE) return true;

        int sidewaysHit = ModData.GetModDataOrDefault(ray, SidewaysHitKey, 0);

        Vec start = FxPositions.Drone(AffectDamageDoneManager.AttackContext.fromDroneX.Value) + new Vec(side == Sideways.LEFT ? -3 : 3, 0);
        Vec end;
        if (sidewaysHit == 0)
			end = !dmg.hitShield ? (FxPositions.Hull(ray.worldX, targetPlayer) + new Vec(0, targetPlayer ? -10 : 10)) : FxPositions.Shield(ray.worldX, targetPlayer);
		else {
            if (!dmg.hitShield) {
                end = FxPositions.Hull(ray.worldX - sidewaysHit, targetPlayer) + new Vec(0, targetPlayer ? -10 : 10);
                Vec diff = end - start;
                end += diff / (diff.x * sidewaysHit) * 8; // Add enough to increase x by 8
            }
            else {
				end = FxPositions.Shield(ray.worldX - sidewaysHit, targetPlayer) + new Vec(0, targetPlayer ? 16 : -16);
				Vec diff = end - start;
                end += diff / (diff.x * sidewaysHit) * 4; // Add enough to increase x by 4
			}
        }

        if (!ray.hitShip) end += (end - start) * 5;
        combat.fx.Add(new CannonBeamAngled
		{
			start = start,
			end = end
		});
		SidewaysCannonTrail(start, end, side == Sideways.LEFT);
        GUID? gUID = null;
		if (isBeam) {
			gUID = (ray.hitDrone || ray.hitShip) ? Event.Hits_BeamHit : Event.Hits_BeamMiss;
		}
		else
		{
			if (ray.hitShip)
			{
				if (sidewaysHit != 0)
					SidewaysHullImpact(g, end, side == Sideways.LEFT, !ray.hitDrone, ray.fromDrone);
				else 
					ParticleBursts.HullImpact(g, end, targetPlayer, !ray.hitDrone, ray.fromDrone);
			}
			if (dmg.hitShield && !dmg.hitHull)
			{
                if (sidewaysHit != 0) {
					combat.fx.Add(new SidewaysShieldHit
					{
						pos = end
					});
                    SidewaysShieldImpact(g, end, side == Sideways.LEFT);
                }
                else {
					combat.fx.Add(new ShieldHit
					{
						pos = end
					});
                    ParticleBursts.ShieldImpact(g, end, targetPlayer);
                }
            }
			if (dmg.poppedShield)
			{
				if (sidewaysHit != 0)
					combat.fx.Add(new SidewaysShieldPop
					{
						pos = end
					});
				else
					combat.fx.Add(new ShieldPop
					{
						pos = end
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

	public static void SidewaysHullImpact(G g, Vec hitPos, bool left, bool isDirectional, bool fromDrone)
	{
		for (int i = 0; i < 50; i++)
		{
			Vec vel = Mutil.RandVel() * 150.0;
			if (isDirectional)
			{
				vel.x = (left ? (3) : -3) * Math.Abs(vel.y);
			}
			if (fromDrone)
			{
				vel.y *= 0.4;
			}
			PFX.combatAdd.Add(new Particle
			{
				pos = hitPos,
				size = 1.0 + 5.0 * Mutil.NextRand(),
				vel = vel,
				color = new Color(1.0, 0.6, 0.2),
				dragCoef = 12.0 + 4.0 * Mutil.NextRand(),
				lifetime = 0.0 + 1.0 * Mutil.NextRand()
			});
		}
	}

	public static void SidewaysShieldImpact(G g, Vec hitPos, bool left)
	{
		for (int i = 0; i < 60; i++)
		{
			Vec vel = Mutil.RandVel() * 80.0;
			vel.x = (left ? 1 : -1) * Math.Abs(vel.x);
			PFX.combatAdd.Add(new Particle
			{
				pos = hitPos,
				size = 1.0 + 2.0 * Mutil.NextRand(),
				vel = vel,
				color = Colors.healthBarShield.gain(0.5 + Mutil.NextRand() * 0.5),
				dragCoef = 2.0 + 2.0 * Mutil.NextRand(),
				lifetime = 0.0 + 1.0 * Mutil.NextRand(),
				gravity = 0.0
			});
		}
	}

	public class CannonBeamAngled : FX
    {
        public Vec start;
        public Vec end;
        public double w;

        public static Color cannonBeam = new Color("ff8866");

        public static Color cannonBeamCore = new Color("ffffff");

        public override void Render(G g, Vec v)
        {
            double num = 0.05;
            if (age < num)
            {
                double num2 = 2.0 * (1.0 - age / num);

                Draw.Line(v.x + start.x, v.y + start.y, v.x + end.x, v.y + end.y, w + 2.0 * (num2 + 1.0), cannonBeam, BlendMode.Screen);
                Draw.Line(v.x + start.x, v.y + start.y, v.x + end.x, v.y + end.y, w + num2 * 2.0, cannonBeamCore);
            }
        }
    }
	public class SidewaysShieldHit : ShieldHit
	{
        public override void Render(G g, Vec v)
		{
			double num = 0.4;
			if (age < num)
			{
				double num2 = Math.Sin(Math.PI * age / num);
				int num3 = 1;
				Spr? clamped = sprites.GetClamped((int)(num2 * (double)sprites.Count));
				double x = v.x + pos.x;
				double y = v.y + pos.y;
				Vec? originRel = new Vec(0.5, 0.5);
				BlendState add = BlendMode.Add;
				Color? color = new Color(1.0, 1.0, 1.0).gain(num3);
				Draw.Sprite(clamped, x, y, flipX: false, flipY: false, Math.PI / 2, null, originRel, null, null, color, add);
			}
		}
	}
	public class SidewaysShieldPop : ShieldPop
	{
		public override void Render(G g, Vec v)
		{
			double num = 0.3;
			if (age < num)
			{
				double num2 = age / num;
				double n = 2.0 * (1.0 - age / num);
				Spr? clamped = sprites.GetClamped((int)(num2 * (double)sprites.Count));
				double x = v.x + pos.x;
				double y = v.y + pos.y;
				Vec? originRel = new Vec(0.5, 0.5);
				BlendState add = BlendMode.Add;
				Color? color = new Color(1.0, 1.0, 1.0).gain(n);
				Draw.Sprite(clamped, x, y, flipX: false, flipY: false, Math.PI / 2, null, originRel, null, null, color, add);
			}
		}
	}
}