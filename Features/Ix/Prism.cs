using System.Collections.Generic;
using System.Linq;
using Nickel;
using TheJazMaster.Nibbs.Actions;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Characters;
using static TheJazMaster.Nibbs.Features.Ix.AngledAttacksManager;

namespace TheJazMaster.Nibbs.Features.Ix;

public class PrismManager
{
	private static ModEntry Instance => ModEntry.Instance;
	private static IxManager IxManager => Instance.IxManager;
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
			refractor.AfterHit(s, c, AffectDamageDoneManager.AttackContext, this);
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
						healAmount = 2,
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
        public virtual void AfterHit(State s, Combat c, in AAttack attack, StuffBase stuff) { }
        public virtual bool DoesRefract(StuffBase stuff) => true;
        public abstract Spr? GetIcon(StuffBase stuff);
        public abstract void Render(G g, Vec v, StuffBase stuff);
        public abstract List<Tooltip> GetTooltips(StuffBase stuff);
    }

	public class Prism : RefractorStats
	{
        public override List<CardAction>? Refract(State s, Combat c, in AAttack attack, StuffBase stuff) {
			if (s.EnumerateAllArtifacts().OfType<RayleighScatteringArtifact>().FirstOrDefault() is not null) {
				return [
					SetTo1(CopyAttack(attack)).ApplyModData(SidewaysAttackKey, Sideways.LEFT),
					SetTo1(CopyAttack(attack)).ApplyModData(SidewaysAttackKey, Sideways.RIGHT),
					SetTo1(CopyAttack(attack))
				];
			}
            return [
				SetTo1(CopyAttack(attack)).ApplyModData(SidewaysAttackKey, Sideways.LEFT),
				SetTo1(CopyAttack(attack)).ApplyModData(SidewaysAttackKey, Sideways.RIGHT)
			];

			static AAttack SetTo1(AAttack attack) {
                attack.damage = 1;
                return attack;
            }
		}

        public override bool DestroyAfter(State s, Combat c, in AAttack attack, StuffBase stuff) => false;

        public override Spr? GetIcon(StuffBase stuff) => IxManager.PrismIcon;

		public override void Render(G g, Vec v, StuffBase stuff)
		{
			stuff.DrawWithHilight(g, IxManager.PrismSprite, v + stuff.GetOffset(g), Mutil.Rand(stuff.x + 0.1) > 0.5, stuff.targetPlayer);
		}

		public override List<Tooltip> GetTooltips(StuffBase stuff) => [
			new GlossaryTooltip("midrow.prism") {
				TitleColor = Colors.midrow,
				Icon = IxManager.PrismIcon,
				Title = ModEntry.Instance.Localizations.Localize(["midrow", "prism", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["midrow", "prism", "description"]),
			}
		];
    }

	public class PerfectPrism : Prism
	{
		public override List<CardAction>? Refract(State s, Combat c, in AAttack attack, StuffBase stuff) {
			if (s.EnumerateAllArtifacts().OfType<RayleighScatteringArtifact>().FirstOrDefault() is not null) {
				return [
					CopyAttack(attack).ApplyModData(SidewaysAttackKey, Sideways.LEFT),
					CopyAttack(attack).ApplyModData(SidewaysAttackKey, Sideways.RIGHT),
					CopyAttack(attack)
				];
			}
			return [
				CopyAttack(attack).ApplyModData(SidewaysAttackKey, Sideways.LEFT),
				CopyAttack(attack).ApplyModData(SidewaysAttackKey, Sideways.RIGHT)
			];
		}
        public override void AfterHit(State s, Combat c, in AAttack attack, StuffBase stuff) {}
        public override bool DestroyAfter(State s, Combat c, in AAttack attack, StuffBase stuff) => false;

        public override Spr? GetIcon(StuffBase stuff) => IxManager.PerfectPrismIcon;

		public override void Render(G g, Vec v, StuffBase stuff)
		{
			stuff.DrawWithHilight(g, IxManager.PerfectPrismSprite, v + stuff.GetOffset(g), Mutil.Rand(stuff.x + 0.1) > 0.5, stuff.targetPlayer);
		}

		public override List<Tooltip> GetTooltips(StuffBase stuff) => [
			new GlossaryTooltip("midrow.prism") {
				TitleColor = Colors.midrow,
				Icon = IxManager.PerfectPrismIcon,
				Title = ModEntry.Instance.Localizations.Localize(["midrow", "perfectPrism", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["midrow", "perfectPrism", "description"]),
			}
		];
    }

	public class Mirror : RefractorStats
	{
        public bool flipped;

        public override List<CardAction>? Refract(State s, Combat c, in AAttack attack, StuffBase stuff) {
            bool left = attack.targetPlayer ^ flipped ^ stuff.targetPlayer;
            if (s.EnumerateAllArtifacts().OfType<RayleighScatteringArtifact>().FirstOrDefault() is not null) {
				return [
					Flip(CopyAttack(attack)).ApplyModData(SidewaysAttackKey, left ? Sideways.LEFT : Sideways.RIGHT),
					Flip(CopyAttack(attack))
				];
			}
			return [
				Flip(CopyAttack(attack)).ApplyModData(SidewaysAttackKey, left ? Sideways.LEFT : Sideways.RIGHT),
			];

			static AAttack Flip(AAttack attack) {
				if (attack.targetPlayer) attack.cardOnHit = null;
				attack.targetPlayer = !attack.targetPlayer;
				return attack;
			}
		}

		public override bool DestroyAfter(State s, Combat c, in AAttack attack, StuffBase stuff) => attack.damage > 0;

		public override Spr? GetIcon(StuffBase stuff) => flipped ? IxManager.MirrorIconFlipped : IxManager.MirrorIcon;

		public override void Render(G g, Vec v, StuffBase stuff)
		{
			stuff.DrawWithHilight(g, IxManager.MirrorSprite, v + stuff.GetOffset(g), flipped, stuff.targetPlayer);
		}

		public override List<Tooltip> GetTooltips(StuffBase stuff) => [
			new GlossaryTooltip("midrow.mirror") {
				TitleColor = Colors.midrow,
				Icon = IxManager.MirrorIcon,
				Title = ModEntry.Instance.Localizations.Localize(["midrow", "mirror", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["midrow", "mirror", "description"]),
			}
		];
	}
}