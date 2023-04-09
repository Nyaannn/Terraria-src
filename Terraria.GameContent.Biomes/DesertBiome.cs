using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria.GameContent.Biomes.Desert;
using Terraria.WorldBuilding;

namespace Terraria.GameContent.Biomes;

public class DesertBiome : MicroBiome
{
	[JsonProperty("ChanceOfEntrance")]
	public double ChanceOfEntrance = 0.3333;

	public override bool Place(Point origin, StructureMap structures)
	{
		DesertDescription list = DesertDescription.CreateFromPlacement(origin);
		if (!list.IsValid)
		{
			return false;
		}
		ExportDescriptionToEngine(list);
		SandMound.Place(list);
		list.UpdateSurfaceMap();
		if (!Main.tenthAnniversaryWorld && GenBase._random.NextDouble() <= ChanceOfEntrance)
		{
			switch (GenBase._random.Next(4))
			{
			case 0:
				ChambersEntrance.Place(list);
				break;
			case 1:
				AnthillEntrance.Place(list);
				break;
			case 2:
				LarvaHoleEntrance.Place(list);
				break;
			case 3:
				PitEntrance.Place(list);
				break;
			}
		}
		DesertHive.Place(list);
		CleanupArea(list.Hive);
		Rectangle area = new Rectangle(list.CombinedArea.X, 50, list.CombinedArea.Width, list.CombinedArea.Bottom - 20);
		structures.AddStructure(area, 10);
		return true;
	}

	private static void ExportDescriptionToEngine(DesertDescription description)
	{
		GenVars.UndergroundDesertLocation = description.CombinedArea;
		GenVars.UndergroundDesertLocation.Inflate(10, 10);
		GenVars.UndergroundDesertHiveLocation = description.Hive;
	}

	private static void CleanupArea(Rectangle area)
	{
		for (int timesMultiplier = -20 + area.Left; timesMultiplier < area.Right + 20; timesMultiplier++)
		{
			for (int str = -20 + area.Top; str < area.Bottom + 20; str++)
			{
				if (timesMultiplier > 0 && timesMultiplier < Main.maxTilesX - 1 && str > 0 && str < Main.maxTilesY - 1)
				{
					WorldGen.SquareWallFrame(timesMultiplier, str);
					WorldUtils.TileFrame(timesMultiplier, str, frameNeighbors: true);
				}
			}
		}
	}
}
