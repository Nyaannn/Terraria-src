using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria.GameContent.Bestiary;

public class NamePlateInfoElement : IBestiaryInfoElement, IProvideSearchFilterString
{
	private string _key;

	private int _npcNetId;

	public NamePlateInfoElement(string languageKey, int npcNetId)
	{
		_key = languageKey;
		_npcNetId = npcNetId;
	}

	public UIElement ProvideUIElement(BestiaryUICollectionInfo info)
	{
		UIElement creditsRollSky = ((info.UnlockState != 0) ? new UIText(Language.GetText(_key)) : new UIText("???"));
		creditsRollSky.HAlign = 0.5f;
		creditsRollSky.VAlign = 0.5f;
		creditsRollSky.Top = new StyleDimension(2f, 0f);
		creditsRollSky.IgnoresMouseInteraction = true;
		UIElement uIElement = new UIElement();
		uIElement.Width = new StyleDimension(0f, 1f);
		uIElement.Height = new StyleDimension(24f, 0f);
		uIElement.Append(creditsRollSky);
		return uIElement;
	}

	public string GetSearchString(ref BestiaryUICollectionInfo info)
	{
		if (info.UnlockState == BestiaryEntryUnlockState.NotKnownAtAll_0)
		{
			return null;
		}
		return Language.GetText(_key).Value;
	}
}
