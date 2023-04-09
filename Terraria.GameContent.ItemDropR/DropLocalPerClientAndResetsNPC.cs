namespace Terraria.GameContent.ItemDropRules;

public class DropLocalPerClientAndResetsNPCMoneyTo0 : CommonDrop
{
	public IItemDropRuleCondition condition;

	public DropLocalPerClientAndResetsNPCMoneyTo0(int itemId, int chanceDenominator, int amountDroppedMinimum, int amountDroppedMaximum, IItemDropRuleCondition optionalCondition)
		: base(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum)
	{
		condition = optionalCondition;
	}

	public override bool CanDrop(DropAttemptInfo info)
	{
		if (condition != null)
		{
			return condition.CanDrop(info);
		}
		return true;
	}

	public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
	{
		ItemDropAttemptResult i;
		if (info.rng.Next(chanceDenominator) < chanceNumerator)
		{
			CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(info.npc, itemId, info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1));
			i = default(ItemDropAttemptResult);
			i.State = ItemDropAttemptResultState.Success;
			return i;
		}
		i = default(ItemDropAttemptResult);
		i.State = ItemDropAttemptResultState.FailedRandomRoll;
		return i;
	}
}
