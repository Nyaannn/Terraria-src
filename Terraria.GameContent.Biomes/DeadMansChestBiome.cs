using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Terraria.GameContent.Biomes;

public class DeadMansChestBiome : MicroBiome
{
	private class DartTrapPlacementAttempt
	{
		public int directionX;

		public int xPush;

		public int x;

		public int y;

		public Point position;

		public Tile t;

		public DartTrapPlacementAttempt(Point position, int directionX, int x, int y, int xPush, Tile t)
		{
			this.position = position;
			this.directionX = directionX;
			this.x = x;
			this.y = y;
			this.xPush = xPush;
			this.t = t;
		}
	}

	private class BoulderPlacementAttempt
	{
		public Point position;

		public int yPush;

		public int requiredHeight;

		public int bestType;

		public BoulderPlacementAttempt(Point position, int yPush, int requiredHeight, int bestType)
		{
			this.position = position;
			this.yPush = yPush;
			this.requiredHeight = requiredHeight;
			this.bestType = bestType;
		}
	}

	private class WirePlacementAttempt
	{
		public Point position;

		public int dirX;

		public int dirY;

		public int steps;

		public WirePlacementAttempt(Point position, int dirX, int dirY, int steps)
		{
			this.position = position;
			this.dirX = dirX;
			this.dirY = dirY;
			this.steps = steps;
		}
	}

	private class ExplosivePlacementAttempt
	{
		public Point position;

		public ExplosivePlacementAttempt(Point position)
		{
			this.position = position;
		}
	}

	private List<DartTrapPlacementAttempt> _dartTrapPlacementSpots = new List<DartTrapPlacementAttempt>();

	private List<WirePlacementAttempt> _wirePlacementSpots = new List<WirePlacementAttempt>();

	private List<BoulderPlacementAttempt> _boulderPlacementSpots = new List<BoulderPlacementAttempt>();

	private List<ExplosivePlacementAttempt> _explosivePlacementAttempt = new List<ExplosivePlacementAttempt>();

	[JsonProperty("NumberOfDartTraps")]
	private IntRange _numberOfDartTraps = new IntRange(3, 6);

	[JsonProperty("NumberOfBoulderTraps")]
	private IntRange _numberOfBoulderTraps = new IntRange(2, 4);

	[JsonProperty("NumberOfStepsBetweenBoulderTraps")]
	private IntRange _numberOfStepsBetweenBoulderTraps = new IntRange(2, 4);

	public override bool Place(Point origin, StructureMap structures)
	{
		if (!IsAGoodSpot(origin))
		{
			return false;
		}
		ClearCaches();
		Point homeHitbox = new Point(origin.X, origin.Y + 1);
		FindBoulderTrapSpots(homeHitbox);
		FindDartTrapSpots(homeHitbox);
		FindExplosiveTrapSpots(homeHitbox);
		if (!AreThereEnoughTraps())
		{
			return false;
		}
		TurnGoldChestIntoDeadMansChest(origin);
		foreach (DartTrapPlacementAttempt point in _dartTrapPlacementSpots)
		{
			ActuallyPlaceDartTrap(point.position, point.directionX, point.x, point.y, point.xPush, point.t);
		}
		foreach (WirePlacementAttempt wirePlacementSpot in _wirePlacementSpots)
		{
			PlaceWireLine(wirePlacementSpot.position, wirePlacementSpot.dirX, wirePlacementSpot.dirY, wirePlacementSpot.steps);
		}
		foreach (BoulderPlacementAttempt boulderPlacementSpot in _boulderPlacementSpots)
		{
			ActuallyPlaceBoulderTrap(boulderPlacementSpot.position, boulderPlacementSpot.yPush, boulderPlacementSpot.requiredHeight, boulderPlacementSpot.bestType);
		}
		foreach (ExplosivePlacementAttempt item in _explosivePlacementAttempt)
		{
			ActuallyPlaceExplosive(item.position);
		}
		PlaceWiresForExplosives(origin);
		return true;
	}

	private void PlaceWiresForExplosives(Point origin)
	{
		if (_explosivePlacementAttempt.Count <= 0)
		{
			return;
		}
		PlaceWireLine(origin, 0, 1, _explosivePlacementAttempt[0].position.Y - origin.Y);
		int num = _explosivePlacementAttempt[0].position.X;
		int num2 = _explosivePlacementAttempt[0].position.X;
		int y = _explosivePlacementAttempt[0].position.Y;
		for (int i = 1; i < _explosivePlacementAttempt.Count; i++)
		{
			int x = _explosivePlacementAttempt[i].position.X;
			if (num > x)
			{
				num = x;
			}
			if (num2 < x)
			{
				num2 = x;
			}
		}
		PlaceWireLine(new Point(num, y), 1, 0, num2 - num);
	}

