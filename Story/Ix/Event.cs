using Nanoray.PluginManager;
using System.Collections.Generic;

namespace TheJazMaster.Nibbs.Story.Ix;

internal class IxEventDialogue() : BaseDialogue(
	ModEntry.Instance.IxManager.IxCharacter.CharacterType,
	ModEntry.Instance.Package.PackageRoot.GetRelativeDirectory("I18n/en/Ix").GetRelativeFile("event.json")
) {
	internal void Inject()
	{
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
			{"Batboy", new StoryNode {
				lookup = [
					"before_batboy"
				],
				priority = false,
				once = true,
				requiredScenes = [
					"Batboy_1"
				],
			}},
			{"IsaacPolluting_1", new StoryNode {
				lookup = [
					"zone_three"
				],
				allPresent = [
					TranslateChar("Isaac"),
					TranslateChar("Ix")
				],
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