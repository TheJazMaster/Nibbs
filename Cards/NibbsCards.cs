using Nickel;
using TheJazMaster.Nibbs.Actions;
using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;

namespace TheJazMaster.Nibbs.Cards;


internal sealed class SpaceHoppingCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		flippable = upgrade == Upgrade.A
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ABacktrackMove {
				dir = 1,
				targetPlayer = true
			},
			new ABacktrackMove {
				dir = 1,
				targetPlayer = true
			},
			new ABacktrackMove {
				dir = 1,
				targetPlayer = true
			}
		],
		_ => [
			new ABacktrackMove {
				dir = 1,
				targetPlayer = true
			},
			new ABacktrackMove {
				dir = 1,
				targetPlayer = true
			}
		]
	};
}


internal sealed class WingsOfFireCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ABacktrackMove {
			dir = upgrade == Upgrade.B ? 3 : 2,
			targetPlayer = true,
			isRandom = true
		},
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 3 : 2)
		}
	];
}


internal sealed class SmeltCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 2
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AAttack {
				damage = GetDmg(s, 5),
				stunEnemy = true
			},
			new AStatus {
				status = Status.shield,
				statusAmount = -1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.maxShield,
				statusAmount = -1,
				targetPlayer = true
			}
		],
		_ => [
			new AAttack {
				damage = GetDmg(s, upgrade == Upgrade.A ? 4 : 3),
				stunEnemy = true
			},
			new AStatus {
				status = Status.shield,
				statusAmount = -1,
				targetPlayer = true
			}
		]
	};
}


internal sealed class TrailblazerCard : Card, IRegisterableCard
{
	public static Spr Art;
	public static Spr ArtFlipped;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _, out Art, out ArtFlipped);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 2 : 1,
		art = flipped ? ArtFlipped : Art
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ABacktrackMove {
				dir = -2,
				targetPlayer = true
			},
			new AStatus {
				status = Status.hermes,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new ABacktrackMove {
				dir = upgrade == Upgrade.A ? -3 : -2,
				targetPlayer = true
			},
			new AStatus {
				status = Status.hermes,
				statusAmount = 1,
				targetPlayer = true
			}
		]
	};
}


internal sealed class WormholeSurfingCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 2 : 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true,
				mode = AStatusMode.Set
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		Upgrade.B => [
			new AStatus {
				status = Status.timeStop,
				statusAmount = 2,
				targetPlayer = true,
				mode = AStatusMode.Set
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true,
				mode = AStatusMode.Set
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true
			}
		],
	};
}


internal sealed class OverHereCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 3 : 1,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.BacktrackAutododgeLeftStatus,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.BacktrackAutododgeRightStatus,
				statusAmount = upgrade == Upgrade.A ? 2 : 1,
				targetPlayer = true
			}
		],
	};
}


internal sealed class HotPursuitCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 1 : 2,
		exhaust = true,
		art = flipped ? TrailblazerCard.Art : TrailblazerCard.ArtFlipped,
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new ABacktrackMove {
				dir = 2,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 3),
				stunEnemy = true
			},
			new ABacktrackMove {
				dir = -2,
				targetPlayer = true
			}
		],
		_ => [
			new ABacktrackMove {
				dir = 2,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 2),
				stunEnemy = true
			}
		],
	};
}


internal sealed class BackflipCard : Card, IRegisterableCard
{
	public static Spr Art;
	public static Spr ArtFlipped;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _, out Art, out ArtFlipped);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		flippable = true,
		art = flipped ? ArtFlipped : Art,
		exhaust = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.BackflipStatus,
			statusAmount = upgrade == Upgrade.B ? 6 : upgrade == Upgrade.A ? 4 : 3,
			targetPlayer = true
		},
		new ABacktrackMove {
			dir = 1,
			targetPlayer = true
		}
	];
}


internal sealed class SmokescreenCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = Status.tempShield,
				statusAmount = 2,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.BackflipStatus,
				statusAmount = 3,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = Status.tempShield,
				statusAmount = upgrade == Upgrade.A ? 3 : 2,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.BackflipStatus,
				statusAmount = 1,
				targetPlayer = true
			}
		],
	};
}


