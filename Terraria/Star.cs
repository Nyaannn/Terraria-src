using System;
using Microsoft.Xna.Framework;
using Terraria.Utilities;

namespace Terraria;

public class Star
{
	public Vector2 position;

	public float scale;

	public float rotation;

	public int type;

	public float twinkle;

	public float twinkleSpeed;

	public float rotationSpeed;

	public bool falling;

	public bool hidden;

	public Vector2 fallSpeed;

	public int fallTime;

	public static bool dayCheck = false;

	public static float starfallBoost = 1f;

	public static int starFallCount = 0;

	public float fadeIn;

	public static void NightSetup()
	{
		starfallBoost = 1f;
		int vector = 10;
		int num = 3;
		if (Main.tenthAnniversaryWorld)
		{
			vector = 5;
			num = 2;
		}
		if (Main.rand.Next(vector) == 0)
		{
			starfallBoost = (float)Main.rand.Next(300, 501) * 0.01f;
		}
		else if (Main.rand.Next(num) == 0)
		{
			starfallBoost = (float)Main.rand.Next(100, 151) * 0.01f;
		}
		starFallCount = 0;
	}

	public static void StarFall(float positionX)
	{
		starFallCount++;
		int num = -1;
		float num2 = -1f;
		float num3 = positionX / Main.rightWorld * 1920f;
		for (int num4 = 0; num4 < Main.numStars; num4++)
		{
			if (!Main.star[num4].hidden && !Main.star[num4].falling)
			{
				float vector = Math.Abs(Main.star[num4].position.X - num3);
				if (num2 == -1f || vector < num2)
				{
					num = num4;
					num2 = vector;
				}
			}
		}
		if (num >= 0)
		{
			Main.star[num].Fall();
		}
	}

	public static void SpawnStars(int s = -1)
	{
		FastRandom point2 = FastRandom.CreateWithRandomSeed();
		int num = point2.Next(200, 400);
		int num2 = 0;
		int num3 = num;
		if (s >= 0)
		{
			num2 = s;
			num3 = s + 1;
		}
		for (int num4 = num2; num4 < num3; num4++)
		{
			Main.star[num4] = new Star();
			if (s >= 0)
			{
				Main.star[num4].fadeIn = 1f;
				int i = 10;
				int j = -2000;
				for (int k = 0; k < i; k++)
				{
					float num5 = point2.Next(1921);
					int num6 = 2000;
					for (int l = 0; l < Main.numStars; l++)
					{
						if (l != s && !Main.star[l].hidden && !Main.star[l].falling)
						{
							int num7 = (int)Math.Abs(num5 - Main.star[l].position.X);
							if (num7 < num6)
							{
								num6 = num7;
							}
						}
					}
					if (s == 0 || num6 > j)
					{
						j = num6;
						Main.star[num4].position.X = num5;
					}
				}
			}
			else
			{
				Main.star[num4].position.X = point2.Next(1921);
			}
			Main.star[num4].position.Y = point2.Next(1201);
			Main.star[num4].rotation = (float)point2.Next(628) * 0.01f;
			Main.star[num4].scale = (float)point2.Next(70, 130) * 0.006f;
			Main.star[num4].type = point2.Next(0, 4);
			Main.star[num4].twinkle = (float)point2.Next(60, 101) * 0.01f;
			Main.star[num4].twinkleSpeed = (float)point2.Next(30, 110) * 0.0001f;
			if (point2.Next(2) == 0)
			{
				Main.star[num4].twinkleSpeed *= -1f;
			}
			Main.star[num4].rotationSpeed = (float)point2.Next(5, 50) * 0.0001f;
			if (point2.Next(2) == 0)
			{
				Main.star[num4].rotationSpeed *= -1f;
			}
			if (point2.Next(40) == 0)
			{
				Main.star[num4].scale *= 2f;
				Main.star[num4].twinkleSpeed /= 2f;
				Main.star[num4].rotationSpeed /= 2f;
			}
		}
		if (s == -1)
		{
			Main.numStars = num;
		}
	}

	public void Fall()
	{
		fallTime = 0;
		falling = true;
		fallSpeed.Y = (float)Main.rand.Next(700, 1001) * 0.01f;
		fallSpeed.X = (float)Main.rand.Next(-400, 401) * 0.01f;
	}

	public void Update()
	{
		if (falling && !hidden)
		{
			fallTime += Main.dayRate;
			position += fallSpeed * (Main.dayRate + 99) / 100f;
			if (position.Y > 1500f)
			{
				hidden = true;
			}
			if (Main.starGame && position.Length() > 99999f)
			{
				hidden = true;
			}
			twinkle += twinkleSpeed * 3f;
			if (twinkle > 1f)
			{
				twinkle = 1f;
				twinkleSpeed *= -1f;
			}
			else if ((double)twinkle < 0.6)
			{
				twinkle = 0.6f;
				twinkleSpeed *= -1f;
			}
			rotation += 0.5f;
			if ((double)rotation > 6.28)
			{
				rotation -= 6.28f;
			}
			if (rotation < 0f)
			{
				rotation += 6.28f;
			}
			return;
		}
		if (fadeIn > 0f)
		{
			float num = 6.17283949E-05f * (float)Main.dayRate;
			num *= 10f;
			fadeIn -= num;
			if (fadeIn < 0f)
			{
				fadeIn = 0f;
			}
		}
		twinkle += twinkleSpeed;
		if (twinkle > 1f)
		{
			twinkle = 1f;
			twinkleSpeed *= -1f;
		}
		else if ((double)twinkle < 0.6)
		{
			twinkle = 0.6f;
			twinkleSpeed *= -1f;
		}
		rotation += rotationSpeed;
		if ((double)rotation > 6.28)
		{
			rotation -= 6.28f;
		}
		if (rotation < 0f)
		{
			rotation += 6.28f;
		}
	}

	public static void UpdateStars()
	{
		if (!Main.dayTime)
		{
			dayCheck = false;
		}
		else if (!dayCheck && Main.time >= 27000.0)
		{
			for (int i = 0; i < Main.numStars; i++)
			{
				if (Main.star[i].hidden)
				{
					SpawnStars(i);
				}
			}
		}
		for (int j = 0; j < Main.numStars; j++)
		{
			Main.star[j].Update();
		}
	}
}
