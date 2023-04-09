using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI;

public class CustomCurrencySingleCoin : CustomCurrencySystem
{
	public float CurrencyDrawScale = 0.8f;

	public string CurrencyTextKey = "Currency.DefenderMedals";

	public Color CurrencyTextColor = new Color(240, 100, 120);

	public CustomCurrencySingleCoin(int coinItemID, long currencyCap)
	{
		Include(coinItemID, 1);
		SetCurrencyCap(currencyCap);
	}

	public override bool TryPurchasing(long price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3, List<Point> slotEmptyBank4)
	{
		List<Tuple<Point, Item>> windCycle = ItemCacheCreate(inv);
		long value = price;
		for (int num = 0; num < slotCoins.Count; num++)
		{
			Point type = slotCoins[num];
			long texture2D = value;
			if (inv[type.X][type.Y].stack < texture2D)
			{
				texture2D = inv[type.X][type.Y].stack;
			}
			value -= texture2D;
			inv[type.X][type.Y].stack -= (int)texture2D;
			if (inv[type.X][type.Y].stack == 0)
			{
				switch (type.X)
				{
				case 0:
					slotsEmpty.Add(type);
					break;
				case 1:
					slotEmptyBank.Add(type);
					break;
				case 2:
					slotEmptyBank2.Add(type);
					break;
				case 3:
					slotEmptyBank3.Add(type);
					break;
				case 4:
					slotEmptyBank4.Add(type);
					break;
				}
				slotCoins.Remove(type);
				num--;
			}
			if (value == 0L)
			{
				break;
			}
		}
		if (value != 0L)
		{
			ItemCacheRestore(windCycle, inv);
			return false;
		}
		return true;
	}

	public override void DrawSavingsMoney(SpriteBatch sb, string text, float shopx, float shopy, long totalCoins, bool horizontal = false)
	{
		int num = _valuePerUnit.Keys.ElementAt(0);
		Main.instance.LoadItem(num);
		Texture2D num2 = TextureAssets.Item[num].get_Value();
		if (horizontal)
		{
			_ = 99;
			Vector2 value = new Vector2(shopx + ChatManager.GetStringSize(FontAssets.MouseText.get_Value(), text, Vector2.One).X + 45f, shopy + 50f);
			sb.Draw(num2, value, null, Color.White, 0f, num2.Size() / 2f, CurrencyDrawScale, SpriteEffects.None, 0f);
			Utils.DrawBorderStringFourWay(sb, FontAssets.ItemStack.get_Value(), totalCoins.ToString(), value.X - 11f, value.Y, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
		}
		else
		{
			int num3 = ((totalCoins > 99) ? (-6) : 0);
			sb.Draw(num2, new Vector2(shopx + 11f, shopy + 75f), null, Color.White, 0f, num2.Size() / 2f, CurrencyDrawScale, SpriteEffects.None, 0f);
			Utils.DrawBorderStringFourWay(sb, FontAssets.ItemStack.get_Value(), totalCoins.ToString(), shopx + (float)num3, shopy + 75f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
		}
	}

	public override void GetPriceText(string[] lines, ref int currentLine, long price)
	{
		Color num = CurrencyTextColor * ((float)(int)Main.mouseTextColor / 255f);
		lines[currentLine++] = $"[c/{num.R:X2}{num.G:X2}{num.B:X2}:{Lang.tip[50].Value} {price} {Language.GetTextValue(CurrencyTextKey).ToLower()}]";
	}
}
