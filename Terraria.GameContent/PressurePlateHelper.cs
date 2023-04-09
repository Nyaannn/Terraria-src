using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Terraria.GameContent;

public class PressurePlateHelper
{
	public static object EntityCreationLock = new object();

	public static Dictionary<Point, bool[]> PressurePlatesPressed = new Dictionary<Point, bool[]>();

	public static bool NeedsFirstUpdate;

	private static Vector2[] PlayerLastPosition = new Vector2[255];

	private static Rectangle pressurePlateBounds = new Rectangle(0, 0, 16, 10);

	public static void Update()
	{
		if (!NeedsFirstUpdate)
		{
			return;
		}
		foreach (Point key in PressurePlatesPressed.Keys)
		{
			PokeLocation(key);
		}
		PressurePlatesPressed.Clear();
		NeedsFirstUpdate = false;
	}

	public static void Reset()
	{
		PressurePlatesPressed.Clear();
		for (int i = 0; i < PlayerLastPosition.Length; i++)
		{
			PlayerLastPosition[i] = Vector2.Zero;
		}
	}

	public static void ResetPlayer(int player)
	{
		Point[] array = PressurePlatesPressed.Keys.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			MoveAwayFrom(array[i], player);
		}
	}

	public static void UpdatePlayerPosition(Player player)
	{
		Point collection = new Point(1, 1);
		Vector2 num = collection.ToVector2();
		List<Point> tilesIn = Collision.GetTilesIn(PlayerLastPosition[player.whoAmI] + num, PlayerLastPosition[player.whoAmI] + player.Size - num * 2f);
		List<Point> entry = Collision.GetTilesIn(player.TopLeft + num, player.BottomRight - num * 2f);
		Rectangle hitbox = player.Hitbox;
		Rectangle hitbox2 = player.Hitbox;
		hitbox.Inflate(-collection.X, -collection.Y);
		hitbox2.Inflate(-collection.X, -collection.Y);
		hitbox2.X = (int)PlayerLastPosition[player.whoAmI].X;
		hitbox2.Y = (int)PlayerLastPosition[player.whoAmI].Y;
		for (int i = 0; i < tilesIn.Count; i++)
		{
			Point point = tilesIn[i];
			Tile tile = Main.tile[point.X, point.Y];
			if (tile.active() && tile.type == 428)
			{
				pressurePlateBounds.X = point.X * 16;
				pressurePlateBounds.Y = point.Y * 16 + 16 - pressurePlateBounds.Height;
				if (!hitbox.Intersects(pressurePlateBounds) && !entry.Contains(point))
				{
					MoveAwayFrom(point, player.whoAmI);
				}
			}
		}
		for (int j = 0; j < entry.Count; j++)
		{
			Point point2 = entry[j];
			Tile tile2 = Main.tile[point2.X, point2.Y];
			if (tile2.active() && tile2.type == 428)
			{
				pressurePlateBounds.X = point2.X * 16;
				pressurePlateBounds.Y = point2.Y * 16 + 16 - pressurePlateBounds.Height;
				if (hitbox.Intersects(pressurePlateBounds) && (!tilesIn.Contains(point2) || !hitbox2.Intersects(pressurePlateBounds)))
				{
					MoveInto(point2, player.whoAmI);
				}
			}
		}
		PlayerLastPosition[player.whoAmI] = player.position;
	}

	public static void DestroyPlate(Point location)
	{
		if (PressurePlatesPressed.TryGetValue(location, out var _))
		{
			PressurePlatesPressed.Remove(location);
			PokeLocation(location);
		}
	}

	private static void UpdatePlatePosition(Point location, int player, bool onIt)
	{
		if (onIt)
		{
			MoveInto(location, player);
		}
		else
		{
			MoveAwayFrom(location, player);
		}
	}

	private static void MoveInto(Point location, int player)
	{
		if (PressurePlatesPressed.TryGetValue(location, out var value))
		{
			value[player] = true;
			return;
		}
		lock (EntityCreationLock)
		{
			PressurePlatesPressed[location] = new bool[255];
		}
		PressurePlatesPressed[location][player] = true;
		PokeLocation(location);
	}

	private static void MoveAwayFrom(Point location, int player)
	{
		if (!PressurePlatesPressed.TryGetValue(location, out var uIElement))
		{
			return;
		}
		uIElement[player] = false;
		bool uIPanel = false;
		for (int uITextPanel = 0; uITextPanel < uIElement.Length; uITextPanel++)
		{
			if (uIElement[uITextPanel])
			{
				uIPanel = true;
				break;
			}
		}
		if (!uIPanel)
		{
			lock (EntityCreationLock)
			{
				PressurePlatesPressed.Remove(location);
			}
			PokeLocation(location);
		}
	}

	private static void PokeLocation(Point location)
	{
		if (Main.netMode != 1)
		{
			Wiring.blockPlayerTeleportationForOneIteration = true;
			Wiring.HitSwitch(location.X, location.Y);
			NetMessage.SendData(59, -1, -1, null, location.X, location.Y);
		}
	}
}
