using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Graphics.Shaders;
using Terraria.Utilities;

namespace Terraria;

public class Dust
{
	public static float dCount;

	public static int lavaBubbles;

	public static int SandStormCount;

	public int dustIndex;

	public Vector2 position;

	public Vector2 velocity;

	public float fadeIn;

	public bool noGravity;

	public float scale;

	public float rotation;

	public bool noLight;

	public bool noLightEmittence;

	public bool active;

	public int type;

	public Color color;

	public int alpha;

	public Rectangle frame;

	public ArmorShaderData shader;

	public object customData;

	public bool firstFrame;

	public static Dust NewDustPerfect(Vector2 Position, int Type, Vector2? Velocity = null, int Alpha = 0, Color newColor = default(Color), float Scale = 1f)
	{
		Dust bestPickaxe = Main.dust[NewDust(Position, 0, 0, Type, 0f, 0f, Alpha, newColor, Scale)];
		bestPickaxe.position = Position;
		if (Velocity.HasValue)
		{
			bestPickaxe.velocity = Velocity.Value;
		}
		return bestPickaxe;
	}

	public static Dust NewDustDirect(Vector2 Position, int Width, int Height, int Type, float SpeedX = 0f, float SpeedY = 0f, int Alpha = 0, Color newColor = default(Color), float Scale = 1f)
	{
		Dust num = Main.dust[NewDust(Position, Width, Height, Type, SpeedX, SpeedY, Alpha, newColor, Scale)];
		if (num.velocity.HasNaNs())
		{
			num.velocity = Vector2.Zero;
		}
		return num;
	}

	public static int NewDust(Vector2 Position, int Width, int Height, int Type, float SpeedX = 0f, float SpeedY = 0f, int Alpha = 0, Color newColor = default(Color), float Scale = 1f)
	{
		if (Main.gameMenu)
		{
			return 6000;
		}
		if (Main.rand == null)
		{
			Main.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
		}
		if (Main.gamePaused)
		{
			return 6000;
		}
		if (WorldGen.gen)
		{
			return 6000;
		}
		if (Main.netMode == 2)
		{
			return 6000;
		}
		int num = (int)(400f * (1f - dCount));
		Rectangle rectangle = new Rectangle((int)(Main.screenPosition.X - (float)num), (int)(Main.screenPosition.Y - (float)num), Main.screenWidth + num * 2, Main.screenHeight + num * 2);
		Rectangle value = new Rectangle((int)Position.X, (int)Position.Y, 10, 10);
		if (!rectangle.Intersects(value))
		{
			return 6000;
		}
		int result = 6000;
		for (int i = 0; i < 6000; i++)
		{
			Dust dust = Main.dust[i];
			if (dust.active)
			{
				continue;
			}
			if ((double)i > (double)Main.maxDustToDraw * 0.9)
			{
				if (Main.rand.Next(4) != 0)
				{
					return 6000;
				}
			}
			else if ((double)i > (double)Main.maxDustToDraw * 0.8)
			{
				if (Main.rand.Next(3) != 0)
				{
					return 6000;
				}
			}
			else if ((double)i > (double)Main.maxDustToDraw * 0.7)
			{
				if (Main.rand.Next(2) == 0)
				{
					return 6000;
				}
			}
			else if ((double)i > (double)Main.maxDustToDraw * 0.6)
			{
				if (Main.rand.Next(4) == 0)
				{
					return 6000;
				}
			}
			else if ((double)i > (double)Main.maxDustToDraw * 0.5)
			{
				if (Main.rand.Next(5) == 0)
				{
					return 6000;
				}
			}
			else
			{
				dCount = 0f;
			}
			int num2 = Width;
			int num3 = Height;
			if (num2 < 5)
			{
				num2 = 5;
			}
			if (num3 < 5)
			{
				num3 = 5;
			}
			result = i;
			dust.fadeIn = 0f;
			dust.active = true;
			dust.type = Type;
			dust.noGravity = false;
			dust.color = newColor;
			dust.alpha = Alpha;
			dust.position.X = Position.X + (float)Main.rand.Next(num2 - 4) + 4f;
			dust.position.Y = Position.Y + (float)Main.rand.Next(num3 - 4) + 4f;
			dust.velocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedX;
			dust.velocity.Y = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedY;
			dust.frame.X = 10 * Type;
			dust.frame.Y = 10 * Main.rand.Next(3);
			dust.shader = null;
			dust.customData = null;
			dust.noLightEmittence = false;
			int num4 = Type;
			while (num4 >= 100)
			{
				num4 -= 100;
				dust.frame.X -= 1000;
				dust.frame.Y += 30;
			}
			dust.frame.Width = 8;
			dust.frame.Height = 8;
			dust.rotation = 0f;
			dust.scale = 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
			dust.scale *= Scale;
			dust.noLight = false;
			dust.firstFrame = true;
			if (dust.type == 228 || dust.type == 279 || dust.type == 269 || dust.type == 135 || dust.type == 6 || dust.type == 242 || dust.type == 75 || dust.type == 169 || dust.type == 29 || (dust.type >= 59 && dust.type <= 65) || dust.type == 158 || dust.type == 293 || dust.type == 294 || dust.type == 295 || dust.type == 296 || dust.type == 297 || dust.type == 298 || dust.type == 302 || dust.type == 307 || dust.type == 310)
			{
				dust.velocity.Y = (float)Main.rand.Next(-10, 6) * 0.1f;
				dust.velocity.X *= 0.3f;
				dust.scale *= 0.7f;
			}
			if (dust.type == 127 || dust.type == 187)
			{
				dust.velocity *= 0.3f;
				dust.scale *= 0.7f;
			}
			if (dust.type == 308)
			{
				dust.velocity *= 0.5f;
				dust.velocity.Y += 1f;
			}
			if (dust.type == 33 || dust.type == 52 || dust.type == 266 || dust.type == 98 || dust.type == 99 || dust.type == 100 || dust.type == 101 || dust.type == 102 || dust.type == 103 || dust.type == 104 || dust.type == 105)
			{
				dust.alpha = 170;
				dust.velocity *= 0.5f;
				dust.velocity.Y += 1f;
			}
			if (dust.type == 41)
			{
				dust.velocity *= 0f;
			}
			if (dust.type == 80)
			{
				dust.alpha = 50;
			}
			if (dust.type == 34 || dust.type == 35 || dust.type == 152)
			{
				dust.velocity *= 0.1f;
				dust.velocity.Y = -0.5f;
				if (dust.type == 34 && !Collision.WetCollision(new Vector2(dust.position.X, dust.position.Y - 8f), 4, 4))
				{
					dust.active = false;
				}
			}
			break;
		}
		return result;
	}

	public static Dust CloneDust(int dustIndex)
	{
		return CloneDust(Main.dust[dustIndex]);
	}

	public static Dust CloneDust(Dust rf)
	{
		if (rf.dustIndex == Main.maxDustToDraw)
		{
			return rf;
		}
		int frameY = NewDust(rf.position, 0, 0, rf.type);
		Dust obj = Main.dust[frameY];
		obj.position = rf.position;
		obj.velocity = rf.velocity;
		obj.fadeIn = rf.fadeIn;
		obj.noGravity = rf.noGravity;
		obj.scale = rf.scale;
		obj.rotation = rf.rotation;
		obj.noLight = rf.noLight;
		obj.active = rf.active;
		obj.type = rf.type;
		obj.color = rf.color;
		obj.alpha = rf.alpha;
		obj.frame = rf.frame;
		obj.shader = rf.shader;
		obj.customData = rf.customData;
		return obj;
	}

	public static Dust QuickDust(int x, int y, Color color)
	{
		return QuickDust(new Point(x, y), color);
	}

	public static Dust QuickDust(Point tileCoords, Color color)
	{
		return QuickDust(tileCoords.ToWorldCoordinates(), color);
	}

	public static void QuickBox(Vector2 topLeft, Vector2 bottomRight, int divisions, Color color, Action<Dust> manipulator)
	{
		float num2 = divisions + 2;
		for (float num3 = 0f; num3 <= (float)(divisions + 2); num3 += 1f)
		{
			Dust num = QuickDust(new Vector2(MathHelper.Lerp(topLeft.X, bottomRight.X, num3 / num2), topLeft.Y), color);
			manipulator?.Invoke(num);
			num = QuickDust(new Vector2(MathHelper.Lerp(topLeft.X, bottomRight.X, num3 / num2), bottomRight.Y), color);
			manipulator?.Invoke(num);
			num = QuickDust(new Vector2(topLeft.X, MathHelper.Lerp(topLeft.Y, bottomRight.Y, num3 / num2)), color);
			manipulator?.Invoke(num);
			num = QuickDust(new Vector2(bottomRight.X, MathHelper.Lerp(topLeft.Y, bottomRight.Y, num3 / num2)), color);
			manipulator?.Invoke(num);
		}
	}

