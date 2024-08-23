// TODO: LOOK UP RETURNING FROM MISSING
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Newtonsoft.Json.Linq;
using Nickel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using TheJazMaster.Nibbs.Artifacts;

namespace TheJazMaster.Nibbs;

internal class EventDialogue : BaseDialogue
{
	internal void Inject() {
		var CharacterType = ModEntry.Instance.NibbsCharacter.CharacterType;
		var highPitchedStaticNode = DB.story.all["ChoiceCardRewardOfYourColorChoice"];
		highPitchedStaticNode.nonePresent ??= [];
		highPitchedStaticNode.nonePresent.Add(TranslateChar("Nibbs"));

		var nodePresets = new Dictionary<string, StoryNode> {
			{$"LoseCharacterCard_{CharacterType}", new StoryNode {
				oncePerRun = true,
				bg = "BGSupernova",
			}},
			{$"CrystallizedFriendEvent_{CharacterType}", new StoryNode {
				oncePerRun = true,
				bg = "BGCrystalizedFriend",
			}},
			{$"ChoiceCardRewardOfYourColorChoice_{CharacterType}", new StoryNode {
				oncePerRun = true,
				bg = "BGBootSequence",
			}},
			{"HighPitchedStatic",  new StoryNode {
				oncePerRun = highPitchedStaticNode.oncePerRun,
				bg = highPitchedStaticNode.bg,
				choiceFunc = highPitchedStaticNode.choiceFunc,
				canSpawnOnMap = highPitchedStaticNode.canSpawnOnMap
			}},
			{"ShopkeeperInfinite", new StoryNode {
				lookup = [
					"shopBefore"
				],
				bg = "BGShop",
			}},
			{"RunStartFirst", new StoryNode {
				lookup = [
					"zone_first"
				],
				priority = true,
				once = true,
			}},
			{"NextNode", new StoryNode {
				lookup = [
					"after_any"
				],
				priority = false,
				requiredScenes = [
					"RunStartFirst"
				],
				once = true,
			}},
			{"RunStartJohnson", new StoryNode {
				lookup = [
					"zone_first"
				],
				priority = true,
				once = true,
			}},
		};

		InjectStory(nodePresets);
		ModEntry.Instance.Helper.Events.OnLoadStringsForLocale += (_, e) => InjectLocalizations(e);
	}

	internal override IFileInfo GetJsonFile()
	{
		return ModEntry.Instance.Package.PackageRoot.GetRelativeDirectory("I18n/en").GetRelativeFile("events.json");
	}

	internal override NodeType GetNodeType()
	{
		return NodeType.@event;
	}

	internal override bool FlipPortrait(string who)
	{
		return who != TranslateChar("Nibbs");
	}
}