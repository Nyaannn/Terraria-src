using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Utilities;

namespace Terraria.GameContent.Liquid;

public class LiquidRenderer
{
	private struct LiquidCache
	{
		public float LiquidLevel;

		public float VisibleLiquidLevel;

		public float Opacity;

		public bool IsSolid;

		public bool IsHalfBrick;

		public bool HasLiquid;

		public bool HasVisibleLiquid;

		public bool HasWall;

		public Point FrameOffset;

		public bool HasLeftEdge;

		public bool HasRightEdge;

		public bool HasTopEdge;

		public bool HasBottomEdge;

		public float LeftWall;

		public float RightWall;

		public float BottomWall;

		public float TopWall;

		public float VisibleLeftWall;

		public float VisibleRightWall;

		public float VisibleBottomWall;

		public float VisibleTopWall;

		public byte Type;

		public byte VisibleType;
	}

	private struct LiquidDrawCache
	{
		public Rectangle SourceRectangle;

		public Vector2 LiquidOffset;

		public bool IsVisible;

		public float Opacity;

		public byte Type;

		public bool IsSurfaceLiquid;

		public bool HasWall;
	}

	private struct SpecialLiquidDrawCache
	{
		public int X;

		public int Y;

		public Rectangle SourceRectangle;

		public Vector2 LiquidOffset;

		public bool IsVisible;

		public float Opacity;

		public byte Type;

		public bool IsSurfaceLiquid;

		public bool HasWall;
	}

	private const int ANIMATION_FRAME_COUNT = 16;

	private const int CACHE_PADDING = 2;

	private const int CACHE_PADDING_2 = 4;

	private static readonly int[] WATERFALL_LENGTH = new int[4] { 10, 3, 2, 10 };

	private static readonly float[] DEFAULT_OPACITY = new float[4] { 0.6f, 0.95f, 0.95f, 0.75f };

	private static readonly byte[] WAVE_MASK_STRENGTH = new byte[5];

	private static readonly byte[] VISCOSITY_MASK = new byte[5] { 0, 200, 240, 0, 0 };

	public const float MIN_LIQUID_SIZE = 0.25f;

	public static LiquidRenderer Instance;

	private readonly Asset<Texture2D>[] _liquidTextures = new Asset<Texture2D>[15];

	private LiquidCache[] _cache = new LiquidCache[1];

	private LiquidDrawCache[] _drawCache = new LiquidDrawCache[1];

	private SpecialLiquidDrawCache[] _drawCacheForShimmer = new SpecialLiquidDrawCache[1];

	private int _animationFrame;

	private Rectangle _drawArea = new Rectangle(0, 0, 1, 1);

	private readonly UnifiedRandom _random = new UnifiedRandom();

	private Color[] _waveMask = new Color[1];

	private float _frameState;

	private static Tile[,] Tiles => Main.tile;

	public event Action<Color[], Rectangle> WaveFilters;

	public static void LoadContent()
	{
		Instance = new LiquidRenderer();
		Instance.PrepareAssets();
	}

	private void PrepareAssets()
	{
		if (!Main.dedServ)
		{
			for (int i = 0; i < _liquidTextures.Length; i++)
			{
				_liquidTextures[i] = Main.Assets.Request<Texture2D>("Images/Misc/water_" + i, (AssetRequestMode)1);
			}
		}
	}

