using System;
using System.Collections.Generic;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Terraria;

public class Liquid
{
	public const int maxLiquidBuffer = 50000;

	public static int maxLiquid = 25000;

	public static int skipCount;

	public static int stuckCount;

	public static int stuckAmount;

	public static int cycles = 10;

	public static int curMaxLiquid = 0;

	public static int numLiquid;

	public static bool stuck;

	public static bool quickFall;

	public static bool quickSettle;

	private static int wetCounter;

	public static int panicCounter;

	public static bool panicMode;

	public static int panicY;

	public int x;

	public int y;

	public int kill;

	public int delay;

	private static HashSet<int> _netChangeSet = new HashSet<int>();

	private static HashSet<int> _swapNetChangeSet = new HashSet<int>();

	public static void NetSendLiquid(int x, int y)
	{
		if (WorldGen.gen)
		{
			return;
		}
		lock (_netChangeSet)
		{
			_netChangeSet.Add(((x & 0xFFFF) << 16) | (y & 0xFFFF));
		}
	}

	public static void tilesIgnoreWater(bool ignoreSolids)
	{
		Main.tileSolid[138] = !ignoreSolids;
		Main.tileSolid[484] = !ignoreSolids;
		Main.tileSolid[546] = !ignoreSolids;
	}

	public static void worldGenTilesIgnoreWater(bool ignoreSolids)
	{
		Main.tileSolid[10] = !ignoreSolids;
		Main.tileSolid[192] = !ignoreSolids;
		Main.tileSolid[191] = !ignoreSolids;
		Main.tileSolid[190] = !ignoreSolids;
	}

	public static void ReInit()
	{
		skipCount = 0;
		stuckCount = 0;
		stuckAmount = 0;
		cycles = 10;
		curMaxLiquid = maxLiquid;
		numLiquid = 0;
		stuck = false;
		quickFall = false;
		quickSettle = false;
		wetCounter = 0;
		panicCounter = 0;
		panicMode = false;
		panicY = 0;
		if (Main.Setting_UseReducedMaxLiquids)
		{
			curMaxLiquid = 5000;
		}
	}

	public static void QuickWater(int verbose = 0, int minY = -1, int maxY = -1)
	{
		if (WorldGen.gen)
		{
			WorldGen.ShimmerRemoveWater();
			if (WorldGen.noTrapsWorldGen)
			{
				Main.tileSolid[138] = false;
			}
		}
		Main.tileSolid[379] = true;
		tilesIgnoreWater(ignoreSolids: true);
		if (minY == -1)
		{
			minY = 3;
		}
		if (maxY == -1)
		{
			maxY = Main.maxTilesY - 3;
		}
		for (int num = maxY; num >= minY; num--)
		{
			UpdateProgressDisplay(verbose, minY, maxY, num);
			for (int i = 4; i < Main.maxTilesX - 4; i++)
			{
				if (Main.tile[i, num].liquid != 0)
				{
					SettleWaterAt(i, num);
				}
			}
		}
		tilesIgnoreWater(ignoreSolids: false);
		if (WorldGen.gen)
		{
			WorldGen.ShimmerRemoveWater();
			if (WorldGen.noTrapsWorldGen)
			{
				Main.tileSolid[138] = true;
			}
		}
	}

