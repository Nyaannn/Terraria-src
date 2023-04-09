using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using ReLogic.Utilities;
using Terraria.GameContent.Biomes.Desert;
using Terraria.WorldBuilding;

namespace Terraria.GameContent.Biomes;

public class DunesBiome : MicroBiome
{
	private class DunesDescription
	{
		public bool IsValid { get; private set; }

		public SurfaceMap Surface { get; private set; }

		public Rectangle Area { get; private set; }

		public WindDirection WindDirection { get; private set; }

		private DunesDescription()
		{
		}

		public static DunesDescription CreateFromPlacement(Point origin, int width, int height)
		{
			Rectangle area = new Rectangle(origin.X - width / 2, origin.Y - height / 2, width, height);
			return new DunesDescription
			{
				Area = area,
				IsValid = true,
				Surface = SurfaceMap.FromArea(area.Left - 20, area.Width + 40),
				WindDirection = ((WorldGen.genRand.Next(2) != 0) ? WindDirection.Right : WindDirection.Left)
			};
		}
	}

	private enum WindDirection
	{
		Left,
		Right
	}

	[JsonProperty("SingleDunesWidth")]
	private WorldGenRange _singleDunesWidth = WorldGenRange.Empty;

	[JsonProperty("HeightScale")]
	private double _heightScale = 1.0;

	public int MaximumWidth => _singleDunesWidth.ScaledMaximum * 2;

	public override bool Place(Point origin, StructureMap structures)
	{
		int height = (int)((double)GenBase._random.Next(60, 100) * _heightScale);
		int height2 = (int)((double)GenBase._random.Next(60, 100) * _heightScale);
		int random = _singleDunesWidth.GetRandom(GenBase._random);
		int random2 = _singleDunesWidth.GetRandom(GenBase._random);
		DunesDescription description = DunesDescription.CreateFromPlacement(new Point(origin.X - random / 2 + 30, origin.Y), random, height);
		DunesDescription description2 = DunesDescription.CreateFromPlacement(new Point(origin.X + random2 / 2 - 30, origin.Y), random2, height2);
		PlaceSingle(description, structures);
		PlaceSingle(description2, structures);
		return true;
	}

	private void PlaceSingle(DunesDescription description, StructureMap structures)
	{
		int i = GenBase._random.Next(3) + 8;
		for (int j = 0; j < i - 1; j++)
		{
			int num = (int)(2.0 / (double)i * (double)description.Area.Width);
			int num2 = (int)((double)j / (double)i * (double)description.Area.Width + (double)description.Area.Left) + num * 2 / 5;
			num2 += GenBase._random.Next(-5, 6);
			double num3 = (double)j / (double)(i - 2);
			double num4 = 1.0 - Math.Abs(num3 - 0.5) * 2.0;
			PlaceHill(num2 - num / 2, num2 + num / 2, (num4 * 0.3 + 0.2) * _heightScale, description);
		}
		int num5 = GenBase._random.Next(2) + 1;
		for (int k = 0; k < num5; k++)
		{
			int num6 = description.Area.Width / 2;
			int x = description.Area.Center.X;
			x += GenBase._random.Next(-10, 11);
			PlaceHill(x - num6 / 2, x + num6 / 2, 0.8 * _heightScale, description);
		}
		structures.AddStructure(description.Area, 20);
	}

	private static void PlaceHill(int startX, int endX, double scale, DunesDescription description)
	{
		Point startPoint = new Point(startX, description.Surface[startX]);
		Point changesByChunkCoord = new Point(endX, description.Surface[endX]);
		Point point = new Point((startPoint.X + changesByChunkCoord.X) / 2, (startPoint.Y + changesByChunkCoord.Y) / 2 - (int)(35.0 * scale));
		int num = (changesByChunkCoord.X - point.X) / 4;
		int minValue = (changesByChunkCoord.X - point.X) / 16;
		if (description.WindDirection == WindDirection.Left)
		{
			point.X -= WorldGen.genRand.Next(minValue, num + 1);
		}
		else
		{
			point.X += WorldGen.genRand.Next(minValue, num + 1);
		}
		Point point2 = new Point(0, (int)(scale * 12.0));
		Point point3 = new Point(point2.X / -2, point2.Y / -2);
		PlaceCurvedLine(startPoint, point, (description.WindDirection != 0) ? point3 : point2, description);
		PlaceCurvedLine(point, changesByChunkCoord, (description.WindDirection == WindDirection.Left) ? point3 : point2, description);
	}

	private static void PlaceCurvedLine(Point startPoint, Point endPoint, Point anchorOffset, DunesDescription description)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		Point p = new Point((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);
		p.X += anchorOffset.X;
		p.Y += anchorOffset.Y;
		Vector2D changesByChunkCoord = startPoint.ToVector2D();
		Vector2D val = endPoint.ToVector2D();
		Vector2D val2 = p.ToVector2D();
		double num = 0.5 / (val.X - changesByChunkCoord.X);
		Point point = new Point(-1, -1);
		for (double num2 = 0.0; num2 <= 1.0; num2 += num)
		{
			Vector2D val3 = Vector2D.Lerp(changesByChunkCoord, val2, num2);
			Vector2D val4 = Vector2D.Lerp(val2, val, num2);
			Point point2 = Vector2D.Lerp(val3, val4, num2).ToPoint();
			if (point2 == point)
			{
				continue;
			}
			point = point2;
			int num3 = description.Area.Width / 2 - Math.Abs(point2.X - description.Area.Center.X);
			int num4 = description.Surface[point2.X] + (int)(Math.Sqrt(num3) * 3.0);
			for (int i = point2.Y - 10; i < point2.Y; i++)
			{
				if (GenBase._tiles[point2.X, i].active() && GenBase._tiles[point2.X, i].type != 53)
				{
					GenBase._tiles[point2.X, i].ClearEverything();
				}
			}
			for (int j = point2.Y; j < num4; j++)
			{
				GenBase._tiles[point2.X, j].ResetToType(53);
				Tile.SmoothSlope(point2.X, j);
			}
		}
	}
}
