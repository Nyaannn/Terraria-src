using System;
using Microsoft.Xna.Framework;
using ReLogic.Threading;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.Utilities;

namespace Terraria.Graphics.Light;

public class TileLightScanner
{
	private FastRandom _random = FastRandom.CreateWithRandomSeed();

	private bool _drawInvisibleWalls;

	public void ExportTo(Rectangle area, LightMap outputMap, TileLightScannerOptions options)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		_drawInvisibleWalls = options.DrawInvisibleWalls;
		FastParallel.For(area.Left, area.Right, (ParallelForAction)delegate(int start, int end, object context)
		{
			for (int i = start; i < end; i++)
			{
				for (int j = area.Top; j <= area.Bottom; j++)
				{
					if (IsTileNullOrTouchingNull(i, j))
					{
						outputMap.SetMaskAt(i - area.X, j - area.Y, LightMaskMode.None);
						outputMap[i - area.X, j - area.Y] = Vector3.Zero;
					}
					else
					{
						LightMaskMode tileMask = GetTileMask(Main.tile[i, j]);
						outputMap.SetMaskAt(i - area.X, j - area.Y, tileMask);
						GetTileLight(i, j, out var outputColor);
						outputMap[i - area.X, j - area.Y] = outputColor;
					}
				}
			}
		}, (object)null);
	}

	private bool IsTileNullOrTouchingNull(int x, int y)
	{
		if (WorldGen.InWorld(x, y, 1))
		{
			if (Main.tile[x, y] != null && Main.tile[x + 1, y] != null && Main.tile[x - 1, y] != null && Main.tile[x, y - 1] != null)
			{
				return Main.tile[x, y + 1] == null;
			}
			return true;
		}
		return true;
	}

	public void Update()
	{
		_random.NextSeed();
	}

	public LightMaskMode GetMaskMode(int x, int y)
	{
		return GetTileMask(Main.tile[x, y]);
	}

	private LightMaskMode GetTileMask(Tile tile)
	{
		if (LightIsBlocked(tile) && tile.type != 131 && !tile.inActive() && tile.slope() == 0)
		{
			return LightMaskMode.Solid;
		}
		if (!tile.lava() && tile.liquid > 128)
		{
			if (!tile.honey())
			{
				return LightMaskMode.Water;
			}
			return LightMaskMode.Honey;
		}
		return LightMaskMode.None;
	}

	public void GetTileLight(int x, int y, out Vector3 outputColor)
	{
		outputColor = Vector3.Zero;
		Tile num = Main.tile[x, y];
		FastRandom num2 = _random.WithModifier(x, y);
		if (y <= (int)Main.worldSurface)
		{
			ApplySurfaceLight(num, x, y, ref outputColor);
		}
		else if (y > Main.UnderworldLayer)
		{
			ApplyHellLight(num, x, y, ref outputColor);
		}
		ApplyWallLight(num, x, y, ref num2, ref outputColor);
		if (num.active())
		{
			ApplyTileLight(num, x, y, ref num2, ref outputColor);
		}
		ApplyLiquidLight(num, ref outputColor);
	}

	private void ApplyLiquidLight(Tile tile, ref Vector3 lightColor)
	{
		if (tile.liquid <= 0)
		{
			return;
		}
		if (tile.lava())
		{
			float result = 0.55f;
			result += (float)(270 - Main.mouseTextColor) / 900f;
			if (lightColor.X < result)
			{
				lightColor.X = result;
			}
			if (lightColor.Y < result)
			{
				lightColor.Y = result * 0.6f;
			}
			if (lightColor.Z < result)
			{
				lightColor.Z = result * 0.2f;
			}
		}
		else if (tile.shimmer())
		{
			float num = 0.7f;
			float num2 = 0.7f;
			num += (float)(270 - Main.mouseTextColor) / 900f;
			num2 += (float)(270 - Main.mouseTextColor) / 125f;
			if (lightColor.X < num)
			{
				lightColor.X = num * 0.6f;
			}
			if (lightColor.Y < num2)
			{
				lightColor.Y = num2 * 0.25f;
			}
			if (lightColor.Z < num)
			{
				lightColor.Z = num * 0.9f;
			}
		}
	}

	private bool LightIsBlocked(Tile tile)
	{
		if (tile.active() && Main.tileBlockLight[tile.type])
		{
			if (tile.invisibleBlock())
			{
				return _drawInvisibleWalls;
			}
			return true;
		}
		return false;
	}

	private void ApplyWallLight(Tile tile, int x, int y, ref FastRandom localRandom, ref Vector3 lightColor)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		switch (tile.wall)
		{
		case 182:
			if (!LightIsBlocked(tile))
			{
				num = 0.24f;
				num2 = 0.12f;
				num3 = 0.0899999961f;
			}
			break;
		case 33:
			if (!LightIsBlocked(tile))
			{
				num = 0.0899999961f;
				num2 = 0.0525000021f;
				num3 = 0.24f;
			}
			break;
		case 174:
			if (!LightIsBlocked(tile))
			{
				num = 0.2975f;
			}
			break;
		case 175:
			if (!LightIsBlocked(tile))
			{
				num = 0.075f;
				num2 = 0.15f;
				num3 = 0.4f;
			}
			break;
		case 176:
			if (!LightIsBlocked(tile))
			{
				num = 0.1f;
				num2 = 0.1f;
				num3 = 0.1f;
			}
			break;
		case 137:
			if (!LightIsBlocked(tile))
			{
				float num4 = 0.4f;
				num4 += (float)(270 - Main.mouseTextColor) / 1500f;
				num4 += (float)localRandom.Next(0, 50) * 0.0005f;
				num = 1f * num4;
				num2 = 0.5f * num4;
				num3 = 0.1f * num4;
			}
			break;
		case 44:
			if (!LightIsBlocked(tile))
			{
				num = (float)Main.DiscoR / 255f * 0.15f;
				num2 = (float)Main.DiscoG / 255f * 0.15f;
				num3 = (float)Main.DiscoB / 255f * 0.15f;
			}
			break;
		case 154:
			num = 0.6f;
			num3 = 0.6f;
			break;
		case 166:
			num = 0.6f;
			num2 = 0.6f;
			break;
		case 165:
			num3 = 0.6f;
			break;
		case 156:
			num2 = 0.6f;
			break;
		case 164:
			num = 0.6f;
			break;
		case 155:
			num = 0.6f;
			num2 = 0.6f;
			num3 = 0.6f;
			break;
		case 153:
			num = 0.6f;
			num2 = 0.3f;
			break;
		case 341:
			if (!LightIsBlocked(tile))
			{
				num = 0.25f;
				num2 = 0.1f;
				num3 = 0f;
			}
			break;
		case 343:
			if (!LightIsBlocked(tile))
			{
				num = 0f;
				num2 = 0.25f;
				num3 = 0f;
			}
			break;
		case 344:
			if (!LightIsBlocked(tile))
			{
				num = 0f;
				num2 = 0.16f;
				num3 = 0.34f;
			}
			break;
		case 342:
			if (!LightIsBlocked(tile))
			{
				num = 0.3f;
				num2 = 0f;
				num3 = 0.17f;
			}
			break;
		case 345:
			if (!LightIsBlocked(tile))
			{
				num = 0.3f;
				num2 = 0f;
				num3 = 0.35f;
			}
			break;
		case 346:
			if (!LightIsBlocked(tile))
			{
				num = (float)Main.DiscoR / 255f * 0.25f;
				num2 = (float)Main.DiscoG / 255f * 0.25f;
				num3 = (float)Main.DiscoB / 255f * 0.25f;
			}
			break;
		}
		if (lightColor.X < num)
		{
			lightColor.X = num;
		}
		if (lightColor.Y < num2)
		{
			lightColor.Y = num2;
		}
		if (lightColor.Z < num3)
		{
			lightColor.Z = num3;
		}
	}

	private void ApplyTileLight(Tile tile, int x, int y, ref FastRandom localRandom, ref Vector3 lightColor)
	{
		float list = 0f;
		float index = 0f;
		float B = 0f;
		if (Main.tileLighted[tile.type])
		{
			switch (tile.type)
			{
			case 658:
				if (!tile.invisibleBlock())
				{
					TorchID.TorchColor(23, out list, out index, out B);
					switch (tile.frameY / 54)
					{
					default:
						list *= 0.2f;
						index *= 0.2f;
						B *= 0.2f;
						break;
					case 1:
						list *= 0.3f;
						index *= 0.3f;
						B *= 0.3f;
						break;
					case 2:
						list *= 0.1f;
						index *= 0.1f;
						B *= 0.1f;
						break;
					}
				}
				break;
			case 356:
				if (Main.sundialCooldown == 0)
				{
					list = 0.45f;
					index = 0.25f;
					B = 0f;
				}
				break;
			case 663:
				if (Main.moondialCooldown == 0)
				{
					list = 0f;
					index = 0.25f;
					B = 0.45f;
				}
				break;
			case 656:
				list = 0.2f;
				index = 0.55f;
				B = 0.5f;
				break;
			case 20:
			{
				int num22 = tile.frameX / 18;
				if (num22 >= 30 && num22 <= 32)
				{
					list = 0.325f;
					index = 0.15f;
					B = 0.05f;
				}
				break;
			}
			case 634:
				list = 0.65f;
				index = 0.3f;
				B = 0.1f;
				break;
			case 633:
			case 637:
			case 638:
				list = 0.325f;
				index = 0.15f;
				B = 0.05f;
				break;
			case 463:
				list = 0.2f;
				index = 0.4f;
				B = 0.8f;
				break;
			case 491:
				list = 0.5f;
				index = 0.4f;
				B = 0.7f;
				break;
			case 209:
				if (tile.frameX == 234 || tile.frameX == 252)
				{
					Vector3 vector6 = PortalHelper.GetPortalColor(Main.myPlayer, 0).ToVector3() * 0.65f;
					list = vector6.X;
					index = vector6.Y;
					B = vector6.Z;
				}
				else if (tile.frameX == 306 || tile.frameX == 324)
				{
					Vector3 vector7 = PortalHelper.GetPortalColor(Main.myPlayer, 1).ToVector3() * 0.65f;
					list = vector7.X;
					index = vector7.Y;
					B = vector7.Z;
				}
				break;
			case 415:
				list = 0.7f;
				index = 0.5f;
				B = 0.1f;
				break;
			case 500:
				list = 0.525f;
				index = 0.375f;
				B = 0.075f;
				break;
			case 416:
				list = 0f;
				index = 0.6f;
				B = 0.7f;
				break;
			case 501:
				list = 0f;
				index = 0.45f;
				B = 0.525f;
				break;
			case 417:
				list = 0.6f;
				index = 0.2f;
				B = 0.6f;
				break;
			case 502:
				list = 0.45f;
				index = 0.15f;
				B = 0.45f;
				break;
			case 418:
				list = 0.6f;
				index = 0.6f;
				B = 0.9f;
				break;
			case 503:
				list = 0.45f;
				index = 0.45f;
				B = 0.675f;
				break;
			case 390:
				list = 0.4f;
				index = 0.2f;
				B = 0.1f;
				break;
			case 597:
				switch (tile.frameX / 54)
				{
				case 0:
					list = 0.05f;
					index = 0.8f;
					B = 0.3f;
					break;
				case 1:
					list = 0.7f;
					index = 0.8f;
					B = 0.05f;
					break;
				case 2:
					list = 0.7f;
					index = 0.5f;
					B = 0.9f;
					break;
				case 3:
					list = 0.6f;
					index = 0.6f;
					B = 0.8f;
					break;
				case 4:
					list = 0.4f;
					index = 0.4f;
					B = 1.15f;
					break;
				case 5:
					list = 0.85f;
					index = 0.45f;
					B = 0.1f;
					break;
				case 6:
					list = 0.8f;
					index = 0.8f;
					B = 1f;
					break;
				case 7:
					list = 0.5f;
					index = 0.8f;
					B = 1.2f;
					break;
				}
				list *= 0.75f;
				index *= 0.75f;
				B *= 0.75f;
				break;
			case 564:
				if (tile.frameX < 36)
				{
					list = 0.05f;
					index = 0.3f;
					B = 0.55f;
				}
				break;
			case 568:
				list = 1f;
				index = 0.61f;
				B = 0.65f;
				break;
			case 569:
				list = 0.12f;
				index = 1f;
				B = 0.66f;
				break;
			case 570:
				list = 0.57f;
				index = 0.57f;
				B = 1f;
				break;
			case 580:
				list = 0.7f;
				index = 0.3f;
				B = 0.2f;
				break;
			case 391:
				list = 0.3f;
				index = 0.1f;
				B = 0.25f;
				break;
			case 381:
			case 517:
			case 687:
				list = 0.25f;
				index = 0.1f;
				B = 0f;
				break;
			case 534:
			case 535:
			case 689:
				list = 0f;
				index = 0.25f;
				B = 0f;
				break;
			case 536:
			case 537:
			case 690:
				list = 0f;
				index = 0.16f;
				B = 0.34f;
				break;
			case 539:
			case 540:
			case 688:
				list = 0.3f;
				index = 0f;
				B = 0.17f;
				break;
			case 625:
			case 626:
			case 691:
				list = 0.3f;
				index = 0f;
				B = 0.35f;
				break;
			case 627:
			case 628:
			case 692:
				list = (float)Main.DiscoR / 255f * 0.25f;
				index = (float)Main.DiscoG / 255f * 0.25f;
				B = (float)Main.DiscoB / 255f * 0.25f;
				break;
			case 184:
				if (tile.frameX == 110)
				{
					list = 0.25f;
					index = 0.1f;
					B = 0f;
				}
				if (tile.frameX == 132)
				{
					list = 0f;
					index = 0.25f;
					B = 0f;
				}
				if (tile.frameX == 154)
				{
					list = 0f;
					index = 0.16f;
					B = 0.34f;
				}
				if (tile.frameX == 176)
				{
					list = 0.3f;
					index = 0f;
					B = 0.17f;
				}
				if (tile.frameX == 198)
				{
					list = 0.3f;
					index = 0f;
					B = 0.35f;
				}
				if (tile.frameX == 220)
				{
					list = (float)Main.DiscoR / 255f * 0.25f;
					index = (float)Main.DiscoG / 255f * 0.25f;
					B = (float)Main.DiscoB / 255f * 0.25f;
				}
				break;
			case 370:
				list = 0.32f;
				index = 0.16f;
				B = 0.12f;
				break;
			case 659:
			case 667:
			{
				Vector4 shimmerBaseColor = LiquidRenderer.GetShimmerBaseColor(x, y);
				list = shimmerBaseColor.X;
				index = shimmerBaseColor.Y;
				B = shimmerBaseColor.Z;
				break;
			}
			case 27:
				if (tile.frameY < 36)
				{
					list = 0.3f;
					index = 0.27f;
				}
				break;
			case 336:
				list = 0.85f;
				index = 0.5f;
				B = 0.3f;
				break;
			case 340:
				list = 0.45f;
				index = 1f;
				B = 0.45f;
				break;
			case 341:
				list = 0.4f * Main.demonTorch + 0.6f * (1f - Main.demonTorch);
				index = 0.35f;
				B = 1f * Main.demonTorch + 0.6f * (1f - Main.demonTorch);
				break;
			case 342:
				list = 0.5f;
				index = 0.5f;
				B = 1.1f;
				break;
			case 343:
				list = 0.85f;
				index = 0.85f;
				B = 0.3f;
				break;
			case 344:
				list = 0.6f;
				index = 1.026f;
				B = 0.960000038f;
				break;
			case 327:
			{
				float num18 = 0.5f;
				num18 += (float)(270 - Main.mouseTextColor) / 1500f;
				num18 += (float)localRandom.Next(0, 50) * 0.0005f;
				list = 1f * num18;
				index = 0.5f * num18;
				B = 0.1f * num18;
				break;
			}
			case 316:
			case 317:
			case 318:
			{
				int num12 = x - tile.frameX / 18;
				int num13 = y - tile.frameY / 18;
				int num14 = num12 / 2 * (num13 / 3);
				num14 %= Main.cageFrames;
				bool flag4 = Main.jellyfishCageMode[tile.type - 316, num14] == 2;
				if (tile.type == 316)
				{
					if (flag4)
					{
						list = 0.2f;
						index = 0.3f;
						B = 0.8f;
					}
					else
					{
						list = 0.1f;
						index = 0.2f;
						B = 0.5f;
					}
				}
				if (tile.type == 317)
				{
					if (flag4)
					{
						list = 0.2f;
						index = 0.7f;
						B = 0.3f;
					}
					else
					{
						list = 0.05f;
						index = 0.45f;
						B = 0.1f;
					}
				}
				if (tile.type == 318)
				{
					if (flag4)
					{
						list = 0.7f;
						index = 0.2f;
						B = 0.5f;
					}
					else
					{
						list = 0.4f;
						index = 0.1f;
						B = 0.25f;
					}
				}
				break;
			}
			case 429:
			{
				int num6 = tile.frameX / 18;
				bool flag = num6 % 2 >= 1;
				bool flag2 = num6 % 4 >= 2;
				bool flag3 = num6 % 8 >= 4;
				bool num7 = num6 % 16 >= 8;
				if (flag)
				{
					list += 0.5f;
				}
				if (flag2)
				{
					index += 0.5f;
				}
				if (flag3)
				{
					B += 0.5f;
				}
				if (num7)
				{
					list += 0.2f;
					index += 0.2f;
				}
				break;
			}
			case 286:
			case 619:
				list = 0.1f;
				index = 0.2f;
				B = 0.7f;
				break;
			case 620:
			{
				Color color = new Color(230, 230, 230, 0).MultiplyRGBA(Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.5f % 1f, 1f, 0.5f));
				color *= 0.4f;
				list = (float)(int)color.R / 255f;
				index = (float)(int)color.G / 255f;
				B = (float)(int)color.B / 255f;
				break;
			}
			case 582:
			case 598:
				list = 0.7f;
				index = 0.2f;
				B = 0.1f;
				break;
			case 270:
				list = 0.73f;
				index = 1f;
				B = 0.41f;
				break;
			case 271:
				list = 0.45f;
				index = 0.95f;
				B = 1f;
				break;
			case 581:
				list = 1f;
				index = 0.75f;
				B = 0.5f;
				break;
			case 660:
				TorchID.TorchColor(23, out list, out index, out B);
				break;
			case 572:
				switch (tile.frameY / 36)
				{
				case 0:
					list = 0.9f;
					index = 0.5f;
					B = 0.7f;
					break;
				case 1:
					list = 0.7f;
					index = 0.55f;
					B = 0.96f;
					break;
				case 2:
					list = 0.45f;
					index = 0.96f;
					B = 0.95f;
					break;
				case 3:
					list = 0.5f;
					index = 0.96f;
					B = 0.62f;
					break;
				case 4:
					list = 0.47f;
					index = 0.69f;
					B = 0.95f;
					break;
				case 5:
					list = 0.92f;
					index = 0.57f;
					B = 0.51f;
					break;
				}
				break;
			case 262:
				list = 0.75f;
				B = 0.75f;
				break;
			case 263:
				list = 0.75f;
				index = 0.75f;
				break;
			case 264:
				B = 0.75f;
				break;
			case 265:
				index = 0.75f;
				break;
			case 266:
				list = 0.75f;
				break;
			case 267:
				list = 0.75f;
				index = 0.75f;
				B = 0.75f;
				break;
			case 268:
				list = 0.75f;
				index = 0.375f;
				break;
			case 237:
				list = 0.1f;
				index = 0.1f;
				break;
			case 238:
				if ((double)lightColor.X < 0.5)
				{
					lightColor.X = 0.5f;
				}
				if ((double)lightColor.Z < 0.5)
				{
					lightColor.Z = 0.5f;
				}
				break;
			case 235:
				if ((double)lightColor.X < 0.6)
				{
					lightColor.X = 0.6f;
				}
				if ((double)lightColor.Y < 0.6)
				{
					lightColor.Y = 0.6f;
				}
				break;
			case 405:
				if (tile.frameX < 54)
				{
					float num21 = (float)localRandom.Next(28, 42) * 0.005f;
					num21 += (float)(270 - Main.mouseTextColor) / 700f;
					switch (tile.frameX / 54)
					{
					case 1:
						list = 0.7f;
						index = 1f;
						B = 0.5f;
						break;
					case 2:
						list = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
						index = 0.3f;
						B = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
						break;
					case 3:
						list = 0.45f;
						index = 0.75f;
						B = 1f;
						break;
					case 4:
						list = 1.15f;
						index = 1.15f;
						B = 0.5f;
						break;
					case 5:
						list = (float)Main.DiscoR / 255f;
						index = (float)Main.DiscoG / 255f;
						B = (float)Main.DiscoB / 255f;
						break;
					default:
						list = 0.9f;
						index = 0.3f;
						B = 0.1f;
						break;
					}
					list += num21;
					index += num21;
					B += num21;
				}
				break;
			case 215:
				if (tile.frameY < 36)
				{
					float num20 = (float)localRandom.Next(28, 42) * 0.005f;
					num20 += (float)(270 - Main.mouseTextColor) / 700f;
					switch (tile.frameX / 54)
					{
					case 1:
						list = 0.7f;
						index = 1f;
						B = 0.5f;
						break;
					case 2:
						list = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
						index = 0.3f;
						B = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
						break;
					case 3:
						list = 0.45f;
						index = 0.75f;
						B = 1f;
						break;
					case 4:
						list = 1.15f;
						index = 1.15f;
						B = 0.5f;
						break;
					case 5:
						list = (float)Main.DiscoR / 255f;
						index = (float)Main.DiscoG / 255f;
						B = (float)Main.DiscoB / 255f;
						break;
					case 6:
						list = 0.75f;
						index = 1.28249991f;
						B = 1.2f;
						break;
					case 7:
						list = 0.95f;
						index = 0.65f;
						B = 1.3f;
						break;
					case 8:
						list = 1.4f;
						index = 0.85f;
						B = 0.55f;
						break;
					case 9:
						list = 0.25f;
						index = 1.3f;
						B = 0.8f;
						break;
					case 10:
						list = 0.95f;
						index = 0.4f;
						B = 1.4f;
						break;
					case 11:
						list = 1.4f;
						index = 0.7f;
						B = 0.5f;
						break;
					case 12:
						list = 1.25f;
						index = 0.6f;
						B = 1.2f;
						break;
					case 13:
						list = 0.75f;
						index = 1.45f;
						B = 0.9f;
						break;
					case 14:
						list = 0.25f;
						index = 0.65f;
						B = 1f;
						break;
					case 15:
						TorchID.TorchColor(23, out list, out index, out B);
						break;
					default:
						list = 0.9f;
						index = 0.3f;
						B = 0.1f;
						break;
					}
					list += num20;
					index += num20;
					B += num20;
				}
				break;
			case 92:
				if (tile.frameY <= 18 && tile.frameX == 0)
				{
					list = 1f;
					index = 1f;
					B = 1f;
				}
				break;
			case 592:
				if (tile.frameY > 0)
				{
					float num19 = (float)localRandom.Next(28, 42) * 0.005f;
					num19 += (float)(270 - Main.mouseTextColor) / 700f;
					list = 1.35f;
					index = 0.45f;
					B = 0.15f;
					list += num19;
					index += num19;
					B += num19;
				}
				break;
			case 593:
				if (tile.frameX < 18)
				{
					list = 0.8f;
					index = 0.3f;
					B = 0.1f;
				}
				break;
			case 594:
				if (tile.frameX < 36)
				{
					list = 0.8f;
					index = 0.3f;
					B = 0.1f;
				}
				break;
			case 548:
				if (tile.frameX / 54 >= 7)
				{
					list = 0.7f;
					index = 0.3f;
					B = 0.2f;
				}
				break;
			case 613:
			case 614:
				list = 0.7f;
				index = 0.3f;
				B = 0.2f;
				break;
			case 93:
				if (tile.frameX == 0)
				{
					switch (tile.frameY / 54)
					{
					case 1:
						list = 0.95f;
						index = 0.95f;
						B = 0.5f;
						break;
					case 2:
						list = 0.85f;
						index = 0.6f;
						B = 1f;
						break;
					case 3:
						list = 0.75f;
						index = 1f;
						B = 0.6f;
						break;
					case 4:
					case 5:
						list = 0.75f;
						index = 0.85f;
						B = 1f;
						break;
					case 6:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 7:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 9:
						list = 1f;
						index = 1f;
						B = 0.7f;
						break;
					case 10:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 12:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 13:
						list = 1f;
						index = 1f;
						B = 0.6f;
						break;
					case 14:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 18:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 19:
						list = 0.37f;
						index = 0.8f;
						B = 1f;
						break;
					case 20:
						list = 0f;
						index = 0.9f;
						B = 1f;
						break;
					case 21:
						list = 0.25f;
						index = 0.7f;
						B = 1f;
						break;
					case 23:
						list = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
						index = 0.3f;
						B = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
						break;
					case 24:
						list = 0.35f;
						index = 0.5f;
						B = 0.3f;
						break;
					case 25:
						list = 0.34f;
						index = 0.4f;
						B = 0.31f;
						break;
					case 26:
						list = 0.25f;
						index = 0.32f;
						B = 0.5f;
						break;
					case 29:
						list = 0.9f;
						index = 0.75f;
						B = 1f;
						break;
					case 30:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 31:
					{
						Vector3 vector5 = Main.hslToRgb(Main.demonTorch * 0.12f + 0.69f, 1f, 0.75f).ToVector3() * 1.2f;
						list = vector5.X;
						index = vector5.Y;
						B = vector5.Z;
						break;
					}
					case 32:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 33:
						list = 0.55f;
						index = 0.45f;
						B = 0.95f;
						break;
					case 34:
						list = 1f;
						index = 0.6f;
						B = 0.1f;
						break;
					case 35:
						list = 0.3f;
						index = 0.75f;
						B = 0.55f;
						break;
					case 36:
						list = 0.9f;
						index = 0.55f;
						B = 0.7f;
						break;
					case 37:
						list = 0.55f;
						index = 0.85f;
						B = 1f;
						break;
					case 38:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 39:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 40:
						list = 0.4f;
						index = 0.8f;
						B = 0.9f;
						break;
					case 41:
						list = 1f;
						index = 1f;
						B = 1f;
						break;
					case 42:
						list = 0.95f;
						index = 0.5f;
						B = 0.4f;
						break;
					default:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					}
				}
				break;
			case 96:
				if (tile.frameX >= 36)
				{
					list = 0.5f;
					index = 0.35f;
					B = 0.1f;
				}
				break;
			case 98:
				if (tile.frameY == 0)
				{
					list = 1f;
					index = 0.97f;
					B = 0.85f;
				}
				break;
			case 4:
				if (tile.frameX < 66)
				{
					TorchID.TorchColor(tile.frameY / 22, out list, out index, out B);
				}
				break;
			case 372:
				if (tile.frameX == 0)
				{
					list = 0.9f;
					index = 0.1f;
					B = 0.75f;
				}
				break;
			case 646:
				if (tile.frameX == 0)
				{
					list = 0.2f;
					index = 0.3f;
					B = 0.32f;
				}
				break;
			case 33:
				if (tile.frameX == 0)
				{
					switch (tile.frameY / 22)
					{
					case 0:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 1:
						list = 0.55f;
						index = 0.85f;
						B = 0.35f;
						break;
					case 2:
						list = 0.65f;
						index = 0.95f;
						B = 0.5f;
						break;
					case 3:
						list = 0.2f;
						index = 0.75f;
						B = 1f;
						break;
					case 5:
						list = 0.85f;
						index = 0.6f;
						B = 1f;
						break;
					case 7:
					case 8:
						list = 0.75f;
						index = 0.85f;
						B = 1f;
						break;
					case 9:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 10:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 14:
						list = 1f;
						index = 1f;
						B = 0.6f;
						break;
					case 15:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 18:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 19:
						list = 0.37f;
						index = 0.8f;
						B = 1f;
						break;
					case 20:
						list = 0f;
						index = 0.9f;
						B = 1f;
						break;
					case 21:
						list = 0.25f;
						index = 0.7f;
						B = 1f;
						break;
					case 23:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 24:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 25:
						list = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
						index = 0.3f;
						B = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
						break;
					case 28:
						list = 0.9f;
						index = 0.75f;
						B = 1f;
						break;
					case 29:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 30:
					{
						Vector3 vector4 = Main.hslToRgb(Main.demonTorch * 0.12f + 0.69f, 1f, 0.75f).ToVector3() * 1.2f;
						list = vector4.X;
						index = vector4.Y;
						B = vector4.Z;
						break;
					}
					case 31:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 32:
						list = 0.55f;
						index = 0.45f;
						B = 0.95f;
						break;
					case 33:
						list = 1f;
						index = 0.6f;
						B = 0.1f;
						break;
					case 34:
						list = 0.3f;
						index = 0.75f;
						B = 0.55f;
						break;
					case 35:
						list = 0.9f;
						index = 0.55f;
						B = 0.7f;
						break;
					case 36:
						list = 0.55f;
						index = 0.85f;
						B = 1f;
						break;
					case 37:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 38:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 39:
						list = 0.4f;
						index = 0.8f;
						B = 0.9f;
						break;
					case 40:
						list = 1f;
						index = 1f;
						B = 1f;
						break;
					case 41:
						list = 0.95f;
						index = 0.5f;
						B = 0.4f;
						break;
					default:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					}
				}
				break;
			case 174:
				if (tile.frameX == 0)
				{
					list = 1f;
					index = 0.95f;
					B = 0.65f;
				}
				break;
			case 100:
			case 173:
				if (tile.frameX < 36)
				{
					switch (tile.frameY / 36)
					{
					case 1:
						list = 0.95f;
						index = 0.95f;
						B = 0.5f;
						break;
					case 2:
						list = 0.85f;
						index = 0.6f;
						B = 1f;
						break;
					case 3:
						list = 1f;
						index = 0.6f;
						B = 0.6f;
						break;
					case 5:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 6:
					case 7:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 8:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 9:
						list = 0.75f;
						index = 0.85f;
						B = 1f;
						break;
					case 11:
						list = 1f;
						index = 1f;
						B = 0.7f;
						break;
					case 12:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 13:
						list = 1f;
						index = 1f;
						B = 0.6f;
						break;
					case 14:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 18:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 19:
						list = 0.37f;
						index = 0.8f;
						B = 1f;
						break;
					case 20:
						list = 0f;
						index = 0.9f;
						B = 1f;
						break;
					case 21:
						list = 0.25f;
						index = 0.7f;
						B = 1f;
						break;
					case 25:
						list = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
						index = 0.3f;
						B = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
						break;
					case 22:
						list = 0.35f;
						index = 0.5f;
						B = 0.3f;
						break;
					case 23:
						list = 0.34f;
						index = 0.4f;
						B = 0.31f;
						break;
					case 24:
						list = 0.25f;
						index = 0.32f;
						B = 0.5f;
						break;
					case 29:
						list = 0.9f;
						index = 0.75f;
						B = 1f;
						break;
					case 30:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 31:
					{
						Vector3 vector3 = Main.hslToRgb(Main.demonTorch * 0.12f + 0.69f, 1f, 0.75f).ToVector3() * 1.2f;
						list = vector3.X;
						index = vector3.Y;
						B = vector3.Z;
						break;
					}
					case 32:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 33:
						list = 0.55f;
						index = 0.45f;
						B = 0.95f;
						break;
					case 34:
						list = 1f;
						index = 0.6f;
						B = 0.1f;
						break;
					case 35:
						list = 0.3f;
						index = 0.75f;
						B = 0.55f;
						break;
					case 36:
						list = 0.9f;
						index = 0.55f;
						B = 0.7f;
						break;
					case 37:
						list = 0.55f;
						index = 0.85f;
						B = 1f;
						break;
					case 38:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 39:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 40:
						list = 0.4f;
						index = 0.8f;
						B = 0.9f;
						break;
					case 41:
						list = 1f;
						index = 1f;
						B = 1f;
						break;
					case 42:
						list = 0.95f;
						index = 0.5f;
						B = 0.4f;
						break;
					default:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					}
				}
				break;
			case 34:
				if (tile.frameX % 108 < 54)
				{
					int num17 = tile.frameY / 54;
					switch (num17 + 37 * (tile.frameX / 108))
					{
					case 7:
						list = 0.95f;
						index = 0.95f;
						B = 0.5f;
						break;
					case 8:
						list = 0.85f;
						index = 0.6f;
						B = 1f;
						break;
					case 9:
						list = 1f;
						index = 0.6f;
						B = 0.6f;
						break;
					case 12:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 13:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 11:
					case 17:
						list = 0.75f;
						index = 0.85f;
						B = 1f;
						break;
					case 15:
						list = 1f;
						index = 1f;
						B = 0.7f;
						break;
					case 16:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 18:
						list = 1f;
						index = 1f;
						B = 0.6f;
						break;
					case 19:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 23:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 24:
						list = 0.37f;
						index = 0.8f;
						B = 1f;
						break;
					case 25:
						list = 0f;
						index = 0.9f;
						B = 1f;
						break;
					case 26:
						list = 0.25f;
						index = 0.7f;
						B = 1f;
						break;
					case 27:
						list = 0.55f;
						index = 0.85f;
						B = 0.35f;
						break;
					case 28:
						list = 0.65f;
						index = 0.95f;
						B = 0.5f;
						break;
					case 29:
						list = 0.2f;
						index = 0.75f;
						B = 1f;
						break;
					case 30:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 32:
						list = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
						index = 0.3f;
						B = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
						break;
					case 35:
						list = 0.9f;
						index = 0.75f;
						B = 1f;
						break;
					case 36:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 37:
					{
						Vector3 vector2 = Main.hslToRgb(Main.demonTorch * 0.12f + 0.69f, 1f, 0.75f).ToVector3() * 1.2f;
						list = vector2.X;
						index = vector2.Y;
						B = vector2.Z;
						break;
					}
					case 38:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 39:
						list = 0.55f;
						index = 0.45f;
						B = 0.95f;
						break;
					case 40:
						list = 1f;
						index = 0.6f;
						B = 0.1f;
						break;
					case 41:
						list = 0.3f;
						index = 0.75f;
						B = 0.55f;
						break;
					case 42:
						list = 0.9f;
						index = 0.55f;
						B = 0.7f;
						break;
					case 43:
						list = 0.55f;
						index = 0.85f;
						B = 1f;
						break;
					case 44:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 45:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 46:
						list = 0.4f;
						index = 0.8f;
						B = 0.9f;
						break;
					case 47:
						list = 1f;
						index = 1f;
						B = 1f;
						break;
					case 48:
						list = 0.95f;
						index = 0.5f;
						B = 0.4f;
						break;
					default:
						list = 1f;
						index = 0.95f;
						B = 0.8f;
						break;
					}
				}
				break;
			case 35:
				if (tile.frameX < 36)
				{
					list = 0.75f;
					index = 0.6f;
					B = 0.3f;
				}
				break;
			case 95:
				if (tile.frameX < 36)
				{
					list = 1f;
					index = 0.95f;
					B = 0.8f;
				}
				break;
			case 17:
			case 133:
			case 302:
				list = 0.83f;
				index = 0.6f;
				B = 0.5f;
				break;
			case 77:
				list = 0.75f;
				index = 0.45f;
				B = 0.25f;
				break;
			case 37:
				list = 0.56f;
				index = 0.43f;
				B = 0.15f;
				break;
			case 22:
			case 140:
				list = 0.12f;
				index = 0.07f;
				B = 0.32f;
				break;
			case 171:
				if (tile.frameX < 10)
				{
					x -= tile.frameX;
					y -= tile.frameY;
				}
				switch ((Main.tile[x, y].frameY & 0x3C00) >> 10)
				{
				case 1:
					list = 0.1f;
					index = 0.1f;
					B = 0.1f;
					break;
				case 2:
					list = 0.2f;
					break;
				case 3:
					index = 0.2f;
					break;
				case 4:
					B = 0.2f;
					break;
				case 5:
					list = 0.125f;
					index = 0.125f;
					break;
				case 6:
					list = 0.2f;
					index = 0.1f;
					break;
				case 7:
					list = 0.125f;
					index = 0.125f;
					break;
				case 8:
					list = 0.08f;
					index = 0.175f;
					break;
				case 9:
					index = 0.125f;
					B = 0.125f;
					break;
				case 10:
					list = 0.125f;
					B = 0.125f;
					break;
				case 11:
					list = 0.1f;
					index = 0.1f;
					B = 0.2f;
					break;
				default:
					list = (index = (B = 0f));
					break;
				}
				list *= 0.5f;
				index *= 0.5f;
				B *= 0.5f;
				break;
			case 204:
			case 347:
				list = 0.35f;
				break;
			case 42:
				if (tile.frameX == 0)
				{
					switch (tile.frameY / 36)
					{
					case 0:
						list = 0.7f;
						index = 0.65f;
						B = 0.55f;
						break;
					case 1:
						list = 0.9f;
						index = 0.75f;
						B = 0.6f;
						break;
					case 2:
						list = 0.8f;
						index = 0.6f;
						B = 0.6f;
						break;
					case 3:
						list = 0.65f;
						index = 0.5f;
						B = 0.2f;
						break;
					case 4:
						list = 0.5f;
						index = 0.7f;
						B = 0.4f;
						break;
					case 5:
						list = 0.9f;
						index = 0.4f;
						B = 0.2f;
						break;
					case 6:
						list = 0.7f;
						index = 0.75f;
						B = 0.3f;
						break;
					case 7:
					{
						float num16 = Main.demonTorch * 0.2f;
						list = 0.9f - num16;
						index = 0.9f - num16;
						B = 0.7f + num16;
						break;
					}
					case 8:
						list = 0.75f;
						index = 0.6f;
						B = 0.3f;
						break;
					case 9:
						list = 1f;
						index = 0.3f;
						B = 0.5f;
						B += Main.demonTorch * 0.2f;
						list -= Main.demonTorch * 0.1f;
						index -= Main.demonTorch * 0.2f;
						break;
					case 11:
						list = 0.85f;
						index = 0.6f;
						B = 1f;
						break;
					case 14:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 15:
					case 16:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 17:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 18:
						list = 0.75f;
						index = 0.85f;
						B = 1f;
						break;
					case 21:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 22:
						list = 1f;
						index = 1f;
						B = 0.6f;
						break;
					case 23:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 27:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 28:
						list = 0.37f;
						index = 0.8f;
						B = 1f;
						break;
					case 29:
						list = 0f;
						index = 0.9f;
						B = 1f;
						break;
					case 30:
						list = 0.25f;
						index = 0.7f;
						B = 1f;
						break;
					case 32:
						list = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
						index = 0.3f;
						B = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
						break;
					case 35:
						list = 0.7f;
						index = 0.6f;
						B = 0.9f;
						break;
					case 36:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 37:
					{
						Vector3 vector = Main.hslToRgb(Main.demonTorch * 0.12f + 0.69f, 1f, 0.75f).ToVector3() * 1.2f;
						list = vector.X;
						index = vector.Y;
						B = vector.Z;
						break;
					}
					case 38:
						list = 1f;
						index = 0.97f;
						B = 0.85f;
						break;
					case 39:
						list = 0.55f;
						index = 0.45f;
						B = 0.95f;
						break;
					case 40:
						list = 1f;
						index = 0.6f;
						B = 0.1f;
						break;
					case 41:
						list = 0.3f;
						index = 0.75f;
						B = 0.55f;
						break;
					case 42:
						list = 0.9f;
						index = 0.55f;
						B = 0.7f;
						break;
					case 43:
						list = 0.55f;
						index = 0.85f;
						B = 1f;
						break;
					case 44:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 45:
						list = 1f;
						index = 0.95f;
						B = 0.65f;
						break;
					case 46:
						list = 0.4f;
						index = 0.8f;
						B = 0.9f;
						break;
					case 47:
						list = 1f;
						index = 1f;
						B = 1f;
						break;
					case 48:
						list = 0.95f;
						index = 0.5f;
						B = 0.4f;
						break;
					default:
						list = 1f;
						index = 1f;
						B = 1f;
						break;
					}
				}
				break;
			case 49:
				if (tile.frameX == 0)
				{
					list = 0f;
					index = 0.35f;
					B = 0.8f;
				}
				break;
			case 519:
				if (tile.frameY == 90)
				{
					if (tile.color() == 0)
					{
						float num15 = (float)localRandom.Next(28, 42) * 0.005f;
						num15 += (float)(270 - Main.mouseTextColor) / 1000f;
						list = 0.1f;
						index = 0.2f + num15 / 2f;
						B = 0.7f + num15;
					}
					else
					{
						Color color3 = WorldGen.paintColor(tile.color());
						list = (float)(int)color3.R / 255f;
						index = (float)(int)color3.G / 255f;
						B = (float)(int)color3.B / 255f;
					}
				}
				break;
			case 70:
			case 71:
			case 72:
			case 190:
			case 348:
			case 349:
			case 528:
			case 578:
				if (tile.type != 349 || tile.frameX >= 36)
				{
					float num11 = (float)localRandom.Next(28, 42) * 0.005f;
					num11 += (float)(270 - Main.mouseTextColor) / 1000f;
					if (tile.color() == 0)
					{
						list = 0f;
						index = 0.2f + num11 / 2f;
						B = 1f;
					}
					else
					{
						Color color2 = WorldGen.paintColor(tile.color());
						list = (float)(int)color2.R / 255f;
						index = (float)(int)color2.G / 255f;
						B = (float)(int)color2.B / 255f;
					}
				}
				break;
			case 350:
			{
				double num10 = Main.timeForVisualEffects * 0.08;
				B = (index = (list = (float)((0.0 - Math.Cos(((int)(num10 / 6.283) % 3 == 1) ? num10 : 0.0)) * 0.1 + 0.1)));
				break;
			}
			case 61:
				if (tile.frameX == 144)
				{
					float num8 = 1f + (float)(270 - Main.mouseTextColor) / 400f;
					float num9 = 0.8f - (float)(270 - Main.mouseTextColor) / 400f;
					list = 0.42f * num9;
					index = 0.81f * num8;
					B = 0.52f * num9;
				}
				break;
			case 26:
			case 31:
				if ((tile.type == 31 && tile.frameX >= 36) || (tile.type == 26 && tile.frameX >= 54))
				{
					float num4 = (float)localRandom.Next(-5, 6) * 0.0025f;
					list = 0.5f + num4 * 2f;
					index = 0.2f + num4;
					B = 0.1f;
				}
				else
				{
					float num5 = (float)localRandom.Next(-5, 6) * 0.0025f;
					list = 0.31f + num5;
					index = 0.1f;
					B = 0.44f + num5 * 2f;
				}
				break;
			case 84:
			{
				int num2 = tile.frameX / 18;
				float num3 = 0f;
				switch (num2)
				{
				case 2:
					num3 = (float)(270 - Main.mouseTextColor) / 800f;
					if (num3 > 1f)
					{
						num3 = 1f;
					}
					else if (num3 < 0f)
					{
						num3 = 0f;
					}
					list = num3 * 0.7f;
					index = num3;
					B = num3 * 0.1f;
					break;
				case 5:
					num3 = 0.9f;
					list = num3;
					index = num3 * 0.8f;
					B = num3 * 0.2f;
					break;
				case 6:
					num3 = 0.08f;
					index = num3 * 0.8f;
					B = num3;
					break;
				}
				break;
			}
			case 83:
				if (tile.frameX == 18 && !Main.dayTime)
				{
					list = 0.1f;
					index = 0.4f;
					B = 0.6f;
				}
				if (tile.frameX == 90 && !Main.raining && Main.time > 40500.0)
				{
					list = 0.9f;
					index = 0.72f;
					B = 0.18f;
				}
				break;
			case 126:
				if (tile.frameX < 36)
				{
					list = (float)Main.DiscoR / 255f;
					index = (float)Main.DiscoG / 255f;
					B = (float)Main.DiscoB / 255f;
				}
				break;
			case 125:
			{
				float num = (float)localRandom.Next(28, 42) * 0.01f;
				num += (float)(270 - Main.mouseTextColor) / 800f;
				index = (lightColor.Y = 0.3f * num);
				B = (lightColor.Z = 0.6f * num);
				break;
			}
			case 129:
				switch (tile.frameX / 18 % 3)
				{
				case 0:
					list = 0f;
					index = 0.05f;
					B = 0.25f;
					break;
				case 1:
					list = 0.2f;
					index = 0f;
					B = 0.15f;
					break;
				case 2:
					list = 0.1f;
					index = 0f;
					B = 0.2f;
					break;
				}
				break;
			case 149:
				if (tile.frameX <= 36)
				{
					switch (tile.frameX / 18)
					{
					case 0:
						list = 0.1f;
						index = 0.2f;
						B = 0.5f;
						break;
					case 1:
						list = 0.5f;
						index = 0.1f;
						B = 0.1f;
						break;
					case 2:
						list = 0.2f;
						index = 0.5f;
						B = 0.1f;
						break;
					}
					list *= (float)localRandom.Next(970, 1031) * 0.001f;
					index *= (float)localRandom.Next(970, 1031) * 0.001f;
					B *= (float)localRandom.Next(970, 1031) * 0.001f;
				}
				break;
			case 160:
				list = (float)Main.DiscoR / 255f * 0.25f;
				index = (float)Main.DiscoG / 255f * 0.25f;
				B = (float)Main.DiscoB / 255f * 0.25f;
				break;
			case 354:
				list = 0.65f;
				index = 0.35f;
				B = 0.15f;
				break;
			}
		}
		if (lightColor.X < list)
		{
			lightColor.X = list;
		}
		if (lightColor.Y < index)
		{
			lightColor.Y = index;
		}
		if (lightColor.Z < B)
		{
			lightColor.Z = B;
		}
	}

	private void ApplySurfaceLight(Tile tile, int x, int y, ref Vector3 lightColor)
	{
		float finalFractalProfile = 0f;
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)(int)Main.tileColor.R / 255f;
		float num4 = (float)(int)Main.tileColor.G / 255f;
		float num5 = (float)(int)Main.tileColor.B / 255f;
		float num6 = (num3 + num4 + num5) / 3f;
		if (tile.active() && TileID.Sets.AllowLightInWater[tile.type])
		{
			if (lightColor.X < num6 && (Main.wallLight[tile.wall] || tile.wall == 73 || tile.wall == 227 || (tile.invisibleWall() && !_drawInvisibleWalls)))
			{
				finalFractalProfile = num3;
				num = num4;
				num2 = num5;
			}
		}
		else if ((!tile.active() || !Main.tileNoSunLight[tile.type] || ((tile.slope() != 0 || tile.halfBrick() || (tile.invisibleBlock() && !_drawInvisibleWalls)) && Main.tile[x, y - 1].liquid == 0 && Main.tile[x, y + 1].liquid == 0 && Main.tile[x - 1, y].liquid == 0 && Main.tile[x + 1, y].liquid == 0)) && lightColor.X < num6 && (Main.wallLight[tile.wall] || tile.wall == 73 || tile.wall == 227 || (tile.invisibleWall() && !_drawInvisibleWalls)))
		{
			if (tile.liquid < 200)
			{
				if (!tile.halfBrick() || Main.tile[x, y - 1].liquid < 200)
				{
					finalFractalProfile = num3;
					num = num4;
					num2 = num5;
				}
			}
			else if (Main.liquidAlpha[13] > 0f)
			{
				if (Main.rand == null)
				{
					Main.rand = new UnifiedRandom();
				}
				num2 = num5 * 0.175f * (1f + Main.rand.NextFloat() * 0.13f) * Main.liquidAlpha[13];
			}
		}
		if ((!tile.active() || tile.halfBrick() || !Main.tileNoSunLight[tile.type]) && ((tile.wall >= 88 && tile.wall <= 93) || tile.wall == 241) && tile.liquid < byte.MaxValue)
		{
			finalFractalProfile = num3;
			num = num4;
			num2 = num5;
			int num7 = tile.wall - 88;
			if (tile.wall == 241)
			{
				num7 = 6;
			}
			switch (num7)
			{
			case 0:
				finalFractalProfile *= 0.9f;
				num *= 0.15f;
				num2 *= 0.9f;
				break;
			case 1:
				finalFractalProfile *= 0.9f;
				num *= 0.9f;
				num2 *= 0.15f;
				break;
			case 2:
				finalFractalProfile *= 0.15f;
				num *= 0.15f;
				num2 *= 0.9f;
				break;
			case 3:
				finalFractalProfile *= 0.15f;
				num *= 0.9f;
				num2 *= 0.15f;
				break;
			case 4:
				finalFractalProfile *= 0.9f;
				num *= 0.15f;
				num2 *= 0.15f;
				break;
			case 5:
			{
				float num8 = 0.2f;
				float num9 = 0.7f - num8;
				finalFractalProfile *= num9 + (float)Main.DiscoR / 255f * num8;
				num *= num9 + (float)Main.DiscoG / 255f * num8;
				num2 *= num9 + (float)Main.DiscoB / 255f * num8;
				break;
			}
			case 6:
				finalFractalProfile *= 0.9f;
				num *= 0.5f;
				num2 *= 0f;
				break;
			}
		}
		float num10 = 1f - Main.shimmerDarken;
		finalFractalProfile *= num10;
		num *= num10;
		num2 *= num10;
		if (lightColor.X < finalFractalProfile)
		{
			lightColor.X = finalFractalProfile;
		}
		if (lightColor.Y < num)
		{
			lightColor.Y = num;
		}
		if (lightColor.Z < num2)
		{
			lightColor.Z = num2;
		}
	}

	private void ApplyHellLight(Tile tile, int x, int y, ref Vector3 lightColor)
	{
		float value = 0f;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.08f;
		if ((!tile.active() || !Main.tileNoSunLight[tile.type] || ((tile.slope() != 0 || tile.halfBrick()) && Main.tile[x, y - 1].liquid == 0 && Main.tile[x, y + 1].liquid == 0 && Main.tile[x - 1, y].liquid == 0 && Main.tile[x + 1, y].liquid == 0)) && lightColor.X < num3 && (Main.wallLight[tile.wall] || tile.wall == 73 || tile.wall == 227 || (tile.invisibleWall() && !_drawInvisibleWalls)) && tile.liquid < 200 && (!tile.halfBrick() || Main.tile[x, y - 1].liquid < 200))
		{
			value = num3;
			num = num3 * 0.6f;
			num2 = num3 * 0.2f;
		}
		if ((!tile.active() || tile.halfBrick() || !Main.tileNoSunLight[tile.type]) && tile.wall >= 88 && tile.wall <= 93 && tile.liquid < byte.MaxValue)
		{
			value = num3;
			num = num3 * 0.6f;
			num2 = num3 * 0.2f;
			switch (tile.wall)
			{
			case 88:
				value *= 0.9f;
				num *= 0.15f;
				num2 *= 0.9f;
				break;
			case 89:
				value *= 0.9f;
				num *= 0.9f;
				num2 *= 0.15f;
				break;
			case 90:
				value *= 0.15f;
				num *= 0.15f;
				num2 *= 0.9f;
				break;
			case 91:
				value *= 0.15f;
				num *= 0.9f;
				num2 *= 0.15f;
				break;
			case 92:
				value *= 0.9f;
				num *= 0.15f;
				num2 *= 0.15f;
				break;
			case 93:
			{
				float num4 = 0.2f;
				float num5 = 0.7f - num4;
				value *= num5 + (float)Main.DiscoR / 255f * num4;
				num *= num5 + (float)Main.DiscoG / 255f * num4;
				num2 *= num5 + (float)Main.DiscoB / 255f * num4;
				break;
			}
			}
		}
		if (lightColor.X < value)
		{
			lightColor.X = value;
		}
		if (lightColor.Y < num)
		{
			lightColor.Y = num;
		}
		if (lightColor.Z < num2)
		{
			lightColor.Z = num2;
		}
	}
}