	private unsafe void InternalPrepareDraw(Rectangle drawArea)
	{
		Rectangle sizeButtons = new Rectangle(drawArea.X - 2, drawArea.Y - 2, drawArea.Width + 4, drawArea.Height + 4);
		_drawArea = drawArea;
		if (_cache.Length < sizeButtons.Width * sizeButtons.Height + 1)
		{
			_cache = new LiquidCache[sizeButtons.Width * sizeButtons.Height + 1];
		}
		if (_drawCache.Length < drawArea.Width * drawArea.Height + 1)
		{
			_drawCache = new LiquidDrawCache[drawArea.Width * drawArea.Height + 1];
		}
		if (_drawCacheForShimmer.Length < drawArea.Width * drawArea.Height + 1)
		{
			_drawCacheForShimmer = new SpecialLiquidDrawCache[drawArea.Width * drawArea.Height + 1];
		}
		if (_waveMask.Length < drawArea.Width * drawArea.Height)
		{
			_waveMask = new Color[drawArea.Width * drawArea.Height];
		}
		Tile difficultyButtons = null;
		fixed (LiquidCache* evilButtons = &_cache[1])
		{
			LiquidCache* j = evilButtons;
			int num = sizeButtons.Height * 2 + 2;
			j = evilButtons;
			for (int k = sizeButtons.X; k < sizeButtons.X + sizeButtons.Width; k++)
			{
				for (int l = sizeButtons.Y; l < sizeButtons.Y + sizeButtons.Height; l++)
				{
					difficultyButtons = Tiles[k, l];
					if (difficultyButtons == null)
					{
						difficultyButtons = new Tile();
					}
					j->LiquidLevel = (float)(int)difficultyButtons.liquid / 255f;
					j->IsHalfBrick = difficultyButtons.halfBrick() && j[-1].HasLiquid && !TileID.Sets.Platforms[difficultyButtons.type];
					j->IsSolid = WorldGen.SolidOrSlopedTile(difficultyButtons);
					j->HasLiquid = difficultyButtons.liquid != 0;
					j->VisibleLiquidLevel = 0f;
					j->HasWall = difficultyButtons.wall != 0;
					j->Type = difficultyButtons.liquidType();
					if (j->IsHalfBrick && !j->HasLiquid)
					{
						j->Type = j[-1].Type;
					}
					j++;
				}
			}
			j = evilButtons;
			float num2 = 0f;
			j += num;
			for (int m = 2; m < sizeButtons.Width - 2; m++)
			{
				for (int n = 2; n < sizeButtons.Height - 2; n++)
				{
					num2 = 0f;
					if (j->IsHalfBrick && j[-1].HasLiquid)
					{
						num2 = 1f;
					}
					else if (!j->HasLiquid)
					{
						LiquidCache i = j[-1];
						LiquidCache liquidCache = j[1];
						LiquidCache liquidCache2 = j[-sizeButtons.Height];
						LiquidCache liquidCache3 = j[sizeButtons.Height];
						if (i.HasLiquid && liquidCache.HasLiquid && i.Type == liquidCache.Type && !i.IsSolid && !liquidCache.IsSolid)
						{
							num2 = i.LiquidLevel + liquidCache.LiquidLevel;
							j->Type = i.Type;
						}
						if (liquidCache2.HasLiquid && liquidCache3.HasLiquid && liquidCache2.Type == liquidCache3.Type && !liquidCache2.IsSolid && !liquidCache3.IsSolid)
						{
							num2 = Math.Max(num2, liquidCache2.LiquidLevel + liquidCache3.LiquidLevel);
							j->Type = liquidCache2.Type;
						}
						num2 *= 0.5f;
					}
					else
					{
						num2 = j->LiquidLevel;
					}
					j->VisibleLiquidLevel = num2;
					j->HasVisibleLiquid = num2 != 0f;
					j++;
				}
				j += 4;
			}
			j = evilButtons;
			for (int num3 = 0; num3 < sizeButtons.Width; num3++)
			{
				for (int num4 = 0; num4 < sizeButtons.Height - 10; num4++)
				{
					if (j->HasVisibleLiquid && (!j->IsSolid || j->IsHalfBrick))
					{
						j->Opacity = 1f;
						j->VisibleType = j->Type;
						float num5 = 1f / (float)(WATERFALL_LENGTH[j->Type] + 1);
						float num6 = 1f;
						for (int num7 = 1; num7 <= WATERFALL_LENGTH[j->Type]; num7++)
						{
							num6 -= num5;
							if (j[num7].IsSolid)
							{
								break;
							}
							j[num7].VisibleLiquidLevel = Math.Max(j[num7].VisibleLiquidLevel, j->VisibleLiquidLevel * num6);
							j[num7].Opacity = num6;
							j[num7].VisibleType = j->Type;
						}
					}
					if (j->IsSolid && !j->IsHalfBrick)
					{
						j->VisibleLiquidLevel = 1f;
						j->HasVisibleLiquid = false;
					}
					else
					{
						j->HasVisibleLiquid = j->VisibleLiquidLevel != 0f;
					}
					j++;
				}
				j += 10;
			}
			j = evilButtons;
			j += num;
			for (int num8 = 2; num8 < sizeButtons.Width - 2; num8++)
			{
				for (int num9 = 2; num9 < sizeButtons.Height - 2; num9++)
				{
					if (!j->HasVisibleLiquid)
					{
						j->HasLeftEdge = false;
						j->HasTopEdge = false;
						j->HasRightEdge = false;
						j->HasBottomEdge = false;
					}
					else
					{
						LiquidCache i = j[-1];
						LiquidCache liquidCache = j[1];
						LiquidCache liquidCache2 = j[-sizeButtons.Height];
						LiquidCache liquidCache3 = j[sizeButtons.Height];
						float num10 = 0f;
						float num11 = 1f;
						float num12 = 0f;
						float num13 = 1f;
						float visibleLiquidLevel = j->VisibleLiquidLevel;
						if (!i.HasVisibleLiquid)
						{
							num12 += liquidCache.VisibleLiquidLevel * (1f - visibleLiquidLevel);
						}
						if (!liquidCache.HasVisibleLiquid && !liquidCache.IsSolid && !liquidCache.IsHalfBrick)
						{
							num13 -= i.VisibleLiquidLevel * (1f - visibleLiquidLevel);
						}
						if (!liquidCache2.HasVisibleLiquid && !liquidCache2.IsSolid && !liquidCache2.IsHalfBrick)
						{
							num10 += liquidCache3.VisibleLiquidLevel * (1f - visibleLiquidLevel);
						}
						if (!liquidCache3.HasVisibleLiquid && !liquidCache3.IsSolid && !liquidCache3.IsHalfBrick)
						{
							num11 -= liquidCache2.VisibleLiquidLevel * (1f - visibleLiquidLevel);
						}
						j->LeftWall = num10;
						j->RightWall = num11;
						j->BottomWall = num13;
						j->TopWall = num12;
						Point zero = Point.Zero;
						j->HasTopEdge = (!i.HasVisibleLiquid && !i.IsSolid) || num12 != 0f;
						j->HasBottomEdge = (!liquidCache.HasVisibleLiquid && !liquidCache.IsSolid) || num13 != 1f;
						j->HasLeftEdge = (!liquidCache2.HasVisibleLiquid && !liquidCache2.IsSolid) || num10 != 0f;
						j->HasRightEdge = (!liquidCache3.HasVisibleLiquid && !liquidCache3.IsSolid) || num11 != 1f;
						if (!j->HasLeftEdge)
						{
							if (j->HasRightEdge)
							{
								zero.X += 32;
							}
							else
							{
								zero.X += 16;
							}
						}
						if (j->HasLeftEdge && j->HasRightEdge)
						{
							zero.X = 16;
							zero.Y += 32;
							if (j->HasTopEdge)
							{
								zero.Y = 16;
							}
						}
						else if (!j->HasTopEdge)
						{
							if (!j->HasLeftEdge && !j->HasRightEdge)
							{
								zero.Y += 48;
							}
							else
							{
								zero.Y += 16;
							}
						}
						if (zero.Y == 16 && (j->HasLeftEdge ^ j->HasRightEdge) && (num9 + sizeButtons.Y) % 2 == 0)
						{
							zero.Y += 16;
						}
						j->FrameOffset = zero;
					}
					j++;
				}
				j += 4;
			}
			j = evilButtons;
			j += num;
			for (int num14 = 2; num14 < sizeButtons.Width - 2; num14++)
			{
				for (int num15 = 2; num15 < sizeButtons.Height - 2; num15++)
				{
					if (j->HasVisibleLiquid)
					{
						LiquidCache i = j[-1];
						LiquidCache liquidCache = j[1];
						LiquidCache liquidCache2 = j[-sizeButtons.Height];
						LiquidCache liquidCache3 = j[sizeButtons.Height];
						j->VisibleLeftWall = j->LeftWall;
						j->VisibleRightWall = j->RightWall;
						j->VisibleTopWall = j->TopWall;
						j->VisibleBottomWall = j->BottomWall;
						if (i.HasVisibleLiquid && liquidCache.HasVisibleLiquid)
						{
							if (j->HasLeftEdge)
							{
								j->VisibleLeftWall = (j->LeftWall * 2f + i.LeftWall + liquidCache.LeftWall) * 0.25f;
							}
							if (j->HasRightEdge)
							{
								j->VisibleRightWall = (j->RightWall * 2f + i.RightWall + liquidCache.RightWall) * 0.25f;
							}
						}
						if (liquidCache2.HasVisibleLiquid && liquidCache3.HasVisibleLiquid)
						{
							if (j->HasTopEdge)
							{
								j->VisibleTopWall = (j->TopWall * 2f + liquidCache2.TopWall + liquidCache3.TopWall) * 0.25f;
							}
							if (j->HasBottomEdge)
							{
								j->VisibleBottomWall = (j->BottomWall * 2f + liquidCache2.BottomWall + liquidCache3.BottomWall) * 0.25f;
							}
						}
					}
					j++;
				}
				j += 4;
			}
			j = evilButtons;
			j += num;
			for (int num16 = 2; num16 < sizeButtons.Width - 2; num16++)
			{
				for (int num17 = 2; num17 < sizeButtons.Height - 2; num17++)
				{
					if (j->HasLiquid)
					{
						LiquidCache i = j[-1];
						LiquidCache liquidCache = j[1];
						LiquidCache liquidCache2 = j[-sizeButtons.Height];
						LiquidCache liquidCache3 = j[sizeButtons.Height];
						if (j->HasTopEdge && !j->HasBottomEdge && (j->HasLeftEdge ^ j->HasRightEdge))
						{
							if (j->HasRightEdge)
							{
								j->VisibleRightWall = liquidCache.VisibleRightWall;
								j->VisibleTopWall = liquidCache2.VisibleTopWall;
							}
							else
							{
								j->VisibleLeftWall = liquidCache.VisibleLeftWall;
								j->VisibleTopWall = liquidCache3.VisibleTopWall;
							}
						}
						else if (liquidCache.FrameOffset.X == 16 && liquidCache.FrameOffset.Y == 32)
						{
							if (j->VisibleLeftWall > 0.5f)
							{
								j->VisibleLeftWall = 0f;
								j->FrameOffset = new Point(0, 0);
							}
							else if (j->VisibleRightWall < 0.5f)
							{
								j->VisibleRightWall = 1f;
								j->FrameOffset = new Point(32, 0);
							}
						}
					}
					j++;
				}
				j += 4;
			}
			j = evilButtons;
			j += num;
			for (int num18 = 2; num18 < sizeButtons.Width - 2; num18++)
			{
				for (int num19 = 2; num19 < sizeButtons.Height - 2; num19++)
				{
					if (j->HasLiquid)
					{
						LiquidCache i = j[-1];
						LiquidCache liquidCache = j[1];
						LiquidCache liquidCache2 = j[-sizeButtons.Height];
						LiquidCache liquidCache3 = j[sizeButtons.Height];
						if (!j->HasBottomEdge && !j->HasLeftEdge && !j->HasTopEdge && !j->HasRightEdge)
						{
							if (liquidCache2.HasTopEdge && i.HasLeftEdge)
							{
								j->FrameOffset.X = Math.Max(4, (int)(16f - i.VisibleLeftWall * 16f)) - 4;
								j->FrameOffset.Y = 48 + Math.Max(4, (int)(16f - liquidCache2.VisibleTopWall * 16f)) - 4;
								j->VisibleLeftWall = 0f;
								j->VisibleTopWall = 0f;
								j->VisibleRightWall = 1f;
								j->VisibleBottomWall = 1f;
							}
							else if (liquidCache3.HasTopEdge && i.HasRightEdge)
							{
								j->FrameOffset.X = 32 - Math.Min(16, (int)(i.VisibleRightWall * 16f) - 4);
								j->FrameOffset.Y = 48 + Math.Max(4, (int)(16f - liquidCache3.VisibleTopWall * 16f)) - 4;
								j->VisibleLeftWall = 0f;
								j->VisibleTopWall = 0f;
								j->VisibleRightWall = 1f;
								j->VisibleBottomWall = 1f;
							}
						}
					}
					j++;
				}
				j += 4;
			}
			j = evilButtons;
			j += num;
			fixed (LiquidDrawCache* ptr = &_drawCache[0])
			{
				fixed (Color* ptr3 = &_waveMask[0])
				{
					LiquidDrawCache* ptr2 = ptr;
					Color* ptr4 = ptr3;
					for (int num20 = 2; num20 < sizeButtons.Width - 2; num20++)
					{
						for (int num21 = 2; num21 < sizeButtons.Height - 2; num21++)
						{
							if (j->HasVisibleLiquid)
							{
								float num22 = Math.Min(0.75f, j->VisibleLeftWall);
								float num23 = Math.Max(0.25f, j->VisibleRightWall);
								float num24 = Math.Min(0.75f, j->VisibleTopWall);
								float num25 = Math.Max(0.25f, j->VisibleBottomWall);
								if (j->IsHalfBrick && j->IsSolid && num25 > 0.5f)
								{
									num25 = 0.5f;
								}
								ptr2->IsVisible = j->HasWall || !j->IsHalfBrick || !j->HasLiquid || !(j->LiquidLevel < 1f);
								ptr2->SourceRectangle = new Rectangle((int)(16f - num23 * 16f) + j->FrameOffset.X, (int)(16f - num25 * 16f) + j->FrameOffset.Y, (int)Math.Ceiling((num23 - num22) * 16f), (int)Math.Ceiling((num25 - num24) * 16f));
								ptr2->IsSurfaceLiquid = j->FrameOffset.X == 16 && j->FrameOffset.Y == 0 && (double)(num21 + sizeButtons.Y) > Main.worldSurface - 40.0;
								ptr2->Opacity = j->Opacity;
								ptr2->LiquidOffset = new Vector2((float)Math.Floor(num22 * 16f), (float)Math.Floor(num24 * 16f));
								ptr2->Type = j->VisibleType;
								ptr2->HasWall = j->HasWall;
								byte b = WAVE_MASK_STRENGTH[j->VisibleType];
								byte g = (ptr4->R = (byte)(b >> 1));
								ptr4->G = g;
								ptr4->B = VISCOSITY_MASK[j->VisibleType];
								ptr4->A = b;
								LiquidCache* ptr5 = j - 1;
								if (num21 != 2 && !ptr5->HasVisibleLiquid && !ptr5->IsSolid && !ptr5->IsHalfBrick)
								{
									*(ptr4 - 1) = *ptr4;
								}
							}
							else
							{
								ptr2->IsVisible = false;
								int num26 = ((!j->IsSolid && !j->IsHalfBrick) ? 4 : 3);
								byte b3 = WAVE_MASK_STRENGTH[num26];
								byte g2 = (ptr4->R = (byte)(b3 >> 1));
								ptr4->G = g2;
								ptr4->B = VISCOSITY_MASK[num26];
								ptr4->A = b3;
							}
							j++;
							ptr2++;
							ptr4++;
						}
						j += 4;
					}
				}
			}
			j = evilButtons;
			for (int num27 = sizeButtons.X; num27 < sizeButtons.X + sizeButtons.Width; num27++)
			{
				for (int num28 = sizeButtons.Y; num28 < sizeButtons.Y + sizeButtons.Height; num28++)
				{
					if (j->VisibleType == 1 && j->HasVisibleLiquid && Dust.lavaBubbles < 200)
					{
						if (_random.Next(700) == 0)
						{
							Dust.NewDust(new Vector2(num27 * 16, num28 * 16), 16, 16, 35, 0f, 0f, 0, Color.White);
						}
						if (_random.Next(350) == 0)
						{
							int num29 = Dust.NewDust(new Vector2(num27 * 16, num28 * 16), 16, 8, 35, 0f, 0f, 50, Color.White, 1.5f);
							Main.dust[num29].velocity *= 0.8f;
							Main.dust[num29].velocity.X *= 2f;
							Main.dust[num29].velocity.Y -= (float)_random.Next(1, 7) * 0.1f;
							if (_random.Next(10) == 0)
							{
								Main.dust[num29].velocity.Y *= _random.Next(2, 5);
							}
							Main.dust[num29].noGravity = true;
						}
					}
					j++;
				}
			}
			fixed (LiquidDrawCache* ptr6 = &_drawCache[0])
			{
				fixed (SpecialLiquidDrawCache* ptr8 = &_drawCacheForShimmer[0])
				{
					LiquidDrawCache* ptr7 = ptr6;
					SpecialLiquidDrawCache* ptr9 = ptr8;
					for (int num30 = 2; num30 < sizeButtons.Width - 2; num30++)
					{
						for (int num31 = 2; num31 < sizeButtons.Height - 2; num31++)
						{
							if (ptr7->IsVisible && ptr7->Type == 3)
							{
								ptr9->X = num30;
								ptr9->Y = num31;
								ptr9->IsVisible = ptr7->IsVisible;
								ptr9->HasWall = ptr7->HasWall;
								ptr9->IsSurfaceLiquid = ptr7->IsSurfaceLiquid;
								ptr9->LiquidOffset = ptr7->LiquidOffset;
								ptr9->Opacity = ptr7->Opacity;
								ptr9->SourceRectangle = ptr7->SourceRectangle;
								ptr9->Type = ptr7->Type;
								ptr7->IsVisible = false;
								ptr9++;
							}
							ptr7++;
						}
					}
					ptr9->IsVisible = false;
				}
			}
		}
		if (this.WaveFilters != null)
		{
			this.WaveFilters(_waveMask, GetCachedDrawArea());
		}
	}

