using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Terraria.GameContent.Biomes.CaveHouse;

public static class HouseUtils
{
	private static readonly bool[] BlacklistedTiles = TileID.Sets.Factory.CreateBoolSet(true, 225, 41, 43, 44, 226, 203, 112, 25, 151, 21, 467);

	private static readonly bool[] BeelistedTiles = TileID.Sets.Factory.CreateBoolSet(true, 41, 43, 44, 226, 203, 112, 25, 151, 21, 467);

	public static HouseBuilder CreateBuilder(Point origin, StructureMap structures)
	{
		List<Rectangle> result = CreateRooms(origin);
		if (result.Count == 0 || !AreRoomLocationsValid(result))
		{
			return HouseBuilder.Invalid;
		}
		HouseType houseType = GetHouseType(result);
		if (!AreRoomsValid(result, structures, houseType))
		{
			return HouseBuilder.Invalid;
		}
		return houseType switch
		{
			HouseType.Wood => new WoodHouseBuilder(result), 
			HouseType.Desert => new DesertHouseBuilder(result), 
			HouseType.Granite => new GraniteHouseBuilder(result), 
			HouseType.Ice => new IceHouseBuilder(result), 
			HouseType.Jungle => new JungleHouseBuilder(result), 
			HouseType.Marble => new MarbleHouseBuilder(result), 
			HouseType.Mushroom => new MushroomHouseBuilder(result), 
			_ => new WoodHouseBuilder(result), 
		};
	}

	private static List<Rectangle> CreateRooms(Point origin)
	{
		if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(200), new Conditions.IsSolid()), out var num) || num == origin)
		{
			return new List<Rectangle>();
		}
		Rectangle dropRate = FindRoom(num);
		Rectangle rectangle = FindRoom(new Point(dropRate.Center.X, dropRate.Y + 1));
		Rectangle rectangle2 = FindRoom(new Point(dropRate.Center.X, dropRate.Y + dropRate.Height + 10));
		rectangle2.Y = dropRate.Y + dropRate.Height - 1;
		double roomSolidPrecentage = GetRoomSolidPrecentage(rectangle);
		double roomSolidPrecentage2 = GetRoomSolidPrecentage(rectangle2);
		dropRate.Y += 3;
		rectangle.Y += 3;
		rectangle2.Y += 3;
		List<Rectangle> list = new List<Rectangle>();
		if (WorldGen.genRand.NextDouble() > roomSolidPrecentage + 0.2)
		{
			list.Add(rectangle);
		}
		list.Add(dropRate);
		if (WorldGen.genRand.NextDouble() > roomSolidPrecentage2 + 0.2)
		{
			list.Add(rectangle2);
		}
		return list;
	}

	private static Rectangle FindRoom(Point origin)
	{
		Point result;
		bool flag = WorldUtils.Find(origin, Searches.Chain(new Searches.Left(25), new Conditions.IsSolid()), out result);
		Point result2;
		bool num = WorldUtils.Find(origin, Searches.Chain(new Searches.Right(25), new Conditions.IsSolid()), out result2);
		if (!flag)
		{
			result = new Point(origin.X - 25, origin.Y);
		}
		if (!num)
		{
			result2 = new Point(origin.X + 25, origin.Y);
		}
		Rectangle result3 = new Rectangle(origin.X, origin.Y, 0, 0);
		if (origin.X - result.X > result2.X - origin.X)
		{
			result3.X = result.X;
			result3.Width = Utils.Clamp(result2.X - result.X, 15, 30);
		}
		else
		{
			result3.Width = Utils.Clamp(result2.X - result.X, 15, 30);
			result3.X = result2.X - result3.Width;
		}
		Point result4;
		bool flag2 = WorldUtils.Find(result, Searches.Chain(new Searches.Up(10), new Conditions.IsSolid()), out result4);
		Point result5;
		bool num2 = WorldUtils.Find(result2, Searches.Chain(new Searches.Up(10), new Conditions.IsSolid()), out result5);
		if (!flag2)
		{
			result4 = new Point(origin.X, origin.Y - 10);
		}
		if (!num2)
		{
			result5 = new Point(origin.X, origin.Y - 10);
		}
		result3.Height = Utils.Clamp(Math.Max(origin.Y - result4.Y, origin.Y - result5.Y), 8, 12);
		result3.Y -= result3.Height;
		return result3;
	}

	private static double GetRoomSolidPrecentage(Rectangle room)
	{
		double result = room.Width * room.Height;
		Ref<int> @ref = new Ref<int>(0);
		WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.IsSolid(), new Actions.Count(@ref)));
		return (double)@ref.Value / result;
	}

	private static int SortBiomeResults(Tuple<HouseType, int> item1, Tuple<HouseType, int> item2)
	{
		return item2.Item2.CompareTo(item1.Item2);
	}

	private static bool AreRoomLocationsValid(IEnumerable<Rectangle> rooms)
	{
		foreach (Rectangle result in rooms)
		{
			if (result.Y + result.Height > Main.maxTilesY - 220)
			{
				return false;
			}
		}
		return true;
	}

	private static HouseType GetHouseType(IEnumerable<Rectangle> rooms)
	{
		Dictionary<ushort, int> num = new Dictionary<ushort, int>();
		foreach (Rectangle num3 in rooms)
		{
			WorldUtils.Gen(new Point(num3.X - 10, num3.Y - 10), new Shapes.Rectangle(num3.Width + 20, num3.Height + 20), new Actions.TileScanner(0, 59, 147, 1, 161, 53, 396, 397, 368, 367, 60, 70).Output(num));
		}
		List<Tuple<HouseType, int>> list = new List<Tuple<HouseType, int>>();
		list.Add(Tuple.Create(HouseType.Wood, num[0] + num[1]));
		list.Add(Tuple.Create(HouseType.Jungle, num[59] + num[60] * 10));
		list.Add(Tuple.Create(HouseType.Mushroom, num[59] + num[70] * 10));
		list.Add(Tuple.Create(HouseType.Ice, num[147] + num[161]));
		list.Add(Tuple.Create(HouseType.Desert, num[397] + num[396] + num[53]));
		list.Add(Tuple.Create(HouseType.Granite, num[368]));
		list.Add(Tuple.Create(HouseType.Marble, num[367]));
		list.Sort(SortBiomeResults);
		return list[0].Item1;
	}

	private static bool AreRoomsValid(IEnumerable<Rectangle> rooms, StructureMap structures, HouseType style)
	{
		foreach (Rectangle room in rooms)
		{
			if (style != HouseType.Granite && WorldUtils.Find(new Point(room.X - 2, room.Y - 2), Searches.Chain(new Searches.Rectangle(room.Width + 4, room.Height + 4).RequireAll(mode: false), new Conditions.HasLava()), out var _))
			{
				return false;
			}
			if (WorldGen.notTheBees)
			{
				if (!structures.CanPlace(room, BeelistedTiles, 5))
				{
					return false;
				}
			}
			else if (!structures.CanPlace(room, BlacklistedTiles, 5))
			{
				return false;
			}
		}
		return true;
	}
}
