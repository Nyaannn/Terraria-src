using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.IO;

namespace Terraria;

public class WaterfallManager
{
	public struct WaterfallData
	{
		public int x;

		public int y;

		public int type;

		public int stopAtStep;
	}

	private const int minWet = 160;

	private const int maxWaterfallCountDefault = 1000;

	private const int maxLength = 100;

	private const int maxTypes = 26;

	public int maxWaterfallCount = 1000;

	private int qualityMax;

	private int currentMax;

	private WaterfallData[] waterfalls = new WaterfallData[1000];

	private Asset<Texture2D>[] waterfallTexture = new Asset<Texture2D>[26];

	private int wFallFrCounter;

	private int regularFrame;

	private int wFallFrCounter2;

	private int slowFrame;

	private int rainFrameCounter;

	private int rainFrameForeground;

	private int rainFrameBackground;

	private int snowFrameCounter;

	private int snowFrameForeground;

	private int findWaterfallCount;

	private int waterfallDist = 100;

	public void BindTo(Preferences preferences)
	{
		preferences.OnLoad += Configuration_OnLoad;
	}

	private void Configuration_OnLoad(Preferences preferences)
	{
		maxWaterfallCount = Math.Max(0, preferences.Get("WaterfallDrawLimit", 1000));
		waterfalls = new WaterfallData[maxWaterfallCount];
	}

	public void LoadContent()
	{
		for (int i = 0; i < 26; i++)
		{
			waterfallTexture[i] = Main.Assets.Request<Texture2D>("Images/Waterfall_" + i, (AssetRequestMode)2);
		}
	}

	public bool CheckForWaterfall(int i, int j)
	{
		for (int k = 0; k < currentMax; k++)
		{
			if (waterfalls[k].x == i && waterfalls[k].y == j)
			{
				return true;
			}
		}
		return false;
	}

	public void FindWaterfalls(bool forced = false)
	{
		findWaterfallCount++;
		if (findWaterfallCount < 30 && !forced)
		{
			return;
		}
		findWaterfallCount = 0;
		waterfallDist = (int)(75f * Main.gfxQuality) + 25;
		qualityMax = (int)((float)maxWaterfallCount * Main.gfxQuality);
		currentMax = 0;
		int i = (int)(Main.screenPosition.X / 16f - 1f);
		int num = (int)((Main.screenPosition.X + (float)Main.screenWidth) / 16f) + 2;
		int num2 = (int)(Main.screenPosition.Y / 16f - 1f);
		int num3 = (int)((Main.screenPosition.Y + (float)Main.screenHeight) / 16f) + 2;
		i -= waterfallDist;
		num += waterfallDist;
		num2 -= waterfallDist;
		num3 += 20;
		if (i < 0)
		{
			i = 0;
		}
		if (num > Main.maxTilesX)
		{
			num = Main.maxTilesX;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num3 > Main.maxTilesY)
		{
			num3 = Main.maxTilesY;
		}
		for (int j = i; j < num; j++)
		{
			for (int k = num2; k < num3; k++)
			{
				Tile tile = Main.tile[j, k];
				if (tile == null)
				{
					tile = new Tile();
					Main.tile[j, k] = tile;
				}
				if (!tile.active())
				{
					continue;
				}
				if (tile.halfBrick())
				{
					Tile tile2 = Main.tile[j, k - 1];
					if (tile2 == null)
					{
						tile2 = new Tile();
						Main.tile[j, k - 1] = tile2;
					}
					if (tile2.liquid < 16 || WorldGen.SolidTile(tile2))
					{
						Tile tile3 = Main.tile[j - 1, k];
						if (tile3 == null)
						{
							tile3 = new Tile();
							Main.tile[j - 1, k] = tile3;
						}
						Tile tile4 = Main.tile[j + 1, k];
						if (tile4 == null)
						{
							tile4 = new Tile();
							Main.tile[j + 1, k] = tile4;
						}
						if ((tile3.liquid > 160 || tile4.liquid > 160) && ((tile3.liquid == 0 && !WorldGen.SolidTile(tile3) && tile3.slope() == 0) || (tile4.liquid == 0 && !WorldGen.SolidTile(tile4) && tile4.slope() == 0)) && currentMax < qualityMax)
						{
							waterfalls[currentMax].type = 0;
							if (tile2.lava() || tile4.lava() || tile3.lava())
							{
								waterfalls[currentMax].type = 1;
							}
							else if (tile2.honey() || tile4.honey() || tile3.honey())
							{
								waterfalls[currentMax].type = 14;
							}
							else if (tile2.shimmer() || tile4.shimmer() || tile3.shimmer())
							{
								waterfalls[currentMax].type = 25;
							}
							else
							{
								waterfalls[currentMax].type = 0;
							}
							waterfalls[currentMax].x = j;
							waterfalls[currentMax].y = k;
							currentMax++;
						}
					}
				}
				if (tile.type == 196)
				{
					Tile tile5 = Main.tile[j, k + 1];
					if (tile5 == null)
					{
						tile5 = new Tile();
						Main.tile[j, k + 1] = tile5;
					}
					if (!WorldGen.SolidTile(tile5) && tile5.slope() == 0 && currentMax < qualityMax)
					{
						waterfalls[currentMax].type = 11;
						waterfalls[currentMax].x = j;
						waterfalls[currentMax].y = k + 1;
						currentMax++;
					}
				}
				if (tile.type == 460)
				{
					Tile tile6 = Main.tile[j, k + 1];
					if (tile6 == null)
					{
						tile6 = new Tile();
						Main.tile[j, k + 1] = tile6;
					}
					if (!WorldGen.SolidTile(tile6) && tile6.slope() == 0 && currentMax < qualityMax)
					{
						waterfalls[currentMax].type = 22;
						waterfalls[currentMax].x = j;
						waterfalls[currentMax].y = k + 1;
						currentMax++;
					}
				}
			}
		}
	}

