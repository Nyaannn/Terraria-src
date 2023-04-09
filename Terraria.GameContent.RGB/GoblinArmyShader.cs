using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB;

public class GoblinArmyShader : ChromaShader
{
	private readonly Vector4 _primaryColor;

	private readonly Vector4 _secondaryColor;

	public GoblinArmyShader(Color primaryColor, Color secondaryColor)
	{
		_primaryColor = primaryColor.ToVector4();
		_secondaryColor = secondaryColor.ToVector4();
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		time *= 0.5f;
		for (int i = 0; i < fragment.Count; i++)
		{
			Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
			canvasPositionOfIndex.Y = 1f;
			float staticNoise = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * 0.3f + new Vector2(12.5f, time * 0.2f));
			staticNoise = Math.Max(0f, 1f - staticNoise * staticNoise * 4f * staticNoise);
			staticNoise = MathHelper.Clamp(staticNoise, 0f, 1f);
			Vector4 value = Vector4.Lerp(_primaryColor, _secondaryColor, staticNoise);
			value = Vector4.Lerp(value, Vector4.One, staticNoise * staticNoise);
			Vector4 vector = Vector4.Lerp(new Vector4(0f, 0f, 0f, 1f), value, staticNoise);
			fragment.SetColor(i, vector);
		}
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		for (int num = 0; num < fragment.Count; num++)
		{
			Vector2 dimensions = fragment.GetCanvasPositionOfIndex(num);
			float num2 = NoiseHelper.GetStaticNoise(dimensions * 0.3f + new Vector2(12.5f, time * 0.2f));
			num2 = Math.Max(0f, 1f - num2 * num2 * 4f * num2 * (1.2f - dimensions.Y)) * dimensions.Y * dimensions.Y;
			num2 = MathHelper.Clamp(num2, 0f, 1f);
			Vector4 baseScale = Vector4.Lerp(_primaryColor, _secondaryColor, num2);
			baseScale = Vector4.Lerp(baseScale, Vector4.One, num2 * num2 * num2);
			Vector4 value = Vector4.Lerp(new Vector4(0f, 0f, 0f, 1f), baseScale, num2);
			fragment.SetColor(num, value);
		}
	}
}