	private bool AreThereEnoughTraps()
	{
		if (_boulderPlacementSpots.Count >= 1 || _explosivePlacementAttempt.Count >= 1)
		{
			return _dartTrapPlacementSpots.Count >= 1;
		}
		return false;
	}

	private void ClearCaches()
	{
		_dartTrapPlacementSpots.Clear();
		_wirePlacementSpots.Clear();
		_boulderPlacementSpots.Clear();
		_explosivePlacementAttempt.Clear();
	}

	private void FindBoulderTrapSpots(Point position)
	{
		int x = position.X;
		int num = GenBase._random.Next(_numberOfBoulderTraps);
		int num2 = GenBase._random.Next(_numberOfStepsBetweenBoulderTraps);
		x -= num / 2 * num2;
		int num3 = position.Y - 6;
		for (int i = 0; i <= num; i++)
		{
			FindBoulderTrapSpot(new Point(x, num3));
			x += num2;
		}
		if (_boulderPlacementSpots.Count <= 0)
		{
			return;
		}
		int num4 = _boulderPlacementSpots[0].position.X;
		int num5 = _boulderPlacementSpots[0].position.X;
		for (int j = 1; j < _boulderPlacementSpots.Count; j++)
		{
			int x2 = _boulderPlacementSpots[j].position.X;
			if (num4 > x2)
			{
				num4 = x2;
			}
			if (num5 < x2)
			{
				num5 = x2;
			}
		}
		if (num4 > position.X)
		{
			num4 = position.X;
		}
		if (num5 < position.X)
		{
			num5 = position.X;
		}
		_wirePlacementSpots.Add(new WirePlacementAttempt(new Point(num4, num3 - 1), 1, 0, num5 - num4));
		_wirePlacementSpots.Add(new WirePlacementAttempt(position, 0, -1, 7));
	}

	private void FindBoulderTrapSpot(Point position)
	{
		int x = position.X;
		int y = position.Y;
		for (int i = 0; i < 50; i++)
		{
			if (Main.tile[x, y - i].active())
			{
				PlaceBoulderTrapSpot(new Point(x, y - i), i);
				break;
			}
		}
	}

	private void PlaceBoulderTrapSpot(Point position, int yPush)
	{
		int[] array = new int[TileID.Count];
		for (int i = position.X; i < position.X + 2; i++)
		{
			for (int j = position.Y - 4; j <= position.Y; j++)
			{
				Tile tile = Main.tile[i, j];
				if (tile.active() && !Main.tileFrameImportant[tile.type] && Main.tileSolid[tile.type])
				{
					array[tile.type]++;
				}
				if ((tile.active() && !TileID.Sets.CanBeClearedDuringGeneration[tile.type]) || (tile.active() && TileID.Sets.IsAContainer[tile.type]))
				{
					return;
				}
			}
		}
		for (int k = position.X - 1; k < position.X + 2 + 1; k++)
		{
			for (int l = position.Y - 4 - 1; l <= position.Y - 4 + 2; l++)
			{
				Tile tile2 = Main.tile[k, l];
				if (!tile2.active() || TileID.Sets.IsAContainer[tile2.type])
				{
					return;
				}
			}
		}
		int num = 2;
		int num2 = position.X - num;
		int num3 = position.Y - 4 - num;
		int num4 = position.X + num + 1;
		int num5 = position.Y - 4 + num + 1;
		for (int m = num2; m <= num4; m++)
		{
			for (int n = num3; n <= num5; n++)
			{
				Tile tile3 = Main.tile[m, n];
				if (tile3.active() && TileID.Sets.IsAContainer[tile3.type])
				{
					return;
				}
			}
		}
		int num6 = -1;
		for (int num7 = 0; num7 < array.Length; num7++)
		{
			if (num6 == -1 || array[num6] < array[num7])
			{
				num6 = num7;
			}
		}
		_boulderPlacementSpots.Add(new BoulderPlacementAttempt(position, yPush - 1, 4, num6));
	}

