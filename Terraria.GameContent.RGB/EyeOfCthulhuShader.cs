using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB;

public class EyeOfCthulhuShader : ChromaShader
{
	private readonly Vector4 _eyeColor;

	private readonly Vector4 _veinColor;

	private readonly Vector4 _backgroundColor;

	public EyeOfCthulhuShader(Color eyeColor, Color veinColor, Color backgroundColor)
	{
		_eyeColor = eyeColor.ToVector4();
		_veinColor = veinColor.ToVector4();
		_backgroundColor = backgroundColor.ToVector4();
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		for (int num = 0; num < fragment.Count; num++)
		{
			Vector4 num2 = Vector4.Lerp(amount: (float)Math.Sin(time + fragment.GetCanvasPositionOfIndex(num).X * 4f) * 0.5f + 0.5f, value1: _veinColor, value2: _eyeColor);
			fragment.SetColor(num, num2);
		}
	}

	[RgbProcessor(/*Could not decode attribute arguments.*/)]
	private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if ((int)device.Type != 0 && (int)device.Type != 6)
		{
			ProcessLowDetail(device, fragment, quality, time);
			return;
		}
		float text = time * 0.2f % 2f;
		int num = 1;
		if (text > 1f)
		{
			text = 2f - text;
			num = -1;
		}
		Vector2 j = new Vector2(text * 7f - 3.5f, 0f) + fragment.get_CanvasCenter();
		for (int i = 0; i < fragment.Count; i++)
		{
			Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
			Vector4 vector = _backgroundColor;
			Vector2 vector2 = canvasPositionOfIndex - j;
			float num2 = vector2.Length();
			if (num2 < 0.5f)
			{
				float amount = 1f - MathHelper.Clamp((num2 - 0.5f + 0.2f) / 0.2f, 0f, 1f);
				float num3 = MathHelper.Clamp((vector2.X + 0.5f - 0.2f) / 0.6f, 0f, 1f);
				if (num == 1)
				{
					num3 = 1f - num3;
				}
				Vector4 value = Vector4.Lerp(_eyeColor, _veinColor, num3);
				vector = Vector4.Lerp(vector, value, amount);
			}
			fragment.SetColor(i, vector);
		}
	}
}
