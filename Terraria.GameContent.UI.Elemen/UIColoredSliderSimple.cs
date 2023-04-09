using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements;

public class UIColoredSliderSimple : UIElement
{
	public float FillPercent;

	public Color FilledColor = Main.OurFavoriteColor;

	public Color EmptyColor = Color.Black;

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		DrawValueBarDynamicWidth(spriteBatch);
	}

	private void DrawValueBarDynamicWidth(SpriteBatch sb)
	{
		Texture2D value = TextureAssets.ColorBar.get_Value();
		Rectangle value2 = GetDimensions().ToRectangle();
		Rectangle matrix = new Rectangle(5, 4, 4, 4);
		Utils.DrawSplicedPanel(sb, value, value2.X, value2.Y, value2.Width, value2.Height, matrix.X, matrix.Width, matrix.Y, matrix.Height, Color.White);
		Rectangle i = value2;
		i.X += matrix.Left;
		i.Width -= matrix.Right;
		i.Y += matrix.Top;
		i.Height -= matrix.Bottom;
		Texture2D position = TextureAssets.MagicPixel.get_Value();
		Rectangle j = new Rectangle(0, 0, 1, 1);
		sb.Draw(position, i, j, EmptyColor);
		Rectangle position2 = i;
		position2.Width = (int)((float)position2.Width * FillPercent);
		sb.Draw(position, position2, j, FilledColor);
	}
}
