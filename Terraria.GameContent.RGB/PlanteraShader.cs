using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB;

public class PlanteraShader : ChromaShader
{
	private readonly Vector4 _bulbColor;

	private readonly Vector4 _vineColor;

	private readonly Vector4 _backgroundColor;

	public PlanteraShader(Color bulbColor, Color vineColor, Color backgroundColor)
	{
		_bulbColor = bulbColor.ToVector4();
		_vineColor = vineColor.ToVector4();
		_backgroundColor = backgroundColor.ToVector4();
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		for (int i = 0; i < fragment.Count; i++)
		{
			Vector4 vector = Vector4.Lerp(amount: (float)Math.Sin(time * 2f + fragment.GetCanvasPositionOfIndex(i).X * 10f) * 0.5f + 0.5f, value1: _bulbColor, value2: _vineColor);
			fragment.SetColor(i, vector);
		}
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		for (int dimensions = 0; dimensions < fragment.Count; dimensions++)
		{
			Point point = fragment.GetGridPositionOfIndex(dimensions);
			Vector2 point2 = fragment.GetCanvasPositionOfIndex(dimensions);
			point2.X -= 1.8f;
			if (point2.X < 0f)
			{
				point2.X *= -1f;
				point.Y += 101;
			}
			float width = NoiseHelper.GetStaticNoise(point.Y);
			width = (width * 5f + time * 0.4f) % 5f;
			float height = 1f;
			if (width > 1f)
			{
				height = 1f - MathHelper.Clamp((width - 0.4f - 1f) / 0.4f, 0f, 1f);
				width = 1f;
			}
			float num = width - point2.X / 5f;
			Vector4 vector = _backgroundColor;
			if (num > 0f)
			{
				float num2 = 1f;
				if (num < 0.2f)
				{
					num2 = num / 0.2f;
				}
				vector = (((point.X + 7 * point.Y) % 5 != 0) ? Vector4.Lerp(_backgroundColor, _vineColor, num2 * height) : Vector4.Lerp(_backgroundColor, _bulbColor, num2 * height));
			}
			fragment.SetColor(dimensions, vector);
		}
	}
}
