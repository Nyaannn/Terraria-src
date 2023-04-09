using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terraria.GameContent.UI;

public class CustomCurrencySystem
{
	protected Dictionary<int, int> _valuePerUnit = new Dictionary<int, int>();

	private long _currencyCap = 999999999L;

	public long CurrencyCap => _currencyCap;

	public void Include(int coin, int howMuchIsItWorth)
	{
		_valuePerUnit[coin] = howMuchIsItWorth;
	}

	public void SetCurrencyCap(long cap)
	{
		_currencyCap = cap;
	}

	public virtual long CountCurrency(out bool overFlowing, Item[] inv, params int[] ignoreSlots)
	{
		List<int> unscaledPosition = new List<int>(ignoreSlots);
		long zero = 0L;
		for (int num = 0; num < inv.Length; num++)
		{
			if (!unscaledPosition.Contains(num))
			{
				if (_valuePerUnit.TryGetValue(inv[num].type, out var num2))
				{
					zero += num2 * inv[num].stack;
				}
				if (zero >= CurrencyCap)
				{
					overFlowing = true;
					return CurrencyCap;
				}
			}
		}
		overFlowing = false;
		return zero;
	}

	public virtual long CombineStacks(out bool overFlowing, params long[] coinCounts)
	{
		long num = 0L;
		foreach (long num3 in coinCounts)
		{
			num += num3;
			if (num >= CurrencyCap)
			{
				overFlowing = true;
				return CurrencyCap;
			}
		}
		overFlowing = false;
		return num;
	}

	public virtual bool TryPurchasing(long price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3, List<Point> slotEmptyBank4)
	{
		long windTimeLeft = price;
		Dictionary<Point, Item> direction = new Dictionary<Point, Item>();
		bool result = true;
		while (windTimeLeft > 0)
		{
			long num = 1000000L;
			for (int i = 0; i < 4; i++)
			{
				if (windTimeLeft >= num)
				{
					foreach (Point slotCoin in slotCoins)
					{
						if (inv[slotCoin.X][slotCoin.Y].type == 74 - i)
						{
							long num2 = windTimeLeft / num;
							direction[slotCoin] = inv[slotCoin.X][slotCoin.Y].Clone();
							if (num2 < inv[slotCoin.X][slotCoin.Y].stack)
							{
								inv[slotCoin.X][slotCoin.Y].stack -= (int)num2;
							}
							else
							{
								inv[slotCoin.X][slotCoin.Y].SetDefaults();
								slotsEmpty.Add(slotCoin);
							}
							windTimeLeft -= num * (direction[slotCoin].stack - inv[slotCoin.X][slotCoin.Y].stack);
						}
					}
				}
				num /= 100;
			}
			if (windTimeLeft <= 0)
			{
				continue;
			}
			if (slotsEmpty.Count > 0)
			{
				slotsEmpty.Sort(DelegateMethods.CompareYReverse);
				Point item = new Point(-1, -1);
				for (int j = 0; j < inv.Count; j++)
				{
					num = 10000L;
					for (int k = 0; k < 3; k++)
					{
						if (windTimeLeft >= num)
						{
							foreach (Point slotCoin2 in slotCoins)
							{
								if (slotCoin2.X == j && inv[slotCoin2.X][slotCoin2.Y].type == 74 - k && inv[slotCoin2.X][slotCoin2.Y].stack >= 1)
								{
									List<Point> list = slotsEmpty;
									if (j == 1 && slotEmptyBank.Count > 0)
									{
										list = slotEmptyBank;
									}
									if (j == 2 && slotEmptyBank2.Count > 0)
									{
										list = slotEmptyBank2;
									}
									if (j == 3 && slotEmptyBank3.Count > 0)
									{
										list = slotEmptyBank3;
									}
									if (j == 4 && slotEmptyBank4.Count > 0)
									{
										list = slotEmptyBank4;
									}
									if (--inv[slotCoin2.X][slotCoin2.Y].stack <= 0)
									{
										inv[slotCoin2.X][slotCoin2.Y].SetDefaults();
										list.Add(slotCoin2);
									}
									direction[list[0]] = inv[list[0].X][list[0].Y].Clone();
									inv[list[0].X][list[0].Y].SetDefaults(73 - k);
									inv[list[0].X][list[0].Y].stack = 100;
									item = list[0];
									list.RemoveAt(0);
									break;
								}
							}
						}
						if (item.X != -1 || item.Y != -1)
						{
							break;
						}
						num /= 100;
					}
					for (int l = 0; l < 2; l++)
					{
						if (item.X != -1 || item.Y != -1)
						{
							continue;
						}
						foreach (Point slotCoin3 in slotCoins)
						{
							if (slotCoin3.X == j && inv[slotCoin3.X][slotCoin3.Y].type == 73 + l && inv[slotCoin3.X][slotCoin3.Y].stack >= 1)
							{
								List<Point> list2 = slotsEmpty;
								if (j == 1 && slotEmptyBank.Count > 0)
								{
									list2 = slotEmptyBank;
								}
								if (j == 2 && slotEmptyBank2.Count > 0)
								{
									list2 = slotEmptyBank2;
								}
								if (j == 3 && slotEmptyBank3.Count > 0)
								{
									list2 = slotEmptyBank3;
								}
								if (j == 4 && slotEmptyBank4.Count > 0)
								{
									list2 = slotEmptyBank4;
								}
								if (--inv[slotCoin3.X][slotCoin3.Y].stack <= 0)
								{
									inv[slotCoin3.X][slotCoin3.Y].SetDefaults();
									list2.Add(slotCoin3);
								}
								direction[list2[0]] = inv[list2[0].X][list2[0].Y].Clone();
								inv[list2[0].X][list2[0].Y].SetDefaults(72 + l);
								inv[list2[0].X][list2[0].Y].stack = 100;
								item = list2[0];
								list2.RemoveAt(0);
								break;
							}
						}
					}
					if (item.X != -1 && item.Y != -1)
					{
						slotCoins.Add(item);
						break;
					}
				}
				slotsEmpty.Sort(DelegateMethods.CompareYReverse);
				slotEmptyBank.Sort(DelegateMethods.CompareYReverse);
				slotEmptyBank2.Sort(DelegateMethods.CompareYReverse);
				slotEmptyBank3.Sort(DelegateMethods.CompareYReverse);
				slotEmptyBank4.Sort(DelegateMethods.CompareYReverse);
				continue;
			}
			foreach (KeyValuePair<Point, Item> item2 in direction)
			{
				inv[item2.Key.X][item2.Key.Y] = item2.Value.Clone();
			}
			result = false;
			break;
		}
		return result;
	}

