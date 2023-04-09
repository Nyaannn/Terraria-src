using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;

namespace Terraria;

public class Collision
{
	public struct HurtTile
	{
		public int type;

		public int x;

		public int y;
	}

	public static bool stair;

	public static bool stairFall;

	public static bool honey;

	public static bool shimmer;

	public static bool sloping;

	public static bool landMine = false;

	public static bool up;

	public static bool down;

	public static float Epsilon = (float)Math.E;

	private static List<Point> _cacheForConveyorBelts = new List<Point>();

	public static Vector2[] CheckLinevLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
	{
		if (a1.Equals(a2) && b1.Equals(b2))
		{
			if (a1.Equals(b1))
			{
				return new Vector2[1] { a1 };
			}
			return new Vector2[0];
		}
		if (b1.Equals(b2))
		{
			if (PointOnLine(b1, a1, a2))
			{
				return new Vector2[1] { b1 };
			}
			return new Vector2[0];
		}
		if (a1.Equals(a2))
		{
			if (PointOnLine(a1, b1, b2))
			{
				return new Vector2[1] { a1 };
			}
			return new Vector2[0];
		}
		float num4 = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
		float frameExtra = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
		float direction = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);
		if (!(0f - Epsilon < direction) || !(direction < Epsilon))
		{
			float num3 = num4 / direction;
			float abilityCharge = frameExtra / direction;
			if (0f <= num3 && num3 <= 1f && 0f <= abilityCharge && abilityCharge <= 1f)
			{
				return new Vector2[1]
				{
					new Vector2(a1.X + num3 * (a2.X - a1.X), a1.Y + num3 * (a2.Y - a1.Y))
				};
			}
			return new Vector2[0];
		}
		if ((0f - Epsilon < num4 && num4 < Epsilon) || (0f - Epsilon < frameExtra && frameExtra < Epsilon))
		{
			if (a1.Equals(a2))
			{
				return OneDimensionalIntersection(b1, b2, a1, a2);
			}
			return OneDimensionalIntersection(a1, a2, b1, b2);
		}
		return new Vector2[0];
	}

	private static double DistFromSeg(Vector2 p, Vector2 q0, Vector2 q1, double radius, ref float u)
	{
		double num2 = q1.X - q0.X;
		double texture2D2 = q1.Y - q0.Y;
		double texture2D = q0.X - p.X;
		double type = q0.Y - p.Y;
		double num = Math.Sqrt(num2 * num2 + texture2D2 * texture2D2);
		if (num < (double)Epsilon)
		{
			throw new Exception("Expected line segment, not point.");
		}
		return Math.Abs(num2 * type - texture2D * texture2D2) / num;
	}

	private static bool PointOnLine(Vector2 p, Vector2 a1, Vector2 a2)
	{
		float u = 0f;
		return DistFromSeg(p, a1, a2, Epsilon, ref u) < (double)Epsilon;
	}

	private static Vector2[] OneDimensionalIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
	{
		float num = a2.X - a1.X;
		float num2 = a2.Y - a1.Y;
		float i;
		float j;
		if (Math.Abs(num) > Math.Abs(num2))
		{
			i = (b1.X - a1.X) / num;
			j = (b2.X - a1.X) / num;
		}
		else
		{
			i = (b1.Y - a1.Y) / num2;
			j = (b2.Y - a1.Y) / num2;
		}
		List<Vector2> list = new List<Vector2>();
		float[] array = FindOverlapPoints(i, j);
		foreach (float num3 in array)
		{
			float x = a2.X * num3 + a1.X * (1f - num3);
			float y = a2.Y * num3 + a1.Y * (1f - num3);
			list.Add(new Vector2(x, y));
		}
		return list.ToArray();
	}

	private static float[] FindOverlapPoints(float relativePoint1, float relativePoint2)
	{
		float newColor = Math.Min(relativePoint1, relativePoint2);
		float i = Math.Max(relativePoint1, relativePoint2);
		float type2 = Math.Max(0f, newColor);
		float scale = Math.Min(1f, i);
		if (type2 > scale)
		{
			return new float[0];
		}
		if (type2 != scale)
		{
			return new float[2] { type2, scale };
		}
		return new float[1] { type2 };
	}

	public static bool CheckAABBvAABBCollision(Vector2 position1, Vector2 dimensions1, Vector2 position2, Vector2 dimensions2)
	{
		if (position1.X < position2.X + dimensions2.X && position1.Y < position2.Y + dimensions2.Y && position1.X + dimensions1.X > position2.X)
		{
			return position1.Y + dimensions1.Y > position2.Y;
		}
		return false;
	}

	private static int collisionOutcode(Vector2 aabbPosition, Vector2 aabbDimensions, Vector2 point)
	{
		float num = aabbPosition.X + aabbDimensions.X;
		float num2 = aabbPosition.Y + aabbDimensions.Y;
		int i = 0;
		if (aabbDimensions.X <= 0f)
		{
			i |= 5;
		}
		else if (point.X < aabbPosition.X)
		{
			i |= 1;
		}
		else if (point.X - num > 0f)
		{
			i |= 4;
		}
		if (aabbDimensions.Y <= 0f)
		{
			i |= 0xA;
		}
		else if (point.Y < aabbPosition.Y)
		{
			i |= 2;
		}
		else if (point.Y - num2 > 0f)
		{
			i |= 8;
		}
		return i;
	}

	public static bool CheckAABBvLineCollision(Vector2 aabbPosition, Vector2 aabbDimensions, Vector2 lineStart, Vector2 lineEnd)
	{
		int num;
		if ((num = collisionOutcode(aabbPosition, aabbDimensions, lineEnd)) == 0)
		{
			return true;
		}
		int num2;
		while ((num2 = collisionOutcode(aabbPosition, aabbDimensions, lineStart)) != 0)
		{
			if ((num2 & num) != 0)
			{
				return false;
			}
			if (((uint)num2 & 5u) != 0)
			{
				float num3 = aabbPosition.X;
				if (((uint)num2 & 4u) != 0)
				{
					num3 += aabbDimensions.X;
				}
				lineStart.Y += (num3 - lineStart.X) * (lineEnd.Y - lineStart.Y) / (lineEnd.X - lineStart.X);
				lineStart.X = num3;
			}
			else
			{
				float num4 = aabbPosition.Y;
				if (((uint)num2 & 8u) != 0)
				{
					num4 += aabbDimensions.Y;
				}
				lineStart.X += (num4 - lineStart.Y) * (lineEnd.X - lineStart.X) / (lineEnd.Y - lineStart.Y);
				lineStart.Y = num4;
			}
		}
		return true;
	}

	public static bool CheckAABBvLineCollision2(Vector2 aabbPosition, Vector2 aabbDimensions, Vector2 lineStart, Vector2 lineEnd)
	{
		float collisionPoint = 0f;
		if (!Utils.RectangleLineCollision(aabbPosition, aabbPosition + aabbDimensions, lineStart, lineEnd))
		{
			return CheckAABBvLineCollision(aabbPosition, aabbDimensions, lineStart, lineEnd, 0.0001f, ref collisionPoint);
		}
		return true;
	}

	public static bool CheckAABBvLineCollision(Vector2 objectPosition, Vector2 objectDimensions, Vector2 lineStart, Vector2 lineEnd, float lineWidth, ref float collisionPoint)
	{
		float num = lineWidth * 0.5f;
		Vector2 position2 = lineStart;
		Vector2 dimensions = lineEnd - lineStart;
		if (dimensions.X > 0f)
		{
			dimensions.X += lineWidth;
			position2.X -= num;
		}
		else
		{
			position2.X += dimensions.X - num;
			dimensions.X = 0f - dimensions.X + lineWidth;
		}
		if (dimensions.Y > 0f)
		{
			dimensions.Y += lineWidth;
			position2.Y -= num;
		}
		else
		{
			position2.Y += dimensions.Y - num;
			dimensions.Y = 0f - dimensions.Y + lineWidth;
		}
		if (!CheckAABBvAABBCollision(objectPosition, objectDimensions, position2, dimensions))
		{
			return false;
		}
		Vector2 writer = objectPosition - lineStart;
		Vector2 position = writer + objectDimensions;
		Vector2 num7 = new Vector2(writer.X, position.Y);
		Vector2 spinningpoint = new Vector2(position.X, writer.Y);
		Vector2 player5 = lineEnd - lineStart;
		float bb23 = player5.Length();
		float bb24 = (float)Math.Atan2(player5.Y, player5.X);
		Vector2[] bb25 = new Vector2[4]
		{
			writer.RotatedBy(0f - bb24),
			spinningpoint.RotatedBy(0f - bb24),
			position.RotatedBy(0f - bb24),
			num7.RotatedBy(0f - bb24)
		};
		collisionPoint = bb23;
		bool num2 = false;
		for (int bb14 = 0; bb14 < bb25.Length; bb14++)
		{
			if (Math.Abs(bb25[bb14].Y) < num && bb25[bb14].X < collisionPoint && bb25[bb14].X >= 0f)
			{
				collisionPoint = bb25[bb14].X;
				num2 = true;
			}
		}
		Vector2 num3 = new Vector2(0f, num);
		Vector2 player4 = new Vector2(bb23, num);
		Vector2 item5 = new Vector2(0f, 0f - num);
		Vector2 num8 = new Vector2(bb23, 0f - num);
		for (int bb15 = 0; bb15 < bb25.Length; bb15++)
		{
			int bb16 = (bb15 + 1) % bb25.Length;
			Vector2 bb17 = player4 - num3;
			Vector2 bb18 = bb25[bb16] - bb25[bb15];
			float bb19 = bb17.X * bb18.Y - bb17.Y * bb18.X;
			if (bb19 != 0f)
			{
				Vector2 bb20 = bb25[bb15] - num3;
				float bb21 = (bb20.X * bb18.Y - bb20.Y * bb18.X) / bb19;
				if (bb21 >= 0f && bb21 <= 1f)
				{
					float bb22 = (bb20.X * bb17.Y - bb20.Y * bb17.X) / bb19;
					if (bb22 >= 0f && bb22 <= 1f)
					{
						num2 = true;
						collisionPoint = Math.Min(collisionPoint, num3.X + bb21 * bb17.X);
					}
				}
			}
			bb17 = num8 - item5;
			bb19 = bb17.X * bb18.Y - bb17.Y * bb18.X;
			if (bb19 == 0f)
			{
				continue;
			}
			Vector2 num4 = bb25[bb15] - item5;
			float num5 = (num4.X * bb18.Y - num4.Y * bb18.X) / bb19;
			if (num5 >= 0f && num5 <= 1f)
			{
				float num6 = (num4.X * bb17.Y - num4.Y * bb17.X) / bb19;
				if (num6 >= 0f && num6 <= 1f)
				{
					num2 = true;
					collisionPoint = Math.Min(collisionPoint, item5.X + num5 * bb17.X);
				}
			}
		}
		return num2;
	}

	public static bool CanHit(Entity source, Entity target)
	{
		return CanHit(source.position, source.width, source.height, target.position, target.width, target.height);
	}

	public static bool CanHit(Entity source, NPCAimedTarget target)
	{
		return CanHit(source.position, source.width, source.height, target.Position, target.Width, target.Height);
	}

	public static bool CanHit(Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2)
	{
		return CanHit(Position1.ToPoint(), Width1, Height1, Position2.ToPoint(), Width2, Height2);
	}

	public static bool CanHit(Point Position1, int Width1, int Height1, Point Position2, int Width2, int Height2)
	{
		int tile = (Position1.X + Width1 / 2) / 16;
		int num = (Position1.Y + Height1 / 2) / 16;
		int num2 = (Position2.X + Width2 / 2) / 16;
		int i = (Position2.Y + Height2 / 2) / 16;
		if (tile <= 1)
		{
			tile = 1;
		}
		if (tile >= Main.maxTilesX)
		{
			tile = Main.maxTilesX - 1;
		}
		if (num2 <= 1)
		{
			num2 = 1;
		}
		if (num2 >= Main.maxTilesX)
		{
			num2 = Main.maxTilesX - 1;
		}
		if (num <= 1)
		{
			num = 1;
		}
		if (num >= Main.maxTilesY)
		{
			num = Main.maxTilesY - 1;
		}
		if (i <= 1)
		{
			i = 1;
		}
		if (i >= Main.maxTilesY)
		{
			i = Main.maxTilesY - 1;
		}
		try
		{
			do
			{
				int j = Math.Abs(tile - num2);
				int b = Math.Abs(num - i);
				if (tile == num2 && num == i)
				{
					return true;
				}
				if (j > b)
				{
					tile = ((tile >= num2) ? (tile - 1) : (tile + 1));
					if (Main.tile[tile, num - 1] == null)
					{
						return false;
					}
					if (Main.tile[tile, num + 1] == null)
					{
						return false;
					}
					if (!Main.tile[tile, num - 1].inActive() && Main.tile[tile, num - 1].active() && Main.tileSolid[Main.tile[tile, num - 1].type] && !Main.tileSolidTop[Main.tile[tile, num - 1].type] && Main.tile[tile, num - 1].slope() == 0 && !Main.tile[tile, num - 1].halfBrick() && !Main.tile[tile, num + 1].inActive() && Main.tile[tile, num + 1].active() && Main.tileSolid[Main.tile[tile, num + 1].type] && !Main.tileSolidTop[Main.tile[tile, num + 1].type] && Main.tile[tile, num + 1].slope() == 0 && !Main.tile[tile, num + 1].halfBrick())
					{
						return false;
					}
				}
				else
				{
					num = ((num >= i) ? (num - 1) : (num + 1));
					if (Main.tile[tile - 1, num] == null)
					{
						return false;
					}
					if (Main.tile[tile + 1, num] == null)
					{
						return false;
					}
					if (!Main.tile[tile - 1, num].inActive() && Main.tile[tile - 1, num].active() && Main.tileSolid[Main.tile[tile - 1, num].type] && !Main.tileSolidTop[Main.tile[tile - 1, num].type] && Main.tile[tile - 1, num].slope() == 0 && !Main.tile[tile - 1, num].halfBrick() && !Main.tile[tile + 1, num].inActive() && Main.tile[tile + 1, num].active() && Main.tileSolid[Main.tile[tile + 1, num].type] && !Main.tileSolidTop[Main.tile[tile + 1, num].type] && Main.tile[tile + 1, num].slope() == 0 && !Main.tile[tile + 1, num].halfBrick())
					{
						return false;
					}
				}
				if (Main.tile[tile, num] == null)
				{
					return false;
				}
			}
			while (Main.tile[tile, num].inActive() || !Main.tile[tile, num].active() || !Main.tileSolid[Main.tile[tile, num].type] || Main.tileSolidTop[Main.tile[tile, num].type]);
			return false;
		}
		catch
		{
			return false;
		}
	}

	public static bool CanHitWithCheck(Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2, Utils.TileActionAttempt check)
	{
		int num = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
		int num2 = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
		int num3 = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
		int num4 = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);
		if (num <= 1)
		{
			num = 1;
		}
		if (num >= Main.maxTilesX)
		{
			num = Main.maxTilesX - 1;
		}
		if (num3 <= 1)
		{
			num3 = 1;
		}
		if (num3 >= Main.maxTilesX)
		{
			num3 = Main.maxTilesX - 1;
		}
		if (num2 <= 1)
		{
			num2 = 1;
		}
		if (num2 >= Main.maxTilesY)
		{
			num2 = Main.maxTilesY - 1;
		}
		if (num4 <= 1)
		{
			num4 = 1;
		}
		if (num4 >= Main.maxTilesY)
		{
			num4 = Main.maxTilesY - 1;
		}
		try
		{
			do
			{
				int num5 = Math.Abs(num - num3);
				int num6 = Math.Abs(num2 - num4);
				if (num == num3 && num2 == num4)
				{
					return true;
				}
				if (num5 > num6)
				{
					num = ((num >= num3) ? (num - 1) : (num + 1));
					if (Main.tile[num, num2 - 1] == null)
					{
						return false;
					}
					if (Main.tile[num, num2 + 1] == null)
					{
						return false;
					}
					if (!Main.tile[num, num2 - 1].inActive() && Main.tile[num, num2 - 1].active() && Main.tileSolid[Main.tile[num, num2 - 1].type] && !Main.tileSolidTop[Main.tile[num, num2 - 1].type] && Main.tile[num, num2 - 1].slope() == 0 && !Main.tile[num, num2 - 1].halfBrick() && !Main.tile[num, num2 + 1].inActive() && Main.tile[num, num2 + 1].active() && Main.tileSolid[Main.tile[num, num2 + 1].type] && !Main.tileSolidTop[Main.tile[num, num2 + 1].type] && Main.tile[num, num2 + 1].slope() == 0 && !Main.tile[num, num2 + 1].halfBrick())
					{
						return false;
					}
				}
				else
				{
					num2 = ((num2 >= num4) ? (num2 - 1) : (num2 + 1));
					if (Main.tile[num - 1, num2] == null)
					{
						return false;
					}
					if (Main.tile[num + 1, num2] == null)
					{
						return false;
					}
					if (!Main.tile[num - 1, num2].inActive() && Main.tile[num - 1, num2].active() && Main.tileSolid[Main.tile[num - 1, num2].type] && !Main.tileSolidTop[Main.tile[num - 1, num2].type] && Main.tile[num - 1, num2].slope() == 0 && !Main.tile[num - 1, num2].halfBrick() && !Main.tile[num + 1, num2].inActive() && Main.tile[num + 1, num2].active() && Main.tileSolid[Main.tile[num + 1, num2].type] && !Main.tileSolidTop[Main.tile[num + 1, num2].type] && Main.tile[num + 1, num2].slope() == 0 && !Main.tile[num + 1, num2].halfBrick())
					{
						return false;
					}
				}
				if (Main.tile[num, num2] == null)
				{
					return false;
				}
				if (!Main.tile[num, num2].inActive() && Main.tile[num, num2].active() && Main.tileSolid[Main.tile[num, num2].type] && !Main.tileSolidTop[Main.tile[num, num2].type])
				{
					return false;
				}
			}
			while (check(num, num2));
			return false;
		}
		catch
		{
			return false;
		}
	}

	public static bool CanHitLine(Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2)
	{
		int num4 = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
		int num5 = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
		int num = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
		int num2 = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);
		if (num4 <= 1)
		{
			num4 = 1;
		}
		if (num4 >= Main.maxTilesX)
		{
			num4 = Main.maxTilesX - 1;
		}
		if (num <= 1)
		{
			num = 1;
		}
		if (num >= Main.maxTilesX)
		{
			num = Main.maxTilesX - 1;
		}
		if (num5 <= 1)
		{
			num5 = 1;
		}
		if (num5 >= Main.maxTilesY)
		{
			num5 = Main.maxTilesY - 1;
		}
		if (num2 <= 1)
		{
			num2 = 1;
		}
		if (num2 >= Main.maxTilesY)
		{
			num2 = Main.maxTilesY - 1;
		}
		float num3 = Math.Abs(num4 - num);
		float position = Math.Abs(num5 - num2);
		if (num3 == 0f && position == 0f)
		{
			return true;
		}
		float num6 = 1f;
		float i = 1f;
		if (num3 == 0f || position == 0f)
		{
			if (num3 == 0f)
			{
				num6 = 0f;
			}
			if (position == 0f)
			{
				i = 0f;
			}
		}
		else if (num3 > position)
		{
			num6 = num3 / position;
		}
		else
		{
			i = position / num3;
		}
		float num7 = 0f;
		float num8 = 0f;
		int num9 = 1;
		if (num5 < num2)
		{
			num9 = 2;
		}
		int num10 = (int)num3;
		int num11 = (int)position;
		int num12 = Math.Sign(num - num4);
		int num13 = Math.Sign(num2 - num5);
		bool flag = false;
		bool flag2 = false;
		try
		{
			do
			{
				switch (num9)
				{
				case 2:
				{
					num7 += num6;
					int num15 = (int)num7;
					num7 %= 1f;
					for (int k = 0; k < num15; k++)
					{
						if (Main.tile[num4, num5 - 1] == null)
						{
							return false;
						}
						if (Main.tile[num4, num5] == null)
						{
							return false;
						}
						if (Main.tile[num4, num5 + 1] == null)
						{
							return false;
						}
						Tile tile4 = Main.tile[num4, num5 - 1];
						Tile tile5 = Main.tile[num4, num5 + 1];
						Tile tile6 = Main.tile[num4, num5];
						if ((!tile4.inActive() && tile4.active() && Main.tileSolid[tile4.type] && !Main.tileSolidTop[tile4.type]) || (!tile5.inActive() && tile5.active() && Main.tileSolid[tile5.type] && !Main.tileSolidTop[tile5.type]) || (!tile6.inActive() && tile6.active() && Main.tileSolid[tile6.type] && !Main.tileSolidTop[tile6.type]))
						{
							return false;
						}
						if (num10 == 0 && num11 == 0)
						{
							flag = true;
							break;
						}
						num4 += num12;
						num10--;
						if (num10 == 0 && num11 == 0 && num15 == 1)
						{
							flag2 = true;
						}
					}
					if (num11 != 0)
					{
						num9 = 1;
					}
					break;
				}
				case 1:
				{
					num8 += i;
					int num14 = (int)num8;
					num8 %= 1f;
					for (int j = 0; j < num14; j++)
					{
						if (Main.tile[num4 - 1, num5] == null)
						{
							return false;
						}
						if (Main.tile[num4, num5] == null)
						{
							return false;
						}
						if (Main.tile[num4 + 1, num5] == null)
						{
							return false;
						}
						Tile tile = Main.tile[num4 - 1, num5];
						Tile tile2 = Main.tile[num4 + 1, num5];
						Tile tile3 = Main.tile[num4, num5];
						if ((!tile.inActive() && tile.active() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type]) || (!tile2.inActive() && tile2.active() && Main.tileSolid[tile2.type] && !Main.tileSolidTop[tile2.type]) || (!tile3.inActive() && tile3.active() && Main.tileSolid[tile3.type] && !Main.tileSolidTop[tile3.type]))
						{
							return false;
						}
						if (num10 == 0 && num11 == 0)
						{
							flag = true;
							break;
						}
						num5 += num13;
						num11--;
						if (num10 == 0 && num11 == 0 && num14 == 1)
						{
							flag2 = true;
						}
					}
					if (num10 != 0)
					{
						num9 = 2;
					}
					break;
				}
				}
				if (Main.tile[num4, num5] == null)
				{
					return false;
				}
				Tile tile7 = Main.tile[num4, num5];
				if (!tile7.inActive() && tile7.active() && Main.tileSolid[tile7.type] && !Main.tileSolidTop[tile7.type])
				{
					return false;
				}
			}
			while (!(flag || flag2));
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool TupleHitLine(int x1, int y1, int x2, int y2, int ignoreX, int ignoreY, List<Tuple<int, int>> ignoreTargets, out Tuple<int, int> col)
	{
		int value = x1;
		int value2 = y1;
		int value3 = x2;
		int value4 = y2;
		value = Utils.Clamp(value, 1, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 1, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 1, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 1, Main.maxTilesY - 1);
		float num = Math.Abs(value - value3);
		float num2 = Math.Abs(value2 - value4);
		if (num == 0f && num2 == 0f)
		{
			col = new Tuple<int, int>(value, value2);
			return true;
		}
		float num3 = 1f;
		float num4 = 1f;
		if (num == 0f || num2 == 0f)
		{
			if (num == 0f)
			{
				num3 = 0f;
			}
			if (num2 == 0f)
			{
				num4 = 0f;
			}
		}
		else if (num > num2)
		{
			num3 = num / num2;
		}
		else
		{
			num4 = num2 / num;
		}
		float num5 = 0f;
		float num6 = 0f;
		int num7 = 1;
		if (value2 < value4)
		{
			num7 = 2;
		}
		int num8 = (int)num;
		int num9 = (int)num2;
		int num10 = Math.Sign(value3 - value);
		int num11 = Math.Sign(value4 - value2);
		bool flag = false;
		bool flag2 = false;
		try
		{
			do
			{
				switch (num7)
				{
				case 2:
				{
					num5 += num3;
					int num13 = (int)num5;
					num5 %= 1f;
					for (int j = 0; j < num13; j++)
					{
						if (Main.tile[value, value2 - 1] == null)
						{
							col = new Tuple<int, int>(value, value2 - 1);
							return false;
						}
						if (Main.tile[value, value2 + 1] == null)
						{
							col = new Tuple<int, int>(value, value2 + 1);
							return false;
						}
						Tile tile4 = Main.tile[value, value2 - 1];
						Tile tile5 = Main.tile[value, value2 + 1];
						Tile tile6 = Main.tile[value, value2];
						if (!ignoreTargets.Contains(new Tuple<int, int>(value, value2)) && !ignoreTargets.Contains(new Tuple<int, int>(value, value2 - 1)) && !ignoreTargets.Contains(new Tuple<int, int>(value, value2 + 1)))
						{
							if (ignoreY != -1 && num11 < 0 && !tile4.inActive() && tile4.active() && Main.tileSolid[tile4.type] && !Main.tileSolidTop[tile4.type])
							{
								col = new Tuple<int, int>(value, value2 - 1);
								return true;
							}
							if (ignoreY != 1 && num11 > 0 && !tile5.inActive() && tile5.active() && Main.tileSolid[tile5.type] && !Main.tileSolidTop[tile5.type])
							{
								col = new Tuple<int, int>(value, value2 + 1);
								return true;
							}
							if (!tile6.inActive() && tile6.active() && Main.tileSolid[tile6.type] && !Main.tileSolidTop[tile6.type])
							{
								col = new Tuple<int, int>(value, value2);
								return true;
							}
						}
						if (num8 == 0 && num9 == 0)
						{
							flag = true;
							break;
						}
						value += num10;
						num8--;
						if (num8 == 0 && num9 == 0 && num13 == 1)
						{
							flag2 = true;
						}
					}
					if (num9 != 0)
					{
						num7 = 1;
					}
					break;
				}
				case 1:
				{
					num6 += num4;
					int num12 = (int)num6;
					num6 %= 1f;
					for (int i = 0; i < num12; i++)
					{
						if (Main.tile[value - 1, value2] == null)
						{
							col = new Tuple<int, int>(value - 1, value2);
							return false;
						}
						if (Main.tile[value + 1, value2] == null)
						{
							col = new Tuple<int, int>(value + 1, value2);
							return false;
						}
						Tile tile = Main.tile[value - 1, value2];
						Tile tile2 = Main.tile[value + 1, value2];
						Tile tile3 = Main.tile[value, value2];
						if (!ignoreTargets.Contains(new Tuple<int, int>(value, value2)) && !ignoreTargets.Contains(new Tuple<int, int>(value - 1, value2)) && !ignoreTargets.Contains(new Tuple<int, int>(value + 1, value2)))
						{
							if (ignoreX != -1 && num10 < 0 && !tile.inActive() && tile.active() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type])
							{
								col = new Tuple<int, int>(value - 1, value2);
								return true;
							}
							if (ignoreX != 1 && num10 > 0 && !tile2.inActive() && tile2.active() && Main.tileSolid[tile2.type] && !Main.tileSolidTop[tile2.type])
							{
								col = new Tuple<int, int>(value + 1, value2);
								return true;
							}
							if (!tile3.inActive() && tile3.active() && Main.tileSolid[tile3.type] && !Main.tileSolidTop[tile3.type])
							{
								col = new Tuple<int, int>(value, value2);
								return true;
							}
						}
						if (num8 == 0 && num9 == 0)
						{
							flag = true;
							break;
						}
						value2 += num11;
						num9--;
						if (num8 == 0 && num9 == 0 && num12 == 1)
						{
							flag2 = true;
						}
					}
					if (num8 != 0)
					{
						num7 = 2;
					}
					break;
				}
				}
				if (Main.tile[value, value2] == null)
				{
					col = new Tuple<int, int>(value, value2);
					return false;
				}
				Tile tile7 = Main.tile[value, value2];
				if (!ignoreTargets.Contains(new Tuple<int, int>(value, value2)) && !tile7.inActive() && tile7.active() && Main.tileSolid[tile7.type] && !Main.tileSolidTop[tile7.type])
				{
					col = new Tuple<int, int>(value, value2);
					return true;
				}
			}
			while (!(flag || flag2));
			col = new Tuple<int, int>(value, value2);
			return true;
		}
		catch
		{
			col = new Tuple<int, int>(x1, y1);
			return false;
		}
	}

	public static Tuple<int, int> TupleHitLineWall(int x1, int y1, int x2, int y2)
	{
		int remoteClient = x1;
		int ignoreClient = y1;
		int num = x2;
		int num2 = y2;
		if (remoteClient <= 1)
		{
			remoteClient = 1;
		}
		if (remoteClient >= Main.maxTilesX)
		{
			remoteClient = Main.maxTilesX - 1;
		}
		if (num <= 1)
		{
			num = 1;
		}
		if (num >= Main.maxTilesX)
		{
			num = Main.maxTilesX - 1;
		}
		if (ignoreClient <= 1)
		{
			ignoreClient = 1;
		}
		if (ignoreClient >= Main.maxTilesY)
		{
			ignoreClient = Main.maxTilesY - 1;
		}
		if (num2 <= 1)
		{
			num2 = 1;
		}
		if (num2 >= Main.maxTilesY)
		{
			num2 = Main.maxTilesY - 1;
		}
		float num3 = Math.Abs(remoteClient - num);
		float num4 = Math.Abs(ignoreClient - num2);
		if (num3 == 0f && num4 == 0f)
		{
			return new Tuple<int, int>(remoteClient, ignoreClient);
		}
		float num5 = 1f;
		float num6 = 1f;
		if (num3 == 0f || num4 == 0f)
		{
			if (num3 == 0f)
			{
				num5 = 0f;
			}
			if (num4 == 0f)
			{
				num6 = 0f;
			}
		}
		else if (num3 > num4)
		{
			num5 = num3 / num4;
		}
		else
		{
			num6 = num4 / num3;
		}
		float num7 = 0f;
		float num8 = 0f;
		int num9 = 1;
		if (ignoreClient < num2)
		{
			num9 = 2;
		}
		int num10 = (int)num3;
		int num11 = (int)num4;
		int num12 = Math.Sign(num - remoteClient);
		int num13 = Math.Sign(num2 - ignoreClient);
		bool flag = false;
		bool flag2 = false;
		try
		{
			do
			{
				switch (num9)
				{
				case 2:
				{
					num7 += num5;
					int num15 = (int)num7;
					num7 %= 1f;
					for (int j = 0; j < num15; j++)
					{
						_ = Main.tile[remoteClient, ignoreClient];
						if (HitWallSubstep(remoteClient, ignoreClient))
						{
							return new Tuple<int, int>(remoteClient, ignoreClient);
						}
						if (num10 == 0 && num11 == 0)
						{
							flag = true;
							break;
						}
						remoteClient += num12;
						num10--;
						if (num10 == 0 && num11 == 0 && num15 == 1)
						{
							flag2 = true;
						}
					}
					if (num11 != 0)
					{
						num9 = 1;
					}
					break;
				}
				case 1:
				{
					num8 += num6;
					int num14 = (int)num8;
					num8 %= 1f;
					for (int i = 0; i < num14; i++)
					{
						_ = Main.tile[remoteClient, ignoreClient];
						if (HitWallSubstep(remoteClient, ignoreClient))
						{
							return new Tuple<int, int>(remoteClient, ignoreClient);
						}
						if (num10 == 0 && num11 == 0)
						{
							flag = true;
							break;
						}
						ignoreClient += num13;
						num11--;
						if (num10 == 0 && num11 == 0 && num14 == 1)
						{
							flag2 = true;
						}
					}
					if (num10 != 0)
					{
						num9 = 2;
					}
					break;
				}
				}
				if (Main.tile[remoteClient, ignoreClient] == null)
				{
					return new Tuple<int, int>(-1, -1);
				}
				_ = Main.tile[remoteClient, ignoreClient];
				if (HitWallSubstep(remoteClient, ignoreClient))
				{
					return new Tuple<int, int>(remoteClient, ignoreClient);
				}
			}
			while (!(flag || flag2));
			return new Tuple<int, int>(remoteClient, ignoreClient);
		}
		catch
		{
			return new Tuple<int, int>(-1, -1);
		}
	}

	public static bool HitWallSubstep(int x, int y)
	{
		if (Main.tile[x, y].wall == 0)
		{
			return false;
		}
		bool flag = false;
		if (Main.wallHouse[Main.tile[x, y].wall])
		{
			flag = true;
		}
		if (!flag)
		{
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if ((i != 0 || j != 0) && Main.tile[x + i, y + j].wall == 0)
					{
						flag = true;
					}
				}
			}
		}
		if (Main.tile[x, y].active() && flag)
		{
			bool flag2 = true;
			for (int k = -1; k < 2; k++)
			{
				for (int l = -1; l < 2; l++)
				{
					if (k != 0 || l != 0)
					{
						Tile tile = Main.tile[x + k, y + l];
						if (!tile.active() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type])
						{
							flag2 = false;
						}
					}
				}
			}
			if (flag2)
			{
				flag = false;
			}
		}
		return flag;
	}

	public static bool EmptyTile(int i, int j, bool ignoreTiles = false)
	{
		Rectangle bb = new Rectangle(i * 16, j * 16, 16, 16);
		if (Main.tile[i, j].active() && !ignoreTiles)
		{
			return false;
		}
		for (int k = 0; k < 255; k++)
		{
			if (Main.player[k].active && !Main.player[k].dead && !Main.player[k].ghost && bb.Intersects(new Rectangle((int)Main.player[k].position.X, (int)Main.player[k].position.Y, Main.player[k].width, Main.player[k].height)))
			{
				return false;
			}
		}
		for (int l = 0; l < 200; l++)
		{
			if (Main.npc[l].active && bb.Intersects(new Rectangle((int)Main.npc[l].position.X, (int)Main.npc[l].position.Y, Main.npc[l].width, Main.npc[l].height)))
			{
				return false;
			}
		}
		return true;
	}

	public static bool DrownCollision(Vector2 Position, int Width, int Height, float gravDir = -1f, bool includeSlopes = false)
	{
		Vector2 vector = new Vector2(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
		int num = 10;
		int num2 = 12;
		if (num > Width)
		{
			num = Width;
		}
		if (num2 > Height)
		{
			num2 = Height;
		}
		vector = new Vector2(vector.X - (float)(num / 2), Position.Y + -2f);
		if (gravDir == -1f)
		{
			vector.Y += Height / 2 - 6;
		}
		int value = (int)(Position.X / 16f) - 1;
		int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int value3 = (int)(Position.Y / 16f) - 1;
		int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num3 = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
		int num4 = ((gravDir == 1f) ? value3 : (value4 - 1));
		Vector2 bb = default(Vector2);
		for (int i = num3; i < value2; i++)
		{
			for (int j = value3; j < value4; j++)
			{
				Tile tile = Main.tile[i, j];
				if (tile != null && tile.liquid > 0 && !tile.lava() && !tile.shimmer() && (j != num4 || !tile.active() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type] || (includeSlopes && tile.blockType() != 0)))
				{
					bb.X = i * 16;
					bb.Y = j * 16;
					int num5 = 16;
					float num6 = 256 - Main.tile[i, j].liquid;
					num6 /= 32f;
					bb.Y += num6 * 2f;
					num5 -= (int)(num6 * 2f);
					if (vector.X + (float)num > bb.X && vector.X < bb.X + 16f && vector.Y + (float)num2 > bb.Y && vector.Y < bb.Y + (float)num5)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool IsWorldPointSolid(Vector2 pos, bool treatPlatformsAsNonSolid = false)
	{
		Point point = pos.ToTileCoordinates();
		if (!WorldGen.InWorld(point.X, point.Y, 1))
		{
			return false;
		}
		Tile tile = Main.tile[point.X, point.Y];
		if (tile == null || !tile.active() || tile.inActive() || !Main.tileSolid[tile.type])
		{
			return false;
		}
		if (treatPlatformsAsNonSolid && tile.type > 0 && tile.type <= TileID.Count && (TileID.Sets.Platforms[tile.type] || tile.type == 380))
		{
			return false;
		}
		int num = tile.blockType();
		switch (num)
		{
		case 0:
			if (pos.X >= (float)(point.X * 16) && pos.X <= (float)(point.X * 16 + 16) && pos.Y >= (float)(point.Y * 16))
			{
				return pos.Y <= (float)(point.Y * 16 + 16);
			}
			return false;
		case 1:
			if (pos.X >= (float)(point.X * 16) && pos.X <= (float)(point.X * 16 + 16) && pos.Y >= (float)(point.Y * 16 + 8))
			{
				return pos.Y <= (float)(point.Y * 16 + 16);
			}
			return false;
		case 2:
		case 3:
		case 4:
		case 5:
		{
			if (pos.X < (float)(point.X * 16) && pos.X > (float)(point.X * 16 + 16) && pos.Y < (float)(point.Y * 16) && pos.Y > (float)(point.Y * 16 + 16))
			{
				return false;
			}
			float num2 = pos.X % 16f;
			float num3 = pos.Y % 16f;
			switch (num)
			{
			case 3:
				return num2 + num3 >= 16f;
			case 2:
				return num3 >= num2;
			case 5:
				return num3 <= num2;
			case 4:
				return num2 + num3 <= 16f;
			}
			break;
		}
		}
		return false;
	}

	public static bool GetWaterLine(Point pt, out float waterLineHeight)
	{
		return GetWaterLine(pt.X, pt.Y, out waterLineHeight);
	}

	public static bool GetWaterLine(int X, int Y, out float waterLineHeight)
	{
		waterLineHeight = 0f;
		if (Main.tile[X, Y - 2] == null)
		{
			Main.tile[X, Y - 2] = new Tile();
		}
		if (Main.tile[X, Y - 1] == null)
		{
			Main.tile[X, Y - 1] = new Tile();
		}
		if (Main.tile[X, Y] == null)
		{
			Main.tile[X, Y] = new Tile();
		}
		if (Main.tile[X, Y + 1] == null)
		{
			Main.tile[X, Y + 1] = new Tile();
		}
		if (Main.tile[X, Y - 2].liquid > 0)
		{
			return false;
		}
		if (Main.tile[X, Y - 1].liquid > 0)
		{
			waterLineHeight = Y * 16;
			waterLineHeight -= (int)Main.tile[X, Y - 1].liquid / 16;
			return true;
		}
		if (Main.tile[X, Y].liquid > 0)
		{
			waterLineHeight = (Y + 1) * 16;
			waterLineHeight -= (int)Main.tile[X, Y].liquid / 16;
			return true;
		}
		if (Main.tile[X, Y + 1].liquid > 0)
		{
			waterLineHeight = (Y + 2) * 16;
			waterLineHeight -= (int)Main.tile[X, Y + 1].liquid / 16;
			return true;
		}
		return false;
	}

	public static bool GetWaterLineIterate(Point pt, out float waterLineHeight)
	{
		return GetWaterLineIterate(pt.X, pt.Y, out waterLineHeight);
	}

	public static bool GetWaterLineIterate(int X, int Y, out float waterLineHeight)
	{
		waterLineHeight = 0f;
		while (Y > 0 && Framing.GetTileSafely(X, Y).liquid > 0)
		{
			Y--;
		}
		Y++;
		if (Main.tile[X, Y] == null)
		{
			Main.tile[X, Y] = new Tile();
		}
		if (Main.tile[X, Y].liquid > 0)
		{
			waterLineHeight = Y * 16;
			waterLineHeight -= (int)Main.tile[X, Y - 1].liquid / 16;
			return true;
		}
		return false;
	}

	public static bool WetCollision(Vector2 Position, int Width, int Height)
	{
		honey = false;
		shimmer = false;
		Vector2 vector = new Vector2(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
		int num = 10;
		int num2 = Height / 2;
		if (num > Width)
		{
			num = Width;
		}
		if (num2 > Height)
		{
			num2 = Height;
		}
		vector = new Vector2(vector.X - (float)(num / 2), vector.Y - (float)(num2 / 2));
		int value = (int)(Position.X / 16f) - 1;
		int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int value3 = (int)(Position.Y / 16f) - 1;
		int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num3 = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
		Vector2 vector2 = default(Vector2);
		for (int i = num3; i < value2; i++)
		{
			for (int j = value3; j < value4; j++)
			{
				if (Main.tile[i, j] == null)
				{
					continue;
				}
				if (Main.tile[i, j].liquid > 0)
				{
					vector2.X = i * 16;
					vector2.Y = j * 16;
					int num4 = 16;
					float num5 = 256 - Main.tile[i, j].liquid;
					num5 /= 32f;
					vector2.Y += num5 * 2f;
					num4 -= (int)(num5 * 2f);
					if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num4)
					{
						if (Main.tile[i, j].honey())
						{
							honey = true;
						}
						if (Main.tile[i, j].shimmer())
						{
							shimmer = true;
						}
						return true;
					}
				}
				else
				{
					if (!Main.tile[i, j].active() || Main.tile[i, j].slope() == 0 || j <= 0 || Main.tile[i, j - 1] == null || Main.tile[i, j - 1].liquid <= 0)
					{
						continue;
					}
					vector2.X = i * 16;
					vector2.Y = j * 16;
					int num6 = 16;
					if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num6)
					{
						if (Main.tile[i, j - 1].honey())
						{
							honey = true;
						}
						else if (Main.tile[i, j - 1].shimmer())
						{
							shimmer = true;
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool LavaCollision(Vector2 Position, int Width, int Height)
	{
		int value = (int)(Position.X / 16f) - 1;
		int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int value3 = (int)(Position.Y / 16f) - 1;
		int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
		Vector2 i = default(Vector2);
		for (int j = num; j < value2; j++)
		{
			for (int k = value3; k < value4; k++)
			{
				if (Main.tile[j, k] != null && Main.tile[j, k].liquid > 0 && Main.tile[j, k].lava())
				{
					i.X = j * 16;
					i.Y = k * 16;
					int num2 = 16;
					float num3 = 256 - Main.tile[j, k].liquid;
					num3 /= 32f;
					i.Y += num3 * 2f;
					num2 -= (int)(num3 * 2f);
					if (Position.X + (float)Width > i.X && Position.X < i.X + 16f && Position.Y + (float)Height > i.Y && Position.Y < i.Y + (float)num2)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static Vector4 WalkDownSlope(Vector2 Position, Vector2 Velocity, int Width, int Height, float gravity = 0f)
	{
		if (Velocity.Y != gravity)
		{
			return new Vector4(Position, Velocity.X, Velocity.Y);
		}
		Vector2 vector = Position;
		int number = (int)(vector.X / 16f);
		int num = (int)((vector.X + (float)Width) / 16f);
		int num2 = (int)((Position.Y + (float)Height + 4f) / 16f);
		number = Utils.Clamp(number, 0, Main.maxTilesX - 1);
		num = Utils.Clamp(num, 0, Main.maxTilesX - 1);
		num2 = Utils.Clamp(num2, 0, Main.maxTilesY - 3);
		float i = (num2 + 3) * 16;
		int j = -1;
		int sectionY2 = -1;
		int num3 = 1;
		if (Velocity.X < 0f)
		{
			num3 = 2;
		}
		for (int k = number; k <= num; k++)
		{
			for (int l = num2; l <= num2 + 1; l++)
			{
				if (Main.tile[k, l] == null)
				{
					Main.tile[k, l] = new Tile();
				}
				if (!Main.tile[k, l].nactive() || (!Main.tileSolid[Main.tile[k, l].type] && !Main.tileSolidTop[Main.tile[k, l].type]))
				{
					continue;
				}
				int num4 = l * 16;
				if (Main.tile[k, l].halfBrick())
				{
					num4 += 8;
				}
				if (!new Rectangle(k * 16, l * 16 - 17, 16, 16).Intersects(new Rectangle((int)Position.X, (int)Position.Y, Width, Height)) || !((float)num4 <= i))
				{
					continue;
				}
				if (i == (float)num4)
				{
					if (Main.tile[k, l].slope() == 0)
					{
						continue;
					}
					if (j != -1 && sectionY2 != -1 && Main.tile[j, sectionY2] != null && Main.tile[j, sectionY2].slope() != 0)
					{
						if (Main.tile[k, l].slope() == num3)
						{
							i = num4;
							j = k;
							sectionY2 = l;
						}
					}
					else
					{
						i = num4;
						j = k;
						sectionY2 = l;
					}
				}
				else
				{
					i = num4;
					j = k;
					sectionY2 = l;
				}
			}
		}
		int num5 = j;
		int num6 = sectionY2;
		if (j != -1 && sectionY2 != -1 && Main.tile[num5, num6] != null && Main.tile[num5, num6].slope() > 0)
		{
			int num7 = Main.tile[num5, num6].slope();
			Vector2 vector2 = default(Vector2);
			vector2.X = num5 * 16;
			vector2.Y = num6 * 16;
			switch (num7)
			{
			case 2:
			{
				float num8 = vector2.X + 16f - (Position.X + (float)Width);
				if (Position.Y + (float)Height >= vector2.Y + num8 && Velocity.X < 0f)
				{
					Velocity.Y += Math.Abs(Velocity.X);
				}
				break;
			}
			case 1:
			{
				float num8 = Position.X - vector2.X;
				if (Position.Y + (float)Height >= vector2.Y + num8 && Velocity.X > 0f)
				{
					Velocity.Y += Math.Abs(Velocity.X);
				}
				break;
			}
			}
		}
		return new Vector4(Position, Velocity.X, Velocity.Y);
	}

	public static Vector4 SlopeCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, float gravity = 0f, bool fall = false)
	{
		stair = false;
		stairFall = false;
		bool[] text = new bool[5];
		float i = Position.Y;
		float y = Position.Y;
		sloping = false;
		Vector2 vector = Position;
		Vector2 vector2 = Position;
		Vector2 vector3 = Velocity;
		int value = (int)(Position.X / 16f) - 1;
		int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int value3 = (int)(Position.Y / 16f) - 1;
		int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
		Vector2 vector4 = default(Vector2);
		for (int j = num; j < value2; j++)
		{
			for (int k = value3; k < value4; k++)
			{
				if (Main.tile[j, k] == null || !Main.tile[j, k].active() || Main.tile[j, k].inActive() || (!Main.tileSolid[Main.tile[j, k].type] && (!Main.tileSolidTop[Main.tile[j, k].type] || Main.tile[j, k].frameY != 0)))
				{
					continue;
				}
				vector4.X = j * 16;
				vector4.Y = k * 16;
				int num2 = 16;
				if (Main.tile[j, k].halfBrick())
				{
					vector4.Y += 8f;
					num2 -= 8;
				}
				if (!(Position.X + (float)Width > vector4.X) || !(Position.X < vector4.X + 16f) || !(Position.Y + (float)Height > vector4.Y) || !(Position.Y < vector4.Y + (float)num2))
				{
					continue;
				}
				bool flag = true;
				if (TileID.Sets.Platforms[Main.tile[j, k].type])
				{
					if (Velocity.Y < 0f)
					{
						flag = false;
					}
					if (Position.Y + (float)Height < (float)(k * 16) || Position.Y + (float)Height - (1f + Math.Abs(Velocity.X)) > (float)(k * 16 + 16))
					{
						flag = false;
					}
					if (((Main.tile[j, k].slope() == 1 && Velocity.X >= 0f) || (Main.tile[j, k].slope() == 2 && Velocity.X <= 0f)) && (Position.Y + (float)Height) / 16f - 1f == (float)k)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					continue;
				}
				bool flag2 = false;
				if (fall && TileID.Sets.Platforms[Main.tile[j, k].type])
				{
					flag2 = true;
				}
				int num3 = Main.tile[j, k].slope();
				vector4.X = j * 16;
				vector4.Y = k * 16;
				if (!(Position.X + (float)Width > vector4.X) || !(Position.X < vector4.X + 16f) || !(Position.Y + (float)Height > vector4.Y) || !(Position.Y < vector4.Y + 16f))
				{
					continue;
				}
				float num4 = 0f;
				if (num3 == 3 || num3 == 4)
				{
					if (num3 == 3)
					{
						num4 = Position.X - vector4.X;
					}
					if (num3 == 4)
					{
						num4 = vector4.X + 16f - (Position.X + (float)Width);
					}
					if (num4 >= 0f)
					{
						if (Position.Y <= vector4.Y + 16f - num4)
						{
							float num5 = vector4.Y + 16f - vector.Y - num4;
							if (Position.Y + num5 > y)
							{
								vector2.Y = Position.Y + num5;
								y = vector2.Y;
								if (vector3.Y < 0.0101f)
								{
									vector3.Y = 0.0101f;
								}
								text[num3] = true;
							}
						}
					}
					else if (Position.Y > vector4.Y)
					{
						float num6 = vector4.Y + 16f;
						if (vector2.Y < num6)
						{
							vector2.Y = num6;
							if (vector3.Y < 0.0101f)
							{
								vector3.Y = 0.0101f;
							}
						}
					}
				}
				if (num3 != 1 && num3 != 2)
				{
					continue;
				}
				if (num3 == 1)
				{
					num4 = Position.X - vector4.X;
				}
				if (num3 == 2)
				{
					num4 = vector4.X + 16f - (Position.X + (float)Width);
				}
				if (num4 >= 0f)
				{
					if (!(Position.Y + (float)Height >= vector4.Y + num4))
					{
						continue;
					}
					float num7 = vector4.Y - (vector.Y + (float)Height) + num4;
					if (!(Position.Y + num7 < i))
					{
						continue;
					}
					if (flag2)
					{
						stairFall = true;
						continue;
					}
					if (TileID.Sets.Platforms[Main.tile[j, k].type])
					{
						stair = true;
					}
					else
					{
						stair = false;
					}
					vector2.Y = Position.Y + num7;
					i = vector2.Y;
					if (vector3.Y > 0f)
					{
						vector3.Y = 0f;
					}
					text[num3] = true;
					continue;
				}
				if (TileID.Sets.Platforms[Main.tile[j, k].type] && !(Position.Y + (float)Height - 4f - Math.Abs(Velocity.X) <= vector4.Y))
				{
					if (flag2)
					{
						stairFall = true;
					}
					continue;
				}
				float num8 = vector4.Y - (float)Height;
				if (!(vector2.Y > num8))
				{
					continue;
				}
				if (flag2)
				{
					stairFall = true;
					continue;
				}
				if (TileID.Sets.Platforms[Main.tile[j, k].type])
				{
					stair = true;
				}
				else
				{
					stair = false;
				}
				vector2.Y = num8;
				if (vector3.Y > 0f)
				{
					vector3.Y = 0f;
				}
			}
		}
		Vector2 position = Position;
		Vector2 velocity = vector2 - Position;
		Vector2 vector5 = TileCollision(position, velocity, Width, Height);
		if (vector5.Y > velocity.Y)
		{
			float num9 = velocity.Y - vector5.Y;
			vector2.Y = Position.Y + vector5.Y;
			if (text[1])
			{
				vector2.X = Position.X - num9;
			}
			if (text[2])
			{
				vector2.X = Position.X + num9;
			}
			vector3.X = 0f;
			vector3.Y = 0f;
			up = false;
		}
		else if (vector5.Y < velocity.Y)
		{
			float num10 = vector5.Y - velocity.Y;
			vector2.Y = Position.Y + vector5.Y;
			if (text[3])
			{
				vector2.X = Position.X - num10;
			}
			if (text[4])
			{
				vector2.X = Position.X + num10;
			}
			vector3.X = 0f;
			vector3.Y = 0f;
		}
		return new Vector4(vector2, vector3.X, vector3.Y);
	}

	public static Vector2 noSlopeCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false)
	{
		up = false;
		down = false;
		Vector2 i = Velocity;
		Vector2 num = Velocity;
		Vector2 vector = Position + Velocity;
		Vector2 vector2 = Position;
		int value = (int)(Position.X / 16f) - 1;
		int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int value3 = (int)(Position.Y / 16f) - 1;
		int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num3 = -1;
		int num4 = -1;
		int num5 = -1;
		int num6 = -1;
		int num7 = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
		float num8 = (value4 + 3) * 16;
		Vector2 num2 = default(Vector2);
		for (int j = num7; j < value2; j++)
		{
			for (int k = value3; k < value4; k++)
			{
				if (Main.tile[j, k] == null || !Main.tile[j, k].active() || (!Main.tileSolid[Main.tile[j, k].type] && (!Main.tileSolidTop[Main.tile[j, k].type] || Main.tile[j, k].frameY != 0)))
				{
					continue;
				}
				num2.X = j * 16;
				num2.Y = k * 16;
				int num9 = 16;
				if (Main.tile[j, k].halfBrick())
				{
					num2.Y += 8f;
					num9 -= 8;
				}
				if (!(vector.X + (float)Width > num2.X) || !(vector.X < num2.X + 16f) || !(vector.Y + (float)Height > num2.Y) || !(vector.Y < num2.Y + (float)num9))
				{
					continue;
				}
				if (vector2.Y + (float)Height <= num2.Y)
				{
					down = true;
					if ((!(Main.tileSolidTop[Main.tile[j, k].type] && fallThrough) || !(Velocity.Y <= 1f || fall2)) && num8 > num2.Y)
					{
						num5 = j;
						num6 = k;
						if (num9 < 16)
						{
							num6++;
						}
						if (num5 != num3)
						{
							i.Y = num2.Y - (vector2.Y + (float)Height);
							num8 = num2.Y;
						}
					}
				}
				else if (vector2.X + (float)Width <= num2.X && !Main.tileSolidTop[Main.tile[j, k].type])
				{
					num3 = j;
					num4 = k;
					if (num4 != num6)
					{
						i.X = num2.X - (vector2.X + (float)Width);
					}
					if (num5 == num3)
					{
						i.Y = num.Y;
					}
				}
				else if (vector2.X >= num2.X + 16f && !Main.tileSolidTop[Main.tile[j, k].type])
				{
					num3 = j;
					num4 = k;
					if (num4 != num6)
					{
						i.X = num2.X + 16f - vector2.X;
					}
					if (num5 == num3)
					{
						i.Y = num.Y;
					}
				}
				else if (vector2.Y >= num2.Y + (float)num9 && !Main.tileSolidTop[Main.tile[j, k].type])
				{
					up = true;
					num5 = j;
					num6 = k;
					i.Y = num2.Y + (float)num9 - vector2.Y + 0.01f;
					if (num6 == num4)
					{
						i.X = num.X;
					}
				}
			}
		}
		return i;
	}

	public static Vector2 TileCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false, int gravDir = 1)
	{
		up = false;
		down = false;
		Vector2 result = Velocity;
		Vector2 vector = Velocity;
		Vector2 vector2 = Position + Velocity;
		Vector2 vector3 = Position;
		int value = (int)(Position.X / 16f) - 1;
		int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int value3 = (int)(Position.Y / 16f) - 1;
		int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		int num5 = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
		float num6 = (value4 + 3) * 16;
		Vector2 vector4 = default(Vector2);
		for (int i = num5; i < value2; i++)
		{
			for (int j = value3; j < value4; j++)
			{
				if (Main.tile[i, j] == null || !Main.tile[i, j].active() || Main.tile[i, j].inActive() || (!Main.tileSolid[Main.tile[i, j].type] && (!Main.tileSolidTop[Main.tile[i, j].type] || Main.tile[i, j].frameY != 0)))
				{
					continue;
				}
				vector4.X = i * 16;
				vector4.Y = j * 16;
				int num7 = 16;
				if (Main.tile[i, j].halfBrick())
				{
					vector4.Y += 8f;
					num7 -= 8;
				}
				if (!(vector2.X + (float)Width > vector4.X) || !(vector2.X < vector4.X + 16f) || !(vector2.Y + (float)Height > vector4.Y) || !(vector2.Y < vector4.Y + (float)num7))
				{
					continue;
				}
				bool flag = false;
				bool flag2 = false;
				if (Main.tile[i, j].slope() > 2)
				{
					if (Main.tile[i, j].slope() == 3 && vector3.Y + Math.Abs(Velocity.X) >= vector4.Y && vector3.X >= vector4.X)
					{
						flag2 = true;
					}
					if (Main.tile[i, j].slope() == 4 && vector3.Y + Math.Abs(Velocity.X) >= vector4.Y && vector3.X + (float)Width <= vector4.X + 16f)
					{
						flag2 = true;
					}
				}
				else if (Main.tile[i, j].slope() > 0)
				{
					flag = true;
					if (Main.tile[i, j].slope() == 1 && vector3.Y + (float)Height - Math.Abs(Velocity.X) <= vector4.Y + (float)num7 && vector3.X >= vector4.X)
					{
						flag2 = true;
					}
					if (Main.tile[i, j].slope() == 2 && vector3.Y + (float)Height - Math.Abs(Velocity.X) <= vector4.Y + (float)num7 && vector3.X + (float)Width <= vector4.X + 16f)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					continue;
				}
				if (vector3.Y + (float)Height <= vector4.Y)
				{
					down = true;
					if ((!(Main.tileSolidTop[Main.tile[i, j].type] && fallThrough) || !(Velocity.Y <= 1f || fall2)) && num6 > vector4.Y)
					{
						num3 = i;
						num4 = j;
						if (num7 < 16)
						{
							num4++;
						}
						if (num3 != num && !flag)
						{
							result.Y = vector4.Y - (vector3.Y + (float)Height) + ((gravDir == -1) ? (-0.01f) : 0f);
							num6 = vector4.Y;
						}
					}
				}
				else if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[Main.tile[i, j].type])
				{
					if (i >= 1 && Main.tile[i - 1, j] == null)
					{
						Main.tile[i - 1, j] = new Tile();
					}
					if (i < 1 || (Main.tile[i - 1, j].slope() != 2 && Main.tile[i - 1, j].slope() != 4))
					{
						num = i;
						num2 = j;
						if (num2 != num4)
						{
							result.X = vector4.X - (vector3.X + (float)Width);
						}
						if (num3 == num)
						{
							result.Y = vector.Y;
						}
					}
				}
				else if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[Main.tile[i, j].type])
				{
					if (Main.tile[i + 1, j] == null)
					{
						Main.tile[i + 1, j] = new Tile();
					}
					if (Main.tile[i + 1, j].slope() != 1 && Main.tile[i + 1, j].slope() != 3)
					{
						num = i;
						num2 = j;
						if (num2 != num4)
						{
							result.X = vector4.X + 16f - vector3.X;
						}
						if (num3 == num)
						{
							result.Y = vector.Y;
						}
					}
				}
				else if (vector3.Y >= vector4.Y + (float)num7 && !Main.tileSolidTop[Main.tile[i, j].type])
				{
					up = true;
					num3 = i;
					num4 = j;
					result.Y = vector4.Y + (float)num7 - vector3.Y + ((gravDir == 1) ? 0.01f : 0f);
					if (num4 == num2)
					{
						result.X = vector.X;
					}
				}
			}
		}
		return result;
	}

	public static bool IsClearSpotTest(Vector2 position, float testMagnitude, int Width, int Height, bool fallThrough = false, bool fall2 = false, int gravDir = 1, bool checkCardinals = true, bool checkSlopes = false)
	{
		if (checkCardinals)
		{
			Vector2 i = Vector2.UnitX * testMagnitude;
			if (TileCollision(position - i, i, Width, Height, fallThrough, fall2, gravDir) != i)
			{
				return false;
			}
			i = -Vector2.UnitX * testMagnitude;
			if (TileCollision(position - i, i, Width, Height, fallThrough, fall2, gravDir) != i)
			{
				return false;
			}
			i = Vector2.UnitY * testMagnitude;
			if (TileCollision(position - i, i, Width, Height, fallThrough, fall2, gravDir) != i)
			{
				return false;
			}
			i = -Vector2.UnitY * testMagnitude;
			if (TileCollision(position - i, i, Width, Height, fallThrough, fall2, gravDir) != i)
			{
				return false;
			}
		}
		if (checkSlopes)
		{
			Vector2 i = Vector2.UnitX * testMagnitude;
			Vector4 vector = new Vector4(position, testMagnitude, 0f);
			if (SlopeCollision(position, i, Width, Height, gravDir, fallThrough) != vector)
			{
				return false;
			}
			i = -Vector2.UnitX * testMagnitude;
			vector = new Vector4(position, 0f - testMagnitude, 0f);
			if (SlopeCollision(position, i, Width, Height, gravDir, fallThrough) != vector)
			{
				return false;
			}
			i = Vector2.UnitY * testMagnitude;
			vector = new Vector4(position, 0f, testMagnitude);
			if (SlopeCollision(position, i, Width, Height, gravDir, fallThrough) != vector)
			{
				return false;
			}
			i = -Vector2.UnitY * testMagnitude;
			vector = new Vector4(position, 0f, 0f - testMagnitude);
			if (SlopeCollision(position, i, Width, Height, gravDir, fallThrough) != vector)
			{
				return false;
			}
		}
		return true;
	}

	public static List<Point> FindCollisionTile(int Direction, Vector2 position, float testMagnitude, int Width, int Height, bool fallThrough = false, bool fall2 = false, int gravDir = 1, bool checkCardinals = true, bool checkSlopes = false)
	{
		List<Point> flag = new List<Point>();
		if ((uint)Direction > 1u)
		{
			if ((uint)(Direction - 2) <= 1u)
			{
				Vector2 householdStatus = ((Direction == 2) ? (Vector2.UnitY * testMagnitude) : (-Vector2.UnitY * testMagnitude));
				Vector4 vec = new Vector4(position, householdStatus.X, householdStatus.Y);
				int i = (int)(position.Y + (float)((Direction == 2) ? Height : 0)) / 16;
				float num = Math.Min(16f - position.X % 16f, Width);
				float num2 = num;
				if (checkCardinals && TileCollision(position - householdStatus, householdStatus, (int)num, Height, fallThrough, fall2, gravDir) != householdStatus)
				{
					flag.Add(new Point((int)position.X / 16, i));
				}
				else if (checkSlopes && SlopeCollision(position, householdStatus, (int)num, Height, gravDir, fallThrough).YZW() != vec.YZW())
				{
					flag.Add(new Point((int)position.X / 16, i));
				}
				for (; num2 + 16f <= (float)(Width - 16); num2 += 16f)
				{
					if (checkCardinals && TileCollision(position - householdStatus + Vector2.UnitX * num2, householdStatus, 16, Height, fallThrough, fall2, gravDir) != householdStatus)
					{
						flag.Add(new Point((int)(position.X + num2) / 16, i));
					}
					else if (checkSlopes && SlopeCollision(position + Vector2.UnitX * num2, householdStatus, 16, Height, gravDir, fallThrough).YZW() != vec.YZW())
					{
						flag.Add(new Point((int)(position.X + num2) / 16, i));
					}
				}
				int width = Width - (int)num2;
				if (checkCardinals && TileCollision(position - householdStatus + Vector2.UnitX * num2, householdStatus, width, Height, fallThrough, fall2, gravDir) != householdStatus)
				{
					flag.Add(new Point((int)(position.X + num2) / 16, i));
				}
				else if (checkSlopes && SlopeCollision(position + Vector2.UnitX * num2, householdStatus, width, Height, gravDir, fallThrough).YZW() != vec.YZW())
				{
					flag.Add(new Point((int)(position.X + num2) / 16, i));
				}
			}
		}
		else
		{
			Vector2 householdStatus = ((Direction == 0) ? (Vector2.UnitX * testMagnitude) : (-Vector2.UnitX * testMagnitude));
			Vector4 vec = new Vector4(position, householdStatus.X, householdStatus.Y);
			int i = (int)(position.X + (float)((Direction == 0) ? Width : 0)) / 16;
			float num3 = Math.Min(16f - position.Y % 16f, Height);
			float num4 = num3;
			if (checkCardinals && TileCollision(position - householdStatus, householdStatus, Width, (int)num3, fallThrough, fall2, gravDir) != householdStatus)
			{
				flag.Add(new Point(i, (int)position.Y / 16));
			}
			else if (checkSlopes && SlopeCollision(position, householdStatus, Width, (int)num3, gravDir, fallThrough).XZW() != vec.XZW())
			{
				flag.Add(new Point(i, (int)position.Y / 16));
			}
			for (; num4 + 16f <= (float)(Height - 16); num4 += 16f)
			{
				if (checkCardinals && TileCollision(position - householdStatus + Vector2.UnitY * num4, householdStatus, Width, 16, fallThrough, fall2, gravDir) != householdStatus)
				{
					flag.Add(new Point(i, (int)(position.Y + num4) / 16));
				}
				else if (checkSlopes && SlopeCollision(position + Vector2.UnitY * num4, householdStatus, Width, 16, gravDir, fallThrough).XZW() != vec.XZW())
				{
					flag.Add(new Point(i, (int)(position.Y + num4) / 16));
				}
			}
			int height = Height - (int)num4;
			if (checkCardinals && TileCollision(position - householdStatus + Vector2.UnitY * num4, householdStatus, Width, height, fallThrough, fall2, gravDir) != householdStatus)
			{
				flag.Add(new Point(i, (int)(position.Y + num4) / 16));
			}
			else if (checkSlopes && SlopeCollision(position + Vector2.UnitY * num4, householdStatus, Width, height, gravDir, fallThrough).XZW() != vec.XZW())
			{
				flag.Add(new Point(i, (int)(position.Y + num4) / 16));
			}
		}
		return flag;
	}

	public static bool FindCollisionDirection(out int Direction, Vector2 position, int Width, int Height, bool fallThrough = false, bool fall2 = false, int gravDir = 1)
	{
		Vector2 flag = Vector2.UnitX * 16f;
		if (TileCollision(position - flag, flag, Width, Height, fallThrough, fall2, gravDir) != flag)
		{
			Direction = 0;
			return true;
		}
		flag = -Vector2.UnitX * 16f;
		if (TileCollision(position - flag, flag, Width, Height, fallThrough, fall2, gravDir) != flag)
		{
			Direction = 1;
			return true;
		}
		flag = Vector2.UnitY * 16f;
		if (TileCollision(position - flag, flag, Width, Height, fallThrough, fall2, gravDir) != flag)
		{
			Direction = 2;
			return true;
		}
		flag = -Vector2.UnitY * 16f;
		if (TileCollision(position - flag, flag, Width, Height, fallThrough, fall2, gravDir) != flag)
		{
			Direction = 3;
			return true;
		}
		Direction = -1;
		return false;
	}

	public static bool SolidCollision(Vector2 Position, int Width, int Height)
	{
		int value = (int)(Position.X / 16f) - 1;
		int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int value3 = (int)(Position.Y / 16f) - 1;
		int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
		Vector2 vector = default(Vector2);
		for (int i = num; i < value2; i++)
		{
			for (int j = value3; j < value4; j++)
			{
				if (Main.tile[i, j] != null && !Main.tile[i, j].inActive() && Main.tile[i, j].active() && Main.tileSolid[Main.tile[i, j].type] && !Main.tileSolidTop[Main.tile[i, j].type])
				{
					vector.X = i * 16;
					vector.Y = j * 16;
					int num2 = 16;
					if (Main.tile[i, j].halfBrick())
					{
						vector.Y += 8f;
						num2 -= 8;
					}
					if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num2)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool SolidCollision(Vector2 Position, int Width, int Height, bool acceptTopSurfaces)
	{
		int value = (int)(Position.X / 16f) - 1;
		int i = (int)((Position.X + (float)Width) / 16f) + 2;
		int j = (int)(Position.Y / 16f) - 1;
		int k = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num2 = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		i = Utils.Clamp(i, 0, Main.maxTilesX - 1);
		j = Utils.Clamp(j, 0, Main.maxTilesY - 1);
		k = Utils.Clamp(k, 0, Main.maxTilesY - 1);
		Vector2 num = default(Vector2);
		for (int l = num2; l < i; l++)
		{
			for (int m = j; m < k; m++)
			{
				Tile tile = Main.tile[l, m];
				if (tile == null || !tile.active() || tile.inActive())
				{
					continue;
				}
				bool flag = Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type];
				if (acceptTopSurfaces)
				{
					flag |= Main.tileSolidTop[tile.type] && tile.frameY == 0;
				}
				if (flag)
				{
					num.X = l * 16;
					num.Y = m * 16;
					int num3 = 16;
					if (tile.halfBrick())
					{
						num.Y += 8f;
						num3 -= 8;
					}
					if (Position.X + (float)Width > num.X && Position.X < num.X + 16f && Position.Y + (float)Height > num.Y && Position.Y < num.Y + (float)num3)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static Vector2 WaterCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false, bool lavaWalk = true)
	{
		Vector2 result = Velocity;
		Vector2 vector = Position + Velocity;
		Vector2 vector2 = Position;
		int value = (int)(Position.X / 16f) - 1;
		int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int value3 = (int)(Position.Y / 16f) - 1;
		int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
		Vector2 vector3 = default(Vector2);
		for (int i = num; i < value2; i++)
		{
			for (int j = value3; j < value4; j++)
			{
				if (Main.tile[i, j] != null && Main.tile[i, j].liquid > 0 && Main.tile[i, j - 1].liquid == 0 && (!Main.tile[i, j].lava() || lavaWalk))
				{
					int num2 = (int)Main.tile[i, j].liquid / 32 * 2 + 2;
					vector3.X = i * 16;
					vector3.Y = j * 16 + 16 - num2;
					if (vector.X + (float)Width > vector3.X && vector.X < vector3.X + 16f && vector.Y + (float)Height > vector3.Y && vector.Y < vector3.Y + (float)num2 && vector2.Y + (float)Height <= vector3.Y && !fallThrough)
					{
						result.Y = vector3.Y - (vector2.Y + (float)Height);
					}
				}
			}
		}
		return result;
	}

	public static Vector2 AnyCollisionWithSpecificTiles(Vector2 Position, Vector2 Velocity, int Width, int Height, bool[] tilesWeCanCollideWithByType, bool evenActuated = false)
	{
		Vector2 result = Velocity;
		Vector2 vector = Velocity;
		Vector2 vector2 = Position + Velocity;
		Vector2 vector3 = Position;
		int num = (int)(Position.X / 16f) - 1;
		int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int num3 = (int)(Position.Y / 16f) - 1;
		int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num5 = -1;
		int num6 = -1;
		int num7 = -1;
		int num8 = -1;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 > Main.maxTilesX)
		{
			num2 = Main.maxTilesX;
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		if (num4 > Main.maxTilesY)
		{
			num4 = Main.maxTilesY;
		}
		Vector2 vector4 = default(Vector2);
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				Tile tile = Main.tile[i, j];
				if (tile == null || !tile.active() || (!evenActuated && tile.inActive()) || !tilesWeCanCollideWithByType[tile.type])
				{
					continue;
				}
				vector4.X = i * 16;
				vector4.Y = j * 16;
				int num9 = 16;
				if (tile.halfBrick())
				{
					vector4.Y += 8f;
					num9 -= 8;
				}
				if (!(vector2.X + (float)Width > vector4.X) || !(vector2.X < vector4.X + 16f) || !(vector2.Y + (float)Height > vector4.Y) || !(vector2.Y < vector4.Y + (float)num9))
				{
					continue;
				}
				if (vector3.Y + (float)Height <= vector4.Y)
				{
					num7 = i;
					num8 = j;
					if (num7 != num5)
					{
						result.Y = vector4.Y - (vector3.Y + (float)Height);
					}
				}
				else if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[tile.type])
				{
					num5 = i;
					num6 = j;
					if (num6 != num8)
					{
						result.X = vector4.X - (vector3.X + (float)Width);
					}
					if (num7 == num5)
					{
						result.Y = vector.Y;
					}
				}
				else if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[tile.type])
				{
					num5 = i;
					num6 = j;
					if (num6 != num8)
					{
						result.X = vector4.X + 16f - vector3.X;
					}
					if (num7 == num5)
					{
						result.Y = vector.Y;
					}
				}
				else if (vector3.Y >= vector4.Y + (float)num9 && !Main.tileSolidTop[tile.type])
				{
					num7 = i;
					num8 = j;
					result.Y = vector4.Y + (float)num9 - vector3.Y + 0.01f;
					if (num8 == num6)
					{
						result.X = vector.X + 0.01f;
					}
				}
			}
		}
		return result;
	}

	public static Vector2 AnyCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool evenActuated = false)
	{
		Vector2 result = Velocity;
		Vector2 vector = Velocity;
		Vector2 vector2 = Position + Velocity;
		Vector2 vector3 = Position;
		int num = (int)(Position.X / 16f) - 1;
		int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int num3 = (int)(Position.Y / 16f) - 1;
		int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num5 = -1;
		int num6 = -1;
		int num7 = -1;
		int num8 = -1;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 > Main.maxTilesX)
		{
			num2 = Main.maxTilesX;
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		if (num4 > Main.maxTilesY)
		{
			num4 = Main.maxTilesY;
		}
		Vector2 vector4 = default(Vector2);
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				if (Main.tile[i, j] == null || !Main.tile[i, j].active() || (!evenActuated && Main.tile[i, j].inActive()))
				{
					continue;
				}
				vector4.X = i * 16;
				vector4.Y = j * 16;
				int num9 = 16;
				if (Main.tile[i, j].halfBrick())
				{
					vector4.Y += 8f;
					num9 -= 8;
				}
				if (!(vector2.X + (float)Width > vector4.X) || !(vector2.X < vector4.X + 16f) || !(vector2.Y + (float)Height > vector4.Y) || !(vector2.Y < vector4.Y + (float)num9))
				{
					continue;
				}
				if (vector3.Y + (float)Height <= vector4.Y)
				{
					num7 = i;
					num8 = j;
					if (num7 != num5)
					{
						result.Y = vector4.Y - (vector3.Y + (float)Height);
					}
				}
				else if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[Main.tile[i, j].type])
				{
					num5 = i;
					num6 = j;
					if (num6 != num8)
					{
						result.X = vector4.X - (vector3.X + (float)Width);
					}
					if (num7 == num5)
					{
						result.Y = vector.Y;
					}
				}
				else if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[Main.tile[i, j].type])
				{
					num5 = i;
					num6 = j;
					if (num6 != num8)
					{
						result.X = vector4.X + 16f - vector3.X;
					}
					if (num7 == num5)
					{
						result.Y = vector.Y;
					}
				}
				else if (vector3.Y >= vector4.Y + (float)num9 && !Main.tileSolidTop[Main.tile[i, j].type])
				{
					num7 = i;
					num8 = j;
					result.Y = vector4.Y + (float)num9 - vector3.Y + 0.01f;
					if (num8 == num6)
					{
						result.X = vector.X + 0.01f;
					}
				}
			}
		}
		return result;
	}

	public static void HitTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
	{
		Vector2 vector = Position + Velocity;
		int num = (int)(Position.X / 16f) - 1;
		int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int num3 = (int)(Position.Y / 16f) - 1;
		int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 > Main.maxTilesX)
		{
			num2 = Main.maxTilesX;
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		if (num4 > Main.maxTilesY)
		{
			num4 = Main.maxTilesY;
		}
		Vector2 vector2 = default(Vector2);
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				if (Main.tile[i, j] != null && !Main.tile[i, j].inActive() && Main.tile[i, j].active() && (Main.tileSolid[Main.tile[i, j].type] || (Main.tileSolidTop[Main.tile[i, j].type] && Main.tile[i, j].frameY == 0)))
				{
					vector2.X = i * 16;
					vector2.Y = j * 16;
					int num5 = 16;
					if (Main.tile[i, j].halfBrick())
					{
						vector2.Y += 8f;
						num5 -= 8;
					}
					if (vector.X + (float)Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + (float)Height >= vector2.Y && vector.Y <= vector2.Y + (float)num5)
					{
						WorldGen.KillTile(i, j, fail: true, effectOnly: true);
					}
				}
			}
		}
	}

	public static bool AnyHurtingTiles(Vector2 Position, int Width, int Height)
	{
		return HurtTiles(Position, Width, Height, null).type >= 0;
	}

	public static HurtTile HurtTiles(Vector2 Position, int Width, int Height, Player player)
	{
		int result = (int)(Position.X / 16f) - 1;
		int num = (int)((Position.X + (float)Width) / 16f) + 2;
		int num2 = (int)(Position.Y / 16f) - 1;
		int iPAddress = (int)((Position.Y + (float)Height) / 16f) + 2;
		if (result < 0)
		{
			result = 0;
		}
		if (num > Main.maxTilesX)
		{
			num = Main.maxTilesX;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (iPAddress > Main.maxTilesY)
		{
			iPAddress = Main.maxTilesY;
		}
		Vector2 vector = default(Vector2);
		HurtTile result2;
		for (int i = result; i < num; i++)
		{
			for (int j = num2; j < iPAddress; j++)
			{
				Tile tile = Main.tile[i, j];
				if (tile == null || tile.inActive() || !tile.active())
				{
					continue;
				}
				vector.X = i * 16;
				vector.Y = j * 16;
				int num3 = 16;
				if (tile.halfBrick())
				{
					vector.Y += 8f;
					num3 -= 8;
				}
				int num4 = 0;
				if (TileID.Sets.Suffocate[tile.type])
				{
					num4 = 2;
				}
				if (Position.X + (float)Width - (float)num4 < vector.X || Position.X + (float)num4 > vector.X + 16f || Position.Y + (float)Height - (float)num4 < vector.Y - 0.5f || Position.Y + (float)num4 > vector.Y + (float)num3 + 0.5f || !CanTileHurt(tile.type, i, j, player))
				{
					continue;
				}
				if (tile.slope() > 0)
				{
					if (num4 > 0)
					{
						continue;
					}
					int num5 = 0;
					if (tile.rightSlope() && Position.X > vector.X)
					{
						num5++;
					}
					if (tile.leftSlope() && Position.X + (float)Width < vector.X + 16f)
					{
						num5++;
					}
					if (tile.bottomSlope() && Position.Y > vector.Y)
					{
						num5++;
					}
					if (tile.topSlope() && Position.Y + (float)Height < vector.Y + (float)num3)
					{
						num5++;
					}
					if (num5 == 2)
					{
						continue;
					}
				}
				result2 = default(HurtTile);
				result2.type = tile.type;
				result2.x = i;
				result2.y = j;
				return result2;
			}
		}
		result2 = default(HurtTile);
		result2.type = -1;
		return result2;
	}

	public static bool CanTileHurt(ushort type, int i, int j, Player player)
	{
		if (type == 230 && !Main.getGoodWorld)
		{
			return false;
		}
		if (type == 80 && !Main.dontStarveWorld)
		{
			return false;
		}
		if (TileID.Sets.TouchDamageBleeding[type] || TileID.Sets.Suffocate[type] || TileID.Sets.TouchDamageImmediate[type] > 0)
		{
			return true;
		}
		if (TileID.Sets.TouchDamageHot[type] && (player == null || !player.fireWalk))
		{
			return true;
		}
		return false;
	}

	public static bool SwitchTiles(Vector2 Position, int Width, int Height, Vector2 oldPosition, int objType)
	{
		int j = (int)(Position.X / 16f) - 1;
		int k = (int)((Position.X + (float)Width) / 16f) + 2;
		int num = (int)(Position.Y / 16f) - 1;
		int num2 = (int)((Position.Y + (float)Height) / 16f) + 2;
		if (j < 0)
		{
			j = 0;
		}
		if (k > Main.maxTilesX)
		{
			k = Main.maxTilesX;
		}
		if (num < 0)
		{
			num = 0;
		}
		if (num2 > Main.maxTilesY)
		{
			num2 = Main.maxTilesY;
		}
		Vector2 i = default(Vector2);
		for (int l = j; l < k; l++)
		{
			for (int m = num; m < num2; m++)
			{
				if (Main.tile[l, m] == null)
				{
					continue;
				}
				int type = Main.tile[l, m].type;
				if (!Main.tile[l, m].active() || (type != 135 && type != 210 && type != 443 && type != 442))
				{
					continue;
				}
				i.X = l * 16;
				i.Y = m * 16 + 12;
				bool flag = false;
				if (type == 442)
				{
					if (objType == 4)
					{
						float r1StartX = 0f;
						float r1StartY = 0f;
						float r1Width = 0f;
						float r1Height = 0f;
						switch (Main.tile[l, m].frameX / 22)
						{
						case 0:
							r1StartX = l * 16;
							r1StartY = m * 16 + 16 - 10;
							r1Width = 16f;
							r1Height = 10f;
							break;
						case 1:
							r1StartX = l * 16;
							r1StartY = m * 16;
							r1Width = 16f;
							r1Height = 10f;
							break;
						case 2:
							r1StartX = l * 16;
							r1StartY = m * 16;
							r1Width = 10f;
							r1Height = 16f;
							break;
						case 3:
							r1StartX = l * 16 + 16 - 10;
							r1StartY = m * 16;
							r1Width = 10f;
							r1Height = 16f;
							break;
						}
						if (Utils.FloatIntersect(r1StartX, r1StartY, r1Width, r1Height, Position.X, Position.Y, Width, Height) && !Utils.FloatIntersect(r1StartX, r1StartY, r1Width, r1Height, oldPosition.X, oldPosition.Y, Width, Height))
						{
							Wiring.HitSwitch(l, m);
							NetMessage.SendData(59, -1, -1, null, l, m);
							return true;
						}
					}
					flag = true;
				}
				if (flag || !(Position.X + (float)Width > i.X) || !(Position.X < i.X + 16f) || !(Position.Y + (float)Height > i.Y) || !((double)Position.Y < (double)i.Y + 4.01))
				{
					continue;
				}
				if (type == 210)
				{
					WorldGen.ExplodeMine(l, m, fromWiring: false);
				}
				else
				{
					if (oldPosition.X + (float)Width > i.X && oldPosition.X < i.X + 16f && oldPosition.Y + (float)Height > i.Y && (double)oldPosition.Y < (double)i.Y + 16.01)
					{
						continue;
					}
					if (type == 443)
					{
						if (objType == 1)
						{
							Wiring.HitSwitch(l, m);
							NetMessage.SendData(59, -1, -1, null, l, m);
						}
						continue;
					}
					int num3 = Main.tile[l, m].frameY / 18;
					bool flag2 = true;
					if ((num3 == 4 || num3 == 2 || num3 == 3 || num3 == 6 || num3 == 7) && objType != 1)
					{
						flag2 = false;
					}
					if (num3 == 5 && (objType == 1 || objType == 4))
					{
						flag2 = false;
					}
					if (!flag2)
					{
						continue;
					}
					Wiring.HitSwitch(l, m);
					NetMessage.SendData(59, -1, -1, null, l, m);
					if (num3 == 7)
					{
						WorldGen.KillTile(l, m);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(17, -1, -1, null, 0, l, m);
						}
					}
					return true;
				}
			}
		}
		return false;
	}

	public bool SwitchTilesNew(Vector2 Position, int Width, int Height, Vector2 oldPosition, int objType)
	{
		Point point = Position.ToTileCoordinates();
		Point remoteAddress = (Position + new Vector2(Width, Height)).ToTileCoordinates();
		int streamWriter = Utils.Clamp(point.X, 0, Main.maxTilesX - 1);
		int num = Utils.Clamp(point.Y, 0, Main.maxTilesY - 1);
		int num2 = Utils.Clamp(remoteAddress.X, 0, Main.maxTilesX - 1);
		int num3 = Utils.Clamp(remoteAddress.Y, 0, Main.maxTilesY - 1);
		for (int i = streamWriter; i <= num2; i++)
		{
			for (int j = num; j <= num3; j++)
			{
				if (Main.tile[i, j] != null)
				{
					_ = Main.tile[i, j].type;
					_ = 130;
				}
			}
		}
		return false;
	}

	public static Vector2 StickyTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
	{
		Vector2 streamReader = Position;
		int a = (int)(Position.X / 16f) - 1;
		int num = (int)((Position.X + (float)Width) / 16f) + 2;
		int num2 = (int)(Position.Y / 16f) - 1;
		int num3 = (int)((Position.Y + (float)Height) / 16f) + 2;
		if (a < 0)
		{
			a = 0;
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
		Vector2 identifier = default(Vector2);
		for (int i = a; i < num; i++)
		{
			for (int j = num2; j < num3; j++)
			{
				if (Main.tile[i, j] == null || !Main.tile[i, j].active() || Main.tile[i, j].inActive())
				{
					continue;
				}
				if (Main.tile[i, j].type == 51)
				{
					int num4 = 0;
					identifier.X = i * 16;
					identifier.Y = j * 16;
					if (streamReader.X + (float)Width > identifier.X - (float)num4 && streamReader.X < identifier.X + 16f + (float)num4 && streamReader.Y + (float)Height > identifier.Y && (double)streamReader.Y < (double)identifier.Y + 16.01)
					{
						if (Main.tile[i, j].type == 51 && (double)(Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > 0.7 && Main.rand.Next(30) == 0)
						{
							Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, 30);
						}
						return new Vector2(i, j);
					}
				}
				else
				{
					if (Main.tile[i, j].type != 229 || Main.tile[i, j].slope() != 0)
					{
						continue;
					}
					int num5 = 1;
					identifier.X = i * 16;
					identifier.Y = j * 16;
					float num6 = 16.01f;
					if (Main.tile[i, j].halfBrick())
					{
						identifier.Y += 8f;
						num6 -= 8f;
					}
					if (streamReader.X + (float)Width > identifier.X - (float)num5 && streamReader.X < identifier.X + 16f + (float)num5 && streamReader.Y + (float)Height > identifier.Y && streamReader.Y < identifier.Y + num6)
					{
						if (Main.tile[i, j].type == 51 && (double)(Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > 0.7 && Main.rand.Next(30) == 0)
						{
							Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, 30);
						}
						return new Vector2(i, j);
					}
				}
			}
		}
		return new Vector2(-1f, -1f);
	}

	public static bool SolidTilesVersatile(int startX, int endX, int startY, int endY)
	{
		if (startX > endX)
		{
			Utils.Swap(ref startX, ref endX);
		}
		if (startY > endY)
		{
			Utils.Swap(ref startY, ref endY);
		}
		return SolidTiles(startX, endX, startY, endY);
	}

	public static bool SolidTiles(Vector2 position, int width, int height)
	{
		return SolidTiles((int)(position.X / 16f), (int)((position.X + (float)width) / 16f), (int)(position.Y / 16f), (int)((position.Y + (float)height) / 16f));
	}

	public static bool SolidTiles(int startX, int endX, int startY, int endY)
	{
		if (startX < 0)
		{
			return true;
		}
		if (endX >= Main.maxTilesX)
		{
			return true;
		}
		if (startY < 0)
		{
			return true;
		}
		if (endY >= Main.maxTilesY)
		{
			return true;
		}
		for (int i = startX; i < endX + 1; i++)
		{
			for (int j = startY; j < endY + 1; j++)
			{
				if (Main.tile[i, j] == null)
				{
					return false;
				}
				if (Main.tile[i, j].active() && !Main.tile[i, j].inActive() && Main.tileSolid[Main.tile[i, j].type] && !Main.tileSolidTop[Main.tile[i, j].type])
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool SolidTiles(Vector2 position, int width, int height, bool allowTopSurfaces)
	{
		return SolidTiles((int)(position.X / 16f), (int)((position.X + (float)width) / 16f), (int)(position.Y / 16f), (int)((position.Y + (float)height) / 16f), allowTopSurfaces);
	}

	public static bool SolidTiles(int startX, int endX, int startY, int endY, bool allowTopSurfaces)
	{
		if (startX < 0)
		{
			return true;
		}
		if (endX >= Main.maxTilesX)
		{
			return true;
		}
		if (startY < 0)
		{
			return true;
		}
		if (endY >= Main.maxTilesY)
		{
			return true;
		}
		for (int writer = startX; writer < endX + 1; writer++)
		{
			for (int position = startY; position < endY + 1; position++)
			{
				Tile num = Main.tile[writer, position];
				if (num == null)
				{
					return false;
				}
				if (num.active() && !Main.tile[writer, position].inActive())
				{
					ushort type = num.type;
					bool flag = Main.tileSolid[type] && !Main.tileSolidTop[type];
					if (allowTopSurfaces)
					{
						flag |= Main.tileSolidTop[type] && num.frameY == 0;
					}
					if (flag)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static void StepDown(ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir = 1, bool waterWalk = false)
	{
		Vector2 vector = position;
		vector.X += velocity.X;
		vector.Y = (float)Math.Floor((vector.Y + (float)height) / 16f) * 16f - (float)height;
		bool flag = false;
		int num = (int)(vector.X / 16f);
		int num2 = (int)((vector.X + (float)width) / 16f);
		int num3 = (int)((vector.Y + (float)height + 4f) / 16f);
		int num4 = height / 16 + ((height % 16 != 0) ? 1 : 0);
		float num5 = (num3 + num4) * 16;
		float num6 = Main.bottomWorld / 16f - 42f;
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num3 + 1; j++)
			{
				if (!WorldGen.InWorld(i, j, 1))
				{
					continue;
				}
				if (Main.tile[i, j] == null)
				{
					Main.tile[i, j] = new Tile();
				}
				if (Main.tile[i, j - 1] == null)
				{
					Main.tile[i, j - 1] = new Tile();
				}
				if (waterWalk && Main.tile[i, j].liquid > 0 && Main.tile[i, j - 1].liquid == 0)
				{
					int num7 = (int)Main.tile[i, j].liquid / 32 * 2 + 2;
					int num8 = j * 16 + 16 - num7;
					if (new Rectangle(i * 16, j * 16 - 17, 16, 16).Intersects(new Rectangle((int)position.X, (int)position.Y, width, height)) && (float)num8 < num5)
					{
						num5 = num8;
					}
				}
				if ((float)j >= num6 || (Main.tile[i, j].nactive() && (Main.tileSolid[Main.tile[i, j].type] || Main.tileSolidTop[Main.tile[i, j].type])))
				{
					int num9 = j * 16;
					if (Main.tile[i, j].halfBrick())
					{
						num9 += 8;
					}
					if (Utils.FloatIntersect(i * 16, j * 16 - 17, 16f, 16f, position.X, position.Y, width, height) && (float)num9 < num5)
					{
						num5 = num9;
					}
				}
			}
		}
		float num10 = num5 - (position.Y + (float)height);
		if (num10 > 7f && num10 < 17f && !flag)
		{
			stepSpeed = 1.5f;
			if (num10 > 9f)
			{
				stepSpeed = 2.5f;
			}
			gfxOffY += position.Y + (float)height - num5;
			position.Y = num5 - (float)height;
		}
	}

	public static void StepUp(ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir = 1, bool holdsMatching = false, int specialChecksMode = 0)
	{
		int num = 0;
		if (velocity.X < 0f)
		{
			num = -1;
		}
		if (velocity.X > 0f)
		{
			num = 1;
		}
		Vector2 vector = position;
		vector.X += velocity.X;
		int num2 = (int)((vector.X + (float)(width / 2) + (float)((width / 2 + 1) * num)) / 16f);
		int num3 = (int)(((double)vector.Y + 0.1) / 16.0);
		if (gravDir == 1)
		{
			num3 = (int)((vector.Y + (float)height - 1f) / 16f);
		}
		int num4 = height / 16 + ((height % 16 != 0) ? 1 : 0);
		bool flag = true;
		bool flag2 = true;
		if (Main.tile[num2, num3] == null)
		{
			return;
		}
		for (int i = 1; i < num4 + 2; i++)
		{
			if (!WorldGen.InWorld(num2, num3 - i * gravDir) || Main.tile[num2, num3 - i * gravDir] == null)
			{
				return;
			}
		}
		if (!WorldGen.InWorld(num2 - num, num3 - num4 * gravDir) || Main.tile[num2 - num, num3 - num4 * gravDir] == null)
		{
			return;
		}
		Tile tile;
		for (int j = 2; j < num4 + 1; j++)
		{
			if (!WorldGen.InWorld(num2, num3 - j * gravDir) || Main.tile[num2, num3 - j * gravDir] == null)
			{
				return;
			}
			tile = Main.tile[num2, num3 - j * gravDir];
			flag = flag && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]);
		}
		tile = Main.tile[num2 - num, num3 - num4 * gravDir];
		flag2 = flag2 && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]);
		bool flag3 = true;
		bool flag4 = true;
		bool flag5 = true;
		Tile tile2;
		if (gravDir == 1)
		{
			if (Main.tile[num2, num3 - gravDir] == null || Main.tile[num2, num3 - (num4 + 1) * gravDir] == null)
			{
				return;
			}
			tile = Main.tile[num2, num3 - gravDir];
			tile2 = Main.tile[num2, num3 - (num4 + 1) * gravDir];
			flag3 = flag3 && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type] || (tile.slope() == 1 && position.X + (float)(width / 2) > (float)(num2 * 16)) || (tile.slope() == 2 && position.X + (float)(width / 2) < (float)(num2 * 16 + 16)) || (tile.halfBrick() && (!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type])));
			tile = Main.tile[num2, num3];
			tile2 = Main.tile[num2, num3 - 1];
			if (specialChecksMode == 1)
			{
				flag5 = tile.type != 16 && tile.type != 18 && tile.type != 14 && tile.type != 469 && tile.type != 134;
			}
			flag4 = flag4 && ((tile.nactive() && (!tile.topSlope() || (tile.slope() == 1 && position.X + (float)(width / 2) < (float)(num2 * 16)) || (tile.slope() == 2 && position.X + (float)(width / 2) > (float)(num2 * 16 + 16))) && (!tile.topSlope() || position.Y + (float)height > (float)(num3 * 16)) && ((Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type]) || (holdsMatching && ((Main.tileSolidTop[tile.type] && tile.frameY == 0) || TileID.Sets.Platforms[tile.type]) && (!Main.tileSolid[tile2.type] || !tile2.nactive()) && flag5))) || (tile2.halfBrick() && tile2.nactive()));
			flag4 &= !Main.tileSolidTop[tile.type] || !Main.tileSolidTop[tile2.type];
		}
		else
		{
			tile = Main.tile[num2, num3 - gravDir];
			tile2 = Main.tile[num2, num3 - (num4 + 1) * gravDir];
			flag3 = flag3 && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type] || tile.slope() != 0 || (tile.halfBrick() && (!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type])));
			tile = Main.tile[num2, num3];
			tile2 = Main.tile[num2, num3 + 1];
			flag4 = flag4 && ((tile.nactive() && ((Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type]) || (holdsMatching && Main.tileSolidTop[tile.type] && tile.frameY == 0 && (!Main.tileSolid[tile2.type] || !tile2.nactive())))) || (tile2.halfBrick() && tile2.nactive()));
		}
		if (!((float)(num2 * 16) < vector.X + (float)width) || !((float)(num2 * 16 + 16) > vector.X))
		{
			return;
		}
		if (gravDir == 1)
		{
			if (!(flag4 && flag3 && flag && flag2))
			{
				return;
			}
			float num5 = num3 * 16;
			if (Main.tile[num2, num3 - 1].halfBrick())
			{
				num5 -= 8f;
			}
			else if (Main.tile[num2, num3].halfBrick())
			{
				num5 += 8f;
			}
			if (!(num5 < vector.Y + (float)height))
			{
				return;
			}
			float num6 = vector.Y + (float)height - num5;
			if ((double)num6 <= 16.1)
			{
				gfxOffY += position.Y + (float)height - num5;
				position.Y = num5 - (float)height;
				if (num6 < 9f)
				{
					stepSpeed = 1f;
				}
				else
				{
					stepSpeed = 2f;
				}
			}
		}
		else
		{
			if (!(flag4 && flag3 && flag && flag2) || Main.tile[num2, num3].bottomSlope() || TileID.Sets.Platforms[tile2.type])
			{
				return;
			}
			float num7 = num3 * 16 + 16;
			if (!(num7 > vector.Y))
			{
				return;
			}
			float num8 = num7 - vector.Y;
			if ((double)num8 <= 16.1)
			{
				gfxOffY -= num7 - position.Y;
				position.Y = num7;
				velocity.Y = 0f;
				if (num8 < 9f)
				{
					stepSpeed = 1f;
				}
				else
				{
					stepSpeed = 2f;
				}
			}
		}
	}

	public static bool InTileBounds(int x, int y, int lx, int ly, int hx, int hy)
	{
		if (x < lx || x > hx || y < ly || y > hy)
		{
			return false;
		}
		return true;
	}

	public static float GetTileRotation(Vector2 position)
	{
		float num = position.Y % 16f;
		int num2 = (int)(position.X / 16f);
		int num3 = (int)(position.Y / 16f);
		Tile tile = Main.tile[num2, num3];
		bool flag = false;
		for (int num4 = 2; num4 >= 0; num4--)
		{
			if (tile.active())
			{
				if (Main.tileSolid[tile.type])
				{
					int num5 = tile.blockType();
					if (tile.type != 19)
					{
						return num5 switch
						{
							1 => 0f, 
							2 => (float)Math.PI / 4f, 
							3 => -(float)Math.PI / 4f, 
							_ => 0f, 
						};
					}
					int num6 = tile.frameX / 18;
					if (((num6 >= 0 && num6 <= 7) || (num6 >= 12 && num6 <= 16)) && (num == 0f || flag))
					{
						return 0f;
					}
					switch (num6)
					{
					case 8:
					case 19:
					case 21:
					case 23:
						return -(float)Math.PI / 4f;
					case 10:
					case 20:
					case 22:
					case 24:
						return (float)Math.PI / 4f;
					case 25:
					case 26:
						if (!flag)
						{
							switch (num5)
							{
							case 2:
								return (float)Math.PI / 4f;
							case 3:
								return -(float)Math.PI / 4f;
							}
							break;
						}
						return 0f;
					}
				}
				else if (Main.tileSolidTop[tile.type] && tile.frameY == 0 && flag)
				{
					return 0f;
				}
			}
			num3++;
			tile = Main.tile[num2, num3];
			flag = true;
		}
		return 0f;
	}

	public static void GetEntityEdgeTiles(List<Point> p, Entity entity, bool left = true, bool right = true, bool up = true, bool down = true)
	{
		int num = (int)entity.position.X;
		int i = (int)entity.position.Y;
		_ = num % 16;
		_ = i % 16;
		int num2 = (int)entity.Right.X;
		int num3 = (int)entity.Bottom.Y;
		if (num % 16 == 0)
		{
			num--;
		}
		if (i % 16 == 0)
		{
			i--;
		}
		if (num2 % 16 == 0)
		{
			num2++;
		}
		if (num3 % 16 == 0)
		{
			num3++;
		}
		int num4 = num2 / 16 - num / 16;
		int num5 = num3 / 16 - i / 16;
		num /= 16;
		i /= 16;
		for (int j = num; j <= num + num4; j++)
		{
			if (up)
			{
				p.Add(new Point(j, i));
			}
			if (down)
			{
				p.Add(new Point(j, i + num5));
			}
		}
		for (int k = i; k < i + num5; k++)
		{
			if (left)
			{
				p.Add(new Point(num, k));
			}
			if (right)
			{
				p.Add(new Point(num + num4, k));
			}
		}
	}

	public static void StepConveyorBelt(Entity entity, float gravDir)
	{
		Player i = null;
		if (entity is Player)
		{
			i = (Player)entity;
			if (Math.Abs(i.gfxOffY) > 2f || i.grapCount > 0 || i.pulley)
			{
				return;
			}
			entity.height -= 5;
			entity.position.Y += 5f;
		}
		int num = 0;
		int num2 = 0;
		bool flag = false;
		int num3 = (int)entity.position.Y + entity.height;
		entity.Hitbox.Inflate(2, 2);
		_ = entity.TopLeft;
		_ = entity.TopRight;
		_ = entity.BottomLeft;
		_ = entity.BottomRight;
		List<Point> cacheForConveyorBelts = _cacheForConveyorBelts;
		cacheForConveyorBelts.Clear();
		GetEntityEdgeTiles(cacheForConveyorBelts, entity, left: false, right: false);
		Vector2 vector = new Vector2(0.0001f);
		Vector2 lineStart = default(Vector2);
		Vector2 lineStart2 = default(Vector2);
		Vector2 lineEnd = default(Vector2);
		Vector2 lineEnd2 = default(Vector2);
		for (int j = 0; j < cacheForConveyorBelts.Count; j++)
		{
			Point point = cacheForConveyorBelts[j];
			if (!WorldGen.InWorld(point.X, point.Y) || (i != null && i.onTrack && point.Y < num3))
			{
				continue;
			}
			Tile tile = Main.tile[point.X, point.Y];
			if (tile == null || !tile.active() || !tile.nactive())
			{
				continue;
			}
			int num4 = TileID.Sets.ConveyorDirection[tile.type];
			if (num4 == 0)
			{
				continue;
			}
			lineStart.X = (lineStart2.X = point.X * 16);
			lineEnd.X = (lineEnd2.X = point.X * 16 + 16);
			switch (tile.slope())
			{
			case 1:
				lineStart2.Y = point.Y * 16;
				lineEnd2.Y = (lineEnd.Y = (lineStart.Y = point.Y * 16 + 16));
				break;
			case 2:
				lineEnd2.Y = point.Y * 16;
				lineStart2.Y = (lineEnd.Y = (lineStart.Y = point.Y * 16 + 16));
				break;
			case 3:
				lineEnd.Y = (lineStart2.Y = (lineEnd2.Y = point.Y * 16));
				lineStart.Y = point.Y * 16 + 16;
				break;
			case 4:
				lineStart.Y = (lineStart2.Y = (lineEnd2.Y = point.Y * 16));
				lineEnd.Y = point.Y * 16 + 16;
				break;
			default:
				if (tile.halfBrick())
				{
					lineStart2.Y = (lineEnd2.Y = point.Y * 16 + 8);
				}
				else
				{
					lineStart2.Y = (lineEnd2.Y = point.Y * 16);
				}
				lineStart.Y = (lineEnd.Y = point.Y * 16 + 16);
				break;
			}
			int num5 = 0;
			if (!TileID.Sets.Platforms[tile.type] && CheckAABBvLineCollision2(entity.position - vector, entity.Size + vector * 2f, lineStart, lineEnd))
			{
				num5--;
			}
			if (CheckAABBvLineCollision2(entity.position - vector, entity.Size + vector * 2f, lineStart2, lineEnd2))
			{
				num5++;
			}
			if (num5 != 0)
			{
				flag = true;
				num += num4 * num5 * (int)gravDir;
				if (tile.leftSlope())
				{
					num2 += (int)gravDir * -num4;
				}
				if (tile.rightSlope())
				{
					num2 -= (int)gravDir * -num4;
				}
			}
		}
		if (entity is Player)
		{
			entity.height += 5;
			entity.position.Y -= 5f;
		}
		if (flag && num != 0)
		{
			num = Math.Sign(num);
			num2 = Math.Sign(num2);
			Vector2 velocity = Vector2.Normalize(new Vector2((float)num * gravDir, num2)) * 2.5f;
			Vector2 vector2 = TileCollision(entity.position, velocity, entity.width, entity.height, fallThrough: false, fall2: false, (int)gravDir);
			entity.position += vector2;
			vector2 = TileCollision(Velocity: new Vector2(0f, 2.5f * gravDir), Position: entity.position, Width: entity.width, Height: entity.height, fallThrough: false, fall2: false, gravDir: (int)gravDir);
			entity.position += vector2;
		}
	}

	public static List<Point> GetTilesIn(Vector2 TopLeft, Vector2 BottomRight)
	{
		List<Point> num = new List<Point>();
		Point point = TopLeft.ToTileCoordinates();
		Point point2 = BottomRight.ToTileCoordinates();
		int num2 = Utils.Clamp(point.X, 0, Main.maxTilesX - 1);
		int num3 = Utils.Clamp(point.Y, 0, Main.maxTilesY - 1);
		int num4 = Utils.Clamp(point2.X, 0, Main.maxTilesX - 1);
		int num5 = Utils.Clamp(point2.Y, 0, Main.maxTilesY - 1);
		for (int i = num2; i <= num4; i++)
		{
			for (int j = num3; j <= num5; j++)
			{
				if (Main.tile[i, j] != null)
				{
					num.Add(new Point(i, j));
				}
			}
		}
		return num;
	}

	public static void ExpandVertically(int startX, int startY, out int topY, out int bottomY, int maxExpandUp = 100, int maxExpandDown = 100)
	{
		topY = startY;
		bottomY = startY;
		if (!WorldGen.InWorld(startX, startY, 10))
		{
			return;
		}
		for (int num = 0; num < maxExpandUp; num++)
		{
			if (topY <= 0)
			{
				break;
			}
			if (topY < 10)
			{
				break;
			}
			if (Main.tile[startX, topY] == null)
			{
				break;
			}
			if (WorldGen.SolidTile3(startX, topY))
			{
				break;
			}
			topY--;
		}
		for (int i = 0; i < maxExpandDown; i++)
		{
			if (bottomY >= Main.maxTilesY - 10)
			{
				break;
			}
			if (bottomY > Main.maxTilesY - 10)
			{
				break;
			}
			if (Main.tile[startX, bottomY] == null)
			{
				break;
			}
			if (WorldGen.SolidTile3(startX, bottomY))
			{
				break;
			}
			bottomY++;
		}
	}

	public static Vector2 AdvancedTileCollision(bool[] forcedIgnoredTiles, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false, int gravDir = 1)
	{
		up = false;
		down = false;
		Vector2 result = Velocity;
		Vector2 vector = Velocity;
		Vector2 vector2 = Position + Velocity;
		Vector2 vector3 = Position;
		int value = (int)(Position.X / 16f) - 1;
		int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
		int value3 = (int)(Position.Y / 16f) - 1;
		int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		int num5 = Utils.Clamp(value, 0, Main.maxTilesX - 1);
		value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
		value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
		value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
		float num6 = (value4 + 3) * 16;
		Vector2 vector4 = default(Vector2);
		for (int i = num5; i < value2; i++)
		{
			for (int j = value3; j < value4; j++)
			{
				Tile tile = Main.tile[i, j];
				if (tile == null || !tile.active() || tile.inActive() || forcedIgnoredTiles[tile.type] || (!Main.tileSolid[tile.type] && (!Main.tileSolidTop[tile.type] || tile.frameY != 0)))
				{
					continue;
				}
				vector4.X = i * 16;
				vector4.Y = j * 16;
				int num7 = 16;
				if (tile.halfBrick())
				{
					vector4.Y += 8f;
					num7 -= 8;
				}
				if (!(vector2.X + (float)Width > vector4.X) || !(vector2.X < vector4.X + 16f) || !(vector2.Y + (float)Height > vector4.Y) || !(vector2.Y < vector4.Y + (float)num7))
				{
					continue;
				}
				bool flag = false;
				bool flag2 = false;
				if (tile.slope() > 2)
				{
					if (tile.slope() == 3 && vector3.Y + Math.Abs(Velocity.X) >= vector4.Y && vector3.X >= vector4.X)
					{
						flag2 = true;
					}
					if (tile.slope() == 4 && vector3.Y + Math.Abs(Velocity.X) >= vector4.Y && vector3.X + (float)Width <= vector4.X + 16f)
					{
						flag2 = true;
					}
				}
				else if (tile.slope() > 0)
				{
					flag = true;
					if (tile.slope() == 1 && vector3.Y + (float)Height - Math.Abs(Velocity.X) <= vector4.Y + (float)num7 && vector3.X >= vector4.X)
					{
						flag2 = true;
					}
					if (tile.slope() == 2 && vector3.Y + (float)Height - Math.Abs(Velocity.X) <= vector4.Y + (float)num7 && vector3.X + (float)Width <= vector4.X + 16f)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					continue;
				}
				if (vector3.Y + (float)Height <= vector4.Y)
				{
					down = true;
					if ((!(Main.tileSolidTop[tile.type] && fallThrough) || !(Velocity.Y <= 1f || fall2)) && num6 > vector4.Y)
					{
						num3 = i;
						num4 = j;
						if (num7 < 16)
						{
							num4++;
						}
						if (num3 != num && !flag)
						{
							result.Y = vector4.Y - (vector3.Y + (float)Height) + ((gravDir == -1) ? (-0.01f) : 0f);
							num6 = vector4.Y;
						}
					}
				}
				else if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[tile.type])
				{
					if (Main.tile[i - 1, j] == null)
					{
						Main.tile[i - 1, j] = new Tile();
					}
					if (Main.tile[i - 1, j].slope() != 2 && Main.tile[i - 1, j].slope() != 4)
					{
						num = i;
						num2 = j;
						if (num2 != num4)
						{
							result.X = vector4.X - (vector3.X + (float)Width);
						}
						if (num3 == num)
						{
							result.Y = vector.Y;
						}
					}
				}
				else if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[tile.type])
				{
					if (Main.tile[i + 1, j] == null)
					{
						Main.tile[i + 1, j] = new Tile();
					}
					if (Main.tile[i + 1, j].slope() != 1 && Main.tile[i + 1, j].slope() != 3)
					{
						num = i;
						num2 = j;
						if (num2 != num4)
						{
							result.X = vector4.X + 16f - vector3.X;
						}
						if (num3 == num)
						{
							result.Y = vector.Y;
						}
					}
				}
				else if (vector3.Y >= vector4.Y + (float)num7 && !Main.tileSolidTop[tile.type])
				{
					up = true;
					num3 = i;
					num4 = j;
					result.Y = vector4.Y + (float)num7 - vector3.Y + ((gravDir == 1) ? 0.01f : 0f);
					if (num4 == num2)
					{
						result.X = vector.X;
					}
				}
			}
		}
		return result;
	}

	public static void LaserScan(Vector2 samplingPoint, Vector2 directionUnit, float samplingWidth, float maxDistance, float[] samples)
	{
		for (int i = 0; i < samples.Length; i++)
		{
			float num = (float)i / (float)(samples.Length - 1);
			Vector2 vector = samplingPoint + directionUnit.RotatedBy(1.5707963705062866) * (num - 0.5f) * samplingWidth;
			int num2 = (int)vector.X / 16;
			int num3 = (int)vector.Y / 16;
			Vector2 vector2 = vector + directionUnit * maxDistance;
			int num4 = (int)vector2.X / 16;
			int num5 = (int)vector2.Y / 16;
			float num6 = 0f;
			num6 = (samples[i] = (TupleHitLine(num2, num3, num4, num5, 0, 0, new List<Tuple<int, int>>(), out var col) ? ((col.Item1 != num4 || col.Item2 != num5) ? (new Vector2(Math.Abs(num2 - col.Item1), Math.Abs(num3 - col.Item2)).Length() * 16f) : maxDistance) : (new Vector2(Math.Abs(num2 - col.Item1), Math.Abs(num3 - col.Item2)).Length() * 16f)));
		}
	}

	public static void AimingLaserScan(Vector2 startPoint, Vector2 endPoint, float samplingWidth, int samplesToTake, out Vector2 vectorTowardsTarget, out float[] samples)
	{
		samples = new float[samplesToTake];
		vectorTowardsTarget = endPoint - startPoint;
		LaserScan(startPoint, vectorTowardsTarget.SafeNormalize(Vector2.Zero), samplingWidth, vectorTowardsTarget.Length(), samples);
	}
}