	public unsafe void DrawNormalLiquids(SpriteBatch spriteBatch, Vector2 drawOffset, int waterStyle, float globalAlpha, bool isBackgroundDraw)
	{
		Rectangle localizedText = _drawArea;
		Main.tileBatch.Begin();
		fixed (LiquidDrawCache* groupOptionButton2 = &_drawCache[0])
		{
			LiquidDrawCache* groupOptionButton3 = groupOptionButton2;
			for (int uICharacterNameButton = localizedText.X; uICharacterNameButton < localizedText.X + localizedText.Width; uICharacterNameButton++)
			{
				for (int groupOptionButton4 = localizedText.Y; groupOptionButton4 < localizedText.Y + localizedText.Height; groupOptionButton4++)
				{
					if (groupOptionButton3->IsVisible)
					{
						Rectangle sourceRectangle = groupOptionButton3->SourceRectangle;
						if (groupOptionButton3->IsSurfaceLiquid)
						{
							sourceRectangle.Y = 1280;
						}
						else
						{
							sourceRectangle.Y += _animationFrame * 80;
						}
						Vector2 liquidOffset = groupOptionButton3->LiquidOffset;
						float num = groupOptionButton3->Opacity * (isBackgroundDraw ? 1f : DEFAULT_OPACITY[groupOptionButton3->Type]);
						int num2 = groupOptionButton3->Type;
						switch (num2)
						{
						case 0:
							num2 = waterStyle;
							num *= globalAlpha;
							break;
						case 2:
							num2 = 11;
							break;
						}
						num = Math.Min(1f, num);
						Lighting.GetCornerColors(uICharacterNameButton, groupOptionButton4, out var groupOptionButton);
						groupOptionButton.BottomLeftColor *= num;
						groupOptionButton.BottomRightColor *= num;
						groupOptionButton.TopLeftColor *= num;
						groupOptionButton.TopRightColor *= num;
						Main.DrawTileInWater(drawOffset, uICharacterNameButton, groupOptionButton4);
						Main.tileBatch.Draw(_liquidTextures[num2].get_Value(), new Vector2(uICharacterNameButton << 4, groupOptionButton4 << 4) + drawOffset + liquidOffset, sourceRectangle, groupOptionButton, Vector2.Zero, 1f, SpriteEffects.None);
					}
					groupOptionButton3++;
				}
			}
		}
		Main.tileBatch.End();
	}

