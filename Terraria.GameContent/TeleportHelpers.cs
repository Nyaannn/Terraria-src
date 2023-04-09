using Microsoft.Xna.Framework;

namespace Terraria.GameContent;

public class TeleportHelpers
{
	public static bool RequestMagicConchTeleportPosition(Player player, int crawlOffsetX, int startX, out Point landingPoint)
	{
		landingPoint = default(Point);
		Point list = new Point(startX, 50);
		int num = 1;
		int item = -1;
		int num2 = 1;
		int num3 = 0;
		int num4 = 5000;
		Vector2 vector = new Vector2((float)player.width * 0.5f, player.height);
		int num5 = 40;
		bool flag = WorldGen.SolidOrSlopedTile(Main.tile[list.X, list.Y]);
		int num6 = 0;
		int num7 = 400;
		while (num3 < num4 && num6 < num7)
		{
			num3++;
			Tile tile = Main.tile[list.X, list.Y];
			Tile tile2 = Main.tile[list.X, list.Y + num2];
			bool flag2 = WorldGen.SolidOrSlopedTile(tile) || tile.liquid > 0;
			bool flag3 = WorldGen.SolidOrSlopedTile(tile2) || tile2.liquid > 0;
			if (IsInSolidTilesExtended(new Vector2(list.X * 16 + 8, list.Y * 16 + 15) - vector, player.velocity, player.width, player.height, (int)player.gravDir))
			{
				if (flag)
				{
					list.Y += num;
				}
				else
				{
					list.Y += item;
				}
				continue;
			}
			if (flag2)
			{
				if (flag)
				{
					list.Y += num;
				}
				else
				{
					list.Y += item;
				}
				continue;
			}
			flag = false;
			if (!IsInSolidTilesExtended(new Vector2(list.X * 16 + 8, list.Y * 16 + 15 + 16) - vector, player.velocity, player.width, player.height, (int)player.gravDir) && !flag3 && (double)list.Y < Main.worldSurface)
			{
				list.Y += num;
				continue;
			}
			if (tile2.liquid > 0)
			{
				list.X += crawlOffsetX;
				num6++;
				continue;
			}
			if (TileIsDangerous(list.X, list.Y))
			{
				list.X += crawlOffsetX;
				num6++;
				continue;
			}
			if (TileIsDangerous(list.X, list.Y + num2))
			{
				list.X += crawlOffsetX;
				num6++;
				continue;
			}
			if (list.Y >= num5)
			{
				break;
			}
			list.Y += num;
		}
		if (num3 == num4 || num6 >= num7)
		{
			return false;
		}
		if (!WorldGen.InWorld(list.X, list.Y, 40))
		{
			return false;
		}
		landingPoint = list;
		return true;
	}

	private static bool TileIsDangerous(int x, int y)
	{
		Tile tile = Main.tile[x, y];
		if (tile.liquid > 0 && tile.lava())
		{
			return true;
		}
		if (tile.wall == 87 && (double)y > Main.worldSurface && !NPC.downedPlantBoss)
		{
			return true;
		}
		if (Main.wallDungeon[tile.wall] && (double)y > Main.worldSurface && !NPC.downedBoss3)
		{
			return true;
		}
		return false;
	}

	private static bool IsInSolidTilesExtended(Vector2 testPosition, Vector2 playerVelocity, int width, int height, int gravDir)
	{
		if (Collision.LavaCollision(testPosition, width, height))
		{
			return true;
		}
		if (Collision.AnyHurtingTiles(testPosition, width, height))
		{
			return true;
		}
		if (Collision.SolidCollision(testPosition, width, height))
		{
			return true;
		}
		Vector2 vector = Vector2.UnitX * 16f;
		if (Collision.TileCollision(testPosition - vector, vector, width, height, fallThrough: false, fall2: false, gravDir) != vector)
		{
			return true;
		}
		vector = -Vector2.UnitX * 16f;
		if (Collision.TileCollision(testPosition - vector, vector, width, height, fallThrough: false, fall2: false, gravDir) != vector)
		{
			return true;
		}
		vector = Vector2.UnitY * 16f;
		if (Collision.TileCollision(testPosition - vector, vector, width, height, fallThrough: false, fall2: false, gravDir) != vector)
		{
			return true;
		}
		vector = -Vector2.UnitY * 16f;
		if (Collision.TileCollision(testPosition - vector, vector, width, height, fallThrough: false, fall2: false, gravDir) != vector)
		{
			return true;
		}
		return false;
	}
}
