// SaySwitch
using System.Collections.Generic;
using System.Linq;

namespace TheJazMaster.Nibbs;

public class GreedySwitch : SaySwitch
{
	public HashSet<string> banned = [];

	public bool isExhausted = false;

	public override bool Execute(G g, IScriptTarget target, ScriptCtx ctx)
	{
		Say? say = PickLine(g, target);
		if (say is not null && !isExhausted) {
			if (say.who == "crew" || banned.Count >= g.state.characters.Count) isExhausted = true;
			return say.Execute(g, target, ctx);
		} else {
			isExhausted = true;
		}
		return true;
	}

	public new Say? PickLine(G g, IScriptTarget target)
	{
		Say? result = RandomLineForPresentCharacter(g, target);
		if (result == null) {
			return RandomLineForGenericCrew(g);
		}
		return result;
	}

	private new Say? RandomLineForPresentCharacter(G g, IScriptTarget target)
	{
		List<Say> linesForPresentChars = GetLinesForPresentChars(g, target);
		if (linesForPresentChars.Count > 0)
		{
			Say line = linesForPresentChars.Random(g.state.rngScript);
			banned.Add(line.who);
			return line;
		}
		return null;
	}

	private new List<Say> GetLinesForPresentChars(G g, IScriptTarget target)
	{
		G g2 = g;
		IScriptTarget target2 = target;
		return lines.Where(delegate(Say line)
		{
			if (line.who == "crew") return false;
			bool isBanned = banned.Contains(line.who);
			bool num = g2.state.characters.Any((Character ch) => ch.type == line.who);
			bool flag = target2 is Dialogue && line.who == "comp";
			bool flag2 = target2 is Combat combat && combat.otherShip?.ai?.character.type == line.who;
			bool flag3 = target2 is Dialogue && NewRunOptions.allChars.All((Deck ch) => ch.Key() != line.who);
			return !isBanned && (num || flag || flag2 || flag3);
		}).ToList();
	}
}
