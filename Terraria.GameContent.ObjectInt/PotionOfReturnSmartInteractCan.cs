using Microsoft.Xna.Framework;

namespace Terraria.GameContent.ObjectInteractions;

public class PotionOfReturnSmartInteractCandidateProvider : ISmartInteractCandidateProvider
{
	private class ReusableCandidate : ISmartInteractCandidate
	{
		public float DistanceFromCursor { get; private set; }

		public void WinCandidacy()
		{
			Main.SmartInteractPotionOfReturn = true;
			Main.SmartInteractShowingGenuine = true;
		}

		public void Reuse(float distanceFromCursor)
		{
			DistanceFromCursor = distanceFromCursor;
		}
	}

	private ReusableCandidate _candidate = new ReusableCandidate();

	public void ClearSelfAndPrepareForCheck()
	{
		Main.SmartInteractPotionOfReturn = false;
	}

	public bool ProvideCandidate(SmartInteractScanSettings settings, out ISmartInteractCandidate candidate)
	{
		candidate = null;
		if (!PotionOfReturnHelper.TryGetGateHitbox(settings.player, out var uIElement))
		{
			return false;
		}
		Vector2 vector = uIElement.ClosestPointInRect(settings.mousevec);
		float element = vector.Distance(settings.mousevec);
		Point element2 = vector.ToTileCoordinates();
		if (element2.X < settings.LX || element2.X > settings.HX || element2.Y < settings.LY || element2.Y > settings.HY)
		{
			return false;
		}
		_candidate.Reuse(element);
		candidate = _candidate;
		return true;
	}
}
