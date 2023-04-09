using System.Collections.Generic;

namespace Terraria.GameContent;

public class HairstyleUnlocksHelper
{
	public List<int> AvailableHairstyles = new List<int>();

	private bool _defeatedMartians;

	private bool _defeatedMoonlord;

	private bool _defeatedPlantera;

	private bool _isAtStylist;

	private bool _isAtCharacterCreation;

	public void UpdateUnlocks()
	{
		if (ListWarrantsRemake())
		{
			RebuildList();
		}
	}

	private bool ListWarrantsRemake()
	{
		bool flag = NPC.downedMartians && !Main.gameMenu;
		bool flag2 = NPC.downedMoonlord && !Main.gameMenu;
		bool flag3 = NPC.downedPlantBoss && !Main.gameMenu;
		bool flag4 = Main.hairWindow && !Main.gameMenu;
		bool gameMenu = Main.gameMenu;
		bool result = false;
		if (_defeatedMartians != flag || _defeatedMoonlord != flag2 || _defeatedPlantera != flag3 || _isAtStylist != flag4 || _isAtCharacterCreation != gameMenu)
		{
			result = true;
		}
		_defeatedMartians = flag;
		_defeatedMoonlord = flag2;
		_defeatedPlantera = flag3;
		_isAtStylist = flag4;
		_isAtCharacterCreation = gameMenu;
		return result;
	}

	private void RebuildList()
	{
		List<int> uIBestiaryEntryButton = AvailableHairstyles;
		uIBestiaryEntryButton.Clear();
		if (_isAtCharacterCreation || _isAtStylist)
		{
			for (int i = 0; i < 51; i++)
			{
				uIBestiaryEntryButton.Add(i);
			}
			uIBestiaryEntryButton.Add(136);
			uIBestiaryEntryButton.Add(137);
			uIBestiaryEntryButton.Add(138);
			uIBestiaryEntryButton.Add(139);
			uIBestiaryEntryButton.Add(140);
			uIBestiaryEntryButton.Add(141);
			uIBestiaryEntryButton.Add(142);
			uIBestiaryEntryButton.Add(143);
			uIBestiaryEntryButton.Add(144);
			uIBestiaryEntryButton.Add(147);
			uIBestiaryEntryButton.Add(148);
			uIBestiaryEntryButton.Add(149);
			uIBestiaryEntryButton.Add(150);
			uIBestiaryEntryButton.Add(151);
			uIBestiaryEntryButton.Add(154);
			uIBestiaryEntryButton.Add(155);
			uIBestiaryEntryButton.Add(157);
			uIBestiaryEntryButton.Add(158);
			uIBestiaryEntryButton.Add(161);
		}
		for (int j = 51; j < 123; j++)
		{
			uIBestiaryEntryButton.Add(j);
		}
		uIBestiaryEntryButton.Add(134);
		uIBestiaryEntryButton.Add(135);
		uIBestiaryEntryButton.Add(146);
		uIBestiaryEntryButton.Add(152);
		uIBestiaryEntryButton.Add(153);
		uIBestiaryEntryButton.Add(156);
		uIBestiaryEntryButton.Add(159);
		uIBestiaryEntryButton.Add(160);
		if (_defeatedPlantera)
		{
			uIBestiaryEntryButton.Add(162);
			uIBestiaryEntryButton.Add(164);
			uIBestiaryEntryButton.Add(163);
			uIBestiaryEntryButton.Add(145);
		}
		if (_defeatedMartians)
		{
			uIBestiaryEntryButton.AddRange(new int[10] { 132, 131, 130, 129, 128, 127, 126, 125, 124, 123 });
		}
		if (_defeatedMartians && _defeatedMoonlord)
		{
			uIBestiaryEntryButton.Add(133);
		}
	}
}
