using Nickel;
using TheJazMaster.Nibbs.Actions;
using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using TheJazMaster.Nibbs.Features;

namespace TheJazMaster.Nibbs.Cards;


internal sealed class PlatitudesCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		flippable = upgrade == Upgrade.A
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, 0),
			moveEnemy = upgrade == Upgrade.B ? -2 : -1
		}
	];
}


internal sealed class SpacePrismCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 0
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.None => [
            new ASpawn
            {
                thing = new PrismManager.RefractorObject
                {
                    refractor = new PrismManager.Prism()
                }
            },
        ],
        _ => [
            new AStatus
            {
                status = upgrade == Upgrade.A ? Status.droneShift : ModEntry.Instance.MidShieldStatus,
                statusAmount = 1,
                targetPlayer = true
            },
            new ASpawn
            {
                thing = new PrismManager.RefractorObject
                {
                    refractor = new PrismManager.Prism()
                }
            },
        ]
    };
}


internal sealed class SpaceMirrorCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 2,
		floppable = true,
		exhaust = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ASpawn {
			thing = new PrismManager.RefractorObject {
				bubbleShield = upgrade == Upgrade.A,
				refractor = new PrismManager.Mirror()
            },
			disabled = flipped
        },
		new ADummyAction(),
		new ASpawn {
			thing = new PrismManager.RefractorObject {
				targetPlayer = true,
				bubbleShield = upgrade == Upgrade.A,
				refractor = new PrismManager.Mirror()
            },
			disabled = !flipped
        },
	];
}


internal sealed class MakePeaceCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 1 : 0,
		flippable = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AAttack {
				damage = GetDmg(s, 0),
				status = Status.shield,
				statusAmount = 2,
				stunEnemy = true
			},
			new AMove {
				dir = 1,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 0),
				status = Status.shield,
				statusAmount = 2,
				fast = true,
				stunEnemy = true
			}
		],
		_ => [
			new AAttack {
				damage = GetDmg(s, 0),
				status = upgrade == Upgrade.A ? Status.tempShield : Status.shield,
				statusAmount = 2,
				stunEnemy = true
			}
		]
	};
}


internal sealed class PrismArrayCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 3 : 1
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
    {
		Upgrade.A => [
            new AStatus
            {
                status = ModEntry.Instance.MidShieldStatus,
                statusAmount = 1,
                targetPlayer = true
            },
            new ASpawn
            {
                thing = new PrismManager.RefractorObject
                {
                    refractor = new PrismManager.Prism()
                },
                offset = -1
            },
            new ASpawn
            {
                thing = new PrismManager.RefractorObject
                {
                    refractor = new PrismManager.Prism()
                }
            },
            new ASpawn
            {
                thing = new PrismManager.RefractorObject
                {
                    refractor = new PrismManager.Prism()
                },
                offset = 1
            },
		],
        _ => [
            new AStatus
            {
                status = ModEntry.Instance.MidShieldStatus,
                statusAmount = 1,
                targetPlayer = true
            },
            new ASpawn
            {
                thing = new PrismManager.RefractorObject
                {
                    refractor = upgrade == Upgrade.B ? new PrismManager.PerfectPrism() : new PrismManager.Prism()
                },
                offset = -1
            },
            new ASpawn
            {
                thing = new PrismManager.RefractorObject
                {
                    refractor = upgrade == Upgrade.B ? new PrismManager.PerfectPrism() : new PrismManager.Prism()
                },
                offset = 1
            },
        ]
    };
}


internal sealed class EqualityCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
    {
		Upgrade.A => [
            new AStatus
            {
                status = ModEntry.Instance.MidShieldStatus,
                statusAmount = 1,
                targetPlayer = true
            },
            new ASpawn
            {
                thing = new DualDrone()
            }
		],
        _ => [
            new ASpawn
            {
                thing = new DualDrone {
					bubbleShield = upgrade == Upgrade.B
				}
            }
        ]
    };
}


internal sealed class RighteousShotCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 3 : 2
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade switch {
				Upgrade.A => 3,
				Upgrade.B => 5,
				_ => 1
			}),
			stunEnemy = true
		}
	];
}


internal sealed class GreenEnergyCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = upgrade == Upgrade.B
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
    {
        Upgrade.A => [
            new AStatus
            {
                status = Status.energyNextTurn,
                statusAmount = 1,
                targetPlayer = true
            },
            new AStatus
            {
                status = ModEntry.Instance.MidShieldStatus,
                statusAmount = 1,
                targetPlayer = true
            },
            new AStatus
            {
                status = Status.drawNextTurn,
                statusAmount = 1,
                targetPlayer = true
            },
        ],
        _ => [
            new AStatus
            {
                status = Status.energyNextTurn,
                statusAmount = upgrade == Upgrade.B ? 2 : 1,
                targetPlayer = true
            },
            new AStatus
            {
                status = ModEntry.Instance.MidShieldStatus,
                statusAmount = upgrade == Upgrade.B ? 2 : 1,
                targetPlayer = true
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
		cost = 1,
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = ModEntry.Instance.MidShieldStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true,
			},
			new ASpawn {
				thing = new Asteroid()
			}
		],
		Upgrade.B => [
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
			new ASpawn {
				thing = new Asteroid()
			}
		]
	};
}