internal sealed class QuantumTurbulenceCard : Card, IRegisterableCard
{
	private static string CardName = null!;
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out CardName);
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		recycle = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "Nibbs", CardName, "description", upgrade.ToString()], new {
			Dir = flipped ? Loc.T("card.LoadState.desc.combat.right") : Loc.T("card.LoadState.desc.combat.left"),
			Amount = 1 + state.ship.Get(Status.hermes) 
		})
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AMove {
			dir = 0,
			targetPlayer = true,
			omitFromTooltips = true,
			ignoreHermes = true,
		}
	];

	public override void OnDraw(State s, Combat c)
	{
		c.Queue(new ABacktrackMove {
			dir = flipped ? 1 : -1,
			targetPlayer = true
		});
		if (upgrade == Upgrade.A)
			c.Queue(new AStatus {
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true
			});
		if (upgrade == Upgrade.B)
			c.Queue(new ABacktrackMove {
				dir = flipped ? -1 : 1,
				targetPlayer = true
			});
	}
}


internal sealed class FlapFlapCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 3 : 2,
		art = upgrade == Upgrade.B ? StableSpr.cards_BranchPredictionFlipped : StableSpr.cards_BranchPrediction,
		exhaust = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = upgrade switch {
				Upgrade.A => ModEntry.Instance.BacktrackAutododgeRightStatus,
				Upgrade.B => Status.autododgeLeft,
				_ => Status.autododgeRight
			}, 
			statusAmount = 2,
			targetPlayer = true
		}
	];
}


internal sealed class HydraulicsCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		exhaust = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.hermes,
			statusAmount = 1,
			targetPlayer = true
		},
		new ADrawCard {
			count = upgrade == Upgrade.A ? 4 : 2
		}
	];
}


internal sealed class HoldOnCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		retain = true,
		exhaust = upgrade != Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.hermes,
			statusAmount = 2,
			targetPlayer = true
		},
		new AStatus {
			status = ModEntry.Instance.BackflipStatus,
			statusAmount = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true
		},
	];
}


internal sealed class DimensionalJauntCard : Card, IRegisterableCard
{
	public static Spr HopArt;
	public static Spr SkipArt;

	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		HopArt = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Nibbs/Hop.png")).Sprite;
		SkipArt = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Nibbs/Skip.png")).Sprite;
	
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "Nibbs", "DimensionalJaunt", "description", upgrade.ToString()])
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.BackflipStatus,
				statusAmount = 2,
				targetPlayer = true
			},
			new AAddCard {
				card = new HopCard(),
				destination = CardDestination.Deck
			},
			new AAddCard {
				card = new SkipCard(),
				destination = CardDestination.Deck
			},
		],
		_ => [
			new AAddCard {
				card = new HopCard {
					upgrade = upgrade == Upgrade.A ? upgrade : Upgrade.None
				},
				destination = CardDestination.Deck
			},
			new AAddCard {
				card = new SkipCard {
					upgrade = upgrade == Upgrade.A ? upgrade : Upgrade.None
				},
				destination = CardDestination.Deck
			},
		]
	};
}


internal sealed class HopCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _, true);
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		art = flipped ? DimensionalJauntCard.SkipArt : DimensionalJauntCard.HopArt,
		exhaust = upgrade == Upgrade.B,
		temporary = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new ABacktrackMove {
				dir = -1,
				targetPlayer = true
			},
			new ADrawCard {
				count = 1
			}
		],
		Upgrade.B => [
			new AMove {
				dir = -1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.hermes,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		_ => [
			new ABacktrackMove {
				dir = -1,
				targetPlayer = true
			},
		]
	};
}


internal sealed class SkipCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _, true);
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		art = flipped ? DimensionalJauntCard.HopArt : DimensionalJauntCard.SkipArt,
		exhaust = upgrade == Upgrade.B,
		temporary = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new ABacktrackMove {
				dir = 1,
				targetPlayer = true
			},
			new ADrawCard {
				count = 1
			}
		],
		Upgrade.B => [
			new ABacktrackMove {
				dir = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new ABacktrackMove {
				dir = 1,
				targetPlayer = true
			},
		]
	};
}


internal sealed class TauntCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.uncommon, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 1 : 0,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.BackflipStatus,
				statusAmount = 4,
				targetPlayer = true
			},
			new AStunShip(),
			new AStatus {
				status = Status.evade,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.BackflipStatus,
				statusAmount = 4,
				targetPlayer = true
			},
			new AEnergy {
				changeAmount = upgrade == Upgrade.A ? 2 : 1
			},
			new AStatus {
				status = Status.evade,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
		],
	};
}