	private void FindDartTrapSpots(Point position)
	{
		int smartInteractCandidate = GenBase._random.Next(_numberOfDartTraps);
		int num = ((GenBase._random.Next(2) != 0) ? 1 : (-1));
		int steps = -1;
		for (int candidate = 0; candidate < smartInteractCandidate; candidate++)
		{
			bool num2 = FindDartTrapSpotSingle(position, num);
			num *= -1;
			position.Y--;
			if (num2)
			{
				steps = candidate;
			}
		}
		_wirePlacementSpots.Add(new WirePlacementAttempt(new Point(position.X, position.Y + smartInteractCandidate), 0, -1, steps));
	}

	private bool FindDartTrapSpotSingle(Point position, int directionX)
	{
		int x = position.X;
		int y = position.Y;
		for (int i = 0; i < 20; i++)
		{
			Tile tile = Main.tile[x + i * directionX, y];
			if ((!tile.active() || tile.type < 0 || tile.type >= TileID.Count || !TileID.Sets.IsAContainer[tile.type]) && tile.active() && Main.tileSolid[tile.type])
			{
				if (i >= 5 && !tile.actuator() && !Main.tileFrameImportant[tile.type] && TileID.Sets.CanBeClearedDuringGeneration[tile.type])
				{
					_dartTrapPlacementSpots.Add(new DartTrapPlacementAttempt(position, directionX, x, y, i, tile));
					return true;
				}
				return false;
			}
		}
		return false;
	}

	private void FindExplosiveTrapSpots(Point position)
	{
		int point = position.X;
		int num = position.Y + 3;
		List<int> num2 = new List<int>();
		if (IsGoodSpotsForExplosive(point, num))
		{
			num2.Add(point);
		}
		point++;
		if (IsGoodSpotsForExplosive(point, num))
		{
			num2.Add(point);
		}
		int num3 = -1;
		if (num2.Count > 0)
		{
			num3 = num2[GenBase._random.Next(num2.Count)];
		}
		num2.Clear();
		point += GenBase._random.Next(2, 6);
		int num4 = 4;
		for (int tuple = point; tuple < point + num4; tuple++)
		{
			if (IsGoodSpotsForExplosive(tuple, num))
			{
				num2.Add(tuple);
			}
		}
		int flag = -1;
		if (num2.Count > 0)
		{
			flag = num2[GenBase._random.Next(num2.Count)];
		}
		point = position.X - num4 - GenBase._random.Next(2, 6);
		for (int i = point; i < point + num4; i++)
		{
			if (IsGoodSpotsForExplosive(i, num))
			{
				num2.Add(i);
			}
		}
		int num5 = -1;
		if (num2.Count > 0)
		{
			num5 = num2[GenBase._random.Next(num2.Count)];
		}
		if (num5 != -1)
		{
			_explosivePlacementAttempt.Add(new ExplosivePlacementAttempt(new Point(num5, num)));
		}
		if (num3 != -1)
		{
			_explosivePlacementAttempt.Add(new ExplosivePlacementAttempt(new Point(num3, num)));
		}
		if (flag != -1)
		{
			_explosivePlacementAttempt.Add(new ExplosivePlacementAttempt(new Point(flag, num)));
		}
	}

	private bool IsGoodSpotsForExplosive(int x, int y)
	{
		Tile i = Main.tile[x, y];
		if (i.active() && i.type >= 0 && i.type < TileID.Count && TileID.Sets.IsAContainer[i.type])
		{
			return false;
		}
		if (i.active() && Main.tileSolid[i.type] && !Main.tileFrameImportant[i.type] && !Main.tileSolidTop[i.type])
		{
			return true;
		}
		return false;
	}

	public List<int> GetPossibleChestsToTrapify(StructureMap structures)
	{
		List<int> list = new List<int>();
		bool[] array = new bool[TileID.Sets.GeneralPlacementTiles.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = TileID.Sets.GeneralPlacementTiles[i];
		}
		array[21] = true;
		array[467] = true;
		for (int j = 0; j < 8000; j++)
		{
			Chest chest = Main.chest[j];
			if (chest == null)
			{
				continue;
			}
			Point position = new Point(chest.x, chest.y);
			if (IsAGoodSpot(position))
			{
				ClearCaches();
				Point position2 = new Point(position.X, position.Y + 1);
				FindBoulderTrapSpots(position2);
				FindDartTrapSpots(position2);
				if (AreThereEnoughTraps() && (structures == null || structures.CanPlace(new Rectangle(position.X, position.Y, 1, 1), array, 10)))
				{
					list.Add(j);
				}
			}
		}
		return list;
	}

