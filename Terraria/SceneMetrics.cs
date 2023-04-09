using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Terraria;

public class SceneMetrics
{
	public static int ShimmerTileThreshold = 300;

	public static int CorruptionTileThreshold = 300;

	public static int CorruptionTileMax = 1000;

	public static int CrimsonTileThreshold = 300;

	public static int CrimsonTileMax = 1000;

	public static int HallowTileThreshold = 125;

	public static int HallowTileMax = 600;

	public static int JungleTileThreshold = 140;

	public static int JungleTileMax = 700;

	public static int SnowTileThreshold = 1500;

	public static int SnowTileMax = 6000;

	public static int DesertTileThreshold = 1500;

	public static int MushroomTileThreshold = 100;

	public static int MushroomTileMax = 160;

	public static int MeteorTileThreshold = 75;

	public static int GraveyardTileMax = 36;

	public static int GraveyardTileMin = 16;

	public static int GraveyardTileThreshold = 28;

	public bool CanPlayCreditsRoll;

	public bool[] NPCBannerBuff = new bool[290];

	public bool hasBanner;

	private readonly int[] _tileCounts = new int[TileID.Count];

	private readonly int[] _liquidCounts = new int[LiquidID.Count];

	private readonly List<Point> _oreFinderTileLocations = new List<Point>(512);

	public int bestOre;

	public Point? ClosestOrePosition { get; private set; }

	public int ShimmerTileCount { get; set; }

	public int EvilTileCount { get; set; }

	public int HolyTileCount { get; set; }

	public int HoneyBlockCount { get; set; }

	public int ActiveMusicBox { get; set; }

	public int SandTileCount { get; private set; }

	public int MushroomTileCount { get; private set; }

	public int SnowTileCount { get; private set; }

	public int WaterCandleCount { get; private set; }

	public int PeaceCandleCount { get; private set; }

	public int ShadowCandleCount { get; private set; }

	public int PartyMonolithCount { get; private set; }

	public int MeteorTileCount { get; private set; }

	public int BloodTileCount { get; private set; }

	public int JungleTileCount { get; private set; }

	public int DungeonTileCount { get; private set; }

	public bool HasSunflower { get; private set; }

	public bool HasGardenGnome { get; private set; }

	public bool HasClock { get; private set; }

	public bool HasCampfire { get; private set; }

	public bool HasStarInBottle { get; private set; }

	public bool HasHeartLantern { get; private set; }

	public int ActiveFountainColor { get; private set; }

	public int ActiveMonolithType { get; private set; }

	public bool BloodMoonMonolith { get; private set; }

	public bool MoonLordMonolith { get; private set; }

	public bool EchoMonolith { get; private set; }

	public int ShimmerMonolithState { get; private set; }

	public bool HasCatBast { get; private set; }

	public int GraveyardTileCount { get; private set; }

	public bool EnoughTilesForShimmer => ShimmerTileCount >= ShimmerTileThreshold;

	public bool EnoughTilesForJungle => JungleTileCount >= JungleTileThreshold;

	public bool EnoughTilesForHallow => HolyTileCount >= HallowTileThreshold;

	public bool EnoughTilesForSnow => SnowTileCount >= SnowTileThreshold;

	public bool EnoughTilesForGlowingMushroom => MushroomTileCount >= MushroomTileThreshold;

	public bool EnoughTilesForDesert => SandTileCount >= DesertTileThreshold;

	public bool EnoughTilesForCorruption => EvilTileCount >= CorruptionTileThreshold;

	public bool EnoughTilesForCrimson => BloodTileCount >= CrimsonTileThreshold;

	public bool EnoughTilesForMeteor => MeteorTileCount >= MeteorTileThreshold;

	public bool EnoughTilesForGraveyard => GraveyardTileCount >= GraveyardTileThreshold;

	public SceneMetrics()
	{
		Reset();
	}

