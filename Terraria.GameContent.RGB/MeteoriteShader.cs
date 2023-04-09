using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB;

public class MeteoriteShader : ChromaShader
{
	private readonly Vector4 _baseColor = new Color(39, 15, 26).ToVector4();

	private readonly Vector4 _secondaryColor = new Color(69, 50, 43).ToVector4();

	private readonly Vector4 _glowColor = Color.DarkOrange.ToVector4();

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		for (int i = 0; i < fragment.Count; i++)
		{
			Vector4 vector = Vector4.Lerp(amount: (float)Math.Sin(time + fragment.GetCanvasPositionOfIndex(i).X) * 0.5f + 0.5f, value1: _baseColor, value2: _secondaryColor);
			fragment.SetColor(i, vector);
		}
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		for (int dimensions = 0; dimensions < fragment.Count; dimensions++)
		{
			Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(dimensions);
			Point num = fragment.GetGridPositionOfIndex(dimensions);
			Vector4 num2 = _baseColor;
			float num3 = NoiseHelper.GetDynamicNoise(num.X, num.Y, time / 10f);
			num2 = Vector4.Lerp(num2, _secondaryColor, num3 * num3);
			float dynamicNoise = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.5f + new Vector2(0f, time * 0.05f), time / 20f);
			dynamicNoise = Math.Max(0f, 1f - dynamicNoise * 2f);
			num2 = Vector4.Lerp(num2, _glowColor, (float)Math.Sqrt(dynamicNoise) * 0.75f);
			fragment.SetColor(dimensions, num2);
		}
	}
}
