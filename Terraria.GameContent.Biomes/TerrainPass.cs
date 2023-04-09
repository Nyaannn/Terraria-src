using System;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Terraria.GameContent.Biomes;

public class TerrainPass : GenPass
{
	private enum TerrainFeatureType
	{
		Plateau,
		Hill,
		Dale,
		Mountain,
		Valley
	}

	private class SurfaceHistory
	{
		private readonly double[] _heights;

		private int _index;

		public double this[int index]
		{
			get
			{
				return _heights[(index + _index) % _heights.Length];
			}
			set
			{
				_heights[(index + _index) % _heights.Length] = value;
			}
		}

		public int Length => _heights.Length;

		public SurfaceHistory(int size)
		{
			_heights = new double[size];
		}

		public void Record(double height)
		{
			_heights[_index] = height;
			_index = (_index + 1) % _heights.Length;
		}
	}

	public TerrainPass()
		: base("Terrain", 449.3721923828125)
	{
	}

	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		int result = configuration.Get<int>("FlatBeachPadding");
		progress.Message = Lang.gen[0].Value;
		TerrainFeatureType terrainFeatureType = TerrainFeatureType.Plateau;
		int num = 0;
		double num2 = (double)Main.maxTilesY * 0.3;
		num2 *= (double)GenBase._random.Next(90, 110) * 0.005;
		double num3 = num2 + (double)Main.maxTilesY * 0.2;
		num3 *= (double)GenBase._random.Next(90, 110) * 0.01;
		if (WorldGen.remixWorldGen)
		{
			num3 = (double)Main.maxTilesY * 0.5;
			if (Main.maxTilesX > 2500)
			{
				num3 = (double)Main.maxTilesY * 0.6;
			}
			num3 *= (double)GenBase._random.Next(95, 106) * 0.01;
		}
		double num4 = num2;
		double num5 = num2;
		double num6 = num3;
		double num7 = num3;
		double num8 = (double)Main.maxTilesY * 0.23;
		SurfaceHistory surfaceHistory = new SurfaceHistory(500);
		num = GenVars.leftBeachEnd + result;
		for (int i = 0; i < Main.maxTilesX; i++)
		{
			progress.Set((double)i / (double)Main.maxTilesX);
			num4 = Math.Min(num2, num4);
			num5 = Math.Max(num2, num5);
			num6 = Math.Min(num3, num6);
			num7 = Math.Max(num3, num7);
			if (num <= 0)
			{
				terrainFeatureType = (TerrainFeatureType)GenBase._random.Next(0, 5);
				num = GenBase._random.Next(5, 40);
				if (terrainFeatureType == TerrainFeatureType.Plateau)
				{
					num *= (int)((double)GenBase._random.Next(5, 30) * 0.2);
				}
			}
			num--;
			if ((double)i > (double)Main.maxTilesX * 0.45 && (double)i < (double)Main.maxTilesX * 0.55 && (terrainFeatureType == TerrainFeatureType.Mountain || terrainFeatureType == TerrainFeatureType.Valley))
			{
				terrainFeatureType = (TerrainFeatureType)GenBase._random.Next(3);
			}
			if ((double)i > (double)Main.maxTilesX * 0.48 && (double)i < (double)Main.maxTilesX * 0.52)
			{
				terrainFeatureType = TerrainFeatureType.Plateau;
			}
			num2 += GenerateWorldSurfaceOffset(terrainFeatureType);
			double num9 = 0.17;
			double num10 = 0.26;
			if (WorldGen.drunkWorldGen)
			{
				num9 = 0.15;
				num10 = 0.28;
			}
			if (i < GenVars.leftBeachEnd + result || i > GenVars.rightBeachStart - result)
			{
				num2 = Utils.Clamp(num2, (double)Main.maxTilesY * 0.17, num8);
			}
			else if (num2 < (double)Main.maxTilesY * num9)
			{
				num2 = (double)Main.maxTilesY * num9;
				num = 0;
			}
			else if (num2 > (double)Main.maxTilesY * num10)
			{
				num2 = (double)Main.maxTilesY * num10;
				num = 0;
			}
			while (GenBase._random.Next(0, 3) == 0)
			{
				num3 += (double)GenBase._random.Next(-2, 3);
			}
			if (WorldGen.remixWorldGen)
			{
				if (Main.maxTilesX > 2500)
				{
					if (num3 > (double)Main.maxTilesY * 0.7)
					{
						num3 -= 1.0;
					}
				}
				else if (num3 > (double)Main.maxTilesY * 0.6)
				{
					num3 -= 1.0;
				}
			}
			else
			{
				if (num3 < num2 + (double)Main.maxTilesY * 0.06)
				{
					num3 += 1.0;
				}
				if (num3 > num2 + (double)Main.maxTilesY * 0.35)
				{
					num3 -= 1.0;
				}
			}
			surfaceHistory.Record(num2);
			FillColumn(i, num2, num3);
			if (i == GenVars.rightBeachStart - result)
			{
				if (num2 > num8)
				{
					RetargetSurfaceHistory(surfaceHistory, i, num8);
				}
				terrainFeatureType = TerrainFeatureType.Plateau;
				num = Main.maxTilesX - i;
			}
		}
		Main.worldSurface = (int)(num5 + 25.0);
		Main.rockLayer = num7;
		double num11 = (int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6;
		Main.rockLayer = (int)(Main.worldSurface + num11);
		int num12 = (int)(Main.rockLayer + (double)Main.maxTilesY) / 2 + GenBase._random.Next(-100, 20);
		int lavaLine = num12 + GenBase._random.Next(50, 80);
		if (WorldGen.remixWorldGen)
		{
			lavaLine = (int)(Main.worldSurface * 4.0 + num3) / 5;
		}
		int num13 = 20;
		if (num6 < num5 + (double)num13)
		{
			double num14 = (num6 + num5) / 2.0;
			double num15 = Math.Abs(num6 - num5);
			if (num15 < (double)num13)
			{
				num15 = num13;
			}
			num6 = num14 + num15 / 2.0;
			num5 = num14 - num15 / 2.0;
		}
		GenVars.rockLayer = num3;
		GenVars.rockLayerHigh = num7;
		GenVars.rockLayerLow = num6;
		GenVars.worldSurface = num2;
		GenVars.worldSurfaceHigh = num5;
		GenVars.worldSurfaceLow = num4;
		GenVars.waterLine = num12;
		GenVars.lavaLine = lavaLine;
	}

