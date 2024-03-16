using System.Collections.Generic;
using System.Linq;
using FSPRO;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using TheJazMaster.Nibbs.Patches;

namespace TheJazMaster.Nibbs.Features;
#nullable enable

class BacktrackManager : IEvadeHook, IDroneShiftHook
{
	internal static ModEntry Instance => ModEntry.Instance;

	internal static readonly string NoStrafeKey = "NoStrafe";

	public BacktrackManager() : base()
	{
		Instance.KokoroApi.RegisterEvadeHook(this, 10);
	}

	bool? IEvadeHook.IsEvadePossible(State state, Combat combat, int direction, EvadeHookContext context)
	{
		if (direction < 0 && state.ship.Get(Instance.BacktrackLeftStatus.Status) > 0) return true;
		else if (direction > 0 && state.ship.Get(Instance.BacktrackRightStatus.Status) > 0) return true;
		return null;
	}

	void IEvadeHook.PayForEvade(State state, Combat combat, int direction)
	{
		if (direction < 0)
			state.ship.Add(Instance.BacktrackLeftStatus.Status, -1);
		else if (direction > 0)
			state.ship.Add(Instance.BacktrackRightStatus.Status, -1);
	}

	List<CardAction>? IEvadeHook.ProvideEvadeActions(State state, Combat combat, int direction) {
		var move = new AMove {
			dir = direction,
			targetPlayer = true,
			ignoreHermes = true,
			isTeleport = true
		};
		Instance.Helper.ModData.SetModData(move, NoStrafeKey, true);
		return [move];
	} 
}