	public unsafe void DrawShimmer(SpriteBatch spriteBatch, Vector2 drawOffset, bool isBackgroundDraw)
	{
		Rectangle drawArea = _drawArea;
		Main.tileBatch.Begin();
		fixed (SpecialLiquidDrawCache* ptr = &_drawCacheForShimmer[0])
		{
			SpecialLiquidDrawCache* ptr2 = ptr;
			int num = _drawCacheForShimmer.Length;
			for (int i = 0; i < num; i++)
			{
				if (!ptr2->IsVisible)
				{
					break;
				}
				Rectangle sourceRectangle = ptr2->SourceRectangle;
				if (ptr2->IsSurfaceLiquid)
				{
					sourceRectangle.Y = 1280;
				}
				else
				{
					sourceRectangle.Y += _animationFrame * 80;
				}
				Vector2 liquidOffset = ptr2->LiquidOffset;
				float val = ptr2->Opacity * (isBackgroundDraw ? 1f : 0.75f);
				int num2 = 14;
				val = Math.Min(1f, val);
				int num3 = ptr2->X + drawArea.X - 2;
				int num4 = ptr2->Y + drawArea.Y - 2;
				Lighting.GetCornerColors(num3, num4, out var vertices);
				SetShimmerVertexColors(ref vertices, val, num3, num4);
				Main.DrawTileInWater(drawOffset, num3, num4);
				Main.tileBatch.Draw(_liquidTextures[num2].get_Value(), new Vector2(num3 << 4, num4 << 4) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, SpriteEffects.None);
				sourceRectangle = ptr2->SourceRectangle;
				bool flag = sourceRectangle.X != 16 || sourceRectangle.Y % 80 != 48;
				if (flag || (num3 + num4) % 2 == 0)
				{
					sourceRectangle.X += 48;
					sourceRectangle.Y += 80 * GetShimmerFrame(flag, num3, num4);
					SetShimmerVertexColors_Sparkle(ref vertices, ptr2->Opacity, num3, num4, flag);
					Main.tileBatch.Draw(_liquidTextures[num2].get_Value(), new Vector2(num3 << 4, num4 << 4) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, SpriteEffects.None);
				}
				ptr2++;
			}
		}
		Main.tileBatch.End();
	}