	public void UpdateFrame()
	{
		wFallFrCounter++;
		if (wFallFrCounter > 2)
		{
			wFallFrCounter = 0;
			regularFrame++;
			if (regularFrame > 15)
			{
				regularFrame = 0;
			}
		}
		wFallFrCounter2++;
		if (wFallFrCounter2 > 6)
		{
			wFallFrCounter2 = 0;
			slowFrame++;
			if (slowFrame > 15)
			{
				slowFrame = 0;
			}
		}
		rainFrameCounter++;
		if (rainFrameCounter > 0)
		{
			rainFrameForeground++;
			if (rainFrameForeground > 7)
			{
				rainFrameForeground -= 8;
			}
			if (rainFrameCounter > 2)
			{
				rainFrameCounter = 0;
				rainFrameBackground--;
				if (rainFrameBackground < 0)
				{
					rainFrameBackground = 7;
				}
			}
		}
		if (++snowFrameCounter > 3)
		{
			snowFrameCounter = 0;
			if (++snowFrameForeground > 7)
			{
				snowFrameForeground = 0;
			}
		}
	}

	private void DrawWaterfall(int Style = 0, float Alpha = 1f)
	{
		Main.tileSolid[546] = false;
		float i = 0f;
		float num = 99999f;
		float num2 = 99999f;
		int num3 = -1;
		int num4 = -1;
		float num5 = 0f;
		float num6 = 99999f;
		float num7 = 99999f;
		int num8 = -1;
		int num9 = -1;
		for (int j = 0; j < currentMax; j++)
		{
			int num10 = 0;
			int num11 = waterfalls[j].type;
			int num12 = waterfalls[j].x;
			int num13 = waterfalls[j].y;
			int num14 = 0;
			int num15 = 0;
			int num16 = 0;
			int num17 = 0;
			int num18 = 0;
			int num19 = 0;
			int num20;
			int num21;
			if (num11 == 1 || num11 == 14 || num11 == 25)
			{
				if (Main.drewLava || waterfalls[j].stopAtStep == 0)
				{
					continue;
				}
				num20 = 32 * slowFrame;
			}
			else
			{
				switch (num11)
				{
				case 11:
				case 22:
				{
					if (Main.drewLava)
					{
						continue;
					}
					num21 = waterfallDist / 4;
					if (num11 == 22)
					{
						num21 = waterfallDist / 2;
					}
					if (waterfalls[j].stopAtStep > num21)
					{
						waterfalls[j].stopAtStep = num21;
					}
					if (waterfalls[j].stopAtStep == 0 || (float)(num13 + num21) < Main.screenPosition.Y / 16f || (float)num12 < Main.screenPosition.X / 16f - 20f || (float)num12 > (Main.screenPosition.X + (float)Main.screenWidth) / 16f + 20f)
					{
						continue;
					}
					int num22;
					int num23;
					if (num12 % 2 == 0)
					{
						num22 = rainFrameForeground + 3;
						if (num22 > 7)
						{
							num22 -= 8;
						}
						num23 = rainFrameBackground + 2;
						if (num23 > 7)
						{
							num23 -= 8;
						}
						if (num11 == 22)
						{
							num22 = snowFrameForeground + 3;
							if (num22 > 7)
							{
								num22 -= 8;
							}
						}
					}
					else
					{
						num22 = rainFrameForeground;
						num23 = rainFrameBackground;
						if (num11 == 22)
						{
							num22 = snowFrameForeground;
						}
					}
					Rectangle value = new Rectangle(num23 * 18, 0, 16, 16);
					Rectangle value2 = new Rectangle(num22 * 18, 0, 16, 16);
					Vector2 origin = new Vector2(8f, 8f);
					Vector2 position = ((num13 % 2 != 0) ? (new Vector2(num12 * 16 + 8, num13 * 16 + 8) - Main.screenPosition) : (new Vector2(num12 * 16 + 9, num13 * 16 + 8) - Main.screenPosition));
					Tile tile = Main.tile[num12, num13 - 1];
					if (tile.active() && tile.bottomSlope())
					{
						position.Y -= 16f;
					}
					bool flag = false;
					float rotation = 0f;
					for (int k = 0; k < num21; k++)
					{
						Color color = Lighting.GetColor(num12, num13);
						float num24 = 0.6f;
						float num25 = 0.3f;
						if (k > num21 - 8)
						{
							float num26 = (float)(num21 - k) / 8f;
							num24 *= num26;
							num25 *= num26;
						}
						Color color2 = color * num24;
						Color color3 = color * num25;
						if (num11 == 22)
						{
							Main.spriteBatch.Draw(waterfallTexture[22].get_Value(), position, value2, color2, 0f, origin, 1f, SpriteEffects.None, 0f);
						}
						else
						{
							Main.spriteBatch.Draw(waterfallTexture[12].get_Value(), position, value, color3, rotation, origin, 1f, SpriteEffects.None, 0f);
							Main.spriteBatch.Draw(waterfallTexture[11].get_Value(), position, value2, color2, rotation, origin, 1f, SpriteEffects.None, 0f);
						}
						if (flag)
						{
							break;
						}
						num13++;
						Tile tile2 = Main.tile[num12, num13];
						if (WorldGen.SolidTile(tile2))
						{
							flag = true;
						}
						if (tile2.liquid > 0)
						{
							int num27 = (int)(16f * ((float)(int)tile2.liquid / 255f)) & 0xFE;
							if (num27 >= 15)
							{
								break;
							}
							value2.Height -= num27;
							value.Height -= num27;
						}
						if (num13 % 2 == 0)
						{
							position.X += 1f;
						}
						else
						{
							position.X -= 1f;
						}
						position.Y += 16f;
					}
					waterfalls[j].stopAtStep = 0;
					continue;
				}
				case 0:
					num11 = Style;
					break;
				case 2:
					if (Main.drewLava)
					{
						continue;
					}
					break;
				}
				num20 = 32 * regularFrame;
			}
			int num28 = 0;
			num21 = waterfallDist;
			Color color4 = Color.White;
			for (int l = 0; l < num21; l++)
			{
				if (num28 >= 2)
				{
					break;
				}
				AddLight(num11, num12, num13);
				Tile tile3 = Main.tile[num12, num13];
				if (tile3 == null)
				{
					tile3 = new Tile();
					Main.tile[num12, num13] = tile3;
				}
				if (tile3.nactive() && Main.tileSolid[tile3.type] && !Main.tileSolidTop[tile3.type] && !TileID.Sets.Platforms[tile3.type] && tile3.blockType() == 0)
				{
					break;
				}
				Tile tile4 = Main.tile[num12 - 1, num13];
				if (tile4 == null)
				{
					tile4 = new Tile();
					Main.tile[num12 - 1, num13] = tile4;
				}
				Tile tile5 = Main.tile[num12, num13 + 1];
				if (tile5 == null)
				{
					tile5 = new Tile();
					Main.tile[num12, num13 + 1] = tile5;
				}
				Tile tile6 = Main.tile[num12 + 1, num13];
				if (tile6 == null)
				{
					tile6 = new Tile();
					Main.tile[num12 + 1, num13] = tile6;
				}
				if (WorldGen.SolidTile(tile5) && !tile3.halfBrick())
				{
					num10 = 8;
				}
				else if (num15 != 0)
				{
					num10 = 0;
				}
				int num29 = 0;
				int num30 = num17;
				int num31 = 0;
				int num32 = 0;
				bool flag2 = false;
				if (tile5.topSlope() && !tile3.halfBrick() && tile5.type != 19)
				{
					flag2 = true;
					if (tile5.slope() == 1)
					{
						num29 = 1;
						num31 = 1;
						num16 = 1;
						num17 = num16;
					}
					else
					{
						num29 = -1;
						num31 = -1;
						num16 = -1;
						num17 = num16;
					}
					num32 = 1;
				}
				else if ((!WorldGen.SolidTile(tile5) && !tile5.bottomSlope() && !tile3.halfBrick()) || (!tile5.active() && !tile3.halfBrick()))
				{
					num28 = 0;
					num32 = 1;
					num31 = 0;
				}
				else if ((WorldGen.SolidTile(tile4) || tile4.topSlope() || tile4.liquid > 0) && !WorldGen.SolidTile(tile6) && tile6.liquid == 0)
				{
					if (num16 == -1)
					{
						num28++;
					}
					num31 = 1;
					num32 = 0;
					num16 = 1;
				}
				else if ((WorldGen.SolidTile(tile6) || tile6.topSlope() || tile6.liquid > 0) && !WorldGen.SolidTile(tile4) && tile4.liquid == 0)
				{
					if (num16 == 1)
					{
						num28++;
					}
					num31 = -1;
					num32 = 0;
					num16 = -1;
				}
				else if (((!WorldGen.SolidTile(tile6) && !tile3.topSlope()) || tile6.liquid == 0) && !WorldGen.SolidTile(tile4) && !tile3.topSlope() && tile4.liquid == 0)
				{
					num32 = 0;
					num31 = num16;
				}
				else
				{
					num28++;
					num32 = 0;
					num31 = 0;
				}
				if (num28 >= 2)
				{
					num16 *= -1;
					num31 *= -1;
				}
				int num33 = -1;
				if (num11 != 1 && num11 != 14 && num11 != 25)
				{
					if (tile5.active())
					{
						num33 = tile5.type;
					}
					if (tile3.active())
					{
						num33 = tile3.type;
					}
				}
				switch (num33)
				{
				case 160:
					num11 = 2;
					break;
				case 262:
				case 263:
				case 264:
				case 265:
				case 266:
				case 267:
				case 268:
					num11 = 15 + num33 - 262;
					break;
				}
				Color color5 = Lighting.GetColor(num12, num13);
				if (l > 50)
				{
					TrySparkling(num12, num13, num16, color5);
				}
				float alpha = GetAlpha(Alpha, num21, num11, num13, l, tile3);
				color5 = StylizeColor(alpha, num21, num11, num13, l, tile3, color5);
				if (num11 == 1)
				{
					float num34 = Math.Abs((float)(num12 * 16 + 8) - (Main.screenPosition.X + (float)(Main.screenWidth / 2)));
					float num35 = Math.Abs((float)(num13 * 16 + 8) - (Main.screenPosition.Y + (float)(Main.screenHeight / 2)));
					if (num34 < (float)(Main.screenWidth * 2) && num35 < (float)(Main.screenHeight * 2))
					{
						float num36 = (float)Math.Sqrt(num34 * num34 + num35 * num35);
						float num37 = 1f - num36 / ((float)Main.screenWidth * 0.75f);
						if (num37 > 0f)
						{
							num5 += num37;
						}
					}
					if (num34 < num6)
					{
						num6 = num34;
						num8 = num12 * 16 + 8;
					}
					if (num35 < num7)
					{
						num7 = num34;
						num9 = num13 * 16 + 8;
					}
				}
				else if (num11 != 1 && num11 != 14 && num11 != 25 && num11 != 11 && num11 != 12 && num11 != 22)
				{
					float num38 = Math.Abs((float)(num12 * 16 + 8) - (Main.screenPosition.X + (float)(Main.screenWidth / 2)));
					float num39 = Math.Abs((float)(num13 * 16 + 8) - (Main.screenPosition.Y + (float)(Main.screenHeight / 2)));
					if (num38 < (float)(Main.screenWidth * 2) && num39 < (float)(Main.screenHeight * 2))
					{
						float num40 = (float)Math.Sqrt(num38 * num38 + num39 * num39);
						float num41 = 1f - num40 / ((float)Main.screenWidth * 0.75f);
						if (num41 > 0f)
						{
							i += num41;
						}
					}
					if (num38 < num)
					{
						num = num38;
						num3 = num12 * 16 + 8;
					}
					if (num39 < num2)
					{
						num2 = num38;
						num4 = num13 * 16 + 8;
					}
				}
				int num42 = (int)tile3.liquid / 16;
				if (flag2 && num16 != num30)
				{
					int num43 = 2;
					if (num30 == 1)
					{
						DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16 - 16, num13 * 16 + 16 - num43) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42 - num43), color5, SpriteEffects.FlipHorizontally);
					}
					else
					{
						DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16 + 16 - num43) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42 - num43), color5, SpriteEffects.None);
					}
				}
				if (num14 == 0 && num29 != 0 && num15 == 1 && num16 != num17)
				{
					num29 = 0;
					num16 = num17;
					color5 = Color.White;
					if (num16 == 1)
					{
						DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16 - 16, num13 * 16 + 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color5, SpriteEffects.FlipHorizontally);
					}
					else
					{
						DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16 - 16, num13 * 16 + 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color5, SpriteEffects.FlipHorizontally);
					}
				}
				if (num18 != 0 && num31 == 0 && num32 == 1)
				{
					if (num16 == 1)
					{
						if (num19 != num11)
						{
							DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16 + num10 + 8) - Main.screenPosition, new Rectangle(num20, 0, 16, 16 - num42 - 8), color4, SpriteEffects.FlipHorizontally);
						}
						else
						{
							DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16 + num10 + 8) - Main.screenPosition, new Rectangle(num20, 0, 16, 16 - num42 - 8), color5, SpriteEffects.FlipHorizontally);
						}
					}
					else
					{
						DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16 + num10 + 8) - Main.screenPosition, new Rectangle(num20, 0, 16, 16 - num42 - 8), color5, SpriteEffects.None);
					}
				}
				if (num10 == 8 && num15 == 1 && num18 == 0)
				{
					if (num17 == -1)
					{
						if (num19 != num11)
						{
							DrawWaterfall(num19, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 8), color4, SpriteEffects.None);
						}
						else
						{
							DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 8), color5, SpriteEffects.None);
						}
					}
					else if (num19 != num11)
					{
						DrawWaterfall(num19, num12, num13, alpha, new Vector2(num12 * 16 - 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 8), color4, SpriteEffects.FlipHorizontally);
					}
					else
					{
						DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16 - 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 8), color5, SpriteEffects.FlipHorizontally);
					}
				}
				if (num29 != 0 && num14 == 0)
				{
					if (num30 == 1)
					{
						if (num19 != num11)
						{
							DrawWaterfall(num19, num12, num13, alpha, new Vector2(num12 * 16 - 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color4, SpriteEffects.FlipHorizontally);
						}
						else
						{
							DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16 - 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color5, SpriteEffects.FlipHorizontally);
						}
					}
					else if (num19 != num11)
					{
						DrawWaterfall(num19, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color4, SpriteEffects.None);
					}
					else
					{
						DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color5, SpriteEffects.None);
					}
				}
				if (num32 == 1 && num29 == 0 && num18 == 0)
				{
					if (num16 == -1)
					{
						if (num15 == 0)
						{
							DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16 + num10) - Main.screenPosition, new Rectangle(num20, 0, 16, 16 - num42), color5, SpriteEffects.None);
						}
						else if (num19 != num11)
						{
							DrawWaterfall(num19, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color4, SpriteEffects.None);
						}
						else
						{
							DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color5, SpriteEffects.None);
						}
					}
					else if (num15 == 0)
					{
						DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16 + num10) - Main.screenPosition, new Rectangle(num20, 0, 16, 16 - num42), color5, SpriteEffects.FlipHorizontally);
					}
					else if (num19 != num11)
					{
						DrawWaterfall(num19, num12, num13, alpha, new Vector2(num12 * 16 - 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color4, SpriteEffects.FlipHorizontally);
					}
					else
					{
						DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16 - 16, num13 * 16) - Main.screenPosition, new Rectangle(num20, 24, 32, 16 - num42), color5, SpriteEffects.FlipHorizontally);
					}
				}
				else
				{
					switch (num31)
					{
					case 1:
						if (Main.tile[num12, num13].liquid > 0 && !Main.tile[num12, num13].halfBrick())
						{
							break;
						}
						if (num29 == 1)
						{
							for (int n = 0; n < 8; n++)
							{
								int num47 = n * 2;
								int num48 = 14 - n * 2;
								int num49 = num47;
								num10 = 8;
								if (num14 == 0 && n < 2)
								{
									num49 = 4;
								}
								DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16 + num47, num13 * 16 + num10 + num49) - Main.screenPosition, new Rectangle(16 + num20 + num48, 0, 2, 16 - num10), color5, SpriteEffects.FlipHorizontally);
							}
						}
						else
						{
							int height2 = 16;
							if (TileID.Sets.BlocksWaterDrawingBehindSelf[Main.tile[num12, num13].type])
							{
								height2 = 8;
							}
							else if (TileID.Sets.BlocksWaterDrawingBehindSelf[Main.tile[num12, num13 + 1].type])
							{
								height2 = 8;
							}
							DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16 + num10) - Main.screenPosition, new Rectangle(16 + num20, 0, 16, height2), color5, SpriteEffects.FlipHorizontally);
						}
						break;
					case -1:
						if (Main.tile[num12, num13].liquid > 0 && !Main.tile[num12, num13].halfBrick())
						{
							break;
						}
						if (num29 == -1)
						{
							for (int m = 0; m < 8; m++)
							{
								int num44 = m * 2;
								int num45 = m * 2;
								int num46 = 14 - m * 2;
								num10 = 8;
								if (num14 == 0 && m > 5)
								{
									num46 = 4;
								}
								DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16 + num44, num13 * 16 + num10 + num46) - Main.screenPosition, new Rectangle(16 + num20 + num45, 0, 2, 16 - num10), color5, SpriteEffects.FlipHorizontally);
							}
						}
						else
						{
							int height = 16;
							if (TileID.Sets.BlocksWaterDrawingBehindSelf[Main.tile[num12, num13].type])
							{
								height = 8;
							}
							else if (TileID.Sets.BlocksWaterDrawingBehindSelf[Main.tile[num12, num13 + 1].type])
							{
								height = 8;
							}
							DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16 + num10) - Main.screenPosition, new Rectangle(16 + num20, 0, 16, height), color5, SpriteEffects.None);
						}
						break;
					case 0:
						if (num32 == 0)
						{
							if (Main.tile[num12, num13].liquid <= 0 || Main.tile[num12, num13].halfBrick())
							{
								DrawWaterfall(num11, num12, num13, alpha, new Vector2(num12 * 16, num13 * 16 + num10) - Main.screenPosition, new Rectangle(16 + num20, 0, 16, 16), color5, SpriteEffects.None);
							}
							l = 1000;
						}
						break;
					}
				}
				if (tile3.liquid > 0 && !tile3.halfBrick())
				{
					l = 1000;
				}
				num15 = num32;
				num17 = num16;
				num14 = num31;
				num12 += num31;
				num13 += num32;
				num18 = num29;
				color4 = color5;
				if (num19 != num11)
				{
					num19 = num11;
				}
				if ((tile4.active() && (tile4.type == 189 || tile4.type == 196)) || (tile6.active() && (tile6.type == 189 || tile6.type == 196)) || (tile5.active() && (tile5.type == 189 || tile5.type == 196)))
				{
					num21 = (int)(40f * ((float)Main.maxTilesX / 4200f) * Main.gfxQuality);
				}
			}
		}
		Main.ambientWaterfallX = num3;
		Main.ambientWaterfallY = num4;
		Main.ambientWaterfallStrength = i;
		Main.ambientLavafallX = num8;
		Main.ambientLavafallY = num9;
		Main.ambientLavafallStrength = num5;
		Main.tileSolid[546] = true;
	}

	private void DrawWaterfall(int waterfallType, int x, int y, float opacity, Vector2 position, Rectangle sourceRect, Color color, SpriteEffects effects)
	{
		Texture2D value = waterfallTexture[waterfallType].get_Value();
		if (waterfallType == 25)
		{
			Lighting.GetCornerColors(x, y, out var vertices);
			LiquidRenderer.SetShimmerVertexColors(ref vertices, opacity, x, y);
			Main.tileBatch.Draw(value, position + new Vector2(0f, 0f), sourceRect, vertices, default(Vector2), 1f, effects);
			sourceRect.Y += 42;
			LiquidRenderer.SetShimmerVertexColors_Sparkle(ref vertices, opacity, x, y, top: true);
			Main.tileBatch.Draw(value, position + new Vector2(0f, 0f), sourceRect, vertices, default(Vector2), 1f, effects);
		}
		else
		{
			Main.spriteBatch.Draw(value, position, sourceRect, color, 0f, default(Vector2), 1f, effects, 0f);
		}
	}

	private static Color StylizeColor(float alpha, int maxSteps, int waterfallType, int y, int s, Tile tileCache, Color aColor)
	{
		float i = (float)(int)aColor.R * alpha;
		float num = (float)(int)aColor.G * alpha;
		float num2 = (float)(int)aColor.B * alpha;
		float num3 = (float)(int)aColor.A * alpha;
		switch (waterfallType)
		{
		case 1:
			if (i < 190f * alpha)
			{
				i = 190f * alpha;
			}
			if (num < 190f * alpha)
			{
				num = 190f * alpha;
			}
			if (num2 < 190f * alpha)
			{
				num2 = 190f * alpha;
			}
			break;
		case 2:
			i = (float)Main.DiscoR * alpha;
			num = (float)Main.DiscoG * alpha;
			num2 = (float)Main.DiscoB * alpha;
			break;
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
			i = 255f * alpha;
			num = 255f * alpha;
			num2 = 255f * alpha;
			break;
		}
		aColor = new Color((int)i, (int)num, (int)num2, (int)num3);
		return aColor;
	}

	private static float GetAlpha(float Alpha, int maxSteps, int waterfallType, int y, int s, Tile tileCache)
	{
		float tile = waterfallType switch
		{
			1 => 1f, 
			14 => 0.8f, 
			25 => 0.75f, 
			_ => (tileCache.wall != 0 || !((double)y < Main.worldSurface)) ? (0.6f * Alpha) : Alpha, 
		};
		if (s > maxSteps - 10)
		{
			tile *= (float)(maxSteps - s) / 10f;
		}
		return tile;
	}

	private static void TrySparkling(int x, int y, int direction, Color aColor2)
	{
		if (aColor2.R > 20 || aColor2.B > 20 || aColor2.G > 20)
		{
			float num = (int)aColor2.R;
			if ((float)(int)aColor2.G > num)
			{
				num = (int)aColor2.G;
			}
			if ((float)(int)aColor2.B > num)
			{
				num = (int)aColor2.B;
			}
			if ((float)Main.rand.Next(20000) < num / 30f)
			{
				int num2 = Dust.NewDust(new Vector2(x * 16 - direction * 7, y * 16 + 6), 10, 8, 43, 0f, 0f, 254, Color.White, 0.5f);
				Main.dust[num2].velocity *= 0f;
			}
		}
	}

	private static void AddLight(int waterfallType, int x, int y)
	{
		switch (waterfallType)
		{
		case 1:
		{
			float num;
			float num4 = (num = (0.55f + (float)(270 - Main.mouseTextColor) / 900f) * 0.4f);
			float g = num4 * 0.3f;
			float b = num4 * 0.1f;
			Lighting.AddLight(x, y, num, g, b);
			break;
		}
		case 2:
		{
			float num = (float)Main.DiscoR / 255f;
			float g = (float)Main.DiscoG / 255f;
			float b = (float)Main.DiscoB / 255f;
			num *= 0.2f;
			g *= 0.2f;
			b *= 0.2f;
			Lighting.AddLight(x, y, num, g, b);
			break;
		}
		case 15:
		{
			float num = 0f;
			float g = 0f;
			float b = 0.2f;
			Lighting.AddLight(x, y, num, g, b);
			break;
		}
		case 16:
		{
			float num = 0f;
			float g = 0.2f;
			float b = 0f;
			Lighting.AddLight(x, y, num, g, b);
			break;
		}
		case 17:
		{
			float num = 0f;
			float g = 0f;
			float b = 0.2f;
			Lighting.AddLight(x, y, num, g, b);
			break;
		}
		case 18:
		{
			float num = 0f;
			float g = 0.2f;
			float b = 0f;
			Lighting.AddLight(x, y, num, g, b);
			break;
		}
		case 19:
		{
			float num = 0.2f;
			float g = 0f;
			float b = 0f;
			Lighting.AddLight(x, y, num, g, b);
			break;
		}
		case 20:
			Lighting.AddLight(x, y, 0.2f, 0.2f, 0.2f);
			break;
		case 21:
		{
			float num = 0.2f;
			float g = 0f;
			float b = 0f;
			Lighting.AddLight(x, y, num, g, b);
			break;
		}
		case 25:
		{
			float num2 = 0.7f;
			float num3 = 0.7f;
			num2 += (float)(270 - Main.mouseTextColor) / 900f;
			num3 += (float)(270 - Main.mouseTextColor) / 125f;
			Lighting.AddLight(x, y, num2 * 0.6f, num3 * 0.25f, num2 * 0.9f);
			break;
		}
		}
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		for (int i = 0; i < currentMax; i++)
		{
			waterfalls[i].stopAtStep = waterfallDist;
		}
		Main.drewLava = false;
		if (Main.liquidAlpha[0] > 0f)
		{
			DrawWaterfall(0, Main.liquidAlpha[0]);
		}
		if (Main.liquidAlpha[2] > 0f)
		{
			DrawWaterfall(3, Main.liquidAlpha[2]);
		}
		if (Main.liquidAlpha[3] > 0f)
		{
			DrawWaterfall(4, Main.liquidAlpha[3]);
		}
		if (Main.liquidAlpha[4] > 0f)
		{
			DrawWaterfall(5, Main.liquidAlpha[4]);
		}
		if (Main.liquidAlpha[5] > 0f)
		{
			DrawWaterfall(6, Main.liquidAlpha[5]);
		}
		if (Main.liquidAlpha[6] > 0f)
		{
			DrawWaterfall(7, Main.liquidAlpha[6]);
		}
		if (Main.liquidAlpha[7] > 0f)
		{
			DrawWaterfall(8, Main.liquidAlpha[7]);
		}
		if (Main.liquidAlpha[8] > 0f)
		{
			DrawWaterfall(9, Main.liquidAlpha[8]);
		}
		if (Main.liquidAlpha[9] > 0f)
		{
			DrawWaterfall(10, Main.liquidAlpha[9]);
		}
		if (Main.liquidAlpha[10] > 0f)
		{
			DrawWaterfall(13, Main.liquidAlpha[10]);
		}
		if (Main.liquidAlpha[12] > 0f)
		{
			DrawWaterfall(23, Main.liquidAlpha[12]);
		}
		if (Main.liquidAlpha[13] > 0f)
		{
			DrawWaterfall(24, Main.liquidAlpha[13]);
		}
	}
}
