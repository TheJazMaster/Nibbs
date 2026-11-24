using Nanoray.PluginManager;
using System.Collections.Generic;

namespace TheJazMaster.Nibbs.Story.Nibbs;

internal class NibbsEventDialogue() : BaseDialogue(
	ModEntry.Instance.NibbsManager.NibbsCharacter.CharacterType,
	ModEntry.Instance.Package.PackageRoot.GetRelativeDirectory("I18n/en/Nibbs").GetRelativeFile("events.json")
) {
	internal void Inject()
	{
		//var CharacterType = ModEntry.Instance.NibbsManager.NibbsCharacter.CharacterType;
		var highPitchedStaticNode = DB.story.all["ChoiceCardRewardOfYourColorChoice"];
		highPitchedStaticNode.nonePresent ??= [];
		highPitchedStaticNode.nonePresent.Add(TranslateChar("Nibbs"));

		var nodePresets = new Dictionary<string, StoryNode> {
			{$"LoseCharacterCard_{characterType}", new StoryNode {
				oncePerRun = true,
				bg = "BGSupernova",
			}},
			{$"CrystallizedFriendEvent_{characterType}", new StoryNode {
				oncePerRun = true,
				bg = "BGCrystalizedFriend",
			}},
			{$"ChoiceCardRewardOfYourColorChoice_{characterType}", new StoryNode {
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
					"TheJazMaster.Nibbs::Nibbs_RunStartFirst_Basic_0"
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

	internal override NodeType GetNodeType()
	{
		return NodeType.@event;
	}

	internal override bool FlipPortrait(string who)
	{
		return who != TranslateChar("Nibbs");
	}
}