using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using System.Collections.Generic;
using System.Linq;

namespace TheJazMaster.Nibbs;

internal class SwitchInjections : BaseDialogue
{
	internal void Inject()
	{
		var CharacterType = ModEntry.Instance.NibbsCharacter.CharacterType;
		var dict = hashToLine["en"];

		IFileInfo file = GetJsonFile();
		if (!ModEntry.Instance.Helper.Storage.TryLoadJson<Dictionary<string, List<List<object>>>>(file, out var dialogue))
		{
			ModEntry.Instance.Logger.LogError("Dialogue loading failed. Tell the developer");
			throw new System.Exception();
		}

		foreach (KeyValuePair<string, List<List<object>>> kvp in dialogue)
		{
			string key = kvp.Key;
			string fullKey = key + "::" + CharacterType;
			if (!DB.story.all.TryGetValue(key, out var node))
				continue;
			if (node.lines.OfType<SaySwitch>().LastOrDefault() is not { } saySwitch)
				continue;

			int i = 0;
			foreach (List<object> list in kvp.Value)
			{
				saySwitch.lines.Add(new Say
				{
					hash = fullKey + "_" + i,
					who = CharacterType,
					loopTag = list.Count > 1 ? list[1] as string : "neutral"
				});
				dict.Add(key, new Dictionary<string, string> {
					{fullKey + "_" + i, (list[0] as string)!}
				});
				i++;
			}
		}

		ModEntry.Instance.Helper.Events.OnLoadStringsForLocale += (_, e) => InjectLocalizations(e);
	}


	internal override IFileInfo GetJsonFile()
	{
		return ModEntry.Instance.Package.PackageRoot.GetRelativeDirectory("I18n/en/Nibbs").GetRelativeFile("inject.json");
	}

	internal override NodeType GetNodeType()
	{
		return NodeType.combat;
	}

	internal override bool FlipPortrait(string who)
	{
		return false;
	}
}