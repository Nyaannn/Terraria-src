using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB;

public class UnderworldShader : ChromaShader
{
	private readonly Vector4 _backColor;

	private readonly Vector4 _frontColor;

	private readonly float _speed;

	public UnderworldShader(Color backColor, Color frontColor, float speed)
	{
		_backColor = backColor.ToVector4();
		_frontColor = frontColor.ToVector4();
		_speed = speed;
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		for (int left = 0; left < fragment.Count; left++)
		{
			Vector2 top = fragment.GetCanvasPositionOfIndex(left);
			Vector4 uIText = Vector4.Lerp(_backColor, _frontColor, (float)Math.Sin(time * _speed + top.X) * 0.5f + 0.5f);
			fragment.SetColor(left, uIText);
		}
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		for (int asset = 0; asset < fragment.Count; asset++)
		{
			float dynamicNoise = NoiseHelper.GetDynamicNoise(fragment.GetCanvasPositionOfIndex(asset) * 0.5f, time * _speed / 3f);
			Vector4 vector = Vector4.Lerp(_backColor, _frontColor, dynamicNoise);
			fragment.SetColor(asset, vector);
		}
	}
}
