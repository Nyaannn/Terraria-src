using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;

namespace Terraria.DataStructures;

public class PlayerDrawHelper
{
	public enum ShaderConfiguration
	{
		ArmorShader,
		HairShader,
		TileShader,
		TilePaintID
	}

	public static Color DISPLAY_DOLL_DEFAULT_SKIN_COLOR = new Color(163, 121, 92);

	public static int PackShader(int localShaderIndex, ShaderConfiguration shaderType)
	{
		return localShaderIndex + (int)shaderType * 1000;
	}

	public static void UnpackShader(int packedShaderIndex, out int localShaderIndex, out ShaderConfiguration shaderType)
	{
		shaderType = (ShaderConfiguration)(packedShaderIndex / 1000);
		localShaderIndex = packedShaderIndex % 1000;
	}

	public static void SetShaderForData(Player player, int cHead, ref DrawData cdd)
	{
		UnpackShader(cdd.shader, out var num444, out var num445);
		switch (num445)
		{
		case ShaderConfiguration.ArmorShader:
			GameShaders.Hair.Apply(0, player, cdd);
			GameShaders.Armor.Apply(num444, player, cdd);
			break;
		case ShaderConfiguration.HairShader:
			if (player.head == 0)
			{
				GameShaders.Hair.Apply(0, player, cdd);
				GameShaders.Armor.Apply(cHead, player, cdd);
			}
			else
			{
				GameShaders.Armor.Apply(0, player, cdd);
				GameShaders.Hair.Apply((short)num444, player, cdd);
			}
			break;
		case ShaderConfiguration.TileShader:
			Main.tileShader.CurrentTechnique.Passes[num444].Apply();
			break;
		case ShaderConfiguration.TilePaintID:
		{
			int num446 = Main.ConvertPaintIdToTileShaderIndex(num444, isUsedForPaintingGrass: false, useWallShaderHacks: false);
			Main.tileShader.CurrentTechnique.Passes[num446].Apply();
			break;
		}
		}
	}
}
