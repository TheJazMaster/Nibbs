using Nickel;
using TheJazMaster.Nibbs.Actions;
using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using TheJazMaster.Nibbs.Features;
using Microsoft.Extensions.Logging;

namespace TheJazMaster.Nibbs.Cards;


internal sealed class FractureCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		_ => [
			new AStatus {
				status = ModEntry.Instance.SafetyShieldStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 1),
				piercing = upgrade == Upgrade.B,
				status = ModEntry.Instance.FractureStatus,
				statusAmount = upgrade == Upgrade.A ? 2 : 1
			}
		]
	};
}


internal sealed class PrismCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		floppable = upgrade != Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ASpawn {
				thing = new PrismManager.OmniPrism {
					targetPlayer = true
				}
			},
			new AStatus {
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true
			},
		],
		_ => [
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = false
				},
				disabled = flipped
			},
			new AStatus {
				status = Status.droneShift,
				statusAmount = upgrade == Upgrade.A ? 2 : 1,
				disabled = flipped,
				targetPlayer = true
			},
			new ADummyAction(),
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = true
				},
				disabled = !flipped
			},
			new AStatus {
				status = Status.droneShift,
				statusAmount = upgrade == Upgrade.A ? 2 : 1,
				disabled = !flipped,
				targetPlayer = true
			},
		]
	};
}


internal sealed class KarmaCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		flippable = true,
		retain = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AFlip {
				dir = flipped ? 1 : -1
			},
			new AStatus {
				status = Status.evade,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AFlip {
				dir = flipped ? 1 : -1
			},
			new AStatus {
				status = Status.evade,
				statusAmount = 1,
				targetPlayer = true
			}
		]
	};
}


// internal sealed class CrushCard : Card, IRegisterableCard
// {
// 	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
// 		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
// 	}

// 	public override CardData GetData(State state) => new() {
// 		cost = 3
// 	};

// 	public override List<CardAction> GetActions(State s, Combat c) => [
// 		new AAttack {
// 			damage = GetDmg(s, upgrade == Upgrade.A ? 5 : 3),
// 			piercing = true,
// 			status = ModEntry.Instance.FractureStatus,
// 			statusAmount = upgrade == Upgrade.B ? 4 : 2
// 		}
// 	];
// }


internal sealed class LitterDisposalCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		flippable = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ASpawn {
			thing = upgrade == Upgrade.B ? new SpaceMine() : new Asteroid(),
			offset = 1
		},
		new AStatus {
			status = Status.droneShift,
			statusAmount = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true
		}
	];
}


internal sealed class PrismArrayCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		flippable = upgrade != Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = false
				},
				offset = -3
			},
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = true
				},
			},
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = false
				},
				offset = 3
			},
		],
		_ => [
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = true
				},
			},
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = false
				},
				offset = 3
			},
		]
	};
}


internal sealed class RighteousShotCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.SafetyShieldStatus,
			statusAmount = upgrade == Upgrade.B ? 9 : 4,
			targetPlayer = true
		},
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.A ? 3 : 2)
		}
	];
}


internal sealed class GreenEnergyCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		exhaust = true,
		retain = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AEnergy {
				changeAmount = 2
			},
			new ADrawCard {
				count = 2
			},
			new AStatus {
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		_ => [
			new AEnergy {
				changeAmount = 2
			},
			new AStatus {
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 2,
				targetPlayer = true
			}
		]
	};
}


internal sealed class MakePeaceCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		exhaust = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		_ => [
			new AAttack {
				damage = GetDmg(s, 0),
				status = upgrade == Upgrade.A ? Status.shield : Status.tempShield,
				statusAmount = 2,
				stunEnemy = true
			}
		]
	};
}


internal sealed class NaturesShieldCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.SafetyShieldStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.evade,
				statusAmount = 1,
				targetPlayer = true,
			},
			new ASpawn {
				thing = new Asteroid()
			}
		],
		_ => [
			new AStatus {
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true,
			},
			new AStatus {
				status = ModEntry.Instance.SafetyShieldStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new ASpawn {
				thing = new Asteroid()
			}
		]
	};
}


