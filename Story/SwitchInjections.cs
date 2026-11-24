using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using System.Collections.Generic;
using System.Linq;

namespace TheJazMaster.Nibbs.Story;

internal abstract class BaseSwitchInjections(string characterType, IFileInfo file) : BaseDialogue(characterType, file) {
	internal void Inject()
	{
		var dict = hashToLine["en"];

		IFileInfo file = jsonFile;
		if (!ModEntry.Instance.Helper.Storage.TryLoadJson<Dictionary<string, List<List<object>>>>(file, out var dialogue))
		{
			ModEntry.Instance.Logger.LogError("Dialogue loading failed. Tell the developer");
			throw new System.Exception();
		}

		foreach (KeyValuePair<string, List<List<object>>> kvp in dialogue)
		{
			string key = kvp.Key;
			string fullKey = key + "::" + characterType;
			if (!DB.story.all.TryGetValue(key, out var node)) {
				ModEntry.Instance.Logger.LogWarning($"Node {key} for {characterType} switch injection not found. Some dialogue may not work.");
				continue;
			}
			if (node.lines.OfType<SaySwitch>().LastOrDefault() is not { } saySwitch) {
				ModEntry.Instance.Logger.LogWarning($"Switch injection for node {key} for {characterType} not found. Some dialogue may not work.");
				continue;
			}

			int i = 0;
			foreach (List<object> list in kvp.Value)
			{
				saySwitch.lines.Add(new Say
				{
					hash = fullKey + "_" + i,
					who = characterType,
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

	internal override NodeType GetNodeType()
	{
		return NodeType.combat;
	}

	internal override bool FlipPortrait(string who)
	{
		return false;
	}
}