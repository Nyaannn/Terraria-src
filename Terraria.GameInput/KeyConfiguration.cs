using System.Collections.Generic;
using System.Linq;

namespace Terraria.GameInput;

public class KeyConfiguration
{
	public Dictionary<string, List<string>> KeyStatus = new Dictionary<string, List<string>>();

	public bool DoGrappleAndInteractShareTheSameKey
	{
		get
		{
			if (KeyStatus["Grapple"].Count > 0 && KeyStatus["MouseRight"].Count > 0)
			{
				return KeyStatus["MouseRight"].Contains(KeyStatus["Grapple"][0]);
			}
			return false;
		}
	}

	public void SetupKeys()
	{
		KeyStatus.Clear();
		foreach (string knownTrigger in PlayerInput.KnownTriggers)
		{
			KeyStatus.Add(knownTrigger, new List<string>());
		}
	}

	public void Processkey(TriggersSet set, string newKey, InputMode mode)
	{
		foreach (KeyValuePair<string, List<string>> effects in KeyStatus)
		{
			if (effects.Value.Contains(newKey))
			{
				set.KeyStatus[effects.Key] = true;
				set.LatestInputMode[effects.Key] = mode;
			}
		}
		if (set.Up || set.Down || set.Left || set.Right || set.HotbarPlus || set.HotbarMinus || ((Main.gameMenu || Main.ingameOptionsWindow) && (set.MenuUp || set.MenuDown || set.MenuLeft || set.MenuRight)))
		{
			set.UsedMovementKey = true;
		}
	}

	public void CopyKeyState(TriggersSet oldSet, TriggersSet newSet, string newKey)
	{
		foreach (KeyValuePair<string, List<string>> item in KeyStatus)
		{
			if (item.Value.Contains(newKey))
			{
				newSet.KeyStatus[item.Key] = oldSet.KeyStatus[item.Key];
			}
		}
	}

	public void ReadPreferences(Dictionary<string, List<string>> dict)
	{
		foreach (KeyValuePair<string, List<string>> item in dict)
		{
			if (!KeyStatus.ContainsKey(item.Key))
			{
				continue;
			}
			KeyStatus[item.Key].Clear();
			foreach (string item2 in item.Value)
			{
				KeyStatus[item.Key].Add(item2);
			}
		}
	}

	public Dictionary<string, List<string>> WritePreferences()
	{
		Dictionary<string, List<string>> i = new Dictionary<string, List<string>>();
		foreach (KeyValuePair<string, List<string>> item in KeyStatus)
		{
			if (item.Value.Count > 0)
			{
				i.Add(item.Key, item.Value.ToList());
			}
		}
		if (!i.ContainsKey("MouseLeft") || i["MouseLeft"].Count == 0)
		{
			i["MouseLeft"] = new List<string> { "Mouse1" };
		}
		if (!i.ContainsKey("Inventory") || i["Inventory"].Count == 0)
		{
			i["Inventory"] = new List<string> { "Escape" };
		}
		return i;
	}
}