	public static VertexColors SetShimmerVertexColors_Sparkle(ref VertexColors colors, float opacity, int x, int y, bool top)
	{
		colors.BottomLeftColor = GetShimmerGlitterColor(top, x, y + 1);
		colors.BottomRightColor = GetShimmerGlitterColor(top, x + 1, y + 1);
		colors.TopLeftColor = GetShimmerGlitterColor(top, x, y);
		colors.TopRightColor = GetShimmerGlitterColor(top, x + 1, y);
		colors.BottomLeftColor *= opacity;
		colors.BottomRightColor *= opacity;
		colors.TopLeftColor *= opacity;
		colors.TopRightColor *= opacity;
		return colors;
	}

	public static void SetShimmerVertexColors(ref VertexColors colors, float opacity, int x, int y)
	{
		colors.BottomLeftColor = Color.White;
		colors.BottomRightColor = Color.White;
		colors.TopLeftColor = Color.White;
		colors.TopRightColor = Color.White;
		colors.BottomLeftColor *= opacity;
		colors.BottomRightColor *= opacity;
		colors.TopLeftColor *= opacity;
		colors.TopRightColor *= opacity;
		colors.BottomLeftColor = new Color(colors.BottomLeftColor.ToVector4() * GetShimmerBaseColor(x, y + 1));
		colors.BottomRightColor = new Color(colors.BottomRightColor.ToVector4() * GetShimmerBaseColor(x + 1, y + 1));
		colors.TopLeftColor = new Color(colors.TopLeftColor.ToVector4() * GetShimmerBaseColor(x, y));
		colors.TopRightColor = new Color(colors.TopRightColor.ToVector4() * GetShimmerBaseColor(x + 1, y));
	}

