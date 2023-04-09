using Terraria.ID;
using Terraria.UI;

namespace Terraria.GameContent.Bestiary;

public class CommonEnemyUICollectionInfoProvider : IBestiaryUICollectionInfoProvider
{
	private string _persistentIdentifierToCheck;

	private bool _quickUnlock;

	private int _killCountNeededToFullyUnlock;

	public CommonEnemyUICollectionInfoProvider(string persistentId, bool quickUnlock)
	{
		_persistentIdentifierToCheck = persistentId;
		_quickUnlock = quickUnlock;
		_killCountNeededToFullyUnlock = GetKillCountNeeded(persistentId);
	}

	public static int GetKillCountNeeded(string persistentId)
	{
		int i = ItemID.Sets.DefaultKillsForBannerNeeded;
		if (!ContentSamples.NpcNetIdsByPersistentIds.TryGetValue(persistentId, out var projectile))
		{
			return i;
		}
		if (!ContentSamples.NpcsByNetId.TryGetValue(projectile, out var shotVector))
		{
			return i;
		}
		int num = Item.BannerToItem(Item.NPCtoBanner(shotVector.BannerID()));
		return ItemID.Sets.KillsToBanner[num];
	}

	public BestiaryUICollectionInfo GetEntryUICollectionInfo()
	{
		int killCount = Main.BestiaryTracker.Kills.GetKillCount(_persistentIdentifierToCheck);
		BestiaryEntryUnlockState unlockStateByKillCount = GetUnlockStateByKillCount(killCount, _quickUnlock);
		BestiaryUICollectionInfo result = default(BestiaryUICollectionInfo);
		result.UnlockState = unlockStateByKillCount;
		return result;
	}

	public BestiaryEntryUnlockState GetUnlockStateByKillCount(int killCount, bool quickUnlock)
	{
		int vector = _killCountNeededToFullyUnlock;
		return GetUnlockStateByKillCount(killCount, quickUnlock, vector);
	}

	public static BestiaryEntryUnlockState GetUnlockStateByKillCount(int killCount, bool quickUnlock, int fullKillCountNeeded)
	{
		BestiaryEntryUnlockState vector = BestiaryEntryUnlockState.NotKnownAtAll_0;
		int num = fullKillCountNeeded / 2;
		int num2 = fullKillCountNeeded / 5;
		if (quickUnlock && killCount > 0)
		{
			return BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
		}
		if (killCount >= fullKillCountNeeded)
		{
			return BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
		}
		if (killCount >= num)
		{
			return BestiaryEntryUnlockState.CanShowDropsWithoutDropRates_3;
		}
		if (killCount >= num2)
		{
			return BestiaryEntryUnlockState.CanShowStats_2;
		}
		if (killCount >= 1)
		{
			return BestiaryEntryUnlockState.CanShowPortraitOnly_1;
		}
		return BestiaryEntryUnlockState.NotKnownAtAll_0;
	}

	public UIElement ProvideUIElement(BestiaryUICollectionInfo info)
	{
		return null;
	}
}