	public virtual bool Accepts(Item item)
	{
		return _valuePerUnit.ContainsKey(item.type);
	}

	public virtual void DrawSavingsMoney(SpriteBatch sb, string text, float shopx, float shopy, long totalCoins, bool horizontal = false)
	{
	}

	public virtual void GetPriceText(string[] lines, ref int currentLine, long price)
	{
	}

	protected int SortByHighest(Tuple<int, int> valueA, Tuple<int, int> valueB)
	{
		if (valueA.Item2 > valueB.Item2)
		{
			return -1;
		}
		if (valueA.Item2 == valueB.Item2)
		{
			return 0;
		}
		return -1;
	}

	protected List<Tuple<Point, Item>> ItemCacheCreate(List<Item[]> inventories)
	{
		List<Tuple<Point, Item>> unscaledPosition = new List<Tuple<Point, Item>>();
		for (int zero = 0; zero < inventories.Count; zero++)
		{
			for (int num = 0; num < inventories[zero].Length; num++)
			{
				Item num2 = inventories[zero][num];
				unscaledPosition.Add(new Tuple<Point, Item>(new Point(zero, num), num2.DeepClone()));
			}
		}
		return unscaledPosition;
	}

	protected void ItemCacheRestore(List<Tuple<Point, Item>> cache, List<Item[]> inventories)
	{
		foreach (Tuple<Point, Item> num2 in cache)
		{
			inventories[num2.Item1.X][num2.Item1.Y] = num2.Item2;
		}
	}

	public virtual void GetItemExpectedPrice(Item item, out long calcForSelling, out long calcForBuying)
	{
		int unscaledPosition = item.GetStoreValue();
		calcForSelling = unscaledPosition;
		calcForBuying = unscaledPosition;
	}
}
