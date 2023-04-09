using System.Collections.Generic;
using Terraria.Utilities;

namespace Terraria.GameContent;

public class FlexibleTileWand
{
	private class OptionBucket
	{
		public int ItemTypeToConsume;

		public List<PlacementOption> Options;

		public OptionBucket(int itemTypeToConsume)
		{
			ItemTypeToConsume = itemTypeToConsume;
			Options = new List<PlacementOption>();
		}

		public PlacementOption GetRandomOption(UnifiedRandom random)
		{
			return Options[random.Next(Options.Count)];
		}

		public PlacementOption GetOptionWithCycling(int cycleOffset)
		{
			int count = Options.Count;
			int index = (cycleOffset % count + count) % count;
			return Options[index];
		}
	}

	public class PlacementOption
	{
		public int TileIdToPlace;

		public int TileStyleToPlace;
	}

	public static FlexibleTileWand RubblePlacementSmall = CreateRubblePlacerSmall();

	public static FlexibleTileWand RubblePlacementMedium = CreateRubblePlacerMedium();

	public static FlexibleTileWand RubblePlacementLarge = CreateRubblePlacerLarge();

	private UnifiedRandom _random = new UnifiedRandom();

	private Dictionary<int, OptionBucket> _options = new Dictionary<int, OptionBucket>();

	public void AddVariation(int itemType, int tileIdToPlace, int tileStyleToPlace)
	{
		if (!_options.TryGetValue(itemType, out var value))
		{
			OptionBucket optionBucket2 = (_options[itemType] = new OptionBucket(itemType));
			value = optionBucket2;
		}
		value.Options.Add(new PlacementOption
		{
			TileIdToPlace = tileIdToPlace,
			TileStyleToPlace = tileStyleToPlace
		});
	}

	public void AddVariations(int itemType, int tileIdToPlace, params int[] stylesToPlace)
	{
		foreach (int tileStyleToPlace in stylesToPlace)
		{
			AddVariation(itemType, tileIdToPlace, tileStyleToPlace);
		}
	}

	public void AddVariations_ByRow(int itemType, int tileIdToPlace, int variationsPerRow, params int[] rows)
	{
		for (int uITextPanel = 0; uITextPanel < rows.Length; uITextPanel++)
		{
			for (int i = 0; i < variationsPerRow; i++)
			{
				int tileStyleToPlace = rows[uITextPanel] * variationsPerRow + i;
				AddVariation(itemType, tileIdToPlace, tileStyleToPlace);
			}
		}
	}

	public bool TryGetPlacementOption(Player player, int randomSeed, int selectCycleOffset, out PlacementOption option, out Item itemToConsume)
	{
		option = null;
		itemToConsume = null;
		Item[] uIHorizontalSeparator = player.inventory;
		for (int i = 0; i < 58; i++)
		{
			if (i < 50 || i >= 54)
			{
				Item item = uIHorizontalSeparator[i];
				if (!item.IsAir && _options.TryGetValue(item.type, out var value))
				{
					_random.SetSeed(randomSeed);
					option = value.GetOptionWithCycling(selectCycleOffset);
					itemToConsume = item;
					return true;
				}
			}
		}
		return false;
	}

	public static FlexibleTileWand CreateRubblePlacerLarge()
	{
		FlexibleTileWand flexibleTileWand = new FlexibleTileWand();
		int tileIdToPlace = 647;
		flexibleTileWand.AddVariations(154, tileIdToPlace, 0, 1, 2, 3, 4, 5, 6);
		flexibleTileWand.AddVariations(3, tileIdToPlace, 7, 8, 9, 10, 11, 12, 13, 14, 15);
		flexibleTileWand.AddVariations(71, tileIdToPlace, 16, 17);
		flexibleTileWand.AddVariations(72, tileIdToPlace, 18, 19);
		flexibleTileWand.AddVariations(73, tileIdToPlace, 20, 21);
		flexibleTileWand.AddVariations(9, tileIdToPlace, 22, 23, 24, 25);
		flexibleTileWand.AddVariations(593, tileIdToPlace, 26, 27, 28, 29, 30, 31);
		flexibleTileWand.AddVariations(183, tileIdToPlace, 32, 33, 34);
		tileIdToPlace = 648;
		flexibleTileWand.AddVariations(195, tileIdToPlace, 0, 1, 2);
		flexibleTileWand.AddVariations(195, tileIdToPlace, 3, 4, 5);
		flexibleTileWand.AddVariations(174, tileIdToPlace, 6, 7, 8);
		flexibleTileWand.AddVariations(150, tileIdToPlace, 9, 10, 11, 12, 13);
		flexibleTileWand.AddVariations(3, tileIdToPlace, 14, 15, 16);
		flexibleTileWand.AddVariations(989, tileIdToPlace, 17);
		flexibleTileWand.AddVariations(1101, tileIdToPlace, 18, 19, 20);
		flexibleTileWand.AddVariations(9, tileIdToPlace, 21, 22);
		flexibleTileWand.AddVariations(9, tileIdToPlace, 23, 24, 25, 26, 27, 28);
		flexibleTileWand.AddVariations(3271, tileIdToPlace, 29, 30, 31, 32, 33, 34);
		flexibleTileWand.AddVariations(3086, tileIdToPlace, 35, 36, 37, 38, 39, 40);
		flexibleTileWand.AddVariations(3081, tileIdToPlace, 41, 42, 43, 44, 45, 46);
		flexibleTileWand.AddVariations(62, tileIdToPlace, 47, 48, 49);
		flexibleTileWand.AddVariations(62, tileIdToPlace, 50, 51);
		flexibleTileWand.AddVariations(154, tileIdToPlace, 52, 53, 54);
		tileIdToPlace = 651;
		flexibleTileWand.AddVariations(195, tileIdToPlace, 0, 1, 2);
		flexibleTileWand.AddVariations(62, tileIdToPlace, 3, 4, 5);
		flexibleTileWand.AddVariations(331, tileIdToPlace, 6, 7, 8);
		return flexibleTileWand;
	}