	public static void DrawDebugBox(Rectangle itemRectangle)
	{
		Vector2 g = itemRectangle.TopLeft();
		itemRectangle.BottomRight();
		for (int b = 0; b <= itemRectangle.Width; b++)
		{
			for (int num = 0; num <= itemRectangle.Height; num++)
			{
				if (b == 0 || num == 0 || b == itemRectangle.Width - 1 || num == itemRectangle.Height - 1)
				{
					QuickDust(g + new Vector2(b, num), Color.White).scale = 1f;
				}
			}
		}
	}

	public static Dust QuickDust(Vector2 pos, Color color)
	{
		Dust obj = Main.dust[NewDust(pos, 0, 0, 267)];
		obj.position = pos;
		obj.velocity = Vector2.Zero;
		obj.fadeIn = 1f;
		obj.noLight = true;
		obj.noGravity = true;
		obj.color = color;
		return obj;
	}

	public static Dust QuickDustSmall(Vector2 pos, Color color, bool floorPositionValues = false)
	{
		Dust num = QuickDust(pos, color);
		num.fadeIn = 0f;
		num.scale = 0.35f;
		if (floorPositionValues)
		{
			num.position = num.position.Floor();
		}
		return num;
	}

	public static void QuickDustLine(Vector2 start, Vector2 end, float splits, Color color)
	{
		QuickDust(start, color).scale = 0.3f;
		QuickDust(end, color).scale = 0.3f;
		float timeSpan = 1f / splits;
		for (float num = 0f; num < 1f; num += timeSpan)
		{
			QuickDust(Vector2.Lerp(start, end, num), color).scale = 0.3f;
		}
	}

	public static int dustWater()
	{
		return Main.waterStyle switch
		{
			2 => 98, 
			3 => 99, 
			4 => 100, 
			5 => 101, 
			6 => 102, 
			7 => 103, 
			8 => 104, 
			9 => 105, 
			10 => 123, 
			12 => 288, 
			_ => 33, 
		};
	}

