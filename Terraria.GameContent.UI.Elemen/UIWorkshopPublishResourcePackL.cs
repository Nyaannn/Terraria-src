using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements;

public class UIWorkshopPublishResourcePackListItem : UIPanel
{
	private ResourcePack _data;

	private Asset<Texture2D> _dividerTexture;

	private Asset<Texture2D> _workshopIconTexture;

	private Asset<Texture2D> _iconBorderTexture;

	private Asset<Texture2D> _innerPanelTexture;

	private UIElement _iconArea;

	private UIElement _publishButton;

	private int _orderInList;

	private UIState _ownerState;

	private const int ICON_SIZE = 64;

	private const int ICON_BORDER_PADDING = 4;

	private const int HEIGHT_FLUFF = 10;

	private bool _canPublish;

	public UIWorkshopPublishResourcePackListItem(UIState ownerState, ResourcePack data, int orderInList, bool canBePublished)
	{
		_ownerState = ownerState;
		_orderInList = orderInList;
		_data = data;
		_canPublish = canBePublished;
		LoadTextures();
		InitializeAppearance();
		UIElement playerDeathReason = new UIElement
		{
			Width = new StyleDimension(72f, 0f),
			Height = new StyleDimension(72f, 0f),
			Left = new StyleDimension(4f, 0f),
			VAlign = 0.5f
		};
		playerDeathReason.OnLeftDoubleClick += PublishButtonClick_ImportResourcePackToLocalFiles;
		UIImage element = new UIImage(data.Icon)
		{
			Width = new StyleDimension(-6f, 1f),
			Height = new StyleDimension(-6f, 1f),
			HAlign = 0.5f,
			VAlign = 0.5f,
			ScaleToFit = true,
			AllowResizingDimensions = false,
			IgnoresMouseInteraction = true
		};
		UIImage element2 = new UIImage(Main.Assets.Request<Texture2D>("Images/UI/Achievement_Borders", (AssetRequestMode)1))
		{
			HAlign = 0.5f,
			VAlign = 0.5f,
			IgnoresMouseInteraction = true
		};
		playerDeathReason.Append(element);
		playerDeathReason.Append(element2);
		Append(playerDeathReason);
		_iconArea = playerDeathReason;
		_ = 4f;
		_publishButton = new UIIconTextButton(Language.GetText("Workshop.Publish"), Color.White, "Images/UI/Workshop/Publish");
		_publishButton.HAlign = 1f;
		_publishButton.VAlign = 1f;
		_publishButton.OnLeftClick += PublishButtonClick_ImportResourcePackToLocalFiles;
		base.OnLeftDoubleClick += PublishButtonClick_ImportResourcePackToLocalFiles;
		Append(_publishButton);
		_publishButton.SetSnapPoint("Publish", orderInList);
	}

	private void LoadTextures()
	{
		_dividerTexture = Main.Assets.Request<Texture2D>("Images/UI/Divider", (AssetRequestMode)1);
		_innerPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground", (AssetRequestMode)1);
		_iconBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/Achievement_Borders", (AssetRequestMode)1);
		_workshopIconTexture = TextureAssets.Extra[243];
	}

	private void InitializeAppearance()
	{
		Height.Set(82f, 0f);
		Width.Set(0f, 1f);
		SetPadding(6f);
		SetColorsToNotHovered();
	}

	private void SetColorsToHovered()
	{
		BackgroundColor = new Color(73, 94, 171);
		BorderColor = new Color(89, 116, 213);
		if (!_canPublish)
		{
			BorderColor = new Color(150, 150, 150) * 1f;
			BackgroundColor = Color.Lerp(BackgroundColor, new Color(120, 120, 120), 0.5f) * 1f;
		}
	}

	private void SetColorsToNotHovered()
	{
		BackgroundColor = new Color(63, 82, 151) * 0.7f;
		BorderColor = new Color(89, 116, 213) * 0.7f;
		if (!_canPublish)
		{
			BorderColor = new Color(127, 127, 127) * 0.7f;
			BackgroundColor = Color.Lerp(new Color(63, 82, 151), new Color(80, 80, 80), 0.5f) * 0.7f;
		}
	}