	public static float GetShimmerWave(ref float worldPositionX, ref float worldPositionY)
	{
		return (float)Math.Sin(((double)((worldPositionX + worldPositionY / 6f) / 10f) - Main.timeForVisualEffects / 360.0) * 6.2831854820251465);
	}

	public static Color GetShimmerGlitterColor(bool top, float worldPositionX, float worldPositionY)
	{
		Color color = Main.hslToRgb((float)(((double)(worldPositionX + worldPositionY / 6f) + Main.timeForVisualEffects / 30.0) / 6.0) % 1f, 1f, 0.5f);
		color.A = 0;
		return new Color(color.ToVector4() * GetShimmerGlitterOpacity(top, worldPositionX, worldPositionY));
	}

	public static float GetShimmerGlitterOpacity(bool top, float worldPositionX, float worldPositionY)
	{
		if (top)
		{
			return 0.5f;
		}
		float num = Utils.Remap((float)Math.Sin(((double)((worldPositionX + worldPositionY / 6f) / 10f) - Main.timeForVisualEffects / 360.0) * 6.2831854820251465), -0.5f, 1f, 0f, 0.35f);
		float uIVirtualKeyboard = (float)Math.Sin((double)((float)SimpleWhiteNoise((uint)worldPositionX, (uint)worldPositionY) / 10f) + Main.timeForVisualEffects / 180.0);
		return Utils.Remap(num * uIVirtualKeyboard, 0f, 0.5f, 0f, 1f);
	}