internal sealed class DisperseCard : Card, IRegisterableCard
{
	public bool flippedMovement = false;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		recycle = upgrade == Upgrade.B,
		floppable = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AMove {
			dir = (flippedMovement ? -1 : 1) * (upgrade == Upgrade.A ? 3 : 2),
			disabled = flipped,
			targetPlayer = true
		},
		new ASpawn {
			thing = new PrismManager.RegularPrism {
				targetPlayer = false
			},
			disabled = flipped
		},
		new ADummyAction(),
		new AMove {
			dir = (flippedMovement ? -1 : 1) * (upgrade == Upgrade.A ? -3 : -2),
			disabled = !flipped,
			targetPlayer = true
		},
		new ASpawn {
			thing = new PrismManager.RegularPrism {
				targetPlayer = true
			},
			disabled = !flipped
		},
	];

	public override void OnFlip(G g)
	{
		if (!GetDataWithOverrides(g.state).flippable || !flipped) {
			flippedMovement = !flippedMovement;
		}
	}
}


internal sealed class PulverizeCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		_ => [
			new AStatus {
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 3,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 1),
				piercing = true,
				weaken = true,
				status = upgrade == Upgrade.B ? ModEntry.Instance.FractureStatus : null,
				statusAmount = 3
			}
		]
	};
}


internal sealed class CrystalizeCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = upgrade != Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true
			},
			new AVariableHint {
				status = ModEntry.Instance.FractureStatus
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = s.ship.Get(ModEntry.Instance.FractureStatus),
				xHint = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AVariableHint {
				status = ModEntry.Instance.FractureStatus
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = s.ship.Get(ModEntry.Instance.FractureStatus),
				xHint = 1,
				targetPlayer = true
			}
		]
	};
}


internal sealed class DemonstrateCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.SafetyShieldStatus,
				statusAmount = 6,
				targetPlayer = true
			},
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = false
				},
				offset = -1
			},
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = false
				}
			},
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = false
				},
				offset = 1
			},
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.SafetyShieldStatus,
				statusAmount = 3,
				targetPlayer = true
			},
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = false
				},
				offset = -1
			},
			new ASpawn {
				thing = new PrismManager.RegularPrism {
					targetPlayer = false
				},
				offset = 1
			},
		]
	};
}


internal sealed class CrackdownCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 2
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 3,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 2),
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 1
			},
			new AAttack {
				damage = GetDmg(s, 1),
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 1
			},
			new AAttack {
				damage = GetDmg(s, 0),
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 1
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 3,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 2),
				piercing = upgrade == Upgrade.B,
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 1
			},
			new AAttack {
				damage = GetDmg(s, 1),
				piercing = upgrade == Upgrade.B,
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 1
			}
		]
	};
}


internal sealed class HardAsDiamondCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 2 : 3,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = Status.perfectShield,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.tempPayback,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = Status.perfectShield,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.FractureStatus,
				statusAmount = 3,
				targetPlayer = true
			}
		]
	};
}


internal sealed class RetaliateCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 2
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		_ => [
			new AStatus {
				status = Status.tempPayback,
				statusAmount = upgrade == Upgrade.A ? 3 : 2,
				targetPlayer = true
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = upgrade == Upgrade.B ? 3 : 1,
				targetPlayer = true
			}
		]
	};
}


internal sealed class HealingCrystalsCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AHeal {
				healAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.SafetyShieldStatus,
				statusAmount = 3,
				targetPlayer = true
			},
			new AStatus {
				status = Status.energyLessNextTurn,
				statusAmount = 2,
				targetPlayer = true
			},
		],
		_ => [
			new AHeal {
				healAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.SafetyShieldStatus,
				statusAmount = 5,
				targetPlayer = true
			},
			new AStatus {
				status = Status.energyLessNextTurn,
				statusAmount = upgrade == Upgrade.A ? 1 : 2,
				targetPlayer = true
			},
		]
	};
}


internal sealed class MartyrCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 3 : 4,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.PerseveranceStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AHurt {
				hurtAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = 3,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.PerseveranceStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AHurt {
				hurtAmount = 1,
				targetPlayer = true
			},
		]
	};
}


