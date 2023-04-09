using Microsoft.Xna.Framework;

namespace Terraria.DataStructures;

public class ColorSlidersSet
{
	public float Hue;

	public float Saturation;

	public float Luminance;

	public float Alpha = 1f;

	public void SetHSL(Color color)
	{
		Vector3 num780 = Main.rgbToHsl(color);
		Hue = num780.X;
		Saturation = num780.Y;
		Luminance = num780.Z;
	}

	public void SetHSL(Vector3 vector)
	{
		Hue = vector.X;
		Saturation = vector.Y;
		Luminance = vector.Z;
	}

	public Color GetColor()
	{
		Color num761 = Main.hslToRgb(Hue, Saturation, Luminance);
		num761.A = (byte)(Alpha * 255f);
		return num761;
	}

	public Vector3 GetHSLVector()
	{
		return new Vector3(Hue, Saturation, Luminance);
	}

	public void ApplyToMainLegacyBars()
	{
		Main.hBar = Hue;
		Main.sBar = Saturation;
		Main.lBar = Luminance;
		Main.aBar = Alpha;
	}
}
