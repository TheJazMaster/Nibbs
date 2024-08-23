using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Newtonsoft.Json.Linq;
using Nickel;
using System.Collections.Generic;

namespace TheJazMaster.Nibbs;

internal abstract class BaseDialogue()
{
	// Locale, then key, then hash, gets line
	protected readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> hashToLine = new()
	{
		{"en", new() }
	};

	internal static string TranslateChar(string name) {
		return name switch {
			"Dizzy" => "dizzy",
			"Riggs" => "riggs",
			"Peri" => "peri",
			"Isaac" => "goat",
			"Drake" => "eunice",
			"Max" => "hacker",
			"Books" => "shard",
			"CAT" => "comp",
			"Cleo" => "nerd",
			"Void" => "void",
			"Nibbs" => "TheJazMaster.Nibbs::Nibbs",
			"Johnson" => "Shockah.Johnson::Johnson",
			"Nola" => "Mezz.TwosCompany.NolaDeck",
			"Isabelle" => "Mezz.TwosCompany.IsabelleDeck",
			"Ilya" => "Mezz.TwosCompany.IlyaDeck",
			"Gauss" => "Mezz.TwosCompany.GaussDeck",
			"Jost" => "Mezz.TwosCompany.JostDeck",
			"Eddie" => "Eddie.EddieDeck",
			_ => "crew"
		};
	}

	internal abstract IFileInfo GetJsonFile();
	internal abstract NodeType GetNodeType();
	internal abstract bool FlipPortrait(string who);


	// Returns list of story instructions, but also the translated string
	protected List<Instruction> ConvertToInstructions(List<object> list, string key) {

		List<Instruction> ret = [];
		var dict = hashToLine["en"];

		// Case: generic nibbs line. First string is line, second is looptag
		if (list[0] is string str) {
			ret.Add(new Say {
				hash = "0",
				who = TranslateChar("Nibbs"),
				loopTag = list.Count > 1 ? (list[1] as string) : "neutral",
			});
			if (!dict.TryGetValue(key, out Dictionary<string, string>? value)) {
				dict.Add(key, new Dictionary<string, string> {
					{"0", str}
				});
			} else {
				value["0"] = str;
			}
		}
		// Case: line with multiple people
		else if (list[0] is JObject) {
			// Iterates over the multiple lines
			for (int i = 0, h = 0; i < list.Count; i++, h++) {
				foreach (KeyValuePair<string, JToken?> kvp in (list[i] as JObject)!) {
					if (kvp.Key == "Switch") {
						ret.Add(new SaySwitch {
							lines = ConvertToSays((kvp.Value as JArray)!.ToObject<List<object>>()!, key, ref h)
						});
					}
					else if (kvp.Key == "GreedySwitch") {
						ret.Add(new GreedySwitch {
							lines = ConvertToSays((kvp.Value as JArray)!.ToObject<List<object>>()!, key, ref h),
							banned = [ModEntry.Instance.NibbsCharacter.CharacterType]
						});
					}
					else if (kvp.Key == "Jump") {
						ret.Add(new Jump {
							key = kvp.Value!.ToObject<string>()!
						});
					}
					else {
						var lineInfo = (kvp.Value as JArray)!.ToObject<List<string>>()!;
						ret.Add(new Say {
							hash = h.ToString(),
							who = TranslateChar(kvp.Key),
							flipped = FlipPortrait(TranslateChar(kvp.Key)),
							loopTag = lineInfo.Count > 1 ? lineInfo[1] : "neutral"
						});

						// Add to english loc
						if (!dict.TryGetValue(key, out Dictionary<string, string>? value)) {
							dict.Add(key, new Dictionary<string, string> {
								{h.ToString(), lineInfo[0]}
							});
						} else {
							value[h.ToString()] = lineInfo[0];
						}
					}
				}
			}
		}
		return ret;
	}