internal sealed class InnerPeaceCard : Card, IRegisterableCard
{
    private static string CardType = null!;
    public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out CardType);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		description = ModEntry.Instance.Localizations.Localize(["card", "Ix", CardType, "description", upgrade.ToString()], new { Count = GetDraw() })
	};

    private int GetDraw() => upgrade switch
    {
        Upgrade.A => 6,
        _ => 4
    };

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new ADiscardAttacks(),
            new ADrawCard {
                count = GetDraw()
            },
        ],
        _ => [
            new ADrawCard {
                count = GetDraw(),
				timer = 1.75
            },
            new ADiscardAttacks()
        ]
    };
}


internal sealed class HardenCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.maxShield,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true,
		},
		new AStatus {
			status = ModEntry.Instance.MidShieldStatus,
			statusAmount = upgrade == Upgrade.A ? 3 : 2,
			targetPlayer = true,
		},
	];
}


internal sealed class SabotageCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 3 : 0),
			weaken = true,
		},
	];
}


internal sealed class VindicateCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 3 : 0),
		}.ApplyModData(FluxManager.FluxenKey, true),
	];
}


internal sealed class EyeForAnEyeCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.tempPayback,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		}
	];
}


internal sealed class FaultlessCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 2 : 1,
		exhaust = true,
		retain = upgrade == Upgrade.A
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.perfectShield,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			mode = AStatusMode.Set,
			targetPlayer = true
		},
		new AStatus {
			status = Status.powerdrive,
			statusAmount = upgrade == Upgrade.B ? 3 : 1
		}
	];
}


internal sealed class WeighPerspectivesCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 2 : 3,
		flippable = true,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ASpawn {
			thing = new PrismManager.RefractorObject {
				targetPlayer = flipped,
				refractor = new PrismManager.Mirror()
            },
			offset = flipped ? 1 : -1
        },
		new ASpawn {
			thing = new PrismManager.RefractorObject {
				refractor = upgrade == Upgrade.B ? new PrismManager.PerfectPrism() : new PrismManager.Prism()
            }
        },
		new ASpawn {
			thing = new PrismManager.RefractorObject {
				targetPlayer = !flipped,
				refractor = new PrismManager.Mirror()
            },
			offset = flipped ? -1 : 1
        },
	];
}


internal sealed class MartyrCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = upgrade != Upgrade.A
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.PerseveranceStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		},
	];
}


internal sealed class BalanceCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		recycle = upgrade == Upgrade.B,
		floppable = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.MidShieldStatus,
			statusAmount = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true,
			disabled = flipped
		},
		new AStatus {
			status = Status.evade,
			statusAmount = 1,
			targetPlayer = true,
			disabled = flipped
		},
		new ADummyAction(),
		new AStatus {
			status = Status.shield,
			statusAmount = 1,
			targetPlayer = true,
			disabled = !flipped
		},
		new AStatus {
			status = Status.droneShift,
			statusAmount = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true,
			disabled = !flipped
		},
	];
}


internal sealed class CrystalizeCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AVariableHint {
			status = ModEntry.Instance.MidShieldStatus
		},
		new AStatus {
			status = Status.shield,
			statusAmount = s.ship.Get(ModEntry.Instance.MidShieldStatus),
			xHint = 1,
			targetPlayer = true
		},
		new AStatus {
			status = ModEntry.Instance.MidShieldStatus,
			statusAmount = upgrade == Upgrade.B ? 1 : 0,
			mode = AStatusMode.Set,
			targetPlayer = true
		}
	];
}


internal sealed class RetaliateCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		_ => [
			new AStatus {
				status = Status.payback,
				statusAmount = upgrade == Upgrade.B ? 2 : 1,
				targetPlayer = true
			},
			new ASpawn {
				thing = new Missile {
					missileType = MissileType.normal,
					targetPlayer = true
				}
			}
		]
	};
}


internal sealed class FocusFireCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 2
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.A => [
            new AAttack {
                damage = 0,
                piercing = true,
            },
            new AAttack {
                damage = 0,
                piercing = true,
            },
            new AAttack {
                damage = 0,
                piercing = true,
            },
            new AAttack {
                damage = 0,
                piercing = true,
            },
            new AAttack {
                damage = 0,
                piercing = true,
            },
        ],
        _ =>[
            new AAttack {
                damage = 0,
                piercing = true,
            },
            new AAttack {
                damage = 0,
                piercing = true,
            },
            new AAttack {
                damage = 0,
                piercing = true,
				weaken = upgrade == Upgrade.B
            },
        ]
    };
}