	private static uint SimpleWhiteNoise(uint x, uint y)
	{
		x = 36469 * (x & 0xFFFF) + (x >> 16);
		y = 18012 * (y & 0xFFFF) + (y >> 16);
		return (x << 16) + y;
	}

	public int GetShimmerFrame(bool top, float worldPositionX, float worldPositionY)
	{
		worldPositionX += 0.5f;
		worldPositionY += 0.5f;
		double uIVirtualKeyboard = (double)((worldPositionX + worldPositionY / 6f) / 10f) - Main.timeForVisualEffects / 360.0;
		if (!top)
		{
			uIVirtualKeyboard += (double)(worldPositionX + worldPositionY);
		}
		return ((int)uIVirtualKeyboard % 16 + 16) % 16;
	}

	public static Vector4 GetShimmerBaseColor(float worldPositionX, float worldPositionY)
	{
		float shimmerWave = GetShimmerWave(ref worldPositionX, ref worldPositionY);
		return Vector4.Lerp(new Vector4(0.647058845f, 26f / 51f, 14f / 15f, 1f), new Vector4(41f / 51f, 41f / 51f, 1f, 1f), 0.1f + shimmerWave * 0.4f);
	}

	public bool HasFullWater(int x, int y)
	{
		x -= _drawArea.X;
		y -= _drawArea.Y;
		int num = x * _drawArea.Height + y;
		if (num >= 0 && num < _drawCache.Length)
		{
			if (_drawCache[num].IsVisible)
			{
				return !_drawCache[num].IsSurfaceLiquid;
			}
			return false;
		}
		return true;
	}

