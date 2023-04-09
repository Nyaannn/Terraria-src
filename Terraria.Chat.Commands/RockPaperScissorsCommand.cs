using Terraria.GameContent.UI;

namespace Terraria.Chat.Commands;

[ChatCommand("RPS")]
public class RockPaperScissorsCommand : IChatCommand
{
	public void ProcessIncomingMessage(string text, byte clientId)
	{
	}

	public void ProcessOutgoingMessage(ChatMessage message)
	{
		int reference = Main.rand.NextFromList<int>(37, 38, 36);
		if (Main.netMode == 0)
		{
			EmoteBubble.NewBubble(reference, new WorldUIAnchor(Main.LocalPlayer), 360);
			EmoteBubble.CheckForNPCsToReactToEmoteBubble(reference, Main.LocalPlayer);
		}
		else
		{
			NetMessage.SendData(120, -1, -1, null, Main.myPlayer, reference);
		}
		message.Consume();
	}
}
