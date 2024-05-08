using Nickel;
using TheJazMaster.Nibbs.Actions;
using System.Collections.Generic;
using System.Reflection;
using TheJazMaster.Nibbs.Features;
using System;
using System.Collections.ObjectModel;

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
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/SpaceHopping.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SpaceHopping", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		art = flipped ? StableSpr.cards_ScootLeft : StableSpr.cards_ScootRight,
		flippable = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ABacktrackMove {
			dir = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true
		},
		new ABacktrackMove {
			dir = 1,
			targetPlayer = true
		}
	];
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
			Art = StableSpr.cards_Heat,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/WingsOfFire.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "WingsOfFire", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ABacktrackMove {
			dir = upgrade == Upgrade.A ? -3 : -2,
			targetPlayer = true
		},
		new AStatus {
			status = Status.hermes,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		},
		new AStatus {
			status = Status.heat,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		}
	];
}

internal sealed class FireballCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Fireball", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_DrakeCannon,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Fireball.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Fireball", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1),
			stunEnemy = true
		},
		new AStatus {
			status = Status.heat,
			statusAmount = upgrade == Upgrade.B ? -2 : -1,
			targetPlayer = true
		}
	];
}


internal sealed class BlazingPathCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("BlazingPath", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/BlazingPath.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "BlazingPath", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		art = flipped ? StableSpr.cards_ScootLeft : StableSpr.cards_ScootRight,
		flippable = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new ABacktrackMove {
				dir = 3,
				targetPlayer = true
			},
			new ABacktrackMove {
				dir = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.heat,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new ABacktrackMove {
				dir = 3,
				targetPlayer = true
			},
			new AStatus {
				status = Status.heat,
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
			Art = StableSpr.cards_Panic,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/WormholeSurfing.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "WormholeSurfing", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = upgrade != Upgrade.A
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.timeStop,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		}
	];
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
			Art = StableSpr.cards_Panic,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/OverHere.png")).Sprite,
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
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/HotPursuit.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HotPursuit", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		art = flipped ? StableSpr.cards_ScootLeft : StableSpr.cards_ScootRight,
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AMove {
				dir = 2,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 1),
				stunEnemy = true
			},
			new AStatus {
				status = Status.heat,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		Upgrade.B => [
			new ABacktrackMove {
				dir = 2,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 1),
				stunEnemy = true
			},
			new ABacktrackMove {
				dir = -2,
				targetPlayer = true
			},
			new AStatus {
				status = Status.heat,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new ABacktrackMove {
				dir = 2,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 1),
				stunEnemy = true
			},
			new AStatus {
				status = Status.heat,
				statusAmount = 1,
				targetPlayer = true
			}
		],
	};
}


internal sealed class BackflipCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Backflip", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_Dodge,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Backflip.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Backflip", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		flippable = true,
		exhaust = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.BackflipStatus.Status,
			statusAmount = upgrade == Upgrade.B ? 4 : upgrade == Upgrade.A ? 3 : 2,
			targetPlayer = true
		},
		new ABacktrackMove {
			dir = 1,
			targetPlayer = true
		}
	];
}


internal sealed class SteamCoverCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("SteamCover", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_TrashFumes,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/SteamCover.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SteamCover", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.tempShield,
			statusAmount = upgrade == Upgrade.B ? 1 : upgrade == Upgrade.A ? 3 : 2,
			targetPlayer = true
		},
		new AStatus {
			status = Status.heat,
			statusAmount = -1,
			targetPlayer = true
		}
	];
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
			Art = StableSpr.cards_CombustionEngine,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/QuantumTurbulence.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "QuantumTurbulence", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		recycle = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "QuantumTurbulence", "description", upgrade.ToString()], new { Amount = 1 + state.ship.Get(Status.hermes) } )
	};

	public override void OnDraw(State s, Combat c)
	{
		c.Queue(new ABacktrackMove {
			dir = -1,
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
				dir = 1,
				targetPlayer = true
			});
	}
}


