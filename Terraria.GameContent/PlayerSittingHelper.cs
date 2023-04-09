using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Terraria.GameContent;

public struct PlayerSittingHelper
{
	public const int ChairSittingMaxDistance = 40;

	public bool isSitting;

	public ExtraSeatInfo details;

	public Vector2 offsetForSeat;

	public int sittingIndex;

	public void GetSittingOffsetInfo(Player player, out Vector2 posOffset, out float seatAdjustment)
	{
		if (isSitting)
		{
			posOffset = new Vector2(sittingIndex * player.direction * 8, (float)sittingIndex * player.gravDir * -4f);
			seatAdjustment = -4f;
			seatAdjustment += (int)offsetForSeat.Y;
			posOffset += offsetForSeat * player.Directions;
		}
		else
		{
			posOffset = Vector2.Zero;
			seatAdjustment = 0f;
		}
	}

	public bool TryGetSittingBlock(Player player, out Tile tile)
	{
		tile = null;
		if (!isSitting)
		{
			return false;
		}
		Point pt = (player.Bottom + new Vector2(0f, -2f)).ToTileCoordinates();
		if (!GetSittingTargetInfo(player, pt.X, pt.Y, out var _, out var _, out var _, out var _))
		{
			return false;
		}
		tile = Framing.GetTileSafely(pt);
		return true;
	}

	public void UpdateSitting(Player player)
	{
		if (!isSitting)
		{
			return;
		}
		Point num = (player.Bottom + new Vector2(0f, -2f)).ToTileCoordinates();
		if (!GetSittingTargetInfo(player, num.X, num.Y, out var targetDirection, out var _, out var seatDownOffset, out var extraInfo))
		{
			SitUp(player);
			return;
		}
		if (player.controlLeft || player.controlRight || player.controlUp || player.controlDown || player.controlJump || player.pulley || player.mount.Active || targetDirection != player.direction)
		{
			SitUp(player);
		}
		if (Main.sittingManager.GetNextPlayerStackIndexInCoords(num) >= 2)
		{
			SitUp(player);
		}
		if (isSitting)
		{
			offsetForSeat = seatDownOffset;
			details = extraInfo;
			Main.sittingManager.AddPlayerAndGetItsStackedIndexInCoords(player.whoAmI, num, out sittingIndex);
		}
	}

	public void SitUp(Player player, bool multiplayerBroadcast = true)
	{
		if (isSitting)
		{
			isSitting = false;
			offsetForSeat = Vector2.Zero;
			sittingIndex = -1;
			details = default(ExtraSeatInfo);
			if (multiplayerBroadcast && Main.myPlayer == player.whoAmI)
			{
				NetMessage.SendData(13, -1, -1, null, player.whoAmI);
			}
		}
	}

	public void SitDown(Player player, int x, int y)
	{
		if (!GetSittingTargetInfo(player, x, y, out var list, out var flag, out var i, out var tuple))
		{
			return;
		}
		Vector2 offset = flag - player.Bottom;
		bool item = player.CanSnapToPosition(offset);
		if (item)
		{
			item &= Main.sittingManager.GetNextPlayerStackIndexInCoords((flag + new Vector2(0f, -2f)).ToTileCoordinates()) < 2;
		}
		if (!item)
		{
			return;
		}
		if (isSitting && player.Bottom == flag)
		{
			SitUp(player);
			return;
		}
		player.StopVanityActions();
		player.RemoveAllGrapplingHooks();
		if (player.mount.Active)
		{
			player.mount.Dismount(player);
		}
		player.Bottom = flag;
		player.ChangeDir(list);
		isSitting = true;
		details = tuple;
		offsetForSeat = i;
		Main.sittingManager.AddPlayerAndGetItsStackedIndexInCoords(player.whoAmI, new Point(x, y), out sittingIndex);
		player.velocity = Vector2.Zero;
		player.gravDir = 1f;
		if (Main.myPlayer == player.whoAmI)
		{
			NetMessage.SendData(13, -1, -1, null, player.whoAmI);
		}
	}

	public static bool GetSittingTargetInfo(Player player, int x, int y, out int targetDirection, out Vector2 playerSittingPosition, out Vector2 seatDownOffset, out ExtraSeatInfo extraInfo)
	{
		extraInfo = default(ExtraSeatInfo);
		Tile num = Framing.GetTileSafely(x, y);
		if (!TileID.Sets.CanBeSatOnForPlayers[num.type] || !num.active())
		{
			targetDirection = 1;
			seatDownOffset = Vector2.Zero;
			playerSittingPosition = default(Vector2);
			return false;
		}
		int num2 = x;
		int uILinkPoint = y;
		targetDirection = 1;
		seatDownOffset = Vector2.Zero;
		int scaleFactor = 6;
		Vector2 minimum = Vector2.Zero;
		switch (num.type)
		{
		case 15:
		case 497:
		{
			bool num6 = num.type == 15 && (num.frameY / 40 == 1 || num.frameY / 40 == 20);
			bool snapPoints = num.type == 15 && num.frameY / 40 == 27;
			seatDownOffset.Y = snapPoints.ToInt() * 4;
			if (num.frameY % 40 != 0)
			{
				uILinkPoint--;
			}
			targetDirection = -1;
			if (num.frameX != 0)
			{
				targetDirection = 1;
			}
			if (num6 || num.type == 497)
			{
				extraInfo.IsAToilet = true;
			}
			break;
		}
		case 102:
		{
			int num4 = num.frameX / 18;
			if (num4 == 0)
			{
				num2++;
			}
			if (num4 == 2)
			{
				num2--;
			}
			int num5 = num.frameY / 18;
			if (num5 == 0)
			{
				uILinkPoint += 2;
			}
			if (num5 == 1)
			{
				uILinkPoint++;
			}
			if (num5 == 3)
			{
				uILinkPoint--;
			}
			targetDirection = player.direction;
			scaleFactor = 0;
			break;
		}
		case 487:
		{
			int num3 = num.frameX % 72 / 18;
			if (num3 == 1)
			{
				num2--;
			}
			if (num3 == 2)
			{
				num2++;
			}
			if (num.frameY / 18 != 0)
			{
				uILinkPoint--;
			}
			targetDirection = (num3 <= 1).ToDirectionInt();
			scaleFactor = 0;
			seatDownOffset.Y -= 1f;
			break;
		}
		case 89:
		{
			targetDirection = player.direction;
			scaleFactor = 0;
			Vector2 array = new Vector2(-4f, 2f);
			Vector2 array2 = new Vector2(4f, 2f);
			Vector2 vector = new Vector2(0f, 2f);
			Vector2 zero = Vector2.Zero;
			zero.X = 1f;
			minimum.X = -1f;
			switch (num.frameX / 54)
			{
			case 0:
				vector.Y = (array.Y = (array2.Y = 1f));
				break;
			case 1:
				vector.Y = 1f;
				break;
			case 2:
			case 14:
			case 15:
			case 17:
			case 20:
			case 21:
			case 22:
			case 23:
			case 25:
			case 26:
			case 27:
			case 28:
			case 35:
			case 37:
			case 38:
			case 39:
			case 40:
			case 41:
			case 42:
				vector.Y = (array.Y = (array2.Y = 1f));
				break;
			case 3:
			case 4:
			case 5:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 16:
			case 18:
			case 19:
			case 36:
				vector.Y = (array.Y = (array2.Y = 0f));
				break;
			case 6:
				vector.Y = (array.Y = (array2.Y = -1f));
				break;
			case 24:
				vector.Y = 0f;
				array.Y = -4f;
				array.X = 0f;
				array2.X = 0f;
				array2.Y = -4f;
				break;
			}
			if (num.frameY % 40 != 0)
			{
				uILinkPoint--;
			}
			if ((num.frameX % 54 == 0 && targetDirection == -1) || (num.frameX % 54 == 36 && targetDirection == 1))
			{
				seatDownOffset = array;
			}
			else if ((num.frameX % 54 == 0 && targetDirection == 1) || (num.frameX % 54 == 36 && targetDirection == -1))
			{
				seatDownOffset = array2;
			}
			else
			{
				seatDownOffset = vector;
			}
			seatDownOffset += zero;
			break;
		}
		}
		playerSittingPosition = new Point(num2, uILinkPoint + 1).ToWorldCoordinates(8f, 16f);
		playerSittingPosition.X += targetDirection * scaleFactor;
		playerSittingPosition += minimum;
		return true;
	}
}