	private static void FillColumn(int x, double worldSurface, double rockLayer)
	{
		for (int i = 0; (double)i < worldSurface; i++)
		{
			Main.tile[x, i].active(active: false);
			Main.tile[x, i].frameX = -1;
			Main.tile[x, i].frameY = -1;
		}
		for (int j = (int)worldSurface; j < Main.maxTilesY; j++)
		{
			if ((double)j < rockLayer)
			{
				Main.tile[x, j].active(active: true);
				Main.tile[x, j].type = 0;
				Main.tile[x, j].frameX = -1;
				Main.tile[x, j].frameY = -1;
			}
			else
			{
				Main.tile[x, j].active(active: true);
				Main.tile[x, j].type = 1;
				Main.tile[x, j].frameX = -1;
				Main.tile[x, j].frameY = -1;
			}
		}
	}

	private static void RetargetColumn(int x, double worldSurface)
	{
		for (int i = 0; (double)i < worldSurface; i++)
		{
			Main.tile[x, i].active(active: false);
			Main.tile[x, i].frameX = -1;
			Main.tile[x, i].frameY = -1;
		}
		for (int j = (int)worldSurface; j < Main.maxTilesY; j++)
		{
			if (Main.tile[x, j].type != 1 || !Main.tile[x, j].active())
			{
				Main.tile[x, j].active(active: true);
				Main.tile[x, j].type = 0;
				Main.tile[x, j].frameX = -1;
				Main.tile[x, j].frameY = -1;
			}
		}
	}

	private static double GenerateWorldSurfaceOffset(TerrainFeatureType featureType)
	{
		double result = 0.0;
		if ((WorldGen.drunkWorldGen || WorldGen.getGoodWorldGen || WorldGen.remixWorldGen) && WorldGen.genRand.Next(2) == 0)
		{
			switch (featureType)
			{
			case TerrainFeatureType.Plateau:
				while (GenBase._random.Next(0, 6) == 0)
				{
					result += (double)GenBase._random.Next(-1, 2);
				}
				break;
			case TerrainFeatureType.Hill:
				while (GenBase._random.Next(0, 3) == 0)
				{
					result -= 1.0;
				}
				while (GenBase._random.Next(0, 10) == 0)
				{
					result += 1.0;
				}
				break;
			case TerrainFeatureType.Dale:
				while (GenBase._random.Next(0, 3) == 0)
				{
					result += 1.0;
				}
				while (GenBase._random.Next(0, 10) == 0)
				{
					result -= 1.0;
				}
				break;
			case TerrainFeatureType.Mountain:
				while (GenBase._random.Next(0, 3) != 0)
				{
					result -= 1.0;
				}
				while (GenBase._random.Next(0, 6) == 0)
				{
					result += 1.0;
				}
				break;
			case TerrainFeatureType.Valley:
				while (GenBase._random.Next(0, 3) != 0)
				{
					result += 1.0;
				}
				while (GenBase._random.Next(0, 5) == 0)
				{
					result -= 1.0;
				}
				break;
			}
		}
		else
		{
			switch (featureType)
			{
			case TerrainFeatureType.Plateau:
				while (GenBase._random.Next(0, 7) == 0)
				{
					result += (double)GenBase._random.Next(-1, 2);
				}
				break;
			case TerrainFeatureType.Hill:
				while (GenBase._random.Next(0, 4) == 0)
				{
					result -= 1.0;
				}
				while (GenBase._random.Next(0, 10) == 0)
				{
					result += 1.0;
				}
				break;
			case TerrainFeatureType.Dale:
				while (GenBase._random.Next(0, 4) == 0)
				{
					result += 1.0;
				}
				while (GenBase._random.Next(0, 10) == 0)
				{
					result -= 1.0;
				}
				break;
			case TerrainFeatureType.Mountain:
				while (GenBase._random.Next(0, 2) == 0)
				{
					result -= 1.0;
				}
				while (GenBase._random.Next(0, 6) == 0)
				{
					result += 1.0;
				}
				break;
			case TerrainFeatureType.Valley:
				while (GenBase._random.Next(0, 2) == 0)
				{
					result += 1.0;
				}
				while (GenBase._random.Next(0, 5) == 0)
				{
					result -= 1.0;
				}
				break;
			}
		}
		return result;
	}

	private static void RetargetSurfaceHistory(SurfaceHistory history, int targetX, double targetHeight)
	{
		for (int result = 0; result < history.Length / 2; result++)
		{
			if (history[history.Length - 1] <= targetHeight)
			{
				break;
			}
			for (int i = 0; i < history.Length - result * 2; i++)
			{
				double changesByChunkCoord = history[history.Length - i - 1];
				changesByChunkCoord -= 1.0;
				history[history.Length - i - 1] = changesByChunkCoord;
				if (changesByChunkCoord <= targetHeight)
				{
					break;
				}
			}
		}
		for (int j = 0; j < history.Length; j++)
		{
			double item = history[history.Length - j - 1];
			RetargetColumn(targetX - j, item);
		}
	}
}
