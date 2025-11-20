
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using FSPRO;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using Nickel;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Patches;
using static TheJazMaster.Nibbs.Features.AngledAttacksManager;
using static TheJazMaster.Nibbs.Features.PrismManager;

namespace TheJazMaster.Nibbs.Features;

[HarmonyPatch]
public class PrismManager
{
	private static IModData ModData => ModEntry.Instance.Helper.ModData;

	private static AAttack CopyAttack(AAttack attack) {
		var ret = Mutil.DeepCopy(attack);
		if (ret.cardOnHit != null) ret.cardOnHit = ret.cardOnHit.CopyWithNewId();
		return ret;
	}

	public class RefractorObject : StuffBase
	{
        public required RefractorStats refractor;

        public override List<CardAction>? GetActionsOnShotWhileInvincible(State s, Combat c, bool wasPlayer, int damage)
		{
            if (!refractor.DoesRefract(this) || AffectDamageDoneManager.AttackContext == null) {
				if (bubbleShield) {
                    bubbleShield = false;
                    return null;
                }
				c.DestroyDroneAt(s, x, wasPlayer);
				return null;
			}
			var attack = CopyAttack(AffectDamageDoneManager.AttackContext);
			attack.fromDroneX = x;
			ModData.SetModData(attack, RefractKey, ModData.GetModDataOrDefault(attack, RefractKey, 0) + 1);
			var ret = refractor.Refract(s, c, attack, this);
			if (ret != null) return [new ARefractedAttack {
				attacks = ret,
				destroyAfter = refractor.DestroyAfter(s, c, attack, this) ? this : null,
				wasPlayer = wasPlayer
			}];
			else return null;
		}

        public override List<CardAction>? GetActionsOnDestroyed(State s, Combat c, bool wasPlayer, int worldX)
        {
            if (wasPlayer && s.EnumerateAllArtifacts().OfType<HealingCrystalsArtifact>().FirstOrDefault() is { } artifact && artifact.active) {
                artifact.active = false;
                return [
					new AHeal {
						healAmount = 1,
						targetPlayer = true,
						artifactPulse = artifact.Key()
					}
				];
            }
            return [];
        }

        public override bool Invincible() => AffectDamageDoneManager.AttackContext != null && refractor.DoesRefract(this);

        public override Spr? GetIcon() => refractor.GetIcon(this);

		public override void Render(G g, Vec v)
		{
            refractor.Render(g, v, this);
		}

        public override List<Tooltip> GetTooltips() => refractor.GetTooltips(this);
    }

	public abstract class RefractorStats {

		public abstract List<CardAction>? Refract(State s, Combat c, in AAttack attack, StuffBase stuff);

		public abstract bool DestroyAfter(State s, Combat c, in AAttack attack, StuffBase stuff);

        public virtual bool DoesRefract(StuffBase stuff) => true;

        public abstract Spr? GetIcon(StuffBase stuff);

        public abstract void Render(G g, Vec v, StuffBase stuff);

        public abstract List<Tooltip> GetTooltips(StuffBase stuff);
    }

	public class Prism : RefractorStats
	{
		public override List<CardAction>? Refract(State s, Combat c, in AAttack attack, StuffBase stuff) {
			return [
				Weaken(CopyAttack(attack)).ApplyModData(SidewaysAttackKey, Sideways.LEFT),
				Weaken(CopyAttack(attack)).ApplyModData(SidewaysAttackKey, Sideways.RIGHT)
			];

			AAttack Weaken(AAttack attack) {
				// if (attack.damage > 1 && !attack.targetPlayer && s.EnumerateAllArtifacts().OfType<CorrectiveLensesArtifact>().FirstOrDefault() is { } artifact) {
                //     attack.artifactPulse = artifact.Key();
                //     return attack;
                // }
				// if (attack.targetPlayer && (attack.status.HasValue || attack.cardOnHit != null) && s.EnumerateAllArtifacts().OfType<CorrectiveLensesArtifact>().FirstOrDefault() is { } fartifact) {
                //     attack.status = null;
                //     attack.cardOnHit = null;
                //     attack.artifactPulse = fartifact.Key();
                // }
                attack.damage = 1;
                return attack;
            }
		}
        public override bool DestroyAfter(State s, Combat c, in AAttack attack, StuffBase stuff) => attack.damage >= 3;

        public override Spr? GetIcon(StuffBase stuff) => ModEntry.Instance.PrismIcon;

		public override void Render(G g, Vec v, StuffBase stuff)
		{
			stuff.DrawWithHilight(g, ModEntry.Instance.PrismSprite, v + stuff.GetOffset(g), Mutil.Rand(stuff.x + 0.1) > 0.5, stuff.targetPlayer);
		}

		public override List<Tooltip> GetTooltips(StuffBase stuff) => [
			new GlossaryTooltip("midrow.prism") {
				TitleColor = Colors.midrow,
				Icon = ModEntry.Instance.PrismIcon,
				Title = ModEntry.Instance.Localizations.Localize(["midrow", "prism", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["midrow", "prism", "description"]),
			}
		];
    }

	public class PerfectPrism : Prism
	{
		public override List<CardAction>? Refract(State s, Combat c, in AAttack attack, StuffBase stuff) {
			return [
				CopyAttack(attack).ApplyModData(SidewaysAttackKey, Sideways.LEFT),
				CopyAttack(attack).ApplyModData(SidewaysAttackKey, Sideways.RIGHT)
			];
		}
        public override bool DestroyAfter(State s, Combat c, in AAttack attack, StuffBase stuff) => false;

        public override Spr? GetIcon(StuffBase stuff) => ModEntry.Instance.PerfectPrismIcon;

		public override void Render(G g, Vec v, StuffBase stuff)
		{
			stuff.DrawWithHilight(g, ModEntry.Instance.PerfectPrismIcon, v + stuff.GetOffset(g), Mutil.Rand(stuff.x + 0.1) > 0.5, stuff.targetPlayer);
		}

		public override List<Tooltip> GetTooltips(StuffBase stuff) => [
			new GlossaryTooltip("midrow.prism") {
				TitleColor = Colors.midrow,
				Icon = ModEntry.Instance.PerfectPrismIcon,
				Title = ModEntry.Instance.Localizations.Localize(["midrow", "perfectPrism", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["midrow", "perfectPrism", "description"]),
			}
		];
    }

	public class Mirror : RefractorStats
	{
		public override List<CardAction>? Refract(State s, Combat c, in AAttack attack, StuffBase stuff) {
			return [
				Flip(CopyAttack(attack)).ApplyModData(SidewaysAttackKey, attack.targetPlayer ^ stuff.targetPlayer ? Sideways.LEFT : Sideways.RIGHT),
			];

			static AAttack Flip(AAttack attack) {
				attack.targetPlayer = !attack.targetPlayer;
				return attack;
			}
		}

		public override bool DestroyAfter(State s, Combat c, in AAttack attack, StuffBase stuff) => true;

		public override Spr? GetIcon(StuffBase stuff) => ModEntry.Instance.MirrorIcon;

		public override void Render(G g, Vec v, StuffBase stuff)
		{
			stuff.DrawWithHilight(g, ModEntry.Instance.MirrorSprite, v + stuff.GetOffset(g), stuff.targetPlayer);
		}

		public override List<Tooltip> GetTooltips(StuffBase stuff) => [
			new GlossaryTooltip("midrow.mirror") {
				TitleColor = Colors.midrow,
				Icon = ModEntry.Instance.MirrorIcon,
				Title = ModEntry.Instance.Localizations.Localize(["midrow", "mirror", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["midrow", "mirror", "description"]),
			}
		];
	}
}