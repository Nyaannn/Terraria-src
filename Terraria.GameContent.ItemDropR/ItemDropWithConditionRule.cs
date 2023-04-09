using System.Collections.Generic;

namespace Terraria.GameContent.ItemDropRules;

public class ItemDropWithConditionRule : CommonDrop
{
	public IItemDropRuleCondition condition;

	public ItemDropWithConditionRule(int itemId, int chanceDenominator, int amountDroppedMinimum, int amountDroppedMaximum, IItemDropRuleCondition condition, int chanceNumerator = 1)
		: base(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
	{
		this.condition = condition;
	}

	public override bool CanDrop(DropAttemptInfo info)
	{
		return condition.CanDrop(info);
	}

	public override void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
	{
		DropRateInfoChainFeed inv = ratesInfo.With(1f);
		inv.AddCondition(condition);
		float num = (float)chanceNumerator / (float)chanceDenominator;
		float context = num * inv.parentDroprateChance;
		drops.Add(new DropRateInfo(itemId, amountDroppedMinimum, amountDroppedMaximum, context, inv.conditions));
		Chains.ReportDroprates(base.ChainedRules, num, drops, inv);
	}
}
