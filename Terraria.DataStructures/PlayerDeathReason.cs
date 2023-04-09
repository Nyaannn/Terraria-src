using System.IO;
using Terraria.Localization;

namespace Terraria.DataStructures;

public class PlayerDeathReason
{
	private int _sourcePlayerIndex = -1;

	private int _sourceNPCIndex = -1;

	private int _sourceProjectileLocalIndex = -1;

	private int _sourceOtherIndex = -1;

	private int _sourceProjectileType;

	private int _sourceItemType;

	private int _sourceItemPrefix;

	private string _sourceCustomReason;

	public int? SourceProjectileType
	{
		get
		{
			if (_sourceProjectileLocalIndex == -1)
			{
				return null;
			}
			return _sourceProjectileType;
		}
	}

	public bool TryGetCausingEntity(out Entity entity)
	{
		entity = null;
		if (Main.npc.IndexInRange(_sourceNPCIndex))
		{
			entity = Main.npc[_sourceNPCIndex];
			return true;
		}
		if (Main.projectile.IndexInRange(_sourceProjectileLocalIndex))
		{
			entity = Main.projectile[_sourceProjectileLocalIndex];
			return true;
		}
		if (Main.player.IndexInRange(_sourcePlayerIndex))
		{
			entity = Main.player[_sourcePlayerIndex];
			return true;
		}
		return false;
	}

	public static PlayerDeathReason LegacyEmpty()
	{
		return new PlayerDeathReason
		{
			_sourceOtherIndex = 254
		};
	}

	public static PlayerDeathReason LegacyDefault()
	{
		return new PlayerDeathReason
		{
			_sourceOtherIndex = 255
		};
	}

	public static PlayerDeathReason ByNPC(int index)
	{
		return new PlayerDeathReason
		{
			_sourceNPCIndex = index
		};
	}

	public static PlayerDeathReason ByCustomReason(string reasonInEnglish)
	{
		return new PlayerDeathReason
		{
			_sourceCustomReason = reasonInEnglish
		};
	}

	public static PlayerDeathReason ByPlayer(int index)
	{
		return new PlayerDeathReason
		{
			_sourcePlayerIndex = index,
			_sourceItemType = Main.player[index].inventory[Main.player[index].selectedItem].type,
			_sourceItemPrefix = Main.player[index].inventory[Main.player[index].selectedItem].prefix
		};
	}

	public static PlayerDeathReason ByOther(int type)
	{
		return new PlayerDeathReason
		{
			_sourceOtherIndex = type
		};
	}

	public static PlayerDeathReason ByProjectile(int playerIndex, int projectileIndex)
	{
		PlayerDeathReason num529 = new PlayerDeathReason
		{
			_sourcePlayerIndex = playerIndex,
			_sourceProjectileLocalIndex = projectileIndex,
			_sourceProjectileType = Main.projectile[projectileIndex].type
		};
		if (playerIndex >= 0 && playerIndex <= 255)
		{
			num529._sourceItemType = Main.player[playerIndex].inventory[Main.player[playerIndex].selectedItem].type;
			num529._sourceItemPrefix = Main.player[playerIndex].inventory[Main.player[playerIndex].selectedItem].prefix;
		}
		return num529;
	}

	public NetworkText GetDeathText(string deadPlayerName)
	{
		if (_sourceCustomReason != null)
		{
			return NetworkText.FromLiteral(_sourceCustomReason);
		}
		return Lang.CreateDeathMessage(deadPlayerName, _sourcePlayerIndex, _sourceNPCIndex, _sourceProjectileLocalIndex, _sourceOtherIndex, _sourceProjectileType, _sourceItemType);
	}

	public void WriteSelfTo(BinaryWriter writer)
	{
		BitsByte num510 = (byte)0;
		num510[0] = _sourcePlayerIndex != -1;
		num510[1] = _sourceNPCIndex != -1;
		num510[2] = _sourceProjectileLocalIndex != -1;
		num510[3] = _sourceOtherIndex != -1;
		num510[4] = _sourceProjectileType != 0;
		num510[5] = _sourceItemType != 0;
		num510[6] = _sourceItemPrefix != 0;
		num510[7] = _sourceCustomReason != null;
		writer.Write(num510);
		if (num510[0])
		{
			writer.Write((short)_sourcePlayerIndex);
		}
		if (num510[1])
		{
			writer.Write((short)_sourceNPCIndex);
		}
		if (num510[2])
		{
			writer.Write((short)_sourceProjectileLocalIndex);
		}
		if (num510[3])
		{
			writer.Write((byte)_sourceOtherIndex);
		}
		if (num510[4])
		{
			writer.Write((short)_sourceProjectileType);
		}
		if (num510[5])
		{
			writer.Write((short)_sourceItemType);
		}
		if (num510[6])
		{
			writer.Write((byte)_sourceItemPrefix);
		}
		if (num510[7])
		{
			writer.Write(_sourceCustomReason);
		}
	}

	public static PlayerDeathReason FromReader(BinaryReader reader)
	{
		PlayerDeathReason num501 = new PlayerDeathReason();
		BitsByte num502 = reader.ReadByte();
		if (num502[0])
		{
			num501._sourcePlayerIndex = reader.ReadInt16();
		}
		if (num502[1])
		{
			num501._sourceNPCIndex = reader.ReadInt16();
		}
		if (num502[2])
		{
			num501._sourceProjectileLocalIndex = reader.ReadInt16();
		}
		if (num502[3])
		{
			num501._sourceOtherIndex = reader.ReadByte();
		}
		if (num502[4])
		{
			num501._sourceProjectileType = reader.ReadInt16();
		}
		if (num502[5])
		{
			num501._sourceItemType = reader.ReadInt16();
		}
		if (num502[6])
		{
			num501._sourceItemPrefix = reader.ReadByte();
		}
		if (num502[7])
		{
			num501._sourceCustomReason = reader.ReadString();
		}
		return num501;
	}
}