internal sealed class FireBreathCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("FireBreath", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_eunice,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/FireBreath.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FireBreath", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2
	};

	private int GetHeatAmt(State s)
	{
		int num = 0;
		if (s.route is Combat)
		{
			num = s.ship.Get(Status.heat);
			if (num < 0)
			{
				num = 0;
			}
		}
		return num;
	}

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
	{
		Upgrade.A => [
			new AVariableHint {
				status = Status.heat
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetHeatAmt(s))
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetHeatAmt(s))
			},
			new AAttack {
				damage = GetDmg(s, 2)
			},
			new AStatus {
				status = Status.heat,
				mode = AStatusMode.Set,
				statusAmount = 0,
				targetPlayer = true
			},
		],
		Upgrade.B => [
			new AVariableHint {
				status = Status.heat
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetHeatAmt(s)),
				stunEnemy = true
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetHeatAmt(s)),
				stunEnemy = true
			},
			new AStatus {
				status = Status.heat,
				mode = AStatusMode.Set,
				statusAmount = 0,
				targetPlayer = true
			},
		],
		_ => [
			new AVariableHint {
				status = Status.heat
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetHeatAmt(s))
			},
			new AAttack {
				xHint = 1,
				damage = GetDmg(s, GetHeatAmt(s))
			},
			new AStatus {
				status = Status.heat,
				mode = AStatusMode.Set,
				statusAmount = 0,
				targetPlayer = true
			},
		],
	};
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

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
	{
		Upgrade.A => [
			new AStatus {
				status = Status.autododgeRight,
				statusAmount = 2,
				targetPlayer = true
			},
			new AStatus {
				status = Status.timeStop,
				statusAmount = 1,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
		],
		Upgrade.B => [
			new AStatus {
				status = Status.autododgeLeft,
				statusAmount = 2,
				targetPlayer = true
			},
		],
		_ => [
			new AStatus {
				status = Status.autododgeRight,
				statusAmount = 2,
				targetPlayer = true
			},
		],
	};
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
			Art = StableSpr.cards_Fleetfoot,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/HoldOn.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HoldOn", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		retain = true,
		exhaust = upgrade != Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = Status.hermes,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = Status.hermes,
				statusAmount = 2,
				targetPlayer = true
			},
			new AStatus {
				status = Status.heat,
				statusAmount = 2,
				targetPlayer = true
			}
		]
	};
}


internal sealed class DimensionalJauntCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("DimensionalJaunt", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_Reroute,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/DimensionalJaunt.png")).Sprite,
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
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Hop.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Hop", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		art = flipped ? StableSpr.cards_ScootRight : StableSpr.cards_ScootLeft,
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
		art = flipped ? StableSpr.cards_ScootLeft : StableSpr.cards_ScootRight,
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
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.BackflipStatus.Status,
				statusAmount = 5,
				targetPlayer = true
			},
			new ADrawCard {
				count = 3	
			},
			new AStatus {
				status = Status.evade,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
			new AStatus {
				status = Status.shield,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.BackflipStatus.Status,
				statusAmount = 4,
				targetPlayer = true
			},
			new ADrawCard {
				count = 2
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
			Art = StableSpr.cards_StunSource,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/FluxCompressor.png")).Sprite,
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
			new AAttack {
				damage = GetDmg(s, 1),
				stunEnemy = true
			}
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
			Art = StableSpr.cards_BlockerBurnout,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/DragonFrenzy.png")).Sprite,
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
		// new AStatus {
		// 	status = Status.temporaryCheap,
		// 	statusAmount = 0,
		// 	mode = AStatusMode.Set,
		// 	targetPlayer = true
		// },
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
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_CloudSave,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Smokescreen.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Smokescreen", "name"]).Localize
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
			Art = StableSpr.cards_Vamoose,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Superposition.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Superposition", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 2 : 3
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ABacktrackMove {
				dir = 6,
				targetPlayer = true
			},
			new ABacktrackMove {
				dir = -6,
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
				dir = 6,
				targetPlayer = true
			},
			new ABacktrackMove {
				dir = -6,
				targetPlayer = true
			}
		]
	};
}


internal sealed class CoolantPumpCard : Card, INibbsCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("CoolantPump", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.NibbsDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = StableSpr.cards_HeatSink,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/CoolantPump.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CoolantPump", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		exhaust = upgrade == Upgrade.B
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.heat,
			statusAmount = -3,
			targetPlayer = true
		},
		new ADrawCard {
			count = upgrade == Upgrade.A ? 4 : 2
		}
	];
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
			Art = StableSpr.cards_ThermalBattery,
			// Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Nova.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Nova", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 2 : 1,
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
			new ADrawCard {
				count = 3
			},
			new AStatus {
				status = Status.heat,
				statusAmount = 3,
				targetPlayer = true
			},
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
			new ADrawCard {
				count = 1
			},
			new AStatus {
				status = Status.heat,
				statusAmount = 3,
				targetPlayer = true
			},
		]
	};
}
