using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Terraria.GameContent.Biomes;

public class EnchantedSwordBiome : MicroBiome
{
	[JsonProperty("ChanceOfEntrance")]
	private double _chanceOfEntrance;

	[JsonProperty("ChanceOfRealSword")]
	private double _chanceOfRealSword;

	public override bool Place(Point origin, StructureMap structures)
	{
		Dictionary<ushort, int> simulatorInfo = new Dictionary<ushort, int>();
		WorldUtils.Gen(new Point(origin.X - 25, origin.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(0, 1).Output(simulatorInfo));
		if (simulatorInfo[0] + simulatorInfo[1] < 1250)
		{
			return false;
		}
		Point nPC;
		bool lootSimulationItemCounter = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out nPC);
		if (WorldUtils.Find(origin, Searches.Chain(new Searches.Up(origin.Y - nPC.Y), new Conditions.IsTile(53)), out var _))
		{
			return false;
		}
		if (!lootSimulationItemCounter)
		{
			return false;
		}
		nPC.Y += 50;
		ShapeData text2 = new ShapeData();
		ShapeData name = new ShapeData();
		Point text3 = new Point(origin.X, origin.Y + 20);
		Point point = new Point(origin.X, origin.Y + 30);
		bool[] array = new bool[TileID.Sets.GeneralPlacementTiles.Length];
		for (int neededTestCondition = 0; neededTestCondition < array.Length; neededTestCondition++)
		{
			array[neededTestCondition] = TileID.Sets.GeneralPlacementTiles[neededTestCondition];
		}
		array[21] = false;
		array[467] = false;
		double num2 = 0.8 + GenBase._random.NextDouble() * 0.5;
		if (!structures.CanPlace(new Rectangle(text3.X - (int)(20.0 * num2), text3.Y - 20, (int)(40.0 * num2), 40), array))
		{
			return false;
		}
		if (!structures.CanPlace(new Rectangle(origin.X, nPC.Y + 10, 1, origin.Y - nPC.Y - 9), array, 2))
		{
			return false;
		}
		WorldUtils.Gen(text3, new Shapes.Slime(20, num2, 1.0), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(text2)));
		WorldUtils.Gen(point, new Shapes.Mound(14, 14), Actions.Chain(new Modifiers.Blotches(2, 1, 0.8), new Actions.SetTile(0), new Actions.SetFrames(frameNeighbors: true).Output(name)));
		text2.Subtract(name, text3, point);
		WorldUtils.Gen(text3, new ModShapes.InnerOutline(text2), Actions.Chain(new Actions.SetTile(2), new Actions.SetFrames(frameNeighbors: true)));
		WorldUtils.Gen(text3, new ModShapes.All(text2), Actions.Chain(new Modifiers.RectangleMask(-40, 40, 0, 40), new Modifiers.IsEmpty(), new Actions.SetLiquid()));
		WorldUtils.Gen(text3, new ModShapes.All(text2), Actions.Chain(new Actions.PlaceWall(68), new Modifiers.OnlyTiles(2), new Modifiers.Offset(0, 1), new ActionVines(3, 5, 382)));
		if (GenBase._random.NextDouble() <= _chanceOfEntrance || WorldGen.tenthAnniversaryWorldGen)
		{
			ShapeData num = new ShapeData();
			WorldUtils.Gen(new Point(origin.X, nPC.Y + 10), new Shapes.Rectangle(1, origin.Y - nPC.Y - 9), Actions.Chain(new Modifiers.Blotches(2, 0.2), new Modifiers.SkipTiles(191, 192), new Actions.ClearTile().Output(num), new Modifiers.Expand(1), new Modifiers.OnlyTiles(53), new Actions.SetTile(397).Output(num)));
			WorldUtils.Gen(new Point(origin.X, nPC.Y + 10), new ModShapes.All(num), new Actions.SetFrames(frameNeighbors: true));
		}
		if (GenBase._random.NextDouble() <= _chanceOfRealSword)
		{
			WorldGen.PlaceTile(point.X, point.Y - 15, 187, mute: true, forced: false, -1, 17);
		}
		else
		{
			WorldGen.PlaceTile(point.X, point.Y - 15, 186, mute: true, forced: false, -1, 15);
		}
		WorldUtils.Gen(point, new ModShapes.All(name), Actions.Chain(new Modifiers.Offset(0, -1), new Modifiers.OnlyTiles(2), new Modifiers.Offset(0, -1), new ActionGrass()));
		structures.AddProtectedStructure(new Rectangle(text3.X - (int)(20.0 * num2), text3.Y - 20, (int)(40.0 * num2), 40), 10);
		return true;
	}
}
