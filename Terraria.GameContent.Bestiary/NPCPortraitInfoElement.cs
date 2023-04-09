using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Terraria.GameContent.Bestiary;

public class NPCPortraitInfoElement : IBestiaryInfoElement
{
	private int? _filledStarsCount;

	public NPCPortraitInfoElement(int? rarityStars = null)
	{
		_filledStarsCount = rarityStars;
	}

	public UIElement ProvideUIElement(BestiaryUICollectionInfo info)
	{
		UIElement uIElement = new UIElement
		{
			Width = new StyleDimension(0f, 1f),
			Height = new StyleDimension(112f, 0f)
		};
		uIElement.SetPadding(0f);
		BestiaryEntry bestiaryEntry = new BestiaryEntry();
		Asset<Texture2D> portraitBackgroundAsset = null;
		Color portraitColor = Color.White;
		bestiaryEntry.Icon = info.OwnerEntry.Icon.CreateClone();
		bestiaryEntry.UIInfoProvider = info.OwnerEntry.UIInfoProvider;
		List<IBestiaryBackgroundOverlayAndColorProvider> list = new List<IBestiaryBackgroundOverlayAndColorProvider>();
		bool flag = info.UnlockState > BestiaryEntryUnlockState.NotKnownAtAll_0;
		if (flag)
		{
			List<IBestiaryInfoElement> list2 = new List<IBestiaryInfoElement>();
			IEnumerable<IBestiaryBackgroundImagePathAndColorProvider> source = info.OwnerEntry.Info.OfType<IBestiaryBackgroundImagePathAndColorProvider>();
			IEnumerable<IPreferenceProviderElement> preferences = info.OwnerEntry.Info.OfType<IPreferenceProviderElement>();
			IEnumerable<IBestiaryBackgroundImagePathAndColorProvider> enumerable = source.Where((IBestiaryBackgroundImagePathAndColorProvider provider) => preferences.Any((IPreferenceProviderElement preference) => preference.Matches(provider)));
			bool flag2 = false;
			foreach (IBestiaryBackgroundImagePathAndColorProvider item in enumerable)
			{
				Asset<Texture2D> backgroundImage = item.GetBackgroundImage();
				if (backgroundImage != null)
				{
					portraitBackgroundAsset = backgroundImage;
					flag2 = true;
					Color? backgroundColor = item.GetBackgroundColor();
					if (backgroundColor.HasValue)
					{
						portraitColor = backgroundColor.Value;
					}
					break;
				}
			}
			foreach (IBestiaryInfoElement item2 in info.OwnerEntry.Info)
			{
				if (item2 is IBestiaryBackgroundImagePathAndColorProvider bestiaryBackgroundImagePathAndColorProvider)
				{
					Asset<Texture2D> backgroundImage2 = bestiaryBackgroundImagePathAndColorProvider.GetBackgroundImage();
					if (backgroundImage2 == null)
					{
						continue;
					}
					if (!flag2)
					{
						portraitBackgroundAsset = backgroundImage2;
					}
					Color? backgroundColor2 = bestiaryBackgroundImagePathAndColorProvider.GetBackgroundColor();
					if (backgroundColor2.HasValue)
					{
						portraitColor = backgroundColor2.Value;
					}
				}
				if (!flag2 && item2 is IBestiaryBackgroundOverlayAndColorProvider bestiaryBackgroundOverlayAndColorProvider && bestiaryBackgroundOverlayAndColorProvider.GetBackgroundOverlayImage() != null)
				{
					list2.Add(item2);
				}
			}
			list.AddRange(from x in list2.OrderBy(GetSortingValueForElement)
				select x as IBestiaryBackgroundOverlayAndColorProvider);
		}
		UIBestiaryNPCEntryPortrait element = new UIBestiaryNPCEntryPortrait(bestiaryEntry, portraitBackgroundAsset, portraitColor, list)
		{
			Left = new StyleDimension(4f, 0f),
			HAlign = 0f
		};
		uIElement.Append(element);
		if (flag && _filledStarsCount.HasValue)
		{
			UIElement element2 = CreateStarsContainer();
			uIElement.Append(element2);
		}
		return uIElement;
	}

	private float GetSortingValueForElement(IBestiaryInfoElement element)
	{
		if (element is IBestiaryBackgroundOverlayAndColorProvider spawnPoints)
		{
			return spawnPoints.DisplayPriority;
		}
		return 0f;
	}

	private UIElement CreateStarsContainer()
	{
		int array = 14;
		int num = 14;
		int point = -4;
		int i = array + point;
		int j = 5;
		int num2 = 5;
		int num3 = _filledStarsCount.Value;
		float num4 = 1f;
		int num5 = i * Math.Min(num2, j) - point;
		double num6 = (double)i * Math.Ceiling((double)j / (double)num2) - (double)point;
		UIElement uIElement = new UIPanel(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_Panel", (AssetRequestMode)1), null, 5, 21)
		{
			Width = new StyleDimension((float)num5 + num4 * 2f, 0f),
			Height = new StyleDimension((float)num6 + num4 * 2f, 0f),
			BackgroundColor = Color.Gray * 0f,
			BorderColor = Color.Transparent,
			Left = new StyleDimension(10f, 0f),
			Top = new StyleDimension(6f, 0f),
			VAlign = 0f
		};
		uIElement.SetPadding(0f);
		for (int num7 = j - 1; num7 >= 0; num7--)
		{
			string text = "Images/UI/Bestiary/Icon_Rank_Light";
			if (num7 >= num3)
			{
				text = "Images/UI/Bestiary/Icon_Rank_Dim";
			}
			UIImage element = new UIImage(Main.Assets.Request<Texture2D>(text, (AssetRequestMode)1))
			{
				Left = new StyleDimension((float)(i * (num7 % num2)) - (float)num5 * 0.5f + (float)array * 0.5f, 0f),
				Top = new StyleDimension((float)(i * (num7 / num2)) - (float)num6 * 0.5f + (float)num * 0.5f, 0f),
				HAlign = 0.5f,
				VAlign = 0.5f
			};
			uIElement.Append(element);
		}
		return uIElement;
	}
}
