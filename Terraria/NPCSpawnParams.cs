using Terraria.DataStructures;

namespace Terraria;

public struct NPCSpawnParams
{
	public float? sizeScaleOverride;

	public int? playerCountForMultiplayerDifficultyOverride;

	public GameModeData gameModeData;

	public float? strengthMultiplierOverride;

	public NPCSpawnParams WithScale(float scaleOverride)
	{
		NPCSpawnParams num = default(NPCSpawnParams);
		num.sizeScaleOverride = scaleOverride;
		num.playerCountForMultiplayerDifficultyOverride = playerCountForMultiplayerDifficultyOverride;
		num.gameModeData = gameModeData;
		num.strengthMultiplierOverride = strengthMultiplierOverride;
		return num;
	}
}