	private static void SettleWaterAt(int originX, int originY)
	{
		Tile tile = Main.tile[originX, originY];
		tilesIgnoreWater(ignoreSolids: true);
		if (tile.liquid == 0)
		{
			return;
		}
		int num = originX;
		int num2 = originY;
		bool tileAtXYHasLava = tile.lava();
		bool flag = tile.honey();
		bool flag2 = tile.shimmer();
		int num3 = tile.liquid;
		byte b = tile.liquidType();
		tile.liquid = 0;
		bool flag3 = true;
		while (true)
		{
			Tile tile2 = Main.tile[num, num2 + 1];
			bool flag4 = false;
			while (num2 < Main.maxTilesY - 5 && tile2.liquid == 0 && (!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type]))
			{
				num2++;
				flag4 = true;
				flag3 = false;
				tile2 = Main.tile[num, num2 + 1];
			}
			if (WorldGen.gen && flag4 && !flag && !flag2)
			{
				if (WorldGen.remixWorldGen)
				{
					b = (byte)((num2 > GenVars.lavaLine && ((double)num2 < Main.rockLayer - 80.0 || num2 > Main.maxTilesY - 350)) ? ((!WorldGen.oceanDepths(num, num2)) ? 1 : 0) : 0);
				}
				else if (num2 > GenVars.waterLine)
				{
					b = 1;
				}
			}
			int num4 = -1;
			int num5 = 0;
			int num6 = -1;
			int num7 = 0;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			while (true)
			{
				if (Main.tile[num + num5 * num4, num2].liquid == 0)
				{
					num6 = num4;
					num7 = num5;
				}
				if (num4 == -1 && num + num5 * num4 < 5)
				{
					flag6 = true;
				}
				else if (num4 == 1 && num + num5 * num4 > Main.maxTilesX - 5)
				{
					flag5 = true;
				}
				tile2 = Main.tile[num + num5 * num4, num2 + 1];
				if (tile2.liquid != 0 && tile2.liquid != byte.MaxValue && tile2.liquidType() == b)
				{
					int num8 = 255 - tile2.liquid;
					if (num8 > num3)
					{
						num8 = num3;
					}
					tile2.liquid += (byte)num8;
					num3 -= num8;
					if (num3 == 0)
					{
						break;
					}
				}
				if (num2 < Main.maxTilesY - 5 && tile2.liquid == 0 && (!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type]))
				{
					flag7 = true;
					break;
				}
				Tile tile3 = Main.tile[num + (num5 + 1) * num4, num2];
				if ((tile3.liquid != 0 && (!flag3 || num4 != 1)) || (tile3.nactive() && Main.tileSolid[tile3.type] && !Main.tileSolidTop[tile3.type]))
				{
					if (num4 == 1)
					{
						flag5 = true;
					}
					else
					{
						flag6 = true;
					}
				}
				if (flag6 && flag5)
				{
					break;
				}
				if (flag5)
				{
					num4 = -1;
					num5++;
				}
				else if (flag6)
				{
					if (num4 == 1)
					{
						num5++;
					}
					num4 = 1;
				}
				else
				{
					if (num4 == 1)
					{
						num5++;
					}
					num4 = -num4;
				}
			}
			num += num7 * num6;
			if (num3 == 0 || !flag7)
			{
				break;
			}
			num2++;
		}
		Main.tile[num, num2].liquid = (byte)num3;
		Main.tile[num, num2].liquidType(b);
		if (Main.tile[num, num2].liquid > 0)
		{
			AttemptToMoveLava(num, num2, tileAtXYHasLava);
			AttemptToMoveHoney(num, num2, flag);
			AttemptToMoveShimmer(num, num2, flag2);
		}
		tilesIgnoreWater(ignoreSolids: false);
	}

	private static void AttemptToMoveHoney(int X, int Y, bool tileAtXYHasHoney)
	{
		if (Main.tile[X - 1, Y].liquid > 0 && Main.tile[X - 1, Y].honey() != tileAtXYHasHoney)
		{
			if (tileAtXYHasHoney)
			{
				HoneyCheck(X, Y);
			}
			else
			{
				HoneyCheck(X - 1, Y);
			}
		}
		else if (Main.tile[X + 1, Y].liquid > 0 && Main.tile[X + 1, Y].honey() != tileAtXYHasHoney)
		{
			if (tileAtXYHasHoney)
			{
				HoneyCheck(X, Y);
			}
			else
			{
				HoneyCheck(X + 1, Y);
			}
		}
		else if (Main.tile[X, Y - 1].liquid > 0 && Main.tile[X, Y - 1].honey() != tileAtXYHasHoney)
		{
			if (tileAtXYHasHoney)
			{
				HoneyCheck(X, Y);
			}
			else
			{
				HoneyCheck(X, Y - 1);
			}
		}
		else if (Main.tile[X, Y + 1].liquid > 0 && Main.tile[X, Y + 1].honey() != tileAtXYHasHoney)
		{
			if (tileAtXYHasHoney)
			{
				HoneyCheck(X, Y);
			}
			else
			{
				HoneyCheck(X, Y + 1);
			}
		}
	}

	private static void AttemptToMoveLava(int X, int Y, bool tileAtXYHasLava)
	{
		if (Main.tile[X - 1, Y].liquid > 0 && Main.tile[X - 1, Y].lava() != tileAtXYHasLava)
		{
			if (tileAtXYHasLava)
			{
				LavaCheck(X, Y);
			}
			else
			{
				LavaCheck(X - 1, Y);
			}
		}
		else if (Main.tile[X + 1, Y].liquid > 0 && Main.tile[X + 1, Y].lava() != tileAtXYHasLava)
		{
			if (tileAtXYHasLava)
			{
				LavaCheck(X, Y);
			}
			else
			{
				LavaCheck(X + 1, Y);
			}
		}
		else if (Main.tile[X, Y - 1].liquid > 0 && Main.tile[X, Y - 1].lava() != tileAtXYHasLava)
		{
			if (tileAtXYHasLava)
			{
				LavaCheck(X, Y);
			}
			else
			{
				LavaCheck(X, Y - 1);
			}
		}
		else if (Main.tile[X, Y + 1].liquid > 0 && Main.tile[X, Y + 1].lava() != tileAtXYHasLava)
		{
			if (tileAtXYHasLava)
			{
				LavaCheck(X, Y);
			}
			else
			{
				LavaCheck(X, Y + 1);
			}
		}
	}

	private static void AttemptToMoveShimmer(int X, int Y, bool tileAtXYHasShimmer)
	{
		if (Main.tile[X - 1, Y].liquid > 0 && Main.tile[X - 1, Y].shimmer() != tileAtXYHasShimmer)
		{
			if (tileAtXYHasShimmer)
			{
				ShimmerCheck(X, Y);
			}
			else
			{
				ShimmerCheck(X - 1, Y);
			}
		}
		else if (Main.tile[X + 1, Y].liquid > 0 && Main.tile[X + 1, Y].shimmer() != tileAtXYHasShimmer)
		{
			if (tileAtXYHasShimmer)
			{
				ShimmerCheck(X, Y);
			}
			else
			{
				ShimmerCheck(X + 1, Y);
			}
		}
		else if (Main.tile[X, Y - 1].liquid > 0 && Main.tile[X, Y - 1].shimmer() != tileAtXYHasShimmer)
		{
			if (tileAtXYHasShimmer)
			{
				ShimmerCheck(X, Y);
			}
			else
			{
				ShimmerCheck(X, Y - 1);
			}
		}
		else if (Main.tile[X, Y + 1].liquid > 0 && Main.tile[X, Y + 1].shimmer() != tileAtXYHasShimmer)
		{
			if (tileAtXYHasShimmer)
			{
				ShimmerCheck(X, Y);
			}
			else
			{
				ShimmerCheck(X, Y + 1);
			}
		}
	}

	private static void UpdateProgressDisplay(int verbose, int minY, int maxY, int y)
	{
		if (verbose > 0)
		{
			float flag = (float)(maxY - y) / (float)(maxY - minY + 1);
			flag /= (float)verbose;
			Main.statusText = Lang.gen[27].Value + " " + (int)(flag * 100f + 1f) + "%";
		}
		else if (verbose < 0)
		{
			float point = (float)(maxY - y) / (float)(maxY - minY + 1);
			point /= (float)(-verbose);
			Main.statusText = Lang.gen[18].Value + " " + (int)(point * 100f + 1f) + "%";
		}
	}

	public void Update()
	{
		Main.tileSolid[379] = true;
		Tile flag = Main.tile[x - 1, y];
		Tile point = Main.tile[x + 1, y];
		Tile tile = Main.tile[x, y - 1];
		Tile tile2 = Main.tile[x, y + 1];
		Tile tile3 = Main.tile[x, y];
		if (tile3.nactive() && Main.tileSolid[tile3.type] && !Main.tileSolidTop[tile3.type])
		{
			_ = tile3.type;
			_ = 10;
			kill = 999;
			return;
		}
		byte liquid = tile3.liquid;
		float num = 0f;
		if (y > Main.UnderworldLayer && tile3.liquidType() == 0 && tile3.liquid > 0)
		{
			byte b = 2;
			if (tile3.liquid < b)
			{
				b = tile3.liquid;
			}
			tile3.liquid -= b;
		}
		if (tile3.liquid == 0)
		{
			kill = 999;
			return;
		}
		if (tile3.lava())
		{
			LavaCheck(x, y);
			if (!quickFall)
			{
				if (delay < 5)
				{
					delay++;
					return;
				}
				delay = 0;
			}
		}
		else
		{
			if (flag.lava())
			{
				AddWater(x - 1, y);
			}
			if (point.lava())
			{
				AddWater(x + 1, y);
			}
			if (tile.lava())
			{
				AddWater(x, y - 1);
			}
			if (tile2.lava())
			{
				AddWater(x, y + 1);
			}
			if (tile3.honey())
			{
				HoneyCheck(x, y);
				if (!quickFall)
				{
					if (delay < 10)
					{
						delay++;
						return;
					}
					delay = 0;
				}
			}
			else
			{
				if (flag.honey())
				{
					AddWater(x - 1, y);
				}
				if (point.honey())
				{
					AddWater(x + 1, y);
				}
				if (tile.honey())
				{
					AddWater(x, y - 1);
				}
				if (tile2.honey())
				{
					AddWater(x, y + 1);
				}
				if (tile3.shimmer())
				{
					ShimmerCheck(x, y);
				}
				else
				{
					if (flag.shimmer())
					{
						AddWater(x - 1, y);
					}
					if (point.shimmer())
					{
						AddWater(x + 1, y);
					}
					if (tile.shimmer())
					{
						AddWater(x, y - 1);
					}
					if (tile2.shimmer())
					{
						AddWater(x, y + 1);
					}
				}
			}
		}
		if ((!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type]) && (tile2.liquid <= 0 || tile2.liquidType() == tile3.liquidType()) && tile2.liquid < byte.MaxValue)
		{
			bool flag2 = false;
			num = 255 - tile2.liquid;
			if (num > (float)(int)tile3.liquid)
			{
				num = (int)tile3.liquid;
			}
			if (num == 1f && tile3.liquid == byte.MaxValue)
			{
				flag2 = true;
			}
			if (!flag2)
			{
				tile3.liquid -= (byte)num;
			}
			tile2.liquid += (byte)num;
			tile2.liquidType(tile3.liquidType());
			AddWater(x, y + 1);
			tile2.skipLiquid(skipLiquid: true);
			tile3.skipLiquid(skipLiquid: true);
			if (quickSettle && tile3.liquid > 250)
			{
				tile3.liquid = byte.MaxValue;
			}
			else if (!flag2)
			{
				AddWater(x - 1, y);
				AddWater(x + 1, y);
			}
		}
		if (tile3.liquid > 0)
		{
			bool flag3 = true;
			bool flag4 = true;
			bool flag5 = true;
			bool flag6 = true;
			if (flag.nactive() && Main.tileSolid[flag.type] && !Main.tileSolidTop[flag.type])
			{
				flag3 = false;
			}
			else if (flag.liquid > 0 && flag.liquidType() != tile3.liquidType())
			{
				flag3 = false;
			}
			else if (Main.tile[x - 2, y].nactive() && Main.tileSolid[Main.tile[x - 2, y].type] && !Main.tileSolidTop[Main.tile[x - 2, y].type])
			{
				flag5 = false;
			}
			else if (Main.tile[x - 2, y].liquid == 0)
			{
				flag5 = false;
			}
			else if (Main.tile[x - 2, y].liquid > 0 && Main.tile[x - 2, y].liquidType() != tile3.liquidType())
			{
				flag5 = false;
			}
			if (point.nactive() && Main.tileSolid[point.type] && !Main.tileSolidTop[point.type])
			{
				flag4 = false;
			}
			else if (point.liquid > 0 && point.liquidType() != tile3.liquidType())
			{
				flag4 = false;
			}
			else if (Main.tile[x + 2, y].nactive() && Main.tileSolid[Main.tile[x + 2, y].type] && !Main.tileSolidTop[Main.tile[x + 2, y].type])
			{
				flag6 = false;
			}
			else if (Main.tile[x + 2, y].liquid == 0)
			{
				flag6 = false;
			}
			else if (Main.tile[x + 2, y].liquid > 0 && Main.tile[x + 2, y].liquidType() != tile3.liquidType())
			{
				flag6 = false;
			}
			int num2 = 0;
			if (tile3.liquid < 3)
			{
				num2 = -1;
			}
			if (tile3.liquid > 250)
			{
				flag5 = false;
				flag6 = false;
			}
			if (flag3 && flag4)
			{
				if (flag5 && flag6)
				{
					bool flag7 = true;
					bool flag8 = true;
					if (Main.tile[x - 3, y].nactive() && Main.tileSolid[Main.tile[x - 3, y].type] && !Main.tileSolidTop[Main.tile[x - 3, y].type])
					{
						flag7 = false;
					}
					else if (Main.tile[x - 3, y].liquid == 0)
					{
						flag7 = false;
					}
					else if (Main.tile[x - 3, y].liquidType() != tile3.liquidType())
					{
						flag7 = false;
					}
					if (Main.tile[x + 3, y].nactive() && Main.tileSolid[Main.tile[x + 3, y].type] && !Main.tileSolidTop[Main.tile[x + 3, y].type])
					{
						flag8 = false;
					}
					else if (Main.tile[x + 3, y].liquid == 0)
					{
						flag8 = false;
					}
					else if (Main.tile[x + 3, y].liquidType() != tile3.liquidType())
					{
						flag8 = false;
					}
					if (flag7 && flag8)
					{
						num = flag.liquid + point.liquid + Main.tile[x - 2, y].liquid + Main.tile[x + 2, y].liquid + Main.tile[x - 3, y].liquid + Main.tile[x + 3, y].liquid + tile3.liquid + num2;
						num = (float)Math.Round(num / 7f);
						int num3 = 0;
						flag.liquidType(tile3.liquidType());
						if (flag.liquid != (byte)num)
						{
							flag.liquid = (byte)num;
							AddWater(x - 1, y);
						}
						else
						{
							num3++;
						}
						point.liquidType(tile3.liquidType());
						if (point.liquid != (byte)num)
						{
							point.liquid = (byte)num;
							AddWater(x + 1, y);
						}
						else
						{
							num3++;
						}
						Main.tile[x - 2, y].liquidType(tile3.liquidType());
						if (Main.tile[x - 2, y].liquid != (byte)num)
						{
							Main.tile[x - 2, y].liquid = (byte)num;
							AddWater(x - 2, y);
						}
						else
						{
							num3++;
						}
						Main.tile[x + 2, y].liquidType(tile3.liquidType());
						if (Main.tile[x + 2, y].liquid != (byte)num)
						{
							Main.tile[x + 2, y].liquid = (byte)num;
							AddWater(x + 2, y);
						}
						else
						{
							num3++;
						}
						Main.tile[x - 3, y].liquidType(tile3.liquidType());
						if (Main.tile[x - 3, y].liquid != (byte)num)
						{
							Main.tile[x - 3, y].liquid = (byte)num;
							AddWater(x - 3, y);
						}
						else
						{
							num3++;
						}
						Main.tile[x + 3, y].liquidType(tile3.liquidType());
						if (Main.tile[x + 3, y].liquid != (byte)num)
						{
							Main.tile[x + 3, y].liquid = (byte)num;
							AddWater(x + 3, y);
						}
						else
						{
							num3++;
						}
						if (flag.liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x - 1, y);
						}
						if (point.liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x + 1, y);
						}
						if (Main.tile[x - 2, y].liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x - 2, y);
						}
						if (Main.tile[x + 2, y].liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x + 2, y);
						}
						if (Main.tile[x - 3, y].liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x - 3, y);
						}
						if (Main.tile[x + 3, y].liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x + 3, y);
						}
						if (num3 != 6 || tile.liquid <= 0)
						{
							tile3.liquid = (byte)num;
						}
					}
					else
					{
						int num4 = 0;
						num = flag.liquid + point.liquid + Main.tile[x - 2, y].liquid + Main.tile[x + 2, y].liquid + tile3.liquid + num2;
						num = (float)Math.Round(num / 5f);
						flag.liquidType(tile3.liquidType());
						if (flag.liquid != (byte)num)
						{
							flag.liquid = (byte)num;
							AddWater(x - 1, y);
						}
						else
						{
							num4++;
						}
						point.liquidType(tile3.liquidType());
						if (point.liquid != (byte)num)
						{
							point.liquid = (byte)num;
							AddWater(x + 1, y);
						}
						else
						{
							num4++;
						}
						Main.tile[x - 2, y].liquidType(tile3.liquidType());
						if (Main.tile[x - 2, y].liquid != (byte)num)
						{
							Main.tile[x - 2, y].liquid = (byte)num;
							AddWater(x - 2, y);
						}
						else
						{
							num4++;
						}
						Main.tile[x + 2, y].liquidType(tile3.liquidType());
						if (Main.tile[x + 2, y].liquid != (byte)num)
						{
							Main.tile[x + 2, y].liquid = (byte)num;
							AddWater(x + 2, y);
						}
						else
						{
							num4++;
						}
						if (flag.liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x - 1, y);
						}
						if (point.liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x + 1, y);
						}
						if (Main.tile[x - 2, y].liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x - 2, y);
						}
						if (Main.tile[x + 2, y].liquid != (byte)num || tile3.liquid != (byte)num)
						{
							AddWater(x + 2, y);
						}
						if (num4 != 4 || tile.liquid <= 0)
						{
							tile3.liquid = (byte)num;
						}
					}
				}
				else if (flag5)
				{
					num = flag.liquid + point.liquid + Main.tile[x - 2, y].liquid + tile3.liquid + num2;
					num = (float)Math.Round(num / 4f);
					flag.liquidType(tile3.liquidType());
					if (flag.liquid != (byte)num || tile3.liquid != (byte)num)
					{
						flag.liquid = (byte)num;
						AddWater(x - 1, y);
					}
					point.liquidType(tile3.liquidType());
					if (point.liquid != (byte)num || tile3.liquid != (byte)num)
					{
						point.liquid = (byte)num;
						AddWater(x + 1, y);
					}
					Main.tile[x - 2, y].liquidType(tile3.liquidType());
					if (Main.tile[x - 2, y].liquid != (byte)num || tile3.liquid != (byte)num)
					{
						Main.tile[x - 2, y].liquid = (byte)num;
						AddWater(x - 2, y);
					}
					tile3.liquid = (byte)num;
				}
				else if (flag6)
				{
					num = flag.liquid + point.liquid + Main.tile[x + 2, y].liquid + tile3.liquid + num2;
					num = (float)Math.Round(num / 4f);
					flag.liquidType(tile3.liquidType());
					if (flag.liquid != (byte)num || tile3.liquid != (byte)num)
					{
						flag.liquid = (byte)num;
						AddWater(x - 1, y);
					}
					point.liquidType(tile3.liquidType());
					if (point.liquid != (byte)num || tile3.liquid != (byte)num)
					{
						point.liquid = (byte)num;
						AddWater(x + 1, y);
					}
					Main.tile[x + 2, y].liquidType(tile3.liquidType());
					if (Main.tile[x + 2, y].liquid != (byte)num || tile3.liquid != (byte)num)
					{
						Main.tile[x + 2, y].liquid = (byte)num;
						AddWater(x + 2, y);
					}
					tile3.liquid = (byte)num;
				}
				else
				{
					num = flag.liquid + point.liquid + tile3.liquid + num2;
					num = (float)Math.Round(num / 3f);
					if (num == 254f && WorldGen.genRand.Next(30) == 0)
					{
						num = 255f;
					}
					flag.liquidType(tile3.liquidType());
					if (flag.liquid != (byte)num)
					{
						flag.liquid = (byte)num;
						AddWater(x - 1, y);
					}
					point.liquidType(tile3.liquidType());
					if (point.liquid != (byte)num)
					{
						point.liquid = (byte)num;
						AddWater(x + 1, y);
					}
					tile3.liquid = (byte)num;
				}
			}
			else if (flag3)
			{
				num = flag.liquid + tile3.liquid + num2;
				num = (float)Math.Round(num / 2f);
				if (flag.liquid != (byte)num)
				{
					flag.liquid = (byte)num;
				}
				flag.liquidType(tile3.liquidType());
				if (tile3.liquid != (byte)num || flag.liquid != (byte)num)
				{
					AddWater(x - 1, y);
				}
				tile3.liquid = (byte)num;
			}
			else if (flag4)
			{
				num = point.liquid + tile3.liquid + num2;
				num = (float)Math.Round(num / 2f);
				if (point.liquid != (byte)num)
				{
					point.liquid = (byte)num;
				}
				point.liquidType(tile3.liquidType());
				if (tile3.liquid != (byte)num || point.liquid != (byte)num)
				{
					AddWater(x + 1, y);
				}
				tile3.liquid = (byte)num;
			}
		}
		if (tile3.liquid != liquid)
		{
			if (tile3.liquid == 254 && liquid == byte.MaxValue)
			{
				if (quickSettle)
				{
					tile3.liquid = byte.MaxValue;
					kill++;
				}
				else
				{
					kill++;
				}
			}
			else
			{
				AddWater(x, y - 1);
				kill = 0;
			}
		}
		else
		{
			kill++;
		}
	}

	public static void StartPanic()
	{
		if (!panicMode)
		{
			GenVars.waterLine = Main.maxTilesY;
			numLiquid = 0;
			LiquidBuffer.numLiquidBuffer = 0;
			panicCounter = 0;
			panicMode = true;
			panicY = Main.maxTilesY - 3;
			if (Main.dedServ)
			{
				Console.WriteLine(Language.GetTextValue("Misc.ForceWaterSettling"));
			}
		}
	}

	public static void UpdateLiquid()
	{
		int result = 8;
		tilesIgnoreWater(ignoreSolids: true);
		if (Main.netMode == 2)
		{
			int num = 0;
			for (int i = 0; i < 15; i++)
			{
				if (Main.player[i].active)
				{
					num++;
				}
			}
			cycles = 10 + num / 3;
			curMaxLiquid = maxLiquid - num * 250;
			result = 10 + num / 3;
			if (Main.Setting_UseReducedMaxLiquids)
			{
				curMaxLiquid = 5000;
			}
		}
		if (!WorldGen.gen)
		{
			if (!panicMode)
			{
				if ((double)LiquidBuffer.numLiquidBuffer >= 45000.0)
				{
					panicCounter++;
					if (panicCounter > 3600)
					{
						StartPanic();
					}
				}
				else
				{
					panicCounter = 0;
				}
			}
			if (panicMode)
			{
				int num2 = 0;
				while (panicY >= 3 && num2 < 5)
				{
					num2++;
					QuickWater(0, panicY, panicY);
					panicY--;
					if (panicY >= 3)
					{
						continue;
					}
					Console.WriteLine(Language.GetTextValue("Misc.WaterSettled"));
					panicCounter = 0;
					panicMode = false;
					WorldGen.WaterCheck();
					if (Main.netMode != 2)
					{
						continue;
					}
					for (int j = 0; j < 255; j++)
					{
						for (int k = 0; k < Main.maxSectionsX; k++)
						{
							for (int l = 0; l < Main.maxSectionsY; l++)
							{
								Netplay.Clients[j].TileSections[k, l] = false;
							}
						}
					}
				}
				return;
			}
		}
		bool flag = quickSettle;
		if (Main.Setting_UseReducedMaxLiquids)
		{
			flag |= numLiquid > 2000;
		}
		if (flag)
		{
			quickFall = true;
		}
		else
		{
			quickFall = false;
		}
		wetCounter++;
		int num3 = curMaxLiquid / cycles;
		int num4 = num3 * (wetCounter - 1);
		int num5 = num3 * wetCounter;
		if (wetCounter == cycles)
		{
			num5 = numLiquid;
		}
		if (num5 > numLiquid)
		{
			num5 = numLiquid;
			_ = Main.netMode;
			wetCounter = cycles;
		}
		if (quickFall)
		{
			for (int m = num4; m < num5; m++)
			{
				Main.liquid[m].delay = 10;
				Main.liquid[m].Update();
				Main.tile[Main.liquid[m].x, Main.liquid[m].y].skipLiquid(skipLiquid: false);
			}
		}
		else
		{
			for (int n = num4; n < num5; n++)
			{
				if (!Main.tile[Main.liquid[n].x, Main.liquid[n].y].skipLiquid())
				{
					Main.liquid[n].Update();
				}
				else
				{
					Main.tile[Main.liquid[n].x, Main.liquid[n].y].skipLiquid(skipLiquid: false);
				}
			}
		}
		if (wetCounter >= cycles)
		{
			wetCounter = 0;
			for (int num6 = numLiquid - 1; num6 >= 0; num6--)
			{
				if (Main.liquid[num6].kill >= result)
				{
					if (Main.tile[Main.liquid[num6].x, Main.liquid[num6].y].liquid == 254)
					{
						Main.tile[Main.liquid[num6].x, Main.liquid[num6].y].liquid = byte.MaxValue;
					}
					DelWater(num6);
				}
			}
			int num7 = curMaxLiquid - (curMaxLiquid - numLiquid);
			if (num7 > LiquidBuffer.numLiquidBuffer)
			{
				num7 = LiquidBuffer.numLiquidBuffer;
			}
			for (int num8 = 0; num8 < num7; num8++)
			{
				Main.tile[Main.liquidBuffer[0].x, Main.liquidBuffer[0].y].checkingLiquid(checkingLiquid: false);
				AddWater(Main.liquidBuffer[0].x, Main.liquidBuffer[0].y);
				LiquidBuffer.DelBuffer(0);
			}
			if (numLiquid > 0 && numLiquid > stuckAmount - 50 && numLiquid < stuckAmount + 50)
			{
				stuckCount++;
				if (stuckCount >= 10000)
				{
					stuck = true;
					for (int num9 = numLiquid - 1; num9 >= 0; num9--)
					{
						DelWater(num9);
					}
					stuck = false;
					stuckCount = 0;
				}
			}
			else
			{
				stuckCount = 0;
				stuckAmount = numLiquid;
			}
		}
		if (!WorldGen.gen && Main.netMode == 2 && _netChangeSet.Count > 0)
		{
			Utils.Swap(ref _netChangeSet, ref _swapNetChangeSet);
			NetLiquidModule.CreateAndBroadcastByChunk(_swapNetChangeSet);
			_swapNetChangeSet.Clear();
		}
		tilesIgnoreWater(ignoreSolids: false);
	}

	public static void AddWater(int x, int y)
	{
		Tile flag = Main.tile[x, y];
		if (Main.tile[x, y] == null || flag.checkingLiquid() || x >= Main.maxTilesX - 5 || y >= Main.maxTilesY - 5 || x < 5 || y < 5 || flag.liquid == 0 || (flag.nactive() && Main.tileSolid[flag.type] && flag.type != 546 && !Main.tileSolidTop[flag.type]))
		{
			return;
		}
		if (numLiquid >= curMaxLiquid - 1)
		{
			LiquidBuffer.AddBuffer(x, y);
			return;
		}
		flag.checkingLiquid(checkingLiquid: true);
		flag.skipLiquid(skipLiquid: false);
		Main.liquid[numLiquid].kill = 0;
		Main.liquid[numLiquid].x = x;
		Main.liquid[numLiquid].y = y;
		Main.liquid[numLiquid].delay = 0;
		numLiquid++;
		if (Main.netMode == 2)
		{
			NetSendLiquid(x, y);
		}
		if (!flag.active() || WorldGen.gen)
		{
			return;
		}
		bool flag2 = false;
		if (flag.lava())
		{
			if (TileObjectData.CheckLavaDeath(flag))
			{
				flag2 = true;
			}
		}
		else if (TileObjectData.CheckWaterDeath(flag))
		{
			flag2 = true;
		}
		if (flag2)
		{
			WorldGen.KillTile(x, y);
			if (Main.netMode == 2)
			{
				NetMessage.SendData(17, -1, -1, null, 0, x, y);
			}
		}
	}

	private static bool UndergroundDesertCheck(int x, int y)
	{
		int value = 3;
		for (int vector = x - value; vector <= x + value; vector++)
		{
			for (int vector2 = y - value; vector2 <= y + value; vector2++)
			{
				if (WorldGen.InWorld(vector, vector2) && (Main.tile[vector, vector2].wall == 187 || Main.tile[vector, vector2].wall == 216))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void LiquidCheck(int x, int y, int thisLiquidType)
	{
		if (WorldGen.SolidTile(x, y))
		{
			return;
		}
		Tile vector2 = Main.tile[x - 1, y];
		Tile num2 = Main.tile[x + 1, y];
		Tile value = Main.tile[x, y - 1];
		Tile vector = Main.tile[x, y + 1];
		Tile num = Main.tile[x, y];
		if ((vector2.liquid > 0 && vector2.liquidType() != thisLiquidType) || (num2.liquid > 0 && num2.liquidType() != thisLiquidType) || (value.liquid > 0 && value.liquidType() != thisLiquidType))
		{
			int num3 = 0;
			if (vector2.liquidType() != thisLiquidType)
			{
				num3 += vector2.liquid;
				vector2.liquid = 0;
			}
			if (num2.liquidType() != thisLiquidType)
			{
				num3 += num2.liquid;
				num2.liquid = 0;
			}
			if (value.liquidType() != thisLiquidType)
			{
				num3 += value.liquid;
				value.liquid = 0;
			}
			int liquidMergeTileType = 56;
			int liquidMergeType = 0;
			bool waterNearby = vector2.liquidType() == 0 || num2.liquidType() == 0 || value.liquidType() == 0;
			bool lavaNearby = vector2.lava() || num2.lava() || value.lava();
			bool honeyNearby = vector2.honey() || num2.honey() || value.honey();
			bool shimmerNearby = vector2.shimmer() || num2.shimmer() || value.shimmer();
			GetLiquidMergeTypes(thisLiquidType, out liquidMergeTileType, out liquidMergeType, waterNearby, lavaNearby, honeyNearby, shimmerNearby);
			if (num3 < 24 || liquidMergeType == thisLiquidType)
			{
				return;
			}
			if (num.active() && Main.tileObsidianKill[num.type])
			{
				WorldGen.KillTile(x, y);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(17, -1, -1, null, 0, x, y);
				}
			}
			if (!num.active())
			{
				num.liquid = 0;
				switch (thisLiquidType)
				{
				case 1:
					num.lava(lava: false);
					break;
				case 2:
					num.honey(honey: false);
					break;
				case 3:
					num.shimmer(shimmer: false);
					break;
				}
				TileChangeType liquidChangeType = WorldGen.GetLiquidChangeType(thisLiquidType, liquidMergeType);
				if (!WorldGen.gen)
				{
					WorldGen.PlayLiquidChangeSound(liquidChangeType, x, y);
				}
				WorldGen.PlaceTile(x, y, liquidMergeTileType, mute: true, forced: true);
				WorldGen.SquareTileFrame(x, y);
				if (Main.netMode == 2)
				{
					NetMessage.SendTileSquare(-1, x - 1, y - 1, 3, liquidChangeType);
				}
			}
		}
		else
		{
			if (vector.liquid <= 0 || vector.liquidType() == thisLiquidType)
			{
				return;
			}
			bool flag = false;
			if (num.active() && TileID.Sets.IsAContainer[num.type] && !TileID.Sets.IsAContainer[vector.type])
			{
				flag = true;
			}
			if (thisLiquidType != 0 && Main.tileCut[vector.type])
			{
				WorldGen.KillTile(x, y + 1);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(17, -1, -1, null, 0, x, y + 1);
				}
			}
			else if (vector.active() && Main.tileObsidianKill[vector.type])
			{
				WorldGen.KillTile(x, y + 1);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(17, -1, -1, null, 0, x, y + 1);
				}
			}
			if (!(!vector.active() || flag))
			{
				return;
			}
			if (num.liquid < 24)
			{
				num.liquid = 0;
				num.liquidType(0);
				if (Main.netMode == 2)
				{
					NetMessage.SendTileSquare(-1, x - 1, y, 3);
				}
				return;
			}
			int liquidMergeTileType2 = 56;
			int liquidMergeType2 = 0;
			bool waterNearby2 = vector.liquidType() == 0;
			bool lavaNearby2 = vector.lava();
			bool honeyNearby2 = vector.honey();
			bool shimmerNearby2 = vector.shimmer();
			GetLiquidMergeTypes(thisLiquidType, out liquidMergeTileType2, out liquidMergeType2, waterNearby2, lavaNearby2, honeyNearby2, shimmerNearby2);
			num.liquid = 0;
			switch (thisLiquidType)
			{
			case 1:
				num.lava(lava: false);
				break;
			case 2:
				num.honey(honey: false);
				break;
			case 3:
				num.shimmer(shimmer: false);
				break;
			}
			vector.liquid = 0;
			TileChangeType liquidChangeType2 = WorldGen.GetLiquidChangeType(thisLiquidType, liquidMergeType2);
			if (!Main.gameMenu)
			{
				WorldGen.PlayLiquidChangeSound(liquidChangeType2, x, y);
			}
			WorldGen.PlaceTile(x, y + 1, liquidMergeTileType2, mute: true, forced: true);
			WorldGen.SquareTileFrame(x, y + 1);
			if (Main.netMode == 2)
			{
				NetMessage.SendTileSquare(-1, x - 1, y, 3, liquidChangeType2);
			}
		}
	}

	public static void GetLiquidMergeTypes(int thisLiquidType, out int liquidMergeTileType, out int liquidMergeType, bool waterNearby, bool lavaNearby, bool honeyNearby, bool shimmerNearby)
	{
		liquidMergeTileType = 56;
		liquidMergeType = thisLiquidType;
		if (thisLiquidType != 0 && waterNearby)
		{
			switch (thisLiquidType)
			{
			case 1:
				liquidMergeTileType = 56;
				break;
			case 2:
				liquidMergeTileType = 229;
				break;
			case 3:
				liquidMergeTileType = 659;
				break;
			}
			liquidMergeType = 0;
		}
		if (thisLiquidType != 1 && lavaNearby)
		{
			switch (thisLiquidType)
			{
			case 0:
				liquidMergeTileType = 56;
				break;
			case 2:
				liquidMergeTileType = 230;
				break;
			case 3:
				liquidMergeTileType = 659;
				break;
			}
			liquidMergeType = 1;
		}
		if (thisLiquidType != 2 && honeyNearby)
		{
			switch (thisLiquidType)
			{
			case 0:
				liquidMergeTileType = 229;
				break;
			case 1:
				liquidMergeTileType = 230;
				break;
			case 3:
				liquidMergeTileType = 659;
				break;
			}
			liquidMergeType = 2;
		}
		if (thisLiquidType != 3 && shimmerNearby)
		{
			switch (thisLiquidType)
			{
			case 0:
				liquidMergeTileType = 659;
				break;
			case 1:
				liquidMergeTileType = 659;
				break;
			case 2:
				liquidMergeTileType = 659;
				break;
			}
			liquidMergeType = 3;
		}
	}

	public static void LavaCheck(int x, int y)
	{
		if (!WorldGen.remixWorldGen && WorldGen.generatingWorld && UndergroundDesertCheck(x, y))
		{
			for (int i = x - 3; i <= x + 3; i++)
			{
				for (int j = y - 3; j <= y + 3; j++)
				{
					Main.tile[i, j].lava(lava: true);
				}
			}
		}
		LiquidCheck(x, y, 1);
	}

	public static void HoneyCheck(int x, int y)
	{
		LiquidCheck(x, y, 2);
	}

	public static void ShimmerCheck(int x, int y)
	{
		LiquidCheck(x, y, 3);
	}

	public static void DelWater(int l)
	{
		int num = Main.liquid[l].x;
		int num2 = Main.liquid[l].y;
		Tile tile = Main.tile[num - 1, num2];
		Tile tile2 = Main.tile[num + 1, num2];
		Tile tile3 = Main.tile[num, num2 + 1];
		Tile tile4 = Main.tile[num, num2];
		byte b = 2;
		if (tile4.liquid < b)
		{
			tile4.liquid = 0;
			if (tile.liquid < b)
			{
				tile.liquid = 0;
			}
			else
			{
				AddWater(num - 1, num2);
			}
			if (tile2.liquid < b)
			{
				tile2.liquid = 0;
			}
			else
			{
				AddWater(num + 1, num2);
			}
		}
		else if (tile4.liquid < 20)
		{
			if ((tile.liquid < tile4.liquid && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type])) || (tile2.liquid < tile4.liquid && (!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type])) || (tile3.liquid < byte.MaxValue && (!tile3.nactive() || !Main.tileSolid[tile3.type] || Main.tileSolidTop[tile3.type])))
			{
				tile4.liquid = 0;
			}
		}
		else if (tile3.liquid < byte.MaxValue && (!tile3.nactive() || !Main.tileSolid[tile3.type] || Main.tileSolidTop[tile3.type]) && !stuck && (!Main.tile[num, num2].nactive() || !Main.tileSolid[Main.tile[num, num2].type] || Main.tileSolidTop[Main.tile[num, num2].type]))
		{
			Main.liquid[l].kill = 0;
			return;
		}
		if (tile4.liquid < 250 && Main.tile[num, num2 - 1].liquid > 0)
		{
			AddWater(num, num2 - 1);
		}
		if (tile4.liquid == 0)
		{
			tile4.liquidType(0);
		}
		else
		{
			if (tile2.liquid > 0 && tile2.liquid < 250 && (!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type]) && tile4.liquid != tile2.liquid)
			{
				AddWater(num + 1, num2);
			}
			if (tile.liquid > 0 && tile.liquid < 250 && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]) && tile4.liquid != tile.liquid)
			{
				AddWater(num - 1, num2);
			}
			if (tile4.lava())
			{
				LavaCheck(num, num2);
				for (int i = num - 1; i <= num + 1; i++)
				{
					for (int j = num2 - 1; j <= num2 + 1; j++)
					{
						Tile tile5 = Main.tile[i, j];
						if (!tile5.active())
						{
							continue;
						}
						if (tile5.type == 2 || tile5.type == 23 || tile5.type == 109 || tile5.type == 199 || tile5.type == 477 || tile5.type == 492)
						{
							tile5.type = 0;
							WorldGen.SquareTileFrame(i, j);
							if (Main.netMode == 2)
							{
								NetMessage.SendTileSquare(-1, num, num2, 3);
							}
						}
						else if (tile5.type == 60 || tile5.type == 70 || tile5.type == 661 || tile5.type == 662)
						{
							tile5.type = 59;
							WorldGen.SquareTileFrame(i, j);
							if (Main.netMode == 2)
							{
								NetMessage.SendTileSquare(-1, num, num2, 3);
							}
						}
					}
				}
			}
			else if (tile4.honey())
			{
				HoneyCheck(num, num2);
			}
			else if (tile4.shimmer())
			{
				ShimmerCheck(num, num2);
			}
		}
		if (Main.netMode == 2)
		{
			NetSendLiquid(num, num2);
		}
		numLiquid--;
		Main.tile[Main.liquid[l].x, Main.liquid[l].y].checkingLiquid(checkingLiquid: false);
		Main.liquid[l].x = Main.liquid[numLiquid].x;
		Main.liquid[l].y = Main.liquid[numLiquid].y;
		Main.liquid[l].kill = Main.liquid[numLiquid].kill;
		if (Main.tileAlch[tile4.type])
		{
			WorldGen.CheckAlch(num, num2);
		}
		else if (tile4.type == 518)
		{
			if (quickFall)
			{
				WorldGen.CheckLilyPad(num, num2);
			}
			else if (Main.tile[num, num2 + 1].liquid < byte.MaxValue || Main.tile[num, num2 - 1].liquid > 0)
			{
				WorldGen.SquareTileFrame(num, num2);
			}
			else
			{
				WorldGen.CheckLilyPad(num, num2);
			}
		}
	}
}