	private static bool IsAGoodSpot(Point position)
	{
		if (!WorldGen.InWorld(position.X, position.Y, 50))
		{
			return false;
		}
		if (WorldGen.oceanDepths(position.X, position.Y))
		{
			return false;
		}
		Tile value = Main.tile[position.X, position.Y];
		if (value.type != 21)
		{
			return false;
		}
		if (value.frameX / 36 != 1)
		{
			return false;
		}
		value = Main.tile[position.X, position.Y + 2];
		if (!TileID.Sets.CanBeClearedDuringGeneration[value.type])
		{
			return false;
		}
		if (WorldGen.countWires(position.X, position.Y, 20) > 0)
		{
			return false;
		}
		if (WorldGen.countTiles(position.X, position.Y, jungle: false, lavaOk: true) < 40)
		{
			return false;
		}
		return true;
	}

	private void TurnGoldChestIntoDeadMansChest(Point position)
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				int num = position.X + i;
				int num2 = position.Y + j;
				Tile tile = Main.tile[num, num2];
				tile.type = 467;
				tile.frameX = (short)(144 + i * 18);
				tile.frameY = (short)(j * 18);
			}
		}
		if (GenBase._random.Next(3) != 0)
		{
			return;
		}
		int num3 = Chest.FindChest(position.X, position.Y);
		if (num3 <= -1)
		{
			return;
		}
		Item[] item = Main.chest[num3].item;
		for (int num4 = item.Length - 2; num4 > 0; num4--)
		{
			Item item2 = item[num4];
			if (item2.stack != 0)
			{
				item[num4 + 1] = item2.DeepClone();
			}
		}
		item[1] = new Item();
		item[1].SetDefaults(5007);
		Main.chest[num3].item = item;
	}

	private void ActuallyPlaceDartTrap(Point position, int directionX, int x, int y, int xPush, Tile t)
	{
		t.type = 137;
		t.frameY = 0;
		if (directionX == -1)
		{
			t.frameX = 18;
		}
		else
		{
			t.frameX = 0;
		}
		t.slope(0);
		t.halfBrick(halfBrick: false);
		WorldGen.TileFrame(x, y, resetFrame: true);
		PlaceWireLine(position, directionX, 0, xPush);
	}

	private void PlaceWireLine(Point start, int offsetX, int offsetY, int steps)
	{
		for (int result = 0; result <= steps; result++)
		{
			Main.tile[start.X + offsetX * result, start.Y + offsetY * result].wire(wire: true);
		}
	}

	private void ActuallyPlaceBoulderTrap(Point position, int yPush, int requiredHeight, int bestType)
	{
		for (int i = position.X; i < position.X + 2; i++)
		{
			for (int j = position.Y - requiredHeight; j <= position.Y + 2; j++)
			{
				Tile tile = Main.tile[i, j];
				if (j < position.Y - requiredHeight + 2)
				{
					tile.ClearTile();
				}
				else if (j <= position.Y)
				{
					if (!tile.active())
					{
						tile.active(active: true);
						tile.type = (ushort)bestType;
					}
					tile.slope(0);
					tile.halfBrick(halfBrick: false);
					tile.actuator(actuator: true);
					tile.wire(wire: true);
					WorldGen.TileFrame(i, j, resetFrame: true);
				}
				else
				{
					tile.ClearTile();
				}
			}
		}
		int result = position.X + 1;
		int num = position.Y - requiredHeight + 1;
		int num2 = 3;
		int num3 = result - num2;
		int num4 = num - num2;
		int num5 = result + num2 - 1;
		int num6 = num + num2 - 1;
		for (int k = num3; k <= num5; k++)
		{
			for (int l = num4; l <= num6; l++)
			{
				if (Main.tile[k, l].type != 138)
				{
					Main.tile[k, l].type = 1;
				}
			}
		}
		WorldGen.PlaceTile(result, num, 138);
		PlaceWireLine(position, 0, 1, yPush);
	}

	private void ActuallyPlaceExplosive(Point position)
	{
		Tile tile = Main.tile[position.X, position.Y];
		tile.type = 141;
		tile.frameX = (tile.frameY = 0);
		tile.slope(0);
		tile.halfBrick(halfBrick: false);
		WorldGen.TileFrame(position.X, position.Y, resetFrame: true);
	}
}
