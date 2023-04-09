using System.Collections.Generic;

namespace Terraria.GameContent.ItemDropRules;

public class OneFromRulesRule : IItemDropRule, INestedItemDropRule
{
	public IItemDropRule[] options;

	public int chanceDenominator;

	public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

	public OneFromRulesRule(int chanceDenominator, params IItemDropRule[] options)
	{
		this.chanceDenominator = chanceDenominator;
		this.options = options;
		ChainedRules = new List<IItemDropRuleChainAttempt>();
	}

	public bool CanDrop(DropAttemptInfo info)
	{
		return true;
	}

	public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
	{
		ItemDropAttemptResult result = default(ItemDropAttemptResult);
		result.State = ItemDropAttemptResultState.DidNotRunCode;
		return result;
	}

	public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info, ItemDropRuleResolveAction resolveAction)
	{
		int number = -1;
		ItemDropAttemptResult result;
		if (info.rng.Next(chanceDenominator) == 0)
		{
			number = info.rng.Next(options.Length);
			resolveAction(options[number], info);
			result = default(ItemDropAttemptResult);
			result.State = ItemDropAttemptResultState.Success;
			return result;
		}
		result = default(ItemDropAttemptResult);
		result.State = ItemDropAttemptResultState.FailedRandomRoll;
		return result;
	}

	public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
	{
		float tEHatRack = 1f / (float)chanceDenominator;
		float num = tEHatRack * ratesInfo.parentDroprateChance;
		float multiplier = 1f / (float)options.Length * num;
		for (int i = 0; i < options.Length; i++)
		{
			options[i].ReportDroprates(drops, ratesInfo.With(multiplier));
		}
		Chains.ReportDroprates(ChainedRules, tEHatRack, drops, ratesInfo);
	}
}