internal sealed class FluxCompressorCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = Status.stunCharge,
				statusAmount = 3,
				targetPlayer = true
			},
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true
			},
		],
		Upgrade.A => [
			new AStatus {
				status = Status.stunCharge,
				statusAmount = 5,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = Status.stunCharge,
				statusAmount = 3,
				targetPlayer = true
			},
		]
	};
}


internal sealed class DragonFrenzyCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		infinite = true,
		retain = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.A ? 1 : 0),
			stunEnemy = true
		},
	];
}


internal sealed class QuantumCollapseCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		retain = upgrade == Upgrade.A
	};

	private static int GetSchmoveAmt(State s)
	{
		if (s.route is Combat)
		{
			return s.ship.Get(ModEntry.Instance.BacktrackLeftStatus) + s.ship.Get(ModEntry.Instance.BacktrackRightStatus);
		}
		return 0;
	}

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AAttack {
				damage = GetDmg(s, 1)
			},
			new AVariableHint {
				status = ModEntry.Instance.BacktrackLeftStatus,
				secondStatus = ModEntry.Instance.BacktrackRightStatus
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetSchmoveAmt(s)),
			}
		],
		_ => [
			new AAttack {
				damage = GetDmg(s, 1)
			},
			new AVariableHint {
				status = ModEntry.Instance.BacktrackLeftStatus,
				secondStatus = ModEntry.Instance.BacktrackRightStatus
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetSchmoveAmt(s)),
			},
			new AStatus {
				status = ModEntry.Instance.BacktrackLeftStatus,
				mode = AStatusMode.Set,
				statusAmount = 0,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.BacktrackRightStatus,
				mode = AStatusMode.Set,
				statusAmount = 0,
				targetPlayer = true
			},
		]
	};
}


internal sealed class BlurCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		flippable = true,
		exhaust = upgrade != Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.SmokescreenStatus,
			statusAmount = 1,
			targetPlayer = true
		},
		new AMove {
			dir = 1,
			targetPlayer = true
		}
	];
}


internal sealed class SuperpositionCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 2 : 3
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ABacktrackMove {
				dir = 5,
				targetPlayer = true
			},
			new ABacktrackMove {
				dir = -5,
				targetPlayer = true
			},
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new ABacktrackMove {
				dir = 5,
				targetPlayer = true
			},
			new ABacktrackMove {
				dir = -5,
				targetPlayer = true
			}
		]
	};
}


internal sealed class NovaCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, deck, charname, Rarity.rare, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		retain = true,
		exhaust = true,
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.BackflipStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.loseEvadeNextTurn,
				statusAmount = 1,
				targetPlayer = true
			},
			new AEnergy {
				changeAmount = 1
			}
			
		],
		_ => [
			new AStatus {
				status = Status.timeStop,
				statusAmount = upgrade == Upgrade.A ? 2 : 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.BackflipStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.loseEvadeNextTurn,
				statusAmount = 1,
				targetPlayer = true
			},
		]
	};
}

internal sealed class NibbsExeCard : Card, IRegisterableCard
{
	public static void Register(Deck deck, string charname, IModHelper helper, IPluginPackage<IModManifest> package) {
		IRegisterableCard.Register(MethodBase.GetCurrentMethod()!.DeclaringType!, Deck.colorless, charname, Rarity.common, helper, package, out _);
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		description = ColorlessLoc.GetDesc(state, upgrade == Upgrade.B ? 3 : 2, ModEntry.Instance.NibbsDeck),
		artTint = "ffffff"
    };

	public override List<CardAction> GetActions(State s, Combat c)
    {
		Deck deck = ModEntry.Instance.NibbsDeck;
		return upgrade switch
		{
			Upgrade.B => [
				new ACardOffering
				{
					amount = 3,
					limitDeck = deck,
					makeAllCardsTemporary = true,
					overrideUpgradeChances = false,
					canSkip = false,
					inCombat = true,
					discount = -1,
					dialogueSelector = ".summonNibbs"
				}
			],
			_ => [
				new ACardOffering
				{
					amount = 2,
					limitDeck = deck,
					makeAllCardsTemporary = true,
					overrideUpgradeChances = false,
					canSkip = false,
					inCombat = true,
					discount = -1,
					dialogueSelector = ".summonNibbs"
				}
			],
		};
	}
}