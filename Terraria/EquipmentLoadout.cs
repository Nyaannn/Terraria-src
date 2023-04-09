using System.IO;
using Terraria.DataStructures;

namespace Terraria;

public class EquipmentLoadout : IFixLoadedData
{
	public Item[] Armor;

	public Item[] Dye;

	public bool[] Hide;

	public EquipmentLoadout()
	{
		Armor = CreateItemArray(20);
		Dye = CreateItemArray(10);
		Hide = new bool[10];
	}

	private Item[] CreateItemArray(int length)
	{
		Item[] x = new Item[length];
		for (int num = 0; num < length; num++)
		{
			x[num] = new Item();
		}
		return x;
	}

	public void Serialize(BinaryWriter writer)
	{
		ItemSerializationContext context = ItemSerializationContext.SavingAndLoading;
		for (int i = 0; i < Armor.Length; i++)
		{
			Armor[i].Serialize(writer, context);
		}
		for (int j = 0; j < Dye.Length; j++)
		{
			Dye[j].Serialize(writer, context);
		}
		for (int k = 0; k < Hide.Length; k++)
		{
			writer.Write(Hide[k]);
		}
	}

	public void Deserialize(BinaryReader reader, int gameVersion)
	{
		ItemSerializationContext point = ItemSerializationContext.SavingAndLoading;
		for (int value = 0; value < Armor.Length; value++)
		{
			Armor[value].DeserializeFrom(reader, point);
		}
		for (int num = 0; num < Dye.Length; num++)
		{
			Dye[num].DeserializeFrom(reader, point);
		}
		for (int value2 = 0; value2 < Hide.Length; value2++)
		{
			Hide[value2] = reader.ReadBoolean();
		}
	}

	public void Swap(Player player)
	{
		Item[] expertMode = player.armor;
		for (int flag2 = 0; flag2 < expertMode.Length; flag2++)
		{
			Utils.Swap(ref expertMode[flag2], ref Armor[flag2]);
		}
		Item[] num = player.dye;
		for (int flag3 = 0; flag3 < num.Length; flag3++)
		{
			Utils.Swap(ref num[flag3], ref Dye[flag3]);
		}
		bool[] flag = player.hideVisibleAccessory;
		for (int flag4 = 0; flag4 < flag.Length; flag4++)
		{
			Utils.Swap(ref flag[flag4], ref Hide[flag4]);
		}
	}

	public void TryDroppingItems(Player player, IEntitySource source)
	{
		for (int num11 = 0; num11 < Armor.Length; num11++)
		{
			player.TryDroppingSingleItem(source, Armor[num11]);
		}
		for (int num12 = 0; num12 < Dye.Length; num12++)
		{
			player.TryDroppingSingleItem(source, Dye[num12]);
		}
	}

	public void FixLoadedData()
	{
		for (int targetData = 0; targetData < Armor.Length; targetData++)
		{
			Armor[targetData].FixAgainstExploit();
		}
		for (int flag = 0; flag < Dye.Length; flag++)
		{
			Dye[flag].FixAgainstExploit();
		}
		Player.FixLoadedData_EliminiateDuplicateAccessories(Armor);
	}
}
