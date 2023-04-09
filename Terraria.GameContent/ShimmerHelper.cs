using Microsoft.Xna.Framework;

namespace Terraria.GameContent;

public class ShimmerHelper
{
	public static Vector2? FindSpotWithoutShimmer(Entity entity, int startX, int startY, int expand, bool allowSolidTop)
	{
		Vector2 num = new Vector2(-entity.width / 2, -entity.height);
		for (int uIElement = 0; uIElement < expand; uIElement++)
		{
			int num4 = startX - uIElement;
			int uIPanel = startY - expand;
			Vector2 num2 = new Vector2(num4 * 16, uIPanel * 16) + num;
			if (IsSpotShimmerFree(entity, num2, allowSolidTop))
			{
				return num2;
			}
			num2 = new Vector2((startX + uIElement) * 16, uIPanel * 16) + num;
			if (IsSpotShimmerFree(entity, num2, allowSolidTop))
			{
				return num2;
			}
			int num5 = startX - uIElement;
			uIPanel = startY + expand;
			num2 = new Vector2(num5 * 16, uIPanel * 16) + num;
			if (IsSpotShimmerFree(entity, num2, allowSolidTop))
			{
				return num2;
			}
			num2 = new Vector2((startX + uIElement) * 16, uIPanel * 16) + num;
			if (IsSpotShimmerFree(entity, num2, allowSolidTop))
			{
				return num2;
			}
		}
		for (int uIElement2 = 0; uIElement2 < expand; uIElement2++)
		{
			int num6 = startX - expand;
			int uIBestiaryEntryInfoPage = startY - uIElement2;
			Vector2 num3 = new Vector2(num6 * 16, uIBestiaryEntryInfoPage * 16) + num;
			if (IsSpotShimmerFree(entity, num3, allowSolidTop))
			{
				return num3;
			}
			num3 = new Vector2((startX + expand) * 16, uIBestiaryEntryInfoPage * 16) + num;
			if (IsSpotShimmerFree(entity, num3, allowSolidTop))
			{
				return num3;
			}
			int num7 = startX - expand;
			uIBestiaryEntryInfoPage = startY + uIElement2;
			num3 = new Vector2(num7 * 16, uIBestiaryEntryInfoPage * 16) + num;
			if (IsSpotShimmerFree(entity, num3, allowSolidTop))
			{
				return num3;
			}
			num3 = new Vector2((startX + expand) * 16, uIBestiaryEntryInfoPage * 16) + num;
			if (IsSpotShimmerFree(entity, num3, allowSolidTop))
			{
				return num3;
			}
		}
		return null;
	}

	private static bool IsSpotShimmerFree(Entity entity, Vector2 landingPosition, bool allowSolidTop)
	{
		if (Collision.SolidCollision(landingPosition, entity.width, entity.height))
		{
			return false;
		}
		if (!Collision.SolidCollision(landingPosition + new Vector2(0f, entity.height), entity.width, 100, allowSolidTop))
		{
			return false;
		}
		if (Collision.WetCollision(landingPosition, entity.width, entity.height + 100) && Collision.shimmer)
		{
			return false;
		}
		return true;
	}
}
