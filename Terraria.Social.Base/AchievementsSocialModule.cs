namespace Terraria.Social.Base;

public abstract class AchievementsSocialModule : ISocialModule
{
	public abstract void Initialize();

	public abstract void Shutdown();

	public abstract byte[] GetEncryptionKey();

	public abstract string GetSavePath();

	public abstract void UpdateIntStat(string name, int value);

	public abstract void UpdateFloatStat(string name, float value);

	public abstract void CompleteAchievement(string name);

	public abstract bool IsAchievementCompleted(string name);

	public abstract void StoreStats();
}
