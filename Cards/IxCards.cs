using Nickel;
using TheJazMaster.Nibbs.Actions;
using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using TheJazMaster.Nibbs.Features.Ix;
using TheJazMaster.Nibbs.Characters;

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
		cost = 1,
		exhaust = upgrade == Upgrade.B,
		flippable = true
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = ModEntry.Instance.IxManager.MidShieldStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AKillPrisms(),
			new ASpawn {
				thing = new PrismManager.RefractorObject
				{
					refractor = new PrismManager.Prism()
				},
				offset = 1
			}
		],
		Upgrade.B => [
			new ASpawn {
				thing = new PrismManager.RefractorObject
				{
					refractor = new PrismManager.PerfectPrism()
				},
				offset = 1
			}
		],
		_ => [
			new AKillPrisms(),
			new ASpawn {
				thing = new PrismManager.RefractorObject
				{
					refractor = new PrismManager.Prism()
				},
				offset = 1
			}
		]
	};
}


internal sealed class SpaceMirrorCard : Card, IRegisterableCard
{
	private static Spr art;
	private static Spr artFlipped;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _, out art, out artFlipped);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		floppable = true,
		retain = upgrade == Upgrade.B,
		art = flipped ? artFlipped : art
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true,
				disabled = flipped
			},
			new ASpawn {
				thing = new PrismManager.RefractorObject {
					refractor = new PrismManager.Mirror {
						flipped = false
					}
				},
				disabled = flipped
			},
			new ADummyAction(),
			new AStatus {
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true,
				disabled = !flipped
			},
			new ASpawn {
				thing = new PrismManager.RefractorObject {
					refractor = new PrismManager.Mirror {
						flipped = true
					}
				},
				disabled = !flipped
			},
		],
		_ => [
			new ASpawn {
				thing = new PrismManager.RefractorObject {
					refractor = new PrismManager.Mirror {
						flipped = false
					}
				},
				disabled = flipped
			},
			new ADummyAction(),
			new ASpawn {
				thing = new PrismManager.RefractorObject {
					refractor = new PrismManager.Mirror {
						flipped = true
					}
				},
				disabled = !flipped
			},
		]
	};
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
				fast = true,
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
            new AStatus {
                status = Status.droneShift,
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
		cost = 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1),
			moveEnemy = upgrade == Upgrade.B ? 1 : 0
		},
		new AStatus {
			status = ModEntry.Instance.IxManager.MidShieldStatus,
			statusAmount = 1,
			targetPlayer = true
		}
	];
}


internal sealed class GreenEnergyCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
    {
        Upgrade.B => [
            new AStatus
            {
                status = Status.energyNextTurn,
                statusAmount = 1,
                targetPlayer = true
            },
            new AStatus
            {
                status = ModEntry.Instance.IxManager.MidShieldStatus,
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
                statusAmount = 1,
                targetPlayer = true
            },
            new AStatus
            {
                status = ModEntry.Instance.IxManager.MidShieldStatus,
                statusAmount = upgrade == Upgrade.A ? 2 : 1,
                targetPlayer = true
            }
        ]
    };
}


internal sealed class HardenCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new AStatus {
                status = Status.maxShield,
                statusAmount = 2,
                targetPlayer = true,
            },
            new AStatus {
                status = ModEntry.Instance.IxManager.MidShieldStatus,
                statusAmount = 2,
                targetPlayer = true,
            },
        ],
        _ => [
            new AStatus {
                status = Status.maxShield,
                statusAmount = 1,
                targetPlayer = true,
            },
            new AStatus {
                status = ModEntry.Instance.IxManager.MidShieldStatus,
                statusAmount = upgrade == Upgrade.A ? 3 : 2,
                targetPlayer = true,
            },
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
				status = ModEntry.Instance.IxManager.MidShieldStatus,
				statusAmount = 1,
				targetPlayer = true,
			},
			new AStatus {
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true
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
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true,
			},
			new ASpawn {
				thing = new Asteroid()
			}
		]
	};
}



internal sealed class PrismArrayCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = upgrade != Upgrade.A
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.IxManager.MidShieldStatus,
				statusAmount = 3,
				targetPlayer = true
			},
			new AKillPrisms(),
			new ASpawn
			{
				thing = new PrismManager.RefractorObject {
					refractor = new PrismManager.Prism()
				},
				offset = -1
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
			new AKillPrisms(),
			new ASpawn
			{
				thing = new PrismManager.RefractorObject {
					refractor = new PrismManager.Prism()
				},
				offset = -1
			},
			new ASpawn
			{
				thing = new PrismManager.RefractorObject
				{
					refractor = new PrismManager.Prism()
				},
				offset = 1
			},
		]
	};
}


internal sealed class InnerPeaceCard : Card, IRegisterableCard
{
    private static string CardType = null!;
    public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		exhaust = upgrade == Upgrade.B
	};

    public override List<CardAction> GetActions(State s, Combat c) => [
		new ADrawCard {
			count = upgrade == Upgrade.A ? 6 : 4
		},
		new AStatus {
			status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive,
			statusAmount = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true
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


internal sealed class DetoxCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = upgrade != Upgrade.A
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ASpawn {
				thing = new AttackDrone {
                    targetPlayer = true
				}
			},
			new AStatus {
				status = Status.shield,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		_ => [
			new ASpawn {
				thing = new Missile {
					missileType = MissileType.heavy,
                    targetPlayer = true
				}
			},
			new AStatus {
				status = Status.shield,
				statusAmount = 3,
				targetPlayer = true
			}
		]
	};
}




internal sealed class WeighPerspectivesCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 2 : 3,
		flippable = true,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ASpawn {
			thing = new PrismManager.RefractorObject {
				refractor = new PrismManager.Mirror {
					flipped = flipped
				}
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
				refractor = new PrismManager.Mirror {
					flipped = !flipped
				}
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
			status = ModEntry.Instance.IxManager.PerseveranceStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		},
	];
}


internal sealed class BalanceCard : Card, IRegisterableCard
{
	private static Spr art;
	private static Spr artFlipped;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _, out art, out artFlipped);
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		recycle = upgrade == Upgrade.B,
		floppable = true,
		art = flipped ? artFlipped : art
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.IxManager.MidShieldStatus,
			statusAmount = 2,
			targetPlayer = true,
			disabled = flipped
		},
		new AStatus {
			status = Status.evade,
			statusAmount = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true,
			disabled = flipped
		},
		new ADummyAction(),
		new AStatus {
			status = Status.shield,
			statusAmount = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true,
			disabled = !flipped
		},
		new AStatus {
			status = Status.droneShift,
			statusAmount = 2,
			targetPlayer = true,
			disabled = !flipped
		},
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
		cost = upgrade == Upgrade.A ? 1 : 2
	};

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new AAttack {
                damage = GetDmg(s, 0),
                piercing = true,
            },
            new AAttack {
                damage = GetDmg(s, 0),
                piercing = true,
            },
            new AAttack {
                damage = GetDmg(s, 0),
                piercing = true,
            },
            new AAttack {
                damage = GetDmg(s, 0),
                piercing = true,
            },
            new AAttack {
                damage = GetDmg(s, 0),
                piercing = true,
            },
        ],
        _ =>[
            new AAttack {
                damage = GetDmg(s, 0),
                piercing = true,
            },
            new AAttack {
                damage = GetDmg(s, 0),
                piercing = true,
            },
            new AAttack {
                damage = GetDmg(s, 0),
                piercing = true
            },
        ]
    };
}