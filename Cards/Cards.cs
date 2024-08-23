using Nickel;
using TheJazMaster.Nibbs.Actions;
using System.Collections.Generic;
using System.Reflection;

namespace TheJazMaster.Nibbs.Cards;


internal sealed class SpaceHoppingCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("SpaceHopping", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/SpaceHopping.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SpaceHopping", "name"]).Localize
		});
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


internal sealed class WingsOfFireCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("WingsOfFire", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/WingsOfFire.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "WingsOfFire", "name"]).Localize
		});
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


internal sealed class SmeltCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Smelt", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_eunice,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Smelt", "name"]).Localize
		});
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


internal sealed class TrailblazerCard : Card, INibbsCard
{
	internal static Spr Art;
	internal static Spr ArtFlipped;

	public static void Register(IModHelper helper) {
		Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/BlazingPathFlipped.png")).Sprite;
		ArtFlipped = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/BlazingPath.png")).Sprite;
		helper.Content.Cards.RegisterCard("Trailblazer", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Trailblazer", "name"]).Localize
		});
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


internal sealed class WormholeSurfingCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("WormholeSurfing", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/WormholeSurfing.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "WormholeSurfing", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 2 : 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.None => [
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = Status.timeStop,
				statusAmount = upgrade == Upgrade.B ? 2 : 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = upgrade == Upgrade.B ? 2 : 1,
				targetPlayer = true
			}
		],
	};
}


internal sealed class OverHereCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("OverHere", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/OverHere.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "OverHere", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 3 : 1,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.BacktrackAutododgeLeftStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.BacktrackAutododgeRightStatus.Status,
				statusAmount = upgrade == Upgrade.A ? 2 : 1,
				targetPlayer = true
			}
		],
	};
}


internal sealed class HotPursuitCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("HotPursuit", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HotPursuit", "name"]).Localize
		});
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
		Upgrade.B => [
			new AMove {
				dir = 2,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 2),
				stunEnemy = true
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


internal sealed class BackflipCard : Card, INibbsCard
{
	internal static Spr Art;
	internal static Spr ArtFlipped;

	public static void Register(IModHelper helper) {
		Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Backflip.png")).Sprite;
		ArtFlipped = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/BackflipFlipped.png")).Sprite;
		helper.Content.Cards.RegisterCard("Backflip", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Backflip", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		flippable = true,
		art = flipped ? ArtFlipped : Art,
		exhaust = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.BackflipStatus.Status,
			statusAmount = upgrade == Upgrade.B ? 5 : upgrade == Upgrade.A ? 4 : 3,
			targetPlayer = true
		},
		new ABacktrackMove {
			dir = 1,
			targetPlayer = true
		}
	];
}


internal sealed class SmokescreenCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Smokescreen", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Smokescreen.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Smokescreen", "name"]).Localize
		});
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
				status = ModEntry.Instance.BackflipStatus.Status,
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
				status = ModEntry.Instance.BackflipStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			}
		],
	};
}


internal sealed class QuantumTurbulenceCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("QuantumTurbulence", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/QuantumTurbulence.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "QuantumTurbulence", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		recycle = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "QuantumTurbulence", "description", upgrade.ToString()], new {
			Dir = flipped ? Loc.T("card.LoadState.desc.combat.right") : Loc.T("card.LoadState.desc.combat.left"),
			Amount = 1 + state.ship.Get(Status.hermes) 
		})
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AMove {
			dir = 0,
			targetPlayer = true,
			omitFromTooltips = true
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


internal sealed class FlapFlapCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("FlapFlap", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/FlapFlap.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FlapFlap", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 3 : 2,
		art = upgrade == Upgrade.B ? StableSpr.cards_BranchPredictionFlipped : StableSpr.cards_BranchPrediction,
		exhaust = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = upgrade switch {
				Upgrade.A => ModEntry.Instance.BacktrackAutododgeRightStatus.Status,
				Upgrade.B => Status.autododgeLeft,
				_ => Status.autododgeRight
			}, 
			statusAmount = 2,
			targetPlayer = true
		}
	];
}


