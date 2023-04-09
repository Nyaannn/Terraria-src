using System;
using Microsoft.Xna.Framework;
using Terraria.Utilities;

namespace Terraria.GameContent.RGB;

public static class NoiseHelper
{
	private const int RANDOM_SEED = 1;

	private const int NOISE_2D_SIZE = 32;

	private const int NOISE_2D_SIZE_MASK = 31;

	private const int NOISE_SIZE_MASK = 1023;

	private static readonly float[] StaticNoise = CreateStaticNoise(1024);

	private static float[] CreateStaticNoise(int length)
	{
		UnifiedRandom r = new UnifiedRandom(1);
		float[] array = new float[length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = r.NextFloat();
		}
		return array;
	}

	public static float GetDynamicNoise(int index, float currentTime)
	{
		float num = StaticNoise[index & 0x3FF];
		float num2 = currentTime % 1f;
		return Math.Abs(Math.Abs(num - num2) - 0.5f) * 2f;
	}

	public static float GetStaticNoise(int index)
	{
		return StaticNoise[index & 0x3FF];
	}

	public static float GetDynamicNoise(int x, int y, float currentTime)
	{
		return GetDynamicNoiseInternal(x, y, currentTime % 1f);
	}

	private static float GetDynamicNoiseInternal(int x, int y, float wrappedTime)
	{
		x &= 0x1F;
		y &= 0x1F;
		return Math.Abs(Math.Abs(StaticNoise[y * 32 + x] - wrappedTime) - 0.5f) * 2f;
	}

	public static float GetStaticNoise(int x, int y)
	{
		x &= 0x1F;
		y &= 0x1F;
		return StaticNoise[y * 32 + x];
	}

	public static float GetDynamicNoise(Vector2 position, float currentTime)
	{
		position *= 10f;
		currentTime %= 1f;
		Vector2 innerDimensions = new Vector2((float)Math.Floor(position.X), (float)Math.Floor(position.Y));
		Point pos = new Point((int)innerDimensions.X, (int)innerDimensions.Y);
		Vector2 num = new Vector2(position.X - innerDimensions.X, position.Y - innerDimensions.Y);
		float value = MathHelper.Lerp(GetDynamicNoiseInternal(pos.X, pos.Y, currentTime), GetDynamicNoiseInternal(pos.X, pos.Y + 1, currentTime), num.Y);
		float value2 = MathHelper.Lerp(GetDynamicNoiseInternal(pos.X + 1, pos.Y, currentTime), GetDynamicNoiseInternal(pos.X + 1, pos.Y + 1, currentTime), num.Y);
		return MathHelper.Lerp(value, value2, num.X);
	}

	public static float GetStaticNoise(Vector2 position)
	{
		position *= 10f;
		Vector2 vector = new Vector2((float)Math.Floor(position.X), (float)Math.Floor(position.Y));
		Point point = new Point((int)vector.X, (int)vector.Y);
		Vector2 vector2 = new Vector2(position.X - vector.X, position.Y - vector.Y);
		float value = MathHelper.Lerp(GetStaticNoise(point.X, point.Y), GetStaticNoise(point.X, point.Y + 1), vector2.Y);
		float value2 = MathHelper.Lerp(GetStaticNoise(point.X + 1, point.Y), GetStaticNoise(point.X + 1, point.Y + 1), vector2.Y);
		return MathHelper.Lerp(value, value2, vector2.X);
	}
}
