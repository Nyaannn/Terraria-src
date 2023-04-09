using System;
using Microsoft.Xna.Framework;

namespace Terraria;

public class StrayMethods
{
	public static bool CountSandHorizontally(int i, int j, bool[] fittingTypes, int requiredTotalSpread = 4, int spreadInEachAxis = 5)
	{
		if (!WorldGen.InWorld(i, j, 2))
		{
			return false;
		}
		int num = 0;
		int vector = 0;
		int num2 = i - 1;
		while (num < spreadInEachAxis && num2 > 0)
		{
			Tile num3 = Main.tile[num2, j];
			if (num3.active() && fittingTypes[num3.type] && !WorldGen.SolidTileAllowBottomSlope(num2, j - 1))
			{
				num++;
			}
			else if (!num3.active())
			{
				break;
			}
			num2--;
		}
		num2 = i + 1;
		while (vector < spreadInEachAxis && num2 < Main.maxTilesX - 1)
		{
			Tile num4 = Main.tile[num2, j];
			if (num4.active() && fittingTypes[num4.type] && !WorldGen.SolidTileAllowBottomSlope(num2, j - 1))
			{
				vector++;
			}
			else if (!num4.active())
			{
				break;
			}
			num2++;
		}
		return num + vector + 1 >= requiredTotalSpread;
	}

	public static bool CanSpawnSandstormHostile(Vector2 position, int expandUp, int expandDown)
	{
		bool result = true;
		Point point = position.ToTileCoordinates();
		for (int i = -1; i <= 1; i++)
		{
			Collision.ExpandVertically(point.X + i, point.Y, out var topY, out var bottomY, expandUp, expandDown);
			topY++;
			bottomY--;
			if (bottomY - topY < 20)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public static bool CanSpawnSandstormFriendly(Vector2 position, int expandUp, int expandDown)
	{
		bool num = true;
		Point num2 = position.ToTileCoordinates();
		for (int flag = -1; flag <= 1; flag++)
		{
			Collision.ExpandVertically(num2.X + flag, num2.Y, out var num3, out var tile, expandUp, expandDown);
			num3++;
			tile--;
			if (tile - num3 < 10)
			{
				num = false;
				break;
			}
		}
		return num;
	}

	public static void CheckArenaScore(Vector2 arenaCenter, out Point xLeftEnd, out Point xRightEnd, int walkerWidthInTiles = 5, int walkerHeightInTiles = 10)
	{
		bool num = false;
		Point num2 = arenaCenter.ToTileCoordinates();
		xLeftEnd = (xRightEnd = num2);
		Collision.ExpandVertically(num2.X, num2.Y, out var _, out var num4, 0, 4);
		num2.Y = num4;
		if (num)
		{
			Dust.QuickDust(num2, Color.Blue).scale = 5f;
		}
		SendWalker(num2, walkerHeightInTiles, -1, out var _, out var list, 120, num);
		SendWalker(num2, walkerHeightInTiles, 1, out var _, out var i, 120, num);
		list.X++;
		i.X--;
		if (num)
		{
			Dust.QuickDustLine(list.ToWorldCoordinates(), i.ToWorldCoordinates(), 50f, Color.Pink);
		}
		xLeftEnd = list;
		xRightEnd = i;
	}

	public static void SendWalker(Point startFloorPosition, int height, int direction, out int distanceCoveredInTiles, out Point lastIteratedFloorSpot, int maxDistance = 100, bool showDebug = false)
	{
		distanceCoveredInTiles = 0;
		startFloorPosition.Y--;
		lastIteratedFloorSpot = startFloorPosition;
		for (int player = 0; player < maxDistance; player++)
		{
			for (int flag = 0; flag < 3; flag++)
			{
				if (!WorldGen.SolidTile3(startFloorPosition.X, startFloorPosition.Y))
				{
					break;
				}
				startFloorPosition.Y--;
			}
			Collision.ExpandVertically(startFloorPosition.X, startFloorPosition.Y, out var num, out var num2, height, 2);
			num++;
			num2--;
			if (!WorldGen.SolidTile3(startFloorPosition.X, num2 + 1))
			{
				Collision.ExpandVertically(startFloorPosition.X, num2, out var num3, out var vector, 0, 6);
				if (showDebug)
				{
					Dust.QuickBox(new Vector2(startFloorPosition.X * 16 + 8, num3 * 16), new Vector2(startFloorPosition.X * 16 + 8, vector * 16), 1, Color.Blue, null);
				}
				if (!WorldGen.SolidTile3(startFloorPosition.X, vector))
				{
					break;
				}
			}
			if (num2 - num < height - 1)
			{
				break;
			}
			if (showDebug)
			{
				Dust.QuickDust(startFloorPosition, Color.Green).scale = 1f;
				Dust.QuickBox(new Vector2(startFloorPosition.X * 16 + 8, num * 16), new Vector2(startFloorPosition.X * 16 + 8, num2 * 16 + 16), 1, Color.Red, null);
			}
			distanceCoveredInTiles += direction;
			startFloorPosition.X += direction;
			startFloorPosition.Y = num2;
			lastIteratedFloorSpot = startFloorPosition;
			if (Math.Abs(distanceCoveredInTiles) >= maxDistance)
			{
				break;
			}
		}
		distanceCoveredInTiles = Math.Abs(distanceCoveredInTiles);
	}
}