	private void PublishButtonClick_ImportResourcePackToLocalFiles(UIMouseEvent evt, UIElement listeningElement)
	{
		if (listeningElement == evt.Target && !TryMovingToRejectionMenuIfNeeded())
		{
			Main.MenuUI.SetState(new WorkshopPublishInfoStateForResourcePack(_ownerState, _data));
		}
	}

	private bool TryMovingToRejectionMenuIfNeeded()
	{
		if (!_canPublish)
		{
			SoundEngine.PlaySound(10);
			Main.instance.RejectionMenuInfo = new RejectionMenuInfo
			{
				TextToShow = Language.GetTextValue("Workshop.ReportIssue_CannotPublishZips"),
				ExitAction = RejectionMenuExitAction
			};
			Main.menuMode = 1000001;
			return true;
		}
		return false;
	}

	private void RejectionMenuExitAction()
	{
		SoundEngine.PlaySound(11);
		if (_ownerState == null)
		{
			Main.menuMode = 0;
			return;
		}
		Main.menuMode = 888;
		Main.MenuUI.SetState(_ownerState);
	}

	public override int CompareTo(object obj)
	{
		if (obj is UIWorkshopPublishResourcePackListItem hairFrame)
		{
			return _orderInList.CompareTo(hairFrame._orderInList);
		}
		return base.CompareTo(obj);
	}

	public override void MouseOver(UIMouseEvent evt)
	{
		base.MouseOver(evt);
		SetColorsToHovered();
	}

	public override void MouseOut(UIMouseEvent evt)
	{
		base.MouseOut(evt);
		SetColorsToNotHovered();
	}

	private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width, float height)
	{
		Utils.DrawSplicedPanel(spriteBatch, _innerPanelTexture.get_Value(), (int)position.X, (int)position.Y, (int)width, (int)height, 10, 10, 10, 10, Color.White);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);
		CalculatedStyle verticalFrames = GetInnerDimensions();
		CalculatedStyle value = _iconArea.GetDimensions();
		float rectangle = value.X + value.Width;
		Color origin = Color.White;
		Utils.DrawBorderString(spriteBatch, _data.Name, new Vector2(rectangle + 8f, verticalFrames.Y + 3f), origin);
		float num8 = (verticalFrames.Width - 22f - value.Width - _publishButton.GetDimensions().Width) / 2f;
		float num = _publishButton.GetDimensions().Height;
		Vector2 value2 = new Vector2(rectangle + 8f, verticalFrames.Y + verticalFrames.Height - num);
		float num2 = num8;
		DrawPanel(spriteBatch, value2, num2, num);
		string num3 = Language.GetTextValue("UI.Author", _data.Author);
		Color num4 = Color.White;
		Vector2 vector = FontAssets.MouseText.get_Value().MeasureString(num3);
		float num5 = vector.X;
		float flag = vector.Y;
		float num6 = num2 * 0.5f - num5 * 0.5f;
		float num7 = num * 0.5f - flag * 0.5f;
		Utils.DrawBorderString(spriteBatch, num3, value2 + new Vector2(num6, num7 + 3f), num4);
		value2.X += num2 + 5f;
		float x = num8;
		DrawPanel(spriteBatch, value2, x, num);
		string textValue = Language.GetTextValue("UI.Version", _data.Version.GetFormattedVersion());
		Color white = Color.White;
		Vector2 vector2 = FontAssets.MouseText.get_Value().MeasureString(textValue);
		float x2 = vector2.X;
		float y = vector2.Y;
		float x3 = x * 0.5f - x2 * 0.5f;
		float num9 = num * 0.5f - y * 0.5f;
		Utils.DrawBorderString(spriteBatch, textValue, value2 + new Vector2(x3, num9 + 3f), white);
		value2.X += x + 5f;
	}
}