	// As above but returns a list of says from inside a SaySwitch
	protected List<Say> ConvertToSays(List<object> list, string key, ref int hash) {
		List<Say> ret = new();
		var dict = hashToLine["en"];

		// Case: generic nibbs line. First string is line, second is looptag
		if (list[0] is string str) {
			ret.Add(new Say {
				hash = hash.ToString(),
				who = TranslateChar("Nibbs"),
				loopTag = list.Count > 1 ? (list[1] as string) : "neutral"
			});
			if (!dict.TryGetValue(key, out Dictionary<string, string>? value)) {
				dict.Add(key, new Dictionary<string, string> {
					{hash.ToString(), str}
				});
			} else {
				value[hash.ToString()] = str;
			}
			hash++;
		}
		// Case: line with multiple people
		else if (list[0] is JObject) {
			for (int i = 0; i < list.Count; i++, hash++) {
				foreach (KeyValuePair<string, JToken?> kvp in (list[i] as JObject)!) {
					var lineInfo = (kvp.Value as JArray)!.ToObject<List<string>>()!;
					ret.Add(new Say {
						hash = hash.ToString(),
						who = TranslateChar(kvp.Key),
						loopTag = lineInfo.Count > 1 ? lineInfo[1] : "neutral"
					});

					// Add to english loc
					if (!dict.TryGetValue(key, out Dictionary<string, string>? value)) {
						dict.Add(key, new Dictionary<string, string> {
							{hash.ToString(), lineInfo[0]}
						});
					} else {
						value[hash.ToString()] = lineInfo[0];
					}
				}

			}	
		}
		else throw new System.Exception();
		return ret;
	}

	internal void InjectStory(Dictionary<string, StoryNode> newNodes)
	{
		var NibbsType = ModEntry.Instance.NibbsCharacter.CharacterType;

		IFileInfo file = GetJsonFile();
		if(!ModEntry.Instance.Helper.Storage.TryLoadJson<Dictionary<string, Dictionary<string, List<List<object>>>>>(file, out var dialogue)) {
			ModEntry.Instance.Logger.LogError("Dialogue loading failed. Tell the developer");
			throw new System.Exception();
		}
		
		foreach (KeyValuePair<string, Dictionary<string, List<List<object>>>> kvp in dialogue) {
			foreach (KeyValuePair<string, List<List<object>>> pairs in kvp.Value) {
				for (int i = 0; i < pairs.Value.Count; i++) {

					bool prefix = !kvp.Key.Contains("{{CharacterType}}");
					string key = kvp.Key.Replace("{{CharacterType}}", NibbsType);
					string advancedKey = prefix ? "TheJazMaster.Nibbs::" + key : key;
					string fullKey = advancedKey + (prefix ? ("_" + pairs.Key + "_" + i) : "");

					StoryNode node = Mutil.DeepCopy(newNodes[key]);
					node.lines = ConvertToInstructions(pairs.Value[i], fullKey);

					node.oncePerCombatTags = (node.oncePerCombatTags != null && node.oncePerCombatTags.Count == 0) ? [key] : node.oncePerCombatTags;
					node.oncePerRunTags = (node.oncePerRunTags != null && node.oncePerRunTags.Count == 0) ? [key] : node.oncePerRunTags;
					node.allPresent ??= [];
					node.allPresent.Add(TranslateChar("Nibbs"));
					if (pairs.Key != "Basic") {
						foreach (string character in pairs.Key.Split("_"))
							node.allPresent.Add(TranslateChar(character));
					}
					node.type = GetNodeType();
					
					DB.story.all[fullKey] = node;
				}
			}
		}
	}

	protected void InjectLocalizations(LoadStringsForLocaleEventArgs e)
	{
		foreach (var (key, dict) in hashToLine[e.Locale])
		{
			foreach (var (hash, str) in dict)
			{
				e.Localizations[$"{key}:{hash}"] = str;
			}
		}
	}
}