	public void ScanAndExportToMain(SceneMetricsScanSettings settings)
	{
		Reset();
		int list = 0;
		int vector2 = 0;
		int vec2 = 0;
		if (settings.ScanOreFinderData)
		{
			_oreFinderTileLocations.Clear();
		}
		if (settings.BiomeScanCenterPositionInWorld.HasValue)
		{
			Point y2 = settings.BiomeScanCenterPositionInWorld.Value.ToTileCoordinates();
			Rectangle num = new Rectangle(y2.X - Main.buffScanAreaWidth / 2, y2.Y - Main.buffScanAreaHeight / 2, Main.buffScanAreaWidth, Main.buffScanAreaHeight);
			num = WorldUtils.ClampToWorld(num);
			for (int num2 = num.Left; num2 < num.Right; num2++)
			{
				for (int width = num.Top; width < num.Bottom; width++)
				{
					if (!num.Contains(num2, width))
					{
						continue;
					}
					Tile vector = Main.tile[num2, width];
					if (vector == null)
					{
						continue;
					}
					if (!vector.active())
					{
						if (vector.liquid > 0)
						{
							_liquidCounts[vector.liquidType()]++;
						}
						continue;
					}
					num.Contains(num2, width);
					if (!TileID.Sets.isDesertBiomeSand[vector.type] || !WorldGen.oceanDepths(num2, width))
					{
						_tileCounts[vector.type]++;
					}
					if (vector.type == 215 && vector.frameY < 36)
					{
						HasCampfire = true;
					}
					if (vector.type == 49 && vector.frameX < 18)
					{
						list++;
					}
					if (vector.type == 372 && vector.frameX < 18)
					{
						vector2++;
					}
					if (vector.type == 646 && vector.frameX < 18)
					{
						vec2++;
					}
					if (vector.type == 405 && vector.frameX < 54)
					{
						HasCampfire = true;
					}
					if (vector.type == 506 && vector.frameX < 72)
					{
						HasCatBast = true;
					}
					if (vector.type == 42 && vector.frameY >= 324 && vector.frameY <= 358)
					{
						HasHeartLantern = true;
					}
					if (vector.type == 42 && vector.frameY >= 252 && vector.frameY <= 286)
					{
						HasStarInBottle = true;
					}
					if (vector.type == 91 && (vector.frameX >= 396 || vector.frameY >= 54))
					{
						int vec = vector.frameX / 18 - 21;
						for (int y = vector.frameY; y >= 54; y -= 54)
						{
							vec += 90;
							vec += 21;
						}
						int num3 = Item.BannerToItem(vec);
						if (ItemID.Sets.BannerStrength.IndexInRange(num3) && ItemID.Sets.BannerStrength[num3].Enabled)
						{
							NPCBannerBuff[vec] = true;
							hasBanner = true;
						}
					}
					if (settings.ScanOreFinderData && Main.tileOreFinderPriority[vector.type] != 0)
					{
						_oreFinderTileLocations.Add(new Point(num2, width));
					}
				}
			}
		}
		if (settings.VisualScanArea.HasValue)
		{
			Rectangle num4 = WorldUtils.ClampToWorld(settings.VisualScanArea.Value);
			for (int height = num4.Left; height < num4.Right; height++)
			{
				for (int i = num4.Top; i < num4.Bottom; i++)
				{
					Tile tile = Main.tile[height, i];
					if (tile == null || !tile.active())
					{
						continue;
					}
					if (tile.type == 104)
					{
						HasClock = true;
					}
					switch (tile.type)
					{
					case 139:
						if (tile.frameX >= 36)
						{
							ActiveMusicBox = tile.frameY / 36;
						}
						break;
					case 207:
						if (tile.frameY >= 72)
						{
							switch (tile.frameX / 36)
							{
							case 0:
								ActiveFountainColor = 0;
								break;
							case 1:
								ActiveFountainColor = 12;
								break;
							case 2:
								ActiveFountainColor = 3;
								break;
							case 3:
								ActiveFountainColor = 5;
								break;
							case 4:
								ActiveFountainColor = 2;
								break;
							case 5:
								ActiveFountainColor = 10;
								break;
							case 6:
								ActiveFountainColor = 4;
								break;
							case 7:
								ActiveFountainColor = 9;
								break;
							case 8:
								ActiveFountainColor = 8;
								break;
							case 9:
								ActiveFountainColor = 6;
								break;
							default:
								ActiveFountainColor = -1;
								break;
							}
						}
						break;
					case 410:
						if (tile.frameY >= 56)
						{
							int num8 = (ActiveMonolithType = tile.frameX / 36);
						}
						break;
					case 509:
						if (tile.frameY >= 56)
						{
							ActiveMonolithType = 4;
						}
						break;
					case 480:
						if (tile.frameY >= 54)
						{
							BloodMoonMonolith = true;
						}
						break;
					case 657:
						if (tile.frameY >= 54)
						{
							EchoMonolith = true;
						}
						break;
					case 658:
					{
						int num6 = (ShimmerMonolithState = tile.frameY / 54);
						break;
					}
					}
				}
			}
		}
		WaterCandleCount = list;
		PeaceCandleCount = vector2;
		ShadowCandleCount = vec2;
		ExportTileCountsToMain();
		CanPlayCreditsRoll = ActiveMusicBox == 85;
		if (settings.ScanOreFinderData)
		{
			UpdateOreFinderData();
		}
	}

	private void ExportTileCountsToMain()
	{
		if (_tileCounts[27] > 0)
		{
			HasSunflower = true;
		}
		if (_tileCounts[567] > 0)
		{
			HasGardenGnome = true;
		}
		ShimmerTileCount = _liquidCounts[3];
		HoneyBlockCount = _tileCounts[229];
		HolyTileCount = _tileCounts[109] + _tileCounts[492] + _tileCounts[110] + _tileCounts[113] + _tileCounts[117] + _tileCounts[116] + _tileCounts[164] + _tileCounts[403] + _tileCounts[402];
		SnowTileCount = _tileCounts[147] + _tileCounts[148] + _tileCounts[161] + _tileCounts[162] + _tileCounts[164] + _tileCounts[163] + _tileCounts[200];
		if (Main.remixWorld)
		{
			JungleTileCount = _tileCounts[60] + _tileCounts[61] + _tileCounts[62] + _tileCounts[74] + _tileCounts[225];
			EvilTileCount = _tileCounts[23] + _tileCounts[661] + _tileCounts[24] + _tileCounts[25] + _tileCounts[32] + _tileCounts[112] + _tileCounts[163] + _tileCounts[400] + _tileCounts[398] + -10 * _tileCounts[27] + _tileCounts[474];
			BloodTileCount = _tileCounts[199] + _tileCounts[662] + _tileCounts[203] + _tileCounts[200] + _tileCounts[401] + _tileCounts[399] + _tileCounts[234] + _tileCounts[352] - 10 * _tileCounts[27] + _tileCounts[195];
		}
		else
		{
			JungleTileCount = _tileCounts[60] + _tileCounts[61] + _tileCounts[62] + _tileCounts[74] + _tileCounts[226] + _tileCounts[225];
			EvilTileCount = _tileCounts[23] + _tileCounts[661] + _tileCounts[24] + _tileCounts[25] + _tileCounts[32] + _tileCounts[112] + _tileCounts[163] + _tileCounts[400] + _tileCounts[398] + -10 * _tileCounts[27];
			BloodTileCount = _tileCounts[199] + _tileCounts[662] + _tileCounts[203] + _tileCounts[200] + _tileCounts[401] + _tileCounts[399] + _tileCounts[234] + _tileCounts[352] - 10 * _tileCounts[27];
		}
		MushroomTileCount = _tileCounts[70] + _tileCounts[71] + _tileCounts[72] + _tileCounts[528];
		MeteorTileCount = _tileCounts[37];
		DungeonTileCount = _tileCounts[41] + _tileCounts[43] + _tileCounts[44] + _tileCounts[481] + _tileCounts[482] + _tileCounts[483];
		SandTileCount = _tileCounts[53] + _tileCounts[112] + _tileCounts[116] + _tileCounts[234] + _tileCounts[397] + _tileCounts[398] + _tileCounts[402] + _tileCounts[399] + _tileCounts[396] + _tileCounts[400] + _tileCounts[403] + _tileCounts[401];
		PartyMonolithCount = _tileCounts[455];
		GraveyardTileCount = _tileCounts[85];
		GraveyardTileCount -= _tileCounts[27] / 2;
		if (_tileCounts[27] > 0)
		{
			HasSunflower = true;
		}
		if (GraveyardTileCount > GraveyardTileMin)
		{
			HasSunflower = false;
		}
		if (GraveyardTileCount < 0)
		{
			GraveyardTileCount = 0;
		}
		if (HolyTileCount < 0)
		{
			HolyTileCount = 0;
		}
		if (EvilTileCount < 0)
		{
			EvilTileCount = 0;
		}
		if (BloodTileCount < 0)
		{
			BloodTileCount = 0;
		}
		int vector = HolyTileCount;
		HolyTileCount -= EvilTileCount;
		HolyTileCount -= BloodTileCount;
		EvilTileCount -= vector;
		BloodTileCount -= vector;
		if (HolyTileCount < 0)
		{
			HolyTileCount = 0;
		}
		if (EvilTileCount < 0)
		{
			EvilTileCount = 0;
		}
		if (BloodTileCount < 0)
		{
			BloodTileCount = 0;
		}
	}

	public int GetTileCount(ushort tileId)
	{
		return _tileCounts[tileId];
	}

	public void Reset()
	{
		Array.Clear(_tileCounts, 0, _tileCounts.Length);
		Array.Clear(_liquidCounts, 0, _liquidCounts.Length);
		SandTileCount = 0;
		EvilTileCount = 0;
		BloodTileCount = 0;
		GraveyardTileCount = 0;
		MushroomTileCount = 0;
		SnowTileCount = 0;
		HolyTileCount = 0;
		MeteorTileCount = 0;
		JungleTileCount = 0;
		DungeonTileCount = 0;
		HasCampfire = false;
		HasSunflower = false;
		HasGardenGnome = false;
		HasStarInBottle = false;
		HasHeartLantern = false;
		HasClock = false;
		HasCatBast = false;
		ActiveMusicBox = -1;
		WaterCandleCount = 0;
		ActiveFountainColor = -1;
		ActiveMonolithType = -1;
		bestOre = -1;
		BloodMoonMonolith = false;
		MoonLordMonolith = false;
		EchoMonolith = false;
		ShimmerMonolithState = 0;
		Array.Clear(NPCBannerBuff, 0, NPCBannerBuff.Length);
		hasBanner = false;
		CanPlayCreditsRoll = false;
	}

	private void UpdateOreFinderData()
	{
		int result = -1;
		foreach (Point vector2 in _oreFinderTileLocations)
		{
			Tile vector3 = Main.tile[vector2.X, vector2.Y];
			if (IsValidForOreFinder(vector3) && (result < 0 || Main.tileOreFinderPriority[vector3.type] > Main.tileOreFinderPriority[result]))
			{
				result = vector3.type;
				ClosestOrePosition = vector2;
			}
		}
		bestOre = result;
	}

	public static bool IsValidForOreFinder(Tile t)
	{
		if (t.type == 227 && (t.frameX < 272 || t.frameX > 374))
		{
			return false;
		}
		if (t.type == 129 && t.frameX < 324)
		{
			return false;
		}
		return Main.tileOreFinderPriority[t.type] > 0;
	}
}
