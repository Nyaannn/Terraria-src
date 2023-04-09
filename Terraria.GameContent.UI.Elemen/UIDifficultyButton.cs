using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements;

public class UIDifficultyButton : UIElement
{
	private readonly Player _player;

	private readonly Asset<Texture2D> _BasePanelTexture;

	private readonly Asset<Texture2D> _selectedBorderTexture;

	private readonly Asset<Texture2D> _hoveredBorderTexture;

	private readonly byte _difficulty;

	private readonly Color _color;

	private bool _hovered;

	private bool _soundedHover;

	public UIDifficultyButton(Player player, LocalizedText title, LocalizedText description, byte difficulty, Color color)
	{
		_player = player;
		_difficulty = difficulty;
		Width = StyleDimension.FromPixels(44f);
		Height = StyleDimension.FromPixels(110f);
		_BasePanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/PanelGrayscale", (AssetRequestMode)1);
		_selectedBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight", (AssetRequestMode)1);
		_hoveredBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder", (AssetRequestMode)1);
		_color = color;
		UIText item = new UIText(title, 0.9f)
		{
			HAlign = 0.5f,
			VAlign = 0f,
			Width = StyleDimension.FromPixelsAndPercent(-10f, 1f),
			Top = StyleDimension.FromPixels(5f)
		};
		Append(item);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		if (_hovered)
		{
			if (!_soundedHover)
			{
				SoundEngine.PlaySound(12);
			}
			_soundedHover = true;
		}
		else
		{
			_soundedHover = false;
		}
		CalculatedStyle num = GetDimensions();
		int num2 = 7;
		if (num.Height < 30f)
		{
			num2 = 5;
		}
		int item = 10;
		int num3 = 10;
		bool num4 = _difficulty == _player.difficulty;
		Utils.DrawSplicedPanel(spriteBatch, _BasePanelTexture.get_Value(), (int)num.X, (int)num.Y, (int)num.Width, (int)num.Height, item, item, num3, num3, Color.Lerp(Color.Black, _color, 0.8f) * 0.5f);
		if (num4)
		{
			Utils.DrawSplicedPanel(spriteBatch, _BasePanelTexture.get_Value(), (int)num.X + num2, (int)num.Y + num2 - 2, (int)num.Width - num2 * 2, (int)num.Height - num2 * 2, item, item, num3, num3, Color.Lerp(_color, Color.White, 0.7f) * 0.5f);
		}
		if (_hovered)
		{
			Utils.DrawSplicedPanel(spriteBatch, _hoveredBorderTexture.get_Value(), (int)num.X, (int)num.Y, (int)num.Width, (int)num.Height, item, item, num3, num3, Color.White);
		}
	}

	public override void LeftMouseDown(UIMouseEvent evt)
	{
		_player.difficulty = _difficulty;
		SoundEngine.PlaySound(12);
		base.LeftMouseDown(evt);
	}

	public override void MouseOver(UIMouseEvent evt)
	{
		base.MouseOver(evt);
		_hovered = true;
	}

	public override void MouseOut(UIMouseEvent evt)
	{
		base.MouseOut(evt);
		_hovered = false;
	}
}
