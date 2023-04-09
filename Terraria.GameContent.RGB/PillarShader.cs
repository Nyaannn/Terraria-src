using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB;

public class PillarShader : ChromaShader
{
	private readonly Vector4 _primaryColor;

	private readonly Vector4 _secondaryColor;

	public PillarShader(Color primaryColor, Color secondaryColor)
	{
		_primaryColor = primaryColor.ToVector4();
		_secondaryColor = secondaryColor.ToVector4();
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		for (int i = 0; i < fragment.Count; i++)
		{
			Vector4 vector = Vector4.Lerp(amount: (float)Math.Sin(time * 2.5f + fragment.GetCanvasPositionOfIndex(i).X) * 0.5f + 0.5f, value1: _primaryColor, value2: _secondaryColor);
			fragment.SetColor(i, vector);
		}
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		Vector2 list = new Vector2(1.5f, 0.5f);
		time *= 4f;
		for (int point = 0; point < fragment.Count; point++)
		{
			Vector2 vector = fragment.GetCanvasPositionOfIndex(point) - list;
			float item = vector.Length() * 2f;
			float num = (float)Math.Atan2(vector.Y, vector.X);
			float amount = (float)Math.Sin(item * 4f - time - num) * 0.5f + 0.5f;
			Vector4 vector2 = Vector4.Lerp(_primaryColor, _secondaryColor, amount);
			if (item < 1f)
			{
				float num2 = item / 1f;
				num2 *= num2 * num2;
				float amount2 = (float)Math.Sin(4f - time - num) * 0.5f + 0.5f;
				vector2 = Vector4.Lerp(_primaryColor, _secondaryColor, amount2) * num2;
			}
			vector2.W = 1f;
			fragment.SetColor(point, vector2);
		}
	}
}