	public static void UpdateDust()
	{
		int i = 0;
		lavaBubbles = 0;
		Main.snowDust = 0;
		SandStormCount = 0;
		bool flag = Sandstorm.ShouldSandstormDustPersist();
		for (int num = 0; num < 6000; num++)
		{
			Dust num2 = Main.dust[num];
			if (num < Main.maxDustToDraw)
			{
				if (!num2.active)
				{
					continue;
				}
				dCount += 1f;
				if (num2.scale > 10f)
				{
					num2.active = false;
				}
				if (num2.firstFrame && !ChildSafety.Disabled && ChildSafety.DangerousDust(num2.type))
				{
					if (Main.rand.Next(2) == 0)
					{
						num2.firstFrame = false;
						num2.type = 16;
						num2.scale = Main.rand.NextFloat() * 1.6f + 0.3f;
						num2.color = Color.Transparent;
						num2.frame.X = 10 * num2.type;
						num2.frame.Y = 10 * Main.rand.Next(3);
						num2.shader = null;
						num2.customData = null;
						int num4 = num2.type / 100;
						num2.frame.X -= 1000 * num4;
						num2.frame.Y += 30 * num4;
						num2.noGravity = true;
					}
					else
					{
						num2.active = false;
					}
				}
				int num5 = num2.type;
				if ((uint)(num5 - 299) <= 2u || num5 == 305)
				{
					num2.scale *= 0.96f;
					num2.velocity.Y -= 0.01f;
				}
				if (num2.type == 35)
				{
					lavaBubbles++;
				}
				num2.position += num2.velocity;
				if (num2.type == 258)
				{
					num2.noGravity = true;
					num2.scale += 0.015f;
				}
				if (num2.type == 309)
				{
					float r = (float)(int)num2.color.R / 255f * num2.scale;
					float g = (float)(int)num2.color.G / 255f * num2.scale;
					float b = (float)(int)num2.color.B / 255f * num2.scale;
					Lighting.AddLight(num2.position, r, g, b);
					num2.scale *= 0.97f;
				}
				if (((num2.type >= 86 && num2.type <= 92) || num2.type == 286) && !num2.noLight && !num2.noLightEmittence)
				{
					float num6 = num2.scale * 0.6f;
					if (num6 > 1f)
					{
						num6 = 1f;
					}
					int num7 = num2.type - 85;
					float num8 = num6;
					float num9 = num6;
					float num10 = num6;
					switch (num7)
					{
					case 3:
						num8 *= 0f;
						num9 *= 0.1f;
						num10 *= 1.3f;
						break;
					case 5:
						num8 *= 1f;
						num9 *= 0.1f;
						num10 *= 0.1f;
						break;
					case 4:
						num8 *= 0f;
						num9 *= 1f;
						num10 *= 0.1f;
						break;
					case 1:
						num8 *= 0.9f;
						num9 *= 0f;
						num10 *= 0.9f;
						break;
					case 6:
						num8 *= 1.3f;
						num9 *= 1.3f;
						num10 *= 1.3f;
						break;
					case 2:
						num8 *= 0.9f;
						num9 *= 0.9f;
						num10 *= 0f;
						break;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num6 * num8, num6 * num9, num6 * num10);
				}
				if ((num2.type >= 86 && num2.type <= 92) || num2.type == 286)
				{
					if (num2.customData != null && num2.customData is Player)
					{
						Player player = (Player)num2.customData;
						num2.position += player.position - player.oldPosition;
					}
					else if (num2.customData != null && num2.customData is Projectile)
					{
						Projectile projectile = (Projectile)num2.customData;
						if (projectile.active)
						{
							num2.position += projectile.position - projectile.oldPosition;
						}
					}
				}
				if (num2.type == 262 && !num2.noLight)
				{
					Vector3 rgb = new Vector3(0.9f, 0.6f, 0f) * num2.scale * 0.6f;
					Lighting.AddLight(num2.position, rgb);
				}
				if (num2.type == 240 && num2.customData != null && num2.customData is Projectile)
				{
					Projectile projectile2 = (Projectile)num2.customData;
					if (projectile2.active)
					{
						num2.position += projectile2.position - projectile2.oldPosition;
					}
				}
				if ((num2.type == 259 || num2.type == 6 || num2.type == 158 || num2.type == 135) && num2.customData != null && num2.customData is int)
				{
					if ((int)num2.customData == 0)
					{
						if (Collision.SolidCollision(num2.position - Vector2.One * 5f, 10, 10) && num2.fadeIn == 0f)
						{
							num2.scale *= 0.9f;
							num2.velocity *= 0.25f;
						}
					}
					else if ((int)num2.customData == 1)
					{
						num2.scale *= 0.98f;
						num2.velocity.Y *= 0.98f;
						if (Collision.SolidCollision(num2.position - Vector2.One * 5f, 10, 10) && num2.fadeIn == 0f)
						{
							num2.scale *= 0.9f;
							num2.velocity *= 0.25f;
						}
					}
				}
				if (num2.type == 263 || num2.type == 264)
				{
					if (!num2.noLight)
					{
						Vector3 rgb2 = num2.color.ToVector3() * num2.scale * 0.4f;
						Lighting.AddLight(num2.position, rgb2);
					}
					if (num2.customData != null && num2.customData is Player)
					{
						Player player2 = (Player)num2.customData;
						num2.position += player2.position - player2.oldPosition;
						num2.customData = null;
					}
					else if (num2.customData != null && num2.customData is Projectile)
					{
						Projectile projectile3 = (Projectile)num2.customData;
						num2.position += projectile3.position - projectile3.oldPosition;
					}
				}
				if (num2.type == 230)
				{
					float num11 = num2.scale * 0.6f;
					float num12 = num11;
					float num13 = num11;
					float num14 = num11;
					num12 *= 0.5f;
					num13 *= 0.9f;
					num14 *= 1f;
					num2.scale += 0.02f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num11 * num12, num11 * num13, num11 * num14);
					if (num2.customData != null && num2.customData is Player)
					{
						Vector2 center = ((Player)num2.customData).Center;
						Vector2 vector = num2.position - center;
						float num15 = vector.Length();
						vector /= num15;
						num2.scale = Math.Min(num2.scale, num15 / 24f - 1f);
						num2.velocity -= vector * (100f / Math.Max(50f, num15));
					}
				}
				if (num2.type == 154 || num2.type == 218)
				{
					num2.rotation += num2.velocity.X * 0.3f;
					num2.scale -= 0.03f;
				}
				if (num2.type == 172)
				{
					float num16 = num2.scale * 0.5f;
					if (num16 > 1f)
					{
						num16 = 1f;
					}
					float num17 = num16;
					float num18 = num16;
					float num19 = num16;
					num17 *= 0f;
					num18 *= 0.25f;
					num19 *= 1f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num16 * num17, num16 * num18, num16 * num19);
				}
				if (num2.type == 182)
				{
					num2.rotation += 1f;
					if (!num2.noLight)
					{
						float num20 = num2.scale * 0.25f;
						if (num20 > 1f)
						{
							num20 = 1f;
						}
						float num21 = num20;
						float num22 = num20;
						float num23 = num20;
						num21 *= 1f;
						num22 *= 0.2f;
						num23 *= 0.1f;
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num20 * num21, num20 * num22, num20 * num23);
					}
					if (num2.customData != null && num2.customData is Player)
					{
						Player player3 = (Player)num2.customData;
						num2.position += player3.position - player3.oldPosition;
						num2.customData = null;
					}
				}
				if (num2.type == 261)
				{
					if (!num2.noLight && !num2.noLightEmittence)
					{
						float num24 = num2.scale * 0.3f;
						if (num24 > 1f)
						{
							num24 = 1f;
						}
						Lighting.AddLight(num2.position, new Vector3(0.4f, 0.6f, 0.7f) * num24);
					}
					if (num2.noGravity)
					{
						num2.velocity *= 0.93f;
						if (num2.fadeIn == 0f)
						{
							num2.scale += 0.0025f;
						}
					}
					num2.velocity *= new Vector2(0.97f, 0.99f);
					num2.scale -= 0.0025f;
					if (num2.customData != null && num2.customData is Player)
					{
						Player player4 = (Player)num2.customData;
						num2.position += player4.position - player4.oldPosition;
					}
				}
				if (num2.type == 254)
				{
					float num25 = num2.scale * 0.35f;
					if (num25 > 1f)
					{
						num25 = 1f;
					}
					float num26 = num25;
					float num27 = num25;
					float num28 = num25;
					num26 *= 0.9f;
					num27 *= 0.1f;
					num28 *= 0.75f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num25 * num26, num25 * num27, num25 * num28);
				}
				if (num2.type == 255)
				{
					float num29 = num2.scale * 0.25f;
					if (num29 > 1f)
					{
						num29 = 1f;
					}
					float num30 = num29;
					float num31 = num29;
					float num32 = num29;
					num30 *= 0.9f;
					num31 *= 0.1f;
					num32 *= 0.75f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num29 * num30, num29 * num31, num29 * num32);
				}
				if (num2.type == 211 && num2.noLight && Collision.SolidCollision(num2.position, 4, 4))
				{
					num2.active = false;
				}
				if (num2.type == 284 && Collision.SolidCollision(num2.position - Vector2.One * 4f, 8, 8) && num2.fadeIn == 0f)
				{
					num2.velocity *= 0.25f;
				}
				if (num2.type == 213 || num2.type == 260)
				{
					num2.rotation = 0f;
					float num33 = num2.scale / 2.5f * 0.2f;
					Vector3 vector2 = Vector3.Zero;
					switch (num2.type)
					{
					case 213:
						vector2 = new Vector3(255f, 217f, 48f);
						break;
					case 260:
						vector2 = new Vector3(255f, 48f, 48f);
						break;
					}
					vector2 /= 255f;
					if (num33 > 1f)
					{
						num33 = 1f;
					}
					vector2 *= num33;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), vector2.X, vector2.Y, vector2.Z);
				}
				if (num2.type == 157)
				{
					float num34 = num2.scale * 0.2f;
					float num35 = num34;
					float num36 = num34;
					float num37 = num34;
					num35 *= 0.25f;
					num36 *= 1f;
					num37 *= 0.5f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num34 * num35, num34 * num36, num34 * num37);
				}
				if (num2.type == 206)
				{
					num2.scale -= 0.1f;
					float num38 = num2.scale * 0.4f;
					float num39 = num38;
					float num40 = num38;
					float num41 = num38;
					num39 *= 0.1f;
					num40 *= 0.6f;
					num41 *= 1f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num38 * num39, num38 * num40, num38 * num41);
				}
				if (num2.type == 163)
				{
					float num42 = num2.scale * 0.25f;
					float num43 = num42;
					float num44 = num42;
					float num45 = num42;
					num43 *= 0.25f;
					num44 *= 1f;
					num45 *= 0.05f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num42 * num43, num42 * num44, num42 * num45);
				}
				if (num2.type == 205)
				{
					float num46 = num2.scale * 0.25f;
					float num47 = num46;
					float num48 = num46;
					float num49 = num46;
					num47 *= 1f;
					num48 *= 0.05f;
					num49 *= 1f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num46 * num47, num46 * num48, num46 * num49);
				}
				if (num2.type == 170)
				{
					float num50 = num2.scale * 0.5f;
					float num51 = num50;
					float num52 = num50;
					float num53 = num50;
					num51 *= 1f;
					num52 *= 1f;
					num53 *= 0.05f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num50 * num51, num50 * num52, num50 * num53);
				}
				if (num2.type == 156)
				{
					float num54 = num2.scale * 0.6f;
					_ = num2.type;
					float num55 = num54;
					float num56 = num54;
					num55 *= 0.9f;
					num56 *= 1f;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 12, num54);
				}
				if (num2.type == 234)
				{
					float lightAmount = num2.scale * 0.6f;
					_ = num2.type;
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 13, lightAmount);
				}
				if (num2.type == 175)
				{
					num2.scale -= 0.05f;
				}
				if (num2.type == 174)
				{
					num2.scale -= 0.01f;
					float num57 = num2.scale * 1f;
					if (num57 > 0.6f)
					{
						num57 = 0.6f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num57, num57 * 0.4f, 0f);
				}
				if (num2.type == 235)
				{
					Vector2 vector3 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
					vector3.Normalize();
					vector3 *= 15f;
					num2.scale -= 0.01f;
				}
				else if (num2.type == 228 || num2.type == 279 || num2.type == 229 || num2.type == 6 || num2.type == 242 || num2.type == 135 || num2.type == 127 || num2.type == 187 || num2.type == 75 || num2.type == 169 || num2.type == 29 || (num2.type >= 59 && num2.type <= 65) || num2.type == 158 || num2.type == 293 || num2.type == 294 || num2.type == 295 || num2.type == 296 || num2.type == 297 || num2.type == 298 || num2.type == 302 || num2.type == 307 || num2.type == 310)
				{
					if (!num2.noGravity)
					{
						num2.velocity.Y += 0.05f;
					}
					if (num2.type == 229 || num2.type == 228 || num2.type == 279)
					{
						if (num2.customData != null && num2.customData is NPC)
						{
							NPC nPC = (NPC)num2.customData;
							num2.position += nPC.position - nPC.oldPos[1];
						}
						else if (num2.customData != null && num2.customData is Player)
						{
							Player player5 = (Player)num2.customData;
							num2.position += player5.position - player5.oldPosition;
						}
						else if (num2.customData != null && num2.customData is Vector2)
						{
							Vector2 vector4 = (Vector2)num2.customData - num2.position;
							if (vector4 != Vector2.Zero)
							{
								vector4.Normalize();
							}
							num2.velocity = (num2.velocity * 4f + vector4 * num2.velocity.Length()) / 5f;
						}
					}
					if (!num2.noLight && !num2.noLightEmittence)
					{
						float num58 = num2.scale * 1.4f;
						if (num2.type == 29)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num58 * 0.1f, num58 * 0.4f, num58);
						}
						else if (num2.type == 75)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							if (num2.customData is float)
							{
								Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 8, num58 * (float)num2.customData);
							}
							else
							{
								Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 8, num58);
							}
						}
						else if (num2.type == 169)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 11, num58);
						}
						else if (num2.type == 135)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 9, num58);
						}
						else if (num2.type == 158)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 10, num58);
						}
						else if (num2.type == 228)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num58 * 0.7f, num58 * 0.65f, num58 * 0.3f);
						}
						else if (num2.type == 229)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num58 * 0.3f, num58 * 0.65f, num58 * 0.7f);
						}
						else if (num2.type == 242)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 15, num58);
						}
						else if (num2.type == 293)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							num58 *= 0.95f;
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 16, num58);
						}
						else if (num2.type == 294)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 17, num58);
						}
						else if (num2.type >= 59 && num2.type <= 65)
						{
							if (num58 > 0.8f)
							{
								num58 = 0.8f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 1 + num2.type - 59, num58);
						}
						else if (num2.type == 127)
						{
							num58 *= 1.3f;
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num58, num58 * 0.45f, num58 * 0.2f);
						}
						else if (num2.type == 187)
						{
							num58 *= 1.3f;
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num58 * 0.2f, num58 * 0.45f, num58);
						}
						else if (num2.type == 295)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 18, num58);
						}
						else if (num2.type == 296)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 19, num58);
						}
						else if (num2.type == 297)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 20, num58);
						}
						else if (num2.type == 298)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 21, num58);
						}
						else if (num2.type == 307)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 22, num58);
						}
						else if (num2.type == 310)
						{
							if (num58 > 1f)
							{
								num58 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 23, num58);
						}
						else
						{
							if (num58 > 0.6f)
							{
								num58 = 0.6f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num58, num58 * 0.65f, num58 * 0.4f);
						}
					}
				}
				else if (num2.type == 306)
				{
					if (!num2.noGravity)
					{
						num2.velocity.Y += 0.05f;
					}
					num2.scale -= 0.04f;
					if (Collision.SolidCollision(num2.position - Vector2.One * 5f, 10, 10) && num2.fadeIn == 0f)
					{
						num2.scale *= 0.9f;
						num2.velocity *= 0.25f;
					}
				}
				else if (num2.type == 269)
				{
					if (!num2.noLight)
					{
						float num59 = num2.scale * 1.4f;
						if (num59 > 1f)
						{
							num59 = 1f;
						}
						Lighting.AddLight(rgb: new Vector3(0.7f, 0.65f, 0.3f) * num59, position: num2.position);
					}
					if (num2.customData != null && num2.customData is Vector2)
					{
						Vector2 vector5 = (Vector2)num2.customData - num2.position;
						num2.velocity.X += 1f * (float)Math.Sign(vector5.X) * num2.scale;
					}
				}
				else if (num2.type == 159)
				{
					float num60 = num2.scale * 1.3f;
					if (num60 > 1f)
					{
						num60 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num60, num60, num60 * 0.1f);
					if (num2.noGravity)
					{
						if (num2.scale < 0.7f)
						{
							num2.velocity *= 1.075f;
						}
						else if (Main.rand.Next(2) == 0)
						{
							num2.velocity *= -0.95f;
						}
						else
						{
							num2.velocity *= 1.05f;
						}
						num2.scale -= 0.03f;
					}
					else
					{
						num2.scale += 0.005f;
						num2.velocity *= 0.9f;
						num2.velocity.X += (float)Main.rand.Next(-10, 11) * 0.02f;
						num2.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.02f;
						if (Main.rand.Next(5) == 0)
						{
							int num61 = NewDust(num2.position, 4, 4, num2.type);
							Main.dust[num61].noGravity = true;
							Main.dust[num61].scale = num2.scale * 2.5f;
						}
					}
				}
				else if (num2.type == 164)
				{
					float num62 = num2.scale;
					if (num62 > 1f)
					{
						num62 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num62, num62 * 0.1f, num62 * 0.8f);
					if (num2.noGravity)
					{
						if (num2.scale < 0.7f)
						{
							num2.velocity *= 1.075f;
						}
						else if (Main.rand.Next(2) == 0)
						{
							num2.velocity *= -0.95f;
						}
						else
						{
							num2.velocity *= 1.05f;
						}
						num2.scale -= 0.03f;
					}
					else
					{
						num2.scale -= 0.005f;
						num2.velocity *= 0.9f;
						num2.velocity.X += (float)Main.rand.Next(-10, 11) * 0.02f;
						num2.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.02f;
						if (Main.rand.Next(5) == 0)
						{
							int num63 = NewDust(num2.position, 4, 4, num2.type);
							Main.dust[num63].noGravity = true;
							Main.dust[num63].scale = num2.scale * 2.5f;
						}
					}
				}
				else if (num2.type == 173)
				{
					float num64 = num2.scale;
					if (num64 > 1f)
					{
						num64 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num64 * 0.4f, num64 * 0.1f, num64);
					if (num2.noGravity)
					{
						num2.velocity *= 0.8f;
						num2.velocity.X += (float)Main.rand.Next(-20, 21) * 0.01f;
						num2.velocity.Y += (float)Main.rand.Next(-20, 21) * 0.01f;
						num2.scale -= 0.01f;
					}
					else
					{
						num2.scale -= 0.015f;
						num2.velocity *= 0.8f;
						num2.velocity.X += (float)Main.rand.Next(-10, 11) * 0.005f;
						num2.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.005f;
						if (Main.rand.Next(10) == 10)
						{
							int num65 = NewDust(num2.position, 4, 4, num2.type);
							Main.dust[num65].noGravity = true;
							Main.dust[num65].scale = num2.scale;
						}
					}
				}
				else if (num2.type == 304)
				{
					num2.velocity.Y = (float)Math.Sin(num2.rotation) / 5f;
					num2.rotation += 0.015f;
					if (num2.scale < 1.15f)
					{
						num2.alpha = Math.Max(0, num2.alpha - 20);
						num2.scale += 0.0015f;
					}
					else
					{
						num2.alpha += 6;
						if (num2.alpha >= 255)
						{
							num2.active = false;
						}
					}
					if (num2.customData != null && num2.customData is Player)
					{
						Player player6 = (Player)num2.customData;
						float num66 = Utils.Remap(num2.scale, 1f, 1.05f, 1f, 0f);
						if (num66 > 0f)
						{
							num2.position += player6.velocity * num66;
						}
						float num67 = player6.Center.X - num2.position.X;
						if (Math.Abs(num67) > 20f)
						{
							float value = num67 * 0.01f;
							num2.velocity.X = MathHelper.Lerp(num2.velocity.X, value, num66 * 0.2f);
						}
					}
				}
				else if (num2.type == 184)
				{
					if (!num2.noGravity)
					{
						num2.velocity *= 0f;
						num2.scale -= 0.01f;
					}
				}
				else if (num2.type == 160 || num2.type == 162)
				{
					float num68 = num2.scale * 1.3f;
					if (num68 > 1f)
					{
						num68 = 1f;
					}
					if (num2.type == 162)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num68, num68 * 0.7f, num68 * 0.1f);
					}
					else
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num68 * 0.1f, num68, num68);
					}
					if (num2.noGravity)
					{
						num2.velocity *= 0.8f;
						num2.velocity.X += (float)Main.rand.Next(-20, 21) * 0.04f;
						num2.velocity.Y += (float)Main.rand.Next(-20, 21) * 0.04f;
						num2.scale -= 0.1f;
					}
					else
					{
						num2.scale -= 0.1f;
						num2.velocity.X += (float)Main.rand.Next(-10, 11) * 0.02f;
						num2.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.02f;
						if ((double)num2.scale > 0.3 && Main.rand.Next(50) == 0)
						{
							int num69 = NewDust(new Vector2(num2.position.X - 4f, num2.position.Y - 4f), 1, 1, num2.type);
							Main.dust[num69].noGravity = true;
							Main.dust[num69].scale = num2.scale * 1.5f;
						}
					}
				}
				else if (num2.type == 168)
				{
					float num70 = num2.scale * 0.8f;
					if ((double)num70 > 0.55)
					{
						num70 = 0.55f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num70, 0f, num70 * 0.8f);
					num2.scale += 0.03f;
					num2.velocity.X += (float)Main.rand.Next(-10, 11) * 0.02f;
					num2.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.02f;
					num2.velocity *= 0.99f;
				}
				else if (num2.type >= 139 && num2.type < 143)
				{
					num2.velocity.X *= 0.98f;
					num2.velocity.Y *= 0.98f;
					if (num2.velocity.Y < 1f)
					{
						num2.velocity.Y += 0.05f;
					}
					num2.scale += 0.009f;
					num2.rotation -= num2.velocity.X * 0.4f;
					if (num2.velocity.X > 0f)
					{
						num2.rotation += 0.005f;
					}
					else
					{
						num2.rotation -= 0.005f;
					}
				}
				else if (num2.type == 14 || num2.type == 16 || num2.type == 31 || num2.type == 46 || num2.type == 124 || num2.type == 186 || num2.type == 188 || num2.type == 303)
				{
					num2.velocity.Y *= 0.98f;
					num2.velocity.X *= 0.98f;
					if (num2.type == 31)
					{
						if (num2.customData != null && num2.customData is float)
						{
							float num71 = (float)num2.customData;
							num2.velocity.Y += num71;
						}
						if (num2.customData != null && num2.customData is NPC)
						{
							NPC nPC2 = (NPC)num2.customData;
							num2.position += nPC2.position - nPC2.oldPosition;
							if (num2.noGravity)
							{
								num2.velocity *= 1.02f;
							}
							num2.alpha -= 70;
							if (num2.alpha < 0)
							{
								num2.alpha = 0;
							}
							num2.scale *= 0.97f;
							if (num2.scale <= 0.01f)
							{
								num2.scale = 0.0001f;
								num2.alpha = 255;
							}
						}
						else if (num2.noGravity)
						{
							num2.velocity *= 1.02f;
							num2.scale += 0.02f;
							num2.alpha += 4;
							if (num2.alpha > 255)
							{
								num2.scale = 0.0001f;
								num2.alpha = 255;
							}
						}
					}
					if (num2.type == 303 && num2.noGravity)
					{
						num2.velocity *= 1.02f;
						num2.scale += 0.03f;
						if (num2.alpha < 90)
						{
							num2.alpha = 90;
						}
						num2.alpha += 4;
						if (num2.alpha > 255)
						{
							num2.scale = 0.0001f;
							num2.alpha = 255;
						}
					}
				}
				else if (num2.type == 32)
				{
					num2.scale -= 0.01f;
					num2.velocity.X *= 0.96f;
					if (!num2.noGravity)
					{
						num2.velocity.Y += 0.1f;
					}
				}
				else if (num2.type >= 244 && num2.type <= 247)
				{
					num2.rotation += 0.1f * num2.scale;
					Color color = Lighting.GetColor((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f));
					byte num72 = (byte)((color.R + color.G + color.B) / 3);
					float num73 = ((float)(int)num72 / 270f + 1f) / 2f;
					float num74 = ((float)(int)num72 / 270f + 1f) / 2f;
					float num75 = ((float)(int)num72 / 270f + 1f) / 2f;
					num73 *= num2.scale * 0.9f;
					num74 *= num2.scale * 0.9f;
					num75 *= num2.scale * 0.9f;
					if (num2.alpha < 255)
					{
						num2.scale += 0.09f;
						if (num2.scale >= 1f)
						{
							num2.scale = 1f;
							num2.alpha = 255;
						}
					}
					else
					{
						if ((double)num2.scale < 0.8)
						{
							num2.scale -= 0.01f;
						}
						if ((double)num2.scale < 0.5)
						{
							num2.scale -= 0.01f;
						}
					}
					float num76 = 1f;
					if (num2.type == 244)
					{
						num73 *= 0.8862745f;
						num74 *= 0.4627451f;
						num75 *= 0.298039228f;
						num76 = 0.9f;
					}
					else if (num2.type == 245)
					{
						num73 *= 0.5137255f;
						num74 *= 0.6745098f;
						num75 *= 0.6784314f;
						num76 = 1f;
					}
					else if (num2.type == 246)
					{
						num73 *= 0.8f;
						num74 *= 0.709803939f;
						num75 *= 24f / 85f;
						num76 = 1.1f;
					}
					else if (num2.type == 247)
					{
						num73 *= 0.6f;
						num74 *= 0.6745098f;
						num75 *= 37f / 51f;
						num76 = 1.2f;
					}
					num73 *= num76;
					num74 *= num76;
					num75 *= num76;
					if (!num2.noLightEmittence)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num73, num74, num75);
					}
				}
				else if (num2.type == 43)
				{
					num2.rotation += 0.1f * num2.scale;
					Color color2 = Lighting.GetColor((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f));
					float num77 = (float)(int)color2.R / 270f;
					float num78 = (float)(int)color2.G / 270f;
					float num79 = (float)(int)color2.B / 270f;
					float num80 = (float)(int)num2.color.R / 255f;
					float num81 = (float)(int)num2.color.G / 255f;
					float num82 = (float)(int)num2.color.B / 255f;
					num77 *= num2.scale * 1.07f * num80;
					num78 *= num2.scale * 1.07f * num81;
					num79 *= num2.scale * 1.07f * num82;
					if (num2.alpha < 255)
					{
						num2.scale += 0.09f;
						if (num2.scale >= 1f)
						{
							num2.scale = 1f;
							num2.alpha = 255;
						}
					}
					else
					{
						if ((double)num2.scale < 0.8)
						{
							num2.scale -= 0.01f;
						}
						if ((double)num2.scale < 0.5)
						{
							num2.scale -= 0.01f;
						}
					}
					if ((double)num77 < 0.05 && (double)num78 < 0.05 && (double)num79 < 0.05)
					{
						num2.active = false;
					}
					else if (!num2.noLightEmittence)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num77, num78, num79);
					}
					if (num2.customData != null && num2.customData is Player)
					{
						Player player7 = (Player)num2.customData;
						num2.position += player7.position - player7.oldPosition;
					}
				}
				else if (num2.type == 15 || num2.type == 57 || num2.type == 58 || num2.type == 274 || num2.type == 292)
				{
					num2.velocity.Y *= 0.98f;
					num2.velocity.X *= 0.98f;
					if (!num2.noLightEmittence)
					{
						float num83 = num2.scale;
						if (num2.type != 15)
						{
							num83 = num2.scale * 0.8f;
						}
						if (num2.noLight)
						{
							num2.velocity *= 0.95f;
						}
						if (num83 > 1f)
						{
							num83 = 1f;
						}
						if (num2.type == 15)
						{
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num83 * 0.45f, num83 * 0.55f, num83);
						}
						else if (num2.type == 57)
						{
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num83 * 0.95f, num83 * 0.95f, num83 * 0.45f);
						}
						else if (num2.type == 58)
						{
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num83, num83 * 0.55f, num83 * 0.75f);
						}
					}
				}
				else if (num2.type == 204)
				{
					if (num2.fadeIn > num2.scale)
					{
						num2.scale += 0.02f;
					}
					else
					{
						num2.scale -= 0.02f;
					}
					num2.velocity *= 0.95f;
				}
				else if (num2.type == 110)
				{
					float num84 = num2.scale * 0.1f;
					if (num84 > 1f)
					{
						num84 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num84 * 0.2f, num84, num84 * 0.5f);
				}
				else if (num2.type == 111)
				{
					float num85 = num2.scale * 0.125f;
					if (num85 > 1f)
					{
						num85 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num85 * 0.2f, num85 * 0.7f, num85);
				}
				else if (num2.type == 112)
				{
					float num86 = num2.scale * 0.1f;
					if (num86 > 1f)
					{
						num86 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num86 * 0.8f, num86 * 0.2f, num86 * 0.8f);
				}
				else if (num2.type == 113)
				{
					float num87 = num2.scale * 0.1f;
					if (num87 > 1f)
					{
						num87 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num87 * 0.2f, num87 * 0.3f, num87 * 1.3f);
				}
				else if (num2.type == 114)
				{
					float num88 = num2.scale * 0.1f;
					if (num88 > 1f)
					{
						num88 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num88 * 1.2f, num88 * 0.5f, num88 * 0.4f);
				}
				else if (num2.type == 311)
				{
					float num89 = num2.scale * 0.1f;
					if (num89 > 1f)
					{
						num89 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 16, num89);
				}
				else if (num2.type == 312)
				{
					float num90 = num2.scale * 0.1f;
					if (num90 > 1f)
					{
						num90 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 9, num90);
				}
				else if (num2.type == 313)
				{
					float num91 = num2.scale * 0.25f;
					if (num91 > 1f)
					{
						num91 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num91 * 1f, num91 * 0.8f, num91 * 0.6f);
				}
				else if (num2.type == 66)
				{
					if (num2.velocity.X < 0f)
					{
						num2.rotation -= 1f;
					}
					else
					{
						num2.rotation += 1f;
					}
					num2.velocity.Y *= 0.98f;
					num2.velocity.X *= 0.98f;
					num2.scale += 0.02f;
					float num92 = num2.scale;
					if (num2.type != 15)
					{
						num92 = num2.scale * 0.8f;
					}
					if (num92 > 1f)
					{
						num92 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num92 * ((float)(int)num2.color.R / 255f), num92 * ((float)(int)num2.color.G / 255f), num92 * ((float)(int)num2.color.B / 255f));
				}
				else if (num2.type == 267)
				{
					if (num2.velocity.X < 0f)
					{
						num2.rotation -= 1f;
					}
					else
					{
						num2.rotation += 1f;
					}
					num2.velocity.Y *= 0.98f;
					num2.velocity.X *= 0.98f;
					num2.scale += 0.02f;
					float num93 = num2.scale * 0.8f;
					if (num93 > 1f)
					{
						num93 = 1f;
					}
					if (num2.noLight)
					{
						num2.noLight = false;
					}
					if (!num2.noLight && !num2.noLightEmittence)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num93 * ((float)(int)num2.color.R / 255f), num93 * ((float)(int)num2.color.G / 255f), num93 * ((float)(int)num2.color.B / 255f));
					}
				}
				else if (num2.type == 20 || num2.type == 21 || num2.type == 231)
				{
					num2.scale += 0.005f;
					num2.velocity.Y *= 0.94f;
					num2.velocity.X *= 0.94f;
					float num94 = num2.scale * 0.8f;
					if (num94 > 1f)
					{
						num94 = 1f;
					}
					if (num2.type == 21 && !num2.noLightEmittence)
					{
						num94 = num2.scale * 0.4f;
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num94 * 0.8f, num94 * 0.3f, num94);
					}
					else if (num2.type == 231)
					{
						num94 = num2.scale * 0.4f;
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num94, num94 * 0.5f, num94 * 0.3f);
					}
					else
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num94 * 0.3f, num94 * 0.6f, num94);
					}
				}
				else if (num2.type == 27 || num2.type == 45)
				{
					if (num2.type == 27 && num2.fadeIn >= 100f)
					{
						if ((double)num2.scale >= 1.5)
						{
							num2.scale -= 0.01f;
						}
						else
						{
							num2.scale -= 0.05f;
						}
						if ((double)num2.scale <= 0.5)
						{
							num2.scale -= 0.05f;
						}
						if ((double)num2.scale <= 0.25)
						{
							num2.scale -= 0.05f;
						}
					}
					num2.velocity *= 0.94f;
					num2.scale += 0.002f;
					float num95 = num2.scale;
					if (num2.noLight)
					{
						num95 *= 0.1f;
						num2.scale -= 0.06f;
						if (num2.scale < 1f)
						{
							num2.scale -= 0.06f;
						}
						if (Main.player[Main.myPlayer].wet)
						{
							num2.position += Main.player[Main.myPlayer].velocity * 0.5f;
						}
						else
						{
							num2.position += Main.player[Main.myPlayer].velocity;
						}
					}
					if (num95 > 1f)
					{
						num95 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num95 * 0.6f, num95 * 0.2f, num95);
				}
				else if (num2.type == 55 || num2.type == 56 || num2.type == 73 || num2.type == 74)
				{
					num2.velocity *= 0.98f;
					if (!num2.noLightEmittence)
					{
						float num96 = num2.scale * 0.8f;
						if (num2.type == 55)
						{
							if (num96 > 1f)
							{
								num96 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num96, num96, num96 * 0.6f);
						}
						else if (num2.type == 73)
						{
							if (num96 > 1f)
							{
								num96 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num96, num96 * 0.35f, num96 * 0.5f);
						}
						else if (num2.type == 74)
						{
							if (num96 > 1f)
							{
								num96 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num96 * 0.35f, num96, num96 * 0.5f);
						}
						else
						{
							num96 = num2.scale * 1.2f;
							if (num96 > 1f)
							{
								num96 = 1f;
							}
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num96 * 0.35f, num96 * 0.5f, num96);
						}
					}
				}
				else if (num2.type == 71 || num2.type == 72)
				{
					num2.velocity *= 0.98f;
					float num97 = num2.scale;
					if (num97 > 1f)
					{
						num97 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num97 * 0.2f, 0f, num97 * 0.1f);
				}
				else if (num2.type == 76)
				{
					Main.snowDust++;
					num2.scale += 0.009f;
					float y = Main.player[Main.myPlayer].velocity.Y;
					if (y > 0f && num2.fadeIn == 0f && num2.velocity.Y < y)
					{
						num2.velocity.Y = MathHelper.Lerp(num2.velocity.Y, y, 0.04f);
					}
					if (!num2.noLight && y > 0f)
					{
						num2.position.Y += Main.player[Main.myPlayer].velocity.Y * 0.2f;
					}
					if (Collision.SolidCollision(num2.position - Vector2.One * 5f, 10, 10) && num2.fadeIn == 0f)
					{
						num2.scale *= 0.9f;
						num2.velocity *= 0.25f;
					}
				}
				else if (num2.type == 270)
				{
					num2.velocity *= 1.00502515f;
					num2.scale += 0.01f;
					num2.rotation = 0f;
					if (Collision.SolidCollision(num2.position - Vector2.One * 5f, 10, 10) && num2.fadeIn == 0f)
					{
						num2.scale *= 0.95f;
						num2.velocity *= 0.25f;
					}
					else
					{
						num2.velocity.Y = (float)Math.Sin(num2.position.X * 0.00439822953f) * 2f;
						num2.velocity.Y -= 3f;
						num2.velocity.Y /= 20f;
					}
				}
				else if (num2.type == 271)
				{
					num2.velocity *= 1.00502515f;
					num2.scale += 0.003f;
					num2.rotation = 0f;
					num2.velocity.Y -= 4f;
					num2.velocity.Y /= 6f;
				}
				else if (num2.type == 268)
				{
					SandStormCount++;
					num2.velocity *= 1.00502515f;
					num2.scale += 0.01f;
					if (!flag)
					{
						num2.scale -= 0.05f;
					}
					num2.rotation = 0f;
					float y2 = Main.player[Main.myPlayer].velocity.Y;
					if (y2 > 0f && num2.fadeIn == 0f && num2.velocity.Y < y2)
					{
						num2.velocity.Y = MathHelper.Lerp(num2.velocity.Y, y2, 0.04f);
					}
					if (!num2.noLight && y2 > 0f)
					{
						num2.position.Y += y2 * 0.2f;
					}
					if (Collision.SolidCollision(num2.position - Vector2.One * 5f, 10, 10) && num2.fadeIn == 0f)
					{
						num2.scale *= 0.9f;
						num2.velocity *= 0.25f;
					}
					else
					{
						num2.velocity.Y = (float)Math.Sin(num2.position.X * 0.00439822953f) * 2f;
						num2.velocity.Y += 3f;
					}
				}
				else if (!num2.noGravity && num2.type != 41 && num2.type != 44 && num2.type != 309)
				{
					if (num2.type == 107)
					{
						num2.velocity *= 0.9f;
					}
					else
					{
						num2.velocity.Y += 0.1f;
					}
				}
				if (num2.type == 5 || (num2.type == 273 && num2.noGravity))
				{
					num2.scale -= 0.04f;
				}
				if (num2.type == 308 || num2.type == 33 || num2.type == 52 || num2.type == 266 || num2.type == 98 || num2.type == 99 || num2.type == 100 || num2.type == 101 || num2.type == 102 || num2.type == 103 || num2.type == 104 || num2.type == 105 || num2.type == 123 || num2.type == 288)
				{
					if (num2.velocity.X == 0f)
					{
						if (Collision.SolidCollision(num2.position, 2, 2))
						{
							num2.scale = 0f;
						}
						num2.rotation += 0.5f;
						num2.scale -= 0.01f;
					}
					if (Collision.WetCollision(new Vector2(num2.position.X, num2.position.Y), 4, 4))
					{
						num2.alpha += 20;
						num2.scale -= 0.1f;
					}
					num2.alpha += 2;
					num2.scale -= 0.005f;
					if (num2.alpha > 255)
					{
						num2.scale = 0f;
					}
					if (num2.velocity.Y > 4f)
					{
						num2.velocity.Y = 4f;
					}
					if (num2.noGravity)
					{
						if (num2.velocity.X < 0f)
						{
							num2.rotation -= 0.2f;
						}
						else
						{
							num2.rotation += 0.2f;
						}
						num2.scale += 0.03f;
						num2.velocity.X *= 1.05f;
						num2.velocity.Y += 0.15f;
					}
				}
				if (num2.type == 35 && num2.noGravity)
				{
					num2.scale += 0.03f;
					if (num2.scale < 1f)
					{
						num2.velocity.Y += 0.075f;
					}
					num2.velocity.X *= 1.08f;
					if (num2.velocity.X > 0f)
					{
						num2.rotation += 0.01f;
					}
					else
					{
						num2.rotation -= 0.01f;
					}
					float num98 = num2.scale * 0.6f;
					if (num98 > 1f)
					{
						num98 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f + 1f), num98, num98 * 0.3f, num98 * 0.1f);
				}
				else if (num2.type == 152 && num2.noGravity)
				{
					num2.scale += 0.03f;
					if (num2.scale < 1f)
					{
						num2.velocity.Y += 0.075f;
					}
					num2.velocity.X *= 1.08f;
					if (num2.velocity.X > 0f)
					{
						num2.rotation += 0.01f;
					}
					else
					{
						num2.rotation -= 0.01f;
					}
				}
				else if (num2.type == 67 || num2.type == 92)
				{
					float num99 = num2.scale;
					if (num99 > 1f)
					{
						num99 = 1f;
					}
					if (num2.noLight)
					{
						num99 *= 0.1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), 0f, num99 * 0.8f, num99);
				}
				else if (num2.type == 185)
				{
					float num100 = num2.scale;
					if (num100 > 1f)
					{
						num100 = 1f;
					}
					if (num2.noLight)
					{
						num100 *= 0.1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num100 * 0.1f, num100 * 0.7f, num100);
				}
				else if (num2.type == 107)
				{
					float num101 = num2.scale * 0.5f;
					if (num101 > 1f)
					{
						num101 = 1f;
					}
					if (!num2.noLightEmittence)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num101 * 0.1f, num101, num101 * 0.4f);
					}
				}
				else if (num2.type == 34 || num2.type == 35 || num2.type == 152)
				{
					if (!Collision.WetCollision(new Vector2(num2.position.X, num2.position.Y - 8f), 4, 4))
					{
						num2.scale = 0f;
					}
					else
					{
						num2.alpha += Main.rand.Next(2);
						if (num2.alpha > 255)
						{
							num2.scale = 0f;
						}
						num2.velocity.Y = -0.5f;
						if (num2.type == 34)
						{
							num2.scale += 0.005f;
						}
						else
						{
							num2.alpha++;
							num2.scale -= 0.01f;
							num2.velocity.Y = -0.2f;
						}
						num2.velocity.X += (float)Main.rand.Next(-10, 10) * 0.002f;
						if ((double)num2.velocity.X < -0.25)
						{
							num2.velocity.X = -0.25f;
						}
						if ((double)num2.velocity.X > 0.25)
						{
							num2.velocity.X = 0.25f;
						}
					}
					if (num2.type == 35)
					{
						float num102 = num2.scale * 0.3f + 0.4f;
						if (num102 > 1f)
						{
							num102 = 1f;
						}
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num102, num102 * 0.5f, num102 * 0.3f);
					}
				}
				if (num2.type == 68)
				{
					float num103 = num2.scale * 0.3f;
					if (num103 > 1f)
					{
						num103 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num103 * 0.1f, num103 * 0.2f, num103);
				}
				if (num2.type == 70)
				{
					float num104 = num2.scale * 0.3f;
					if (num104 > 1f)
					{
						num104 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num104 * 0.5f, 0f, num104);
				}
				if (num2.type == 41)
				{
					num2.velocity.X += (float)Main.rand.Next(-10, 11) * 0.01f;
					num2.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.01f;
					if ((double)num2.velocity.X > 0.75)
					{
						num2.velocity.X = 0.75f;
					}
					if ((double)num2.velocity.X < -0.75)
					{
						num2.velocity.X = -0.75f;
					}
					if ((double)num2.velocity.Y > 0.75)
					{
						num2.velocity.Y = 0.75f;
					}
					if ((double)num2.velocity.Y < -0.75)
					{
						num2.velocity.Y = -0.75f;
					}
					num2.scale += 0.007f;
					float num105 = num2.scale * 0.7f;
					if (num105 > 1f)
					{
						num105 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num105 * 0.4f, num105 * 0.9f, num105);
				}
				else if (num2.type == 44)
				{
					num2.velocity.X += (float)Main.rand.Next(-10, 11) * 0.003f;
					num2.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.003f;
					if ((double)num2.velocity.X > 0.35)
					{
						num2.velocity.X = 0.35f;
					}
					if ((double)num2.velocity.X < -0.35)
					{
						num2.velocity.X = -0.35f;
					}
					if ((double)num2.velocity.Y > 0.35)
					{
						num2.velocity.Y = 0.35f;
					}
					if ((double)num2.velocity.Y < -0.35)
					{
						num2.velocity.Y = -0.35f;
					}
					num2.scale += 0.0085f;
					float num106 = num2.scale * 0.7f;
					if (num106 > 1f)
					{
						num106 = 1f;
					}
					Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num106 * 0.7f, num106, num106 * 0.8f);
				}
				else if (num2.type != 304)
				{
					num2.velocity.X *= 0.99f;
				}
				if (num2.type == 322 && !num2.noGravity)
				{
					num2.scale *= 0.98f;
				}
				if (num2.type != 79 && num2.type != 268 && num2.type != 304)
				{
					num2.rotation += num2.velocity.X * 0.5f;
				}
				if (num2.fadeIn > 0f && num2.fadeIn < 100f)
				{
					if (num2.type == 235)
					{
						num2.scale += 0.007f;
						int num107 = (int)num2.fadeIn - 1;
						if (num107 >= 0 && num107 <= 255)
						{
							Vector2 vector6 = num2.position - Main.player[num107].Center;
							float num108 = vector6.Length();
							num108 = 100f - num108;
							if (num108 > 0f)
							{
								num2.scale -= num108 * 0.0015f;
							}
							vector6.Normalize();
							float num109 = (1f - num2.scale) * 20f;
							vector6 *= 0f - num109;
							num2.velocity = (num2.velocity * 4f + vector6) / 5f;
						}
					}
					else if (num2.type == 46)
					{
						num2.scale += 0.1f;
					}
					else if (num2.type == 213 || num2.type == 260)
					{
						num2.scale += 0.1f;
					}
					else
					{
						num2.scale += 0.03f;
					}
					if (num2.scale > num2.fadeIn)
					{
						num2.fadeIn = 0f;
					}
				}
				else if (num2.type != 304)
				{
					if (num2.type == 213 || num2.type == 260)
					{
						num2.scale -= 0.2f;
					}
					else
					{
						num2.scale -= 0.01f;
					}
				}
				if (num2.type >= 130 && num2.type <= 134)
				{
					float num110 = num2.scale;
					if (num110 > 1f)
					{
						num110 = 1f;
					}
					if (num2.type == 130)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num110 * 1f, num110 * 0.5f, num110 * 0.4f);
					}
					if (num2.type == 131)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num110 * 0.4f, num110 * 1f, num110 * 0.6f);
					}
					if (num2.type == 132)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num110 * 0.3f, num110 * 0.5f, num110 * 1f);
					}
					if (num2.type == 133)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num110 * 0.9f, num110 * 0.9f, num110 * 0.3f);
					}
					if (num2.noGravity)
					{
						num2.velocity *= 0.93f;
						if (num2.fadeIn == 0f)
						{
							num2.scale += 0.0025f;
						}
					}
					else if (num2.type == 131)
					{
						num2.velocity *= 0.98f;
						num2.velocity.Y -= 0.1f;
						num2.scale += 0.0025f;
					}
					else
					{
						num2.velocity *= 0.95f;
						num2.scale -= 0.0025f;
					}
				}
				else if (num2.type == 278)
				{
					float num111 = num2.scale;
					if (num111 > 1f)
					{
						num111 = 1f;
					}
					if (!num2.noLight && !num2.noLightEmittence)
					{
						Lighting.AddLight(num2.position, num2.color.ToVector3() * num111);
					}
					if (num2.noGravity)
					{
						num2.velocity *= 0.93f;
						if (num2.fadeIn == 0f)
						{
							num2.scale += 0.0025f;
						}
					}
					else
					{
						num2.velocity *= 0.95f;
						num2.scale -= 0.0025f;
					}
					if (WorldGen.SolidTile(Framing.GetTileSafely(num2.position)) && num2.fadeIn == 0f && !num2.noGravity)
					{
						num2.scale *= 0.9f;
						num2.velocity *= 0.25f;
					}
				}
				else if (num2.type >= 219 && num2.type <= 223)
				{
					float num112 = num2.scale;
					if (num112 > 1f)
					{
						num112 = 1f;
					}
					if (!num2.noLight)
					{
						if (num2.type == 219)
						{
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num112 * 1f, num112 * 0.5f, num112 * 0.4f);
						}
						if (num2.type == 220)
						{
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num112 * 0.4f, num112 * 1f, num112 * 0.6f);
						}
						if (num2.type == 221)
						{
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num112 * 0.3f, num112 * 0.5f, num112 * 1f);
						}
						if (num2.type == 222)
						{
							Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num112 * 0.9f, num112 * 0.9f, num112 * 0.3f);
						}
					}
					if (num2.noGravity)
					{
						num2.velocity *= 0.93f;
						if (num2.fadeIn == 0f)
						{
							num2.scale += 0.0025f;
						}
					}
					num2.velocity *= new Vector2(0.97f, 0.99f);
					num2.scale -= 0.0025f;
					if (num2.customData != null && num2.customData is Player)
					{
						Player player8 = (Player)num2.customData;
						num2.position += player8.position - player8.oldPosition;
					}
				}
				else if (num2.type == 226)
				{
					float num113 = num2.scale;
					if (num113 > 1f)
					{
						num113 = 1f;
					}
					if (!num2.noLight)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num113 * 0.2f, num113 * 0.7f, num113 * 1f);
					}
					if (num2.noGravity)
					{
						num2.velocity *= 0.93f;
						if (num2.fadeIn == 0f)
						{
							num2.scale += 0.0025f;
						}
					}
					num2.velocity *= new Vector2(0.97f, 0.99f);
					if (num2.customData != null && num2.customData is Player)
					{
						Player player9 = (Player)num2.customData;
						num2.position += player9.position - player9.oldPosition;
					}
					num2.scale -= 0.01f;
				}
				else if (num2.type == 272)
				{
					float num114 = num2.scale;
					if (num114 > 1f)
					{
						num114 = 1f;
					}
					if (!num2.noLight)
					{
						Lighting.AddLight((int)(num2.position.X / 16f), (int)(num2.position.Y / 16f), num114 * 0.5f, num114 * 0.2f, num114 * 0.8f);
					}
					if (num2.noGravity)
					{
						num2.velocity *= 0.93f;
						if (num2.fadeIn == 0f)
						{
							num2.scale += 0.0025f;
						}
					}
					num2.velocity *= new Vector2(0.97f, 0.99f);
					if (num2.customData != null && num2.customData is Player)
					{
						Player player10 = (Player)num2.customData;
						num2.position += player10.position - player10.oldPosition;
					}
					if (num2.customData != null && num2.customData is NPC)
					{
						NPC nPC3 = (NPC)num2.customData;
						num2.position += nPC3.position - nPC3.oldPosition;
					}
					num2.scale -= 0.01f;
				}
				else if (num2.type != 304 && num2.noGravity)
				{
					num2.velocity *= 0.92f;
					if (num2.fadeIn == 0f)
					{
						num2.scale -= 0.04f;
					}
				}
				if (num2.position.Y > Main.screenPosition.Y + (float)Main.screenHeight)
				{
					num2.active = false;
				}
				float num3 = 0.1f;
				if ((double)dCount == 0.5)
				{
					num2.scale -= 0.001f;
				}
				if ((double)dCount == 0.6)
				{
					num2.scale -= 0.0025f;
				}
				if ((double)dCount == 0.7)
				{
					num2.scale -= 0.005f;
				}
				if ((double)dCount == 0.8)
				{
					num2.scale -= 0.01f;
				}
				if ((double)dCount == 0.9)
				{
					num2.scale -= 0.02f;
				}
				if ((double)dCount == 0.5)
				{
					num3 = 0.11f;
				}
				if ((double)dCount == 0.6)
				{
					num3 = 0.13f;
				}
				if ((double)dCount == 0.7)
				{
					num3 = 0.16f;
				}
				if ((double)dCount == 0.8)
				{
					num3 = 0.22f;
				}
				if ((double)dCount == 0.9)
				{
					num3 = 0.25f;
				}
				if (num2.scale < num3)
				{
					num2.active = false;
				}
			}
			else
			{
				num2.active = false;
			}
		}
		int num115 = i;
		if ((double)num115 > (double)Main.maxDustToDraw * 0.9)
		{
			dCount = 0.9f;
		}
		else if ((double)num115 > (double)Main.maxDustToDraw * 0.8)
		{
			dCount = 0.8f;
		}
		else if ((double)num115 > (double)Main.maxDustToDraw * 0.7)
		{
			dCount = 0.7f;
		}
		else if ((double)num115 > (double)Main.maxDustToDraw * 0.6)
		{
			dCount = 0.6f;
		}
		else if ((double)num115 > (double)Main.maxDustToDraw * 0.5)
		{
			dCount = 0.5f;
		}
		else
		{
			dCount = 0f;
		}
	}

	public Color GetAlpha(Color newColor)
	{
		float num = (float)(255 - alpha) / 255f;
		switch (type)
		{
		case 323:
			return Color.White;
		case 308:
		case 309:
			return new Color(225, 200, 250, 190);
		case 299:
		case 300:
		case 301:
		case 305:
		{
			Color color = default(Color);
			return type switch
			{
				299 => new Color(50, 255, 50, 200), 
				300 => new Color(50, 200, 255, 255), 
				301 => new Color(255, 50, 125, 200), 
				305 => new Color(200, 50, 200, 200), 
				_ => new Color(255, 150, 150, 200), 
			};
		}
		default:
		{
			if (type == 304)
			{
				return Color.White * num;
			}
			if (type == 306)
			{
				return this.color * num;
			}
			if (type == 292)
			{
				return Color.White;
			}
			if (type == 259)
			{
				return new Color(230, 230, 230, 230);
			}
			if (type == 261)
			{
				return new Color(230, 230, 230, 115);
			}
			if (type == 254 || type == 255)
			{
				return new Color(255, 255, 255, 0);
			}
			if (type == 258)
			{
				return new Color(150, 50, 50, 0);
			}
			if (type == 263 || type == 264)
			{
				return new Color((int)this.color.R / 2 + 127, this.color.G + 127, this.color.B + 127, (int)this.color.A / 8) * 0.5f;
			}
			if (type == 235)
			{
				return new Color(255, 255, 255, 0);
			}
			if (((type >= 86 && type <= 91) || type == 262 || type == 286) && !noLight)
			{
				return new Color(255, 255, 255, 0);
			}
			if (type == 213 || type == 260)
			{
				int num2 = (int)(scale / 2.5f * 255f);
				return new Color(num2, num2, num2, num2);
			}
			if (type == 64 && alpha == 255 && noLight)
			{
				return new Color(255, 255, 255, 0);
			}
			if (type == 197)
			{
				return new Color(250, 250, 250, 150);
			}
			if ((type >= 110 && type <= 114) || type == 311 || type == 312 || type == 313)
			{
				return new Color(200, 200, 200, 0);
			}
			if (type == 204)
			{
				return new Color(255, 255, 255, 0);
			}
			if (type == 181)
			{
				return new Color(200, 200, 200, 0);
			}
			if (type == 182 || type == 206)
			{
				return new Color(255, 255, 255, 0);
			}
			if (type == 159)
			{
				return new Color(250, 250, 250, 50);
			}
			if (type == 163 || type == 205)
			{
				return new Color(250, 250, 250, 0);
			}
			if (type == 170)
			{
				return new Color(200, 200, 200, 100);
			}
			if (type == 180)
			{
				return new Color(200, 200, 200, 0);
			}
			if (type == 175)
			{
				return new Color(200, 200, 200, 0);
			}
			if (type == 183)
			{
				return new Color(50, 0, 0, 0);
			}
			if (type == 172)
			{
				return new Color(250, 250, 250, 150);
			}
			if (type == 160 || type == 162 || type == 164 || type == 173)
			{
				int num3 = (int)(250f * scale);
				return new Color(num3, num3, num3, 0);
			}
			if (type == 92 || type == 106 || type == 107)
			{
				return new Color(255, 255, 255, 0);
			}
			if (type == 185)
			{
				return new Color(200, 200, 255, 125);
			}
			if (type == 127 || type == 187)
			{
				return new Color(newColor.R, newColor.G, newColor.B, 25);
			}
			if (type == 156 || type == 230 || type == 234)
			{
				return new Color(255, 255, 255, 0);
			}
			if (type == 270)
			{
				return new Color((int)newColor.R / 2 + 127, (int)newColor.G / 2 + 127, (int)newColor.B / 2 + 127, 25);
			}
			if (type == 271)
			{
				return new Color((int)newColor.R / 2 + 127, (int)newColor.G / 2 + 127, (int)newColor.B / 2 + 127, 127);
			}
			if (type == 6 || type == 242 || type == 174 || type == 135 || type == 75 || type == 20 || type == 21 || type == 231 || type == 169 || (type >= 130 && type <= 134) || type == 158 || type == 293 || type == 294 || type == 295 || type == 296 || type == 297 || type == 298 || type == 307 || type == 310)
			{
				return new Color(newColor.R, newColor.G, newColor.B, 25);
			}
			if (type == 278)
			{
				Color result = new Color(newColor.ToVector3() * this.color.ToVector3());
				result.A = 25;
				return result;
			}
			if (type >= 219 && type <= 223)
			{
				newColor = Color.Lerp(newColor, Color.White, 0.5f);
				return new Color(newColor.R, newColor.G, newColor.B, 25);
			}
			if (type == 226 || type == 272)
			{
				newColor = Color.Lerp(newColor, Color.White, 0.8f);
				return new Color(newColor.R, newColor.G, newColor.B, 25);
			}
			if (type == 228)
			{
				newColor = Color.Lerp(newColor, Color.White, 0.8f);
				return new Color(newColor.R, newColor.G, newColor.B, 25);
			}
			if (type == 279)
			{
				int a = newColor.A;
				newColor = Color.Lerp(newColor, Color.White, 0.8f);
				return new Color(newColor.R, newColor.G, newColor.B, a) * MathHelper.Min(scale, 1f);
			}
			if (type == 229 || type == 269)
			{
				newColor = Color.Lerp(newColor, Color.White, 0.6f);
				return new Color(newColor.R, newColor.G, newColor.B, 25);
			}
			if ((type == 68 || type == 70) && noGravity)
			{
				return new Color(255, 255, 255, 0);
			}
			int num6;
			int num5;
			int num4;
			if (type == 157)
			{
				num6 = (num5 = (num4 = 255));
				float num7 = (float)(int)Main.mouseTextColor / 100f - 1.6f;
				num6 = (int)((float)num6 * num7);
				num5 = (int)((float)num5 * num7);
				num4 = (int)((float)num4 * num7);
				int a2 = (int)(100f * num7);
				num6 += 50;
				if (num6 > 255)
				{
					num6 = 255;
				}
				num5 += 50;
				if (num5 > 255)
				{
					num5 = 255;
				}
				num4 += 50;
				if (num4 > 255)
				{
					num4 = 255;
				}
				return new Color(num6, num5, num4, a2);
			}
			if (type == 284)
			{
				Color result2 = new Color(newColor.ToVector4() * this.color.ToVector4());
				result2.A = this.color.A;
				return result2;
			}
			if (type == 15 || type == 274 || type == 20 || type == 21 || type == 29 || type == 35 || type == 41 || type == 44 || type == 27 || type == 45 || type == 55 || type == 56 || type == 57 || type == 58 || type == 73 || type == 74)
			{
				num = (num + 3f) / 4f;
			}
			else if (type == 43)
			{
				num = (num + 9f) / 10f;
			}
			else
			{
				if (type >= 244 && type <= 247)
				{
					return new Color(255, 255, 255, 0);
				}
				if (type == 66)
				{
					return new Color(newColor.R, newColor.G, newColor.B, 0);
				}
				if (type == 267)
				{
					return new Color(this.color.R, this.color.G, this.color.B, 0);
				}
				if (type == 71)
				{
					return new Color(200, 200, 200, 0);
				}
				if (type == 72)
				{
					return new Color(200, 200, 200, 200);
				}
			}
			num6 = (int)((float)(int)newColor.R * num);
			num5 = (int)((float)(int)newColor.G * num);
			num4 = (int)((float)(int)newColor.B * num);
			int num8 = newColor.A - alpha;
			if (num8 < 0)
			{
				num8 = 0;
			}
			if (num8 > 255)
			{
				num8 = 255;
			}
			return new Color(num6, num5, num4, num8);
		}
		}
	}

	public Color GetColor(Color newColor)
	{
		int num = type;
		if (num == 284)
		{
			return Color.Transparent;
		}
		int i = color.R - (255 - newColor.R);
		int num2 = color.G - (255 - newColor.G);
		int num3 = color.B - (255 - newColor.B);
		int num4 = color.A - (255 - newColor.A);
		if (i < 0)
		{
			i = 0;
		}
		if (i > 255)
		{
			i = 255;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num2 > 255)
		{
			num2 = 255;
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		if (num3 > 255)
		{
			num3 = 255;
		}
		if (num4 < 0)
		{
			num4 = 0;
		}
		if (num4 > 255)
		{
			num4 = 255;
		}
		return new Color(i, num2, num3, num4);
	}

	public float GetVisualRotation()
	{
		if (type == 304)
		{
			return 0f;
		}
		return rotation;
	}

	public float GetVisualScale()
	{
		if (type == 304)
		{
			return 1f;
		}
		return scale;
	}
}
