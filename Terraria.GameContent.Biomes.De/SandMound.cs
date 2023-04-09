using System;
using Microsoft.Xna.Framework;

namespace Terraria.GameContent.Biomes.Desert;

public static class SandMound
{
	public static void Place(DesertDescription description)
	{
		Rectangle i = description.Desert;
		i.Height = Math.Min(description.Desert.Height, description.Hive.Height / 2);
		Rectangle player = description.Desert;
		player.Y = i.Bottom;
		player.Height = Math.Max(0, description.Desert.Bottom - i.Bottom);
		SurfaceMap itemIndex = description.Surface;
		int num = 0;
		int num2 = 0;
		for (int j = -5; j < i.Width + 5; j++)
		{
			double value = Math.Abs((double)(j + 5) / (double)(i.Width + 10)) * 2.0 - 1.0;
			value = Utils.Clamp(value, -1.0, 1.0);
			if (j % 3 == 0)
			{
				num += WorldGen.genRand.Next(-1, 2);
				num = Utils.Clamp(num, -10, 10);
			}
			num2 += WorldGen.genRand.Next(-1, 2);
			num2 = Utils.Clamp(num2, -10, 10);
			double num3 = Math.Sqrt(1.0 - value * value * value * value);
			int num4 = i.Bottom - (int)(num3 * (double)i.Height) + num;
			if (Math.Abs(value) < 1.0)
			{
				double num5 = Utils.UnclampedSmoothStep(0.5, 0.8, Math.Abs(value));
				num5 = num5 * num5 * num5;
				int val = 10 + (int)((double)i.Top - num5 * 20.0) + num2;
				val = Math.Min(val, num4);
				for (int k = itemIndex[j + i.X] - 1; k < val; k++)
				{
					int num6 = j + i.X;
					int num7 = k;
					Main.tile[num6, num7].active(active: false);
					Main.tile[num6, num7].wall = 0;
				}
			}
			PlaceSandColumn(j + i.X, num4, player.Bottom - num4);
		}
	}

	private static void PlaceSandColumn(int startX, int startY, int height)
	{
		for (int item = startY + height - 1; item >= startY; item--)
		{
			int num = item;
			Tile tile = Main.tile[startX, num];
			if (!WorldGen.remixWorldGen)
			{
				tile.liquid = 0;
			}
			_ = Main.tile[startX, num + 1];
			_ = Main.tile[startX, num + 2];
			tile.type = 53;
			tile.slope(0);
			tile.halfBrick(halfBrick: false);
			tile.active(active: true);
			if (item < startY)
			{
				tile.active(active: false);
			}
			WorldGen.SquareWallFrame(startX, num);
		}
	}
}