	public float GetVisibleLiquid(int x, int y)
	{
		x -= _drawArea.X;
		y -= _drawArea.Y;
		if (x < 0 || x >= _drawArea.Width || y < 0 || y >= _drawArea.Height)
		{
			return 0f;
		}
		int processedSeed = (x + 2) * (_drawArea.Height + 4) + y + 2;
		if (!_cache[processedSeed].HasVisibleLiquid)
		{
			return 0f;
		}
		return _cache[processedSeed].VisibleLiquidLevel;
	}

	public void Update(GameTime gameTime)
	{
		if (!Main.gamePaused && Main.hasFocus)
		{
			float num = Main.windSpeedCurrent * 25f;
			num = ((!(num < 0f)) ? (num + 6f) : (num - 6f));
			_frameState += num * (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (_frameState < 0f)
			{
				_frameState += 16f;
			}
			_frameState %= 16f;
			_animationFrame = (int)_frameState;
		}
	}

	public void PrepareDraw(Rectangle drawArea)
	{
		InternalPrepareDraw(drawArea);
	}

	public void SetWaveMaskData(ref Texture2D texture)
	{
		try
		{
			if (texture == null || texture.Width < _drawArea.Height || texture.Height < _drawArea.Width)
			{
				Console.WriteLine("WaveMaskData texture recreated. {0}x{1}", _drawArea.Height, _drawArea.Width);
				if (texture != null)
				{
					try
					{
						texture.Dispose();
					}
					catch
					{
					}
				}
				texture = new Texture2D(Main.instance.GraphicsDevice, _drawArea.Height, _drawArea.Width, mipMap: false, SurfaceFormat.Color);
			}
			texture.SetData(0, new Rectangle(0, 0, _drawArea.Height, _drawArea.Width), _waveMask, 0, _drawArea.Width * _drawArea.Height);
		}
		catch
		{
			texture = new Texture2D(Main.instance.GraphicsDevice, _drawArea.Height, _drawArea.Width, mipMap: false, SurfaceFormat.Color);
			texture.SetData(0, new Rectangle(0, 0, _drawArea.Height, _drawArea.Width), _waveMask, 0, _drawArea.Width * _drawArea.Height);
		}
	}

	public Rectangle GetCachedDrawArea()
	{
		return _drawArea;
	}
}
