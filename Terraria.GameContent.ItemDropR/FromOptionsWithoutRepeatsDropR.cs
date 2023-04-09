using System.Collections.Generic;

namespace Terraria.GameContent.ItemDropRules;

public class FromOptionsWithoutRepeatsDropRule : IItemDropRule
{
	public int[] dropIds;

	public int dropCount;

	private List<int> _temporaryAvailableItems = new List<int>();

	public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

	public FromOptionsWithoutRepeatsDropRule(int dropCount, params int[] options)
	{
		this.dropCount = dropCount;
		dropIds = options;
		ChainedRules = new List<IItemDropRuleChainAttempt>();
	}

	public bool CanDrop(DropAttemptInfo info)
	{
		return true;
	}

	public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
	{
		_temporaryAvailableItems.Clear();
		_temporaryAvailableItems.AddRange(dropIds);
		for (int num = 0; num < dropCount; num++)
		{
			if (_temporaryAvailableItems.Count <= 0)
			{
				break;
			}
			int tEFoodPlatter = info.rng.Next(_temporaryAvailableItems.Count);
			CommonCode.DropItemFromNPC(info.npc, _temporaryAvailableItems[tEFoodPlatter], 1);
			_temporaryAvailableItems.RemoveAt(tEFoodPlatter);
		}
		ItemDropAttemptResult num2 = default(ItemDropAttemptResult);
		num2.State = ItemDropAttemptResultState.Success;
		return num2;
	}

	public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
	{
		float num = ratesInfo.parentDroprateChance;
		int num2 = dropIds.Length;
		float num3 = 1f;
		int num4 = 0;
		while (num4 < dropCount && num2 > 0)
		{
			num3 *= (float)(num2 - 1) / (float)num2;
			num4++;
			num2--;
		}
		float dropRate = (1f - num3) * num;
		for (int i = 0; i < dropIds.Length; i++)
		{
			drops.Add(new DropRateInfo(dropIds[i], 1, 1, dropRate, ratesInfo.conditions));
		}
		Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
	}
}
