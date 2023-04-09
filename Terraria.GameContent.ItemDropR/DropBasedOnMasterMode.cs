using System.Collections.Generic;

namespace Terraria.GameContent.ItemDropRules;

public class DropBasedOnMasterMode : IItemDropRule, INestedItemDropRule
{
	public IItemDropRule ruleForDefault;

	public IItemDropRule ruleForMasterMode;

	public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

	public DropBasedOnMasterMode(IItemDropRule ruleForDefault, IItemDropRule ruleForMasterMode)
	{
		this.ruleForDefault = ruleForDefault;
		this.ruleForMasterMode = ruleForMasterMode;
		ChainedRules = new List<IItemDropRuleChainAttempt>();
	}

	public bool CanDrop(DropAttemptInfo info)
	{
		if (info.IsMasterMode)
		{
			return ruleForMasterMode.CanDrop(info);
		}
		return ruleForDefault.CanDrop(info);
	}

	public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
	{
		ItemDropAttemptResult result = default(ItemDropAttemptResult);
		result.State = ItemDropAttemptResultState.DidNotRunCode;
		return result;
	}

	public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info, ItemDropRuleResolveAction resolveAction)
	{
		if (info.IsMasterMode)
		{
			return resolveAction(ruleForMasterMode, info);
		}
		return resolveAction(ruleForDefault, info);
	}

	public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
	{
		DropRateInfoChainFeed value = ratesInfo.With(1f);
		value.AddCondition(new Conditions.IsMasterMode());
		ruleForMasterMode.ReportDroprates(drops, value);
		DropRateInfoChainFeed barIconFrame = ratesInfo.With(1f);
		barIconFrame.AddCondition(new Conditions.NotMasterMode());
		ruleForDefault.ReportDroprates(drops, barIconFrame);
		Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
	}
}
