using Terraria.UI;

namespace Terraria.GameContent.Bestiary;

public class CritterUICollectionInfoProvider : IBestiaryUICollectionInfoProvider
{
	private string _persistentIdentifierToCheck;

	public CritterUICollectionInfoProvider(string persistentId)
	{
		_persistentIdentifierToCheck = persistentId;
	}

	public BestiaryUICollectionInfo GetEntryUICollectionInfo()
	{
		BestiaryUICollectionInfo num = default(BestiaryUICollectionInfo);
		num.UnlockState = (Main.BestiaryTracker.Sights.GetWasNearbyBefore(_persistentIdentifierToCheck) ? BestiaryEntryUnlockState.CanShowDropsWithDropRates_4 : BestiaryEntryUnlockState.NotKnownAtAll_0);
		return num;
	}

	public UIElement ProvideUIElement(BestiaryUICollectionInfo info)
	{
		return null;
	}
}
