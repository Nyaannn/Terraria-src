using System;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Effects;

namespace Terraria;

public class Rain
{
	public Vector2 position;

	public Vector2 velocity;

	public float scale;

	public float rotation;

	public int alpha;

	public bool active;

	public byte type;

	public static void ClearRain()
	{
		for (int rectangle = 0; rectangle < Main.maxRain; rectangle++)
		{
			Main.rain[rectangle].active = false;
		}
	}

	public static void MakeRain()
	{
		if (Main.shimmerAlpha > 0f || Main.netMode == 2 || Main.gamePaused)
		{
			return;
		}
		if (Main.remixWorld)
		{
			if (!((double)(Main.player[Main.myPlayer].position.Y / 16f) > Main.rockLayer) || !(Main.player[Main.myPlayer].position.Y / 16f < (float)(Main.maxTilesY - 350)) || Main.player[Main.myPlayer].ZoneDungeon)
			{
				return;
			}
		}
		else if ((double)Main.screenPosition.Y > Main.worldSurface * 16.0)
		{
			return;
		}
		if (Main.gameMenu)
		{
			return;
		}
		float flag = (float)Main.screenWidth / 1920f;
		flag *= 25f;
		flag *= 0.25f + 1f * Main.cloudAlpha;
		if (Filters.Scene["Sandstorm"].IsActive())
		{
			return;
		}
		Vector2 flag4 = default(Vector2);
		for (int flag2 = 0; (float)flag2 < flag; flag2++)
		{
			int flag3 = 600;
			if (Main.player[Main.myPlayer].velocity.Y < 0f)
			{
				flag3 += (int)(Math.Abs(Main.player[Main.myPlayer].velocity.Y) * 30f);
			}
			flag4.X = Main.rand.Next((int)Main.screenPosition.X - flag3, (int)Main.screenPosition.X + Main.screenWidth + flag3);
			flag4.Y = Main.screenPosition.Y - (float)Main.rand.Next(20, 100);
			flag4.X -= Main.windSpeedCurrent * 15f * 40f;
			flag4.X += Main.player[Main.myPlayer].velocity.X * 40f;
			if (flag4.X < 0f)
			{
				flag4.X = 0f;
			}
			if (flag4.X > (float)((Main.maxTilesX - 1) * 16))
			{
				flag4.X = (Main.maxTilesX - 1) * 16;
			}
			int flag5 = (int)flag4.X / 16;
			int flag6 = (int)flag4.Y / 16;
			if (flag5 < 0)
			{
				flag5 = 0;
			}
			if (flag5 > Main.maxTilesX - 1)
			{
				flag5 = Main.maxTilesX - 1;
			}
			if (flag6 < 0)
			{
				flag6 = 0;
			}
			if (flag6 > Main.maxTilesY - 1)
			{
				flag6 = Main.maxTilesY - 1;
			}
			if (Main.remixWorld || Main.gameMenu || (!WorldGen.SolidTile(flag5, flag6) && Main.tile[flag5, flag6].wall <= 0))
			{
				Vector2 flag7 = GetRainFallVelocity();
				NewRain(flag4, flag7);
			}
		}
	}

	public static Vector2 GetRainFallVelocity()
	{
		return new Vector2(Main.windSpeedCurrent * 18f, 14f);
	}

	public void Update()
	{
		if (Main.gamePaused)
		{
			return;
		}
		position += velocity;
		if (Main.gameMenu)
		{
			if (position.Y > Main.screenPosition.Y + (float)Main.screenHeight + 2000f)
			{
				active = false;
			}
		}
		else if (Main.remixWorld)
		{
			if (position.Y > Main.screenPosition.Y + (float)Main.screenHeight + 100f)
			{
				active = false;
			}
		}
		else if (Collision.SolidCollision(position, 2, 2) || position.Y > Main.screenPosition.Y + (float)Main.screenHeight + 100f || Collision.WetCollision(position, 2, 2))
		{
			active = false;
			if ((float)Main.rand.Next(100) < Main.gfxQuality * 100f)
			{
				int num = Dust.NewDust(position - velocity, 2, 2, Dust.dustWater());
				Main.dust[num].position.X -= 2f;
				Main.dust[num].position.Y += 2f;
				Main.dust[num].alpha = 38;
				Main.dust[num].velocity *= 0.1f;
				Main.dust[num].velocity += -velocity * 0.025f;
				Main.dust[num].velocity.Y -= 2f;
				Main.dust[num].scale = 0.6f;
				Main.dust[num].noGravity = true;
			}
		}
	}

	public static int NewRainForced(Vector2 Position, Vector2 Velocity)
	{
		int num = -1;
		int num2 = Main.maxRain;
		float num3 = (1f + Main.gfxQuality) / 2f;
		if (num3 < 0.9f)
		{
			num2 = (int)((float)num2 * num3);
		}
		for (int i = 0; i < num2; i++)
		{
			if (!Main.rain[i].active)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return Main.maxRain;
		}
		Rain rain = Main.rain[num];
		rain.active = true;
		rain.position = Position;
		rain.scale = 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
		rain.velocity = Velocity * rain.scale;
		rain.rotation = (float)Math.Atan2(rain.velocity.X, 0f - rain.velocity.Y);
		rain.type = (byte)(Main.waterStyle * 3 + Main.rand.Next(3));
		return num;
	}

	private static int NewRain(Vector2 Position, Vector2 Velocity)
	{
		int y = -1;
		int num = (int)((float)Main.maxRain * Main.cloudAlpha);
		if (num > Main.maxRain)
		{
			num = Main.maxRain;
		}
		float vec = (float)Main.maxTilesX / 6400f;
		Math.Max(0f, Math.Min(1f, (Main.player[Main.myPlayer].position.Y / 16f - 85f * vec) / (60f * vec)));
		float player = (1f + Main.gfxQuality) / 2f;
		if ((double)player < 0.9)
		{
			num = (int)((float)num * player);
		}
		float text = 800 - Main.SceneMetrics.SnowTileCount;
		if (text < 0f)
		{
			text = 0f;
		}
		text /= 800f;
		num = (int)((float)num * text);
		num = (int)((double)num * Math.Pow(Main.atmo, 9.0));
		if ((double)Main.atmo < 0.4)
		{
			num = 0;
		}
		for (int vector2 = 0; vector2 < num; vector2++)
		{
			if (!Main.rain[vector2].active)
			{
				y = vector2;
				break;
			}
		}
		if (y == -1)
		{
			return Main.maxRain;
		}
		Rain value = Main.rain[y];
		value.active = true;
		value.position = Position;
		value.scale = 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
		value.velocity = Velocity * value.scale;
		value.rotation = (float)Math.Atan2(value.velocity.X, 0f - value.velocity.Y);
		value.type = (byte)(Main.waterStyle * 3 + Main.rand.Next(3));
		return y;
	}
}