internal sealed class ReclaimCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 2,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		_ => [
			new AStatus {
				status = Status.payback,
				statusAmount = upgrade == Upgrade.B ? 2 : 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.FractureStatus,
				statusAmount = upgrade == Upgrade.B ? 3 : 2,
				targetPlayer = true
			}
		]
	};
}


internal sealed class SolarRayCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	private int GetDamage(State s) => upgrade switch {
		_ => GetDmg(s, 3)
	};

	private int GetEnergy() => upgrade switch {
		_ => 3
	};

	public override CardData GetData(State state) => new() {
		cost = 2
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.SafetyShieldStatus,
				statusAmount = 4,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDamage(s),
				piercing = true,
				givesEnergy = GetEnergy(),
			}
		],
		_ => [
			new AAttack {
				damage = GetDamage(s),
				piercing = true,
				givesEnergy = GetEnergy(),
				stunEnemy = upgrade == Upgrade.A
			}
		]
	};
}


internal sealed class ScorchedEarthCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 2 : 3,
		exhaust = true,
		retain = upgrade == Upgrade.B,
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ASpawn {
			thing = new PrismManager.OmniPrism {
				targetPlayer = false
			},
			offset = -3
		},
		new ASpawn {
			thing = new PrismManager.OmniPrism {
				targetPlayer = false
			}
		},
		new ASpawn {
			thing = new PrismManager.OmniPrism {
				targetPlayer = false
			},
			offset = 3
		},
		new AStatus {
			status = Status.droneShift,
			statusAmount = 1,
			targetPlayer = true
		}
	];
}


// internal sealed class DarkSideCard : Card, IRegisterableCard, IHasCustomCardTraits
// {
// 	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
// 		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _, true);
// 	}

// 	public override CardData GetData(State state) => new() {
// 		cost = 0,
// 		exhaust = true,
// 		temporary = true
// 	};

// 	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry> {
// 		ModEntry.Instance.KokoroApi.Fleeting.Trait
// 	};

// 	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
// 		_ => [
// 			new AAttack {
// 				damage = GetDmg(s, 2),
// 				piercing = upgrade == Upgrade.B,
// 				statusAmount = 1,
// 				status = upgrade == Upgrade.A ? ModEntry.Instance.FractureStatus : null
// 			}
// 		]
// 	};
// }


// internal sealed class ShatterCard : Card, IRegisterableCard
// {
// 	private static string CardName = null!;
// 	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
// 		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out CardName);
// 	}

// 	private int GetMult() => 2;

// 	public override CardData GetData(State state) => new() {
// 		cost = upgrade == Upgrade.A ? 1 : 2,
// 		exhaust = true
// 	};

// 	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
// 		Upgrade.B => [
// 			ModEntry.Instance.KokoroApi.VariableHintTargetPlayerTargetPlayer.MakeVariableHint(
// 				new AVariableHint {
// 					status = ModEntry.Instance.FractureStatus,
// 				}
// 			).SetTargetPlayer(false).AsCardAction,
// 			new AStatus {
// 				status = Status.tempShield,
// 				statusAmount = GetMult() * c.otherShip.Get(ModEntry.Instance.FractureStatus),
// 				xHint = GetMult(),
// 				targetPlayer = true
// 			},
// 			new AHurt {
// 				hurtAmount = GetMult() * c.otherShip.Get(ModEntry.Instance.FractureStatus),
// 				xHint = GetMult(),
// 			},
// 			new AStatus {
// 				status = ModEntry.Instance.FractureStatus,
// 				statusAmount = 0,
// 				mode = AStatusMode.Set,
// 				targetPlayer = false
// 			},
// 		],
// 		_ => [
// 			ModEntry.Instance.KokoroApi.VariableHintTargetPlayerTargetPlayer.MakeVariableHint(
// 				new AVariableHint {
// 					status = ModEntry.Instance.FractureStatus,
// 				}
// 			).SetTargetPlayer(false).AsCardAction,
// 			new AHurt {
// 				hurtAmount = GetMult() * c.otherShip.Get(ModEntry.Instance.FractureStatus),
// 				xHint = GetMult(),
// 			},
// 			new AStatus {
// 				status = ModEntry.Instance.FractureStatus,
// 				statusAmount = 0,
// 				mode = AStatusMode.Set,
// 				targetPlayer = false
// 			},
// 		]
// 	};
// }