	public static FlexibleTileWand CreateRubblePlacerMedium()
	{
		FlexibleTileWand extensions = new FlexibleTileWand();
		ushort text = 652;
		extensions.AddVariations(195, text, 0, 1, 2);
		extensions.AddVariations(62, text, 3, 4, 5);
		extensions.AddVariations(331, text, 6, 7, 8, 9, 10, 11);
		text = 649;
		extensions.AddVariations(3, text, 0, 1, 2, 3, 4, 5);
		extensions.AddVariations(154, text, 6, 7, 8, 9, 10);
		extensions.AddVariations(154, text, 11, 12, 13, 14, 15);
		extensions.AddVariations(71, text, 16);
		extensions.AddVariations(72, text, 17);
		extensions.AddVariations(73, text, 18);
		extensions.AddVariations(181, text, 19);
		extensions.AddVariations(180, text, 20);
		extensions.AddVariations(177, text, 21);
		extensions.AddVariations(179, text, 22);
		extensions.AddVariations(178, text, 23);
		extensions.AddVariations(182, text, 24);
		extensions.AddVariations(593, text, 25, 26, 27, 28, 29, 30);
		extensions.AddVariations(9, text, 31, 32, 33);
		extensions.AddVariations(150, text, 34, 35, 36, 37);
		extensions.AddVariations(3, text, 38, 39, 40);
		extensions.AddVariations(3271, text, 41, 42, 43, 44, 45, 46);
		extensions.AddVariations(3086, text, 47, 48, 49, 50, 51, 52);
		extensions.AddVariations(3081, text, 53, 54, 55, 56, 57, 58);
		extensions.AddVariations(62, text, 59, 60, 61);
		extensions.AddVariations(169, text, 62, 63, 64);
		return extensions;
	}

	public static FlexibleTileWand CreateRubblePlacerSmall()
	{
		FlexibleTileWand anyOf = new FlexibleTileWand();
		ushort num = 650;
		anyOf.AddVariations(3, num, 0, 1, 2, 3, 4, 5);
		anyOf.AddVariations(2, num, 6, 7, 8, 9, 10, 11);
		anyOf.AddVariations(154, num, 12, 13, 14, 15, 16, 17, 18, 19);
		anyOf.AddVariations(154, num, 20, 21, 22, 23, 24, 25, 26, 27);
		anyOf.AddVariations(9, num, 28, 29, 30, 31, 32);
		anyOf.AddVariations(9, num, 33, 34, 35);
		anyOf.AddVariations(593, num, 36, 37, 38, 39, 40, 41);
		anyOf.AddVariations(664, num, 42, 43, 44, 45, 46, 47);
		anyOf.AddVariations(150, num, 48, 49, 50, 51, 52, 53);
		anyOf.AddVariations(3271, num, 54, 55, 56, 57, 58, 59);
		anyOf.AddVariations(3086, num, 60, 61, 62, 63, 64, 65);
		anyOf.AddVariations(3081, num, 66, 67, 68, 69, 70, 71);
		anyOf.AddVariations(62, num, 72);
		anyOf.AddVariations(169, num, 73, 74, 75, 76);
		return anyOf;
	}

	public static void ForModders_AddPotsToWand(FlexibleTileWand wand, ref int echoPileStyle, ref ushort tileType)
	{
		int texture2D = 3;
		echoPileStyle = 0;
		tileType = 653;
		wand.AddVariations_ByRow(133, tileType, texture2D, 0, 1, 2, 3);
		wand.AddVariations_ByRow(664, tileType, texture2D, 4, 5, 6);
		wand.AddVariations_ByRow(176, tileType, texture2D, 7, 8, 9);
		wand.AddVariations_ByRow(154, tileType, texture2D, 10, 11, 12);
		wand.AddVariations_ByRow(173, tileType, texture2D, 13, 14, 15);
		wand.AddVariations_ByRow(61, tileType, texture2D, 16, 17, 18);
		wand.AddVariations_ByRow(150, tileType, texture2D, 19, 20, 21);
		wand.AddVariations_ByRow(836, tileType, texture2D, 22, 23, 24);
		wand.AddVariations_ByRow(607, tileType, texture2D, 25, 26, 27);
		wand.AddVariations_ByRow(1101, tileType, texture2D, 28, 29, 30);
		wand.AddVariations_ByRow(3081, tileType, texture2D, 31, 32, 33);
		wand.AddVariations_ByRow(607, tileType, texture2D, 34, 35, 36);
	}
}
