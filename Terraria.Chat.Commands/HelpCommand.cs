using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Terraria.Chat.Commands;

[ChatCommand("Help")]
public class HelpCommand : IChatCommand
{
	private static readonly Color RESPONSE_COLOR = new Color(255, 240, 20);

	public void ProcessIncomingMessage(string text, byte clientId)
	{
		ChatHelper.SendChatMessageToClient(ComposeMessage(GetCommandAliasesByID()), RESPONSE_COLOR, clientId);
	}

	private static Dictionary<string, List<LocalizedText>> GetCommandAliasesByID()
	{
		object reference = Lang.CreateDialogSubstitutionObject();
		LocalizedText[] array = Language.FindAll(Lang.CreateDialogFilter("ChatCommandDescription.", reference));
		Dictionary<string, List<LocalizedText>> dictionary = new Dictionary<string, List<LocalizedText>>();
		LocalizedText[] array2 = array;
		foreach (LocalizedText localizedText in array2)
		{
			string key = localizedText.Key;
			key = key.Replace("ChatCommandDescription.", "");
			int num = key.IndexOf('_');
			if (num != -1)
			{
				key = key.Substring(0, num);
			}
			if (!dictionary.TryGetValue(key, out var value))
			{
				value = (dictionary[key] = new List<LocalizedText>());
			}
			value.Add(localizedText);
		}
		return dictionary;
	}

	private static NetworkText ComposeMessage(Dictionary<string, List<LocalizedText>> aliases)
	{
		string reference = "";
		for (int i = 0; i < aliases.Count; i++)
		{
			reference = reference + "{" + i + "}\n";
		}
		List<NetworkText> list = new List<NetworkText>();
		foreach (KeyValuePair<string, List<LocalizedText>> alias in aliases)
		{
			list.Add(Language.GetText("ChatCommandDescription." + alias.Key).ToNetworkText());
		}
		string text = reference;
		object[] substitutions = list.ToArray();
		return NetworkText.FromFormattable(text, substitutions);
	}

	public void ProcessOutgoingMessage(ChatMessage message)
	{
	}
}
