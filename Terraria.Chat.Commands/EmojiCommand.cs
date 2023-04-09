using System;
using System.Collections.Generic;
using Terraria.GameContent.UI;
using Terraria.Localization;

namespace Terraria.Chat.Commands;

[ChatCommand("Emoji")]
public class EmojiCommand : IChatCommand
{
	public const int PlayerEmojiDuration = 360;

	private readonly Dictionary<LocalizedText, int> _byName = new Dictionary<LocalizedText, int>();

	public EmojiCommand()
	{
		Initialize();
	}

	public void Initialize()
	{
		_byName.Clear();
		for (int prefix = 0; prefix < EmoteID.Count; prefix++)
		{
			LocalizedText emojiName = Lang.GetEmojiName(prefix);
			if (emojiName != LocalizedText.Empty)
			{
				_byName[emojiName] = prefix;
			}
		}
	}

	public void ProcessIncomingMessage(string text, byte clientId)
	{
	}

	public void ProcessOutgoingMessage(ChatMessage message)
	{
		int reference = -1;
		if (int.TryParse(message.Text, out reference))
		{
			if (reference < 0 || reference >= EmoteID.Count)
			{
				return;
			}
		}
		else
		{
			reference = -1;
		}
		if (reference == -1)
		{
			foreach (LocalizedText key in _byName.Keys)
			{
				if (message.Text == key.Value)
				{
					reference = _byName[key];
					break;
				}
			}
		}
		if (reference != -1)
		{
			if (Main.netMode == 0)
			{
				EmoteBubble.NewBubble(reference, new WorldUIAnchor(Main.LocalPlayer), 360);
				EmoteBubble.CheckForNPCsToReactToEmoteBubble(reference, Main.LocalPlayer);
			}
			else
			{
				NetMessage.SendData(120, -1, -1, null, Main.myPlayer, reference);
			}
		}
		message.Consume();
	}

	public void PrintWarning(string text)
	{
		throw new Exception("This needs localized text!");
	}
}