internal sealed class HydraulicsCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Hydraulics", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/CoolantPump.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Hydraulics", "name"]).Localize
		});
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


internal sealed class HoldOnCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("HoldOn", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/HoldOn.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HoldOn", "name"]).Localize
		});
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
			status = ModEntry.Instance.BackflipStatus.Status,
			statusAmount = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true
		},
	];
}


internal sealed class DimensionalJauntCard : Card, INibbsCard
{
	internal static Spr HopArt;
	internal static Spr SkipArt;

	public static void Register(IModHelper helper) {
		HopArt = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Hop.png")).Sprite;
		SkipArt = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Skip.png")).Sprite;
		helper.Content.Cards.RegisterCard("DimensionalJaunt", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/DimensionalJaunt.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "DimensionalJaunt", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "DimensionalJaunt", "description", upgrade.ToString()])
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.BackflipStatus.Status,
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


internal sealed class HopCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Hop", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Hop", "name"]).Localize
		});
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


internal sealed class SkipCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Skip", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true
			},
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Skip.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Skip", "name"]).Localize
		});
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


internal sealed class TauntCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Taunt", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_Ace,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Taunt.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Taunt", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 1 : 0,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.BackflipStatus.Status,
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
				status = ModEntry.Instance.BackflipStatus.Status,
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


internal sealed class FluxCompressorCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("FluxCompressor", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/FluxCompressor.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FluxCompressor", "name"]).Localize
		});
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


internal sealed class DragonFrenzyCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("DragonFrenzy", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/DragonFrenzy.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "DragonFrenzy", "name"]).Localize
		});
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


internal sealed class QuantumCollapseCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("QuantumCollapse", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/QuantumCollapse.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "QuantumCollapse", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2
	};

	private static int GetSchmoveAmt(State s)
	{
		if (s.route is Combat)
		{
			return s.ship.Get(ModEntry.Instance.BacktrackLeftStatus.Status) + s.ship.Get(ModEntry.Instance.BacktrackRightStatus.Status);
		}
		return 0;
	}

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AAttack {
				damage = GetDmg(s, 1)
			},
			new AVariableHint {
				status = ModEntry.Instance.BacktrackLeftStatus.Status,
				secondStatus = ModEntry.Instance.BacktrackRightStatus.Status
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetSchmoveAmt(s)),
			}
		],
		_ => [
			new AAttack {
				damage = GetDmg(s, upgrade == Upgrade.A ? 3 : 1)
			},
			new AVariableHint {
				status = ModEntry.Instance.BacktrackLeftStatus.Status,
				secondStatus = ModEntry.Instance.BacktrackRightStatus.Status
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetSchmoveAmt(s)),
			},
			new AStatus {
				status = ModEntry.Instance.BacktrackLeftStatus.Status,
				mode = AStatusMode.Set,
				statusAmount = 0,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.BacktrackRightStatus.Status,
				mode = AStatusMode.Set,
				statusAmount = 0,
				targetPlayer = true
			},
		]
	};
}


internal sealed class BlurCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Blur", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_Ace,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Blur", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		flippable = true,
		exhaust = upgrade != Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.SmokescreenStatus.Status,
			statusAmount = 1,
			targetPlayer = true
		},
		new AMove {
			dir = 1,
			targetPlayer = true
		}
	];
}


internal sealed class SuperpositionCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Superposition", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Superposition.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Superposition", "name"]).Localize
		});
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


internal sealed class NovaCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Nova", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_SolarFlair,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Nova", "name"]).Localize
		});
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
				status = ModEntry.Instance.BackflipStatus.Status,
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
				status = ModEntry.Instance.BackflipStatus.Status,
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

internal sealed class NibbsExeCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("NibbsExe", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = Deck.colorless,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/NibbsExe.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "NibbsExe", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		description = ColorlessLoc.GetDesc(state, upgrade == Upgrade.B ? 3 : 2, ModEntry.Instance.NibbsDeck.Deck),
		artTint = "ffffff"
    };

	public override List<CardAction> GetActions(State s, Combat c)
    {
		Deck deck = ModEntry.Instance.NibbsDeck.Deck;
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