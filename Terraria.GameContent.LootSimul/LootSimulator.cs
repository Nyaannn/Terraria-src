using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ReLogic.OS;
using Terraria.ID;

namespace Terraria.GameContent.LootSimulation;

public class LootSimulator
{
	private List<ISimulationConditionSetter> _neededTestConditions = new List<ISimulationConditionSetter>();

	private int[] _excludedItemIds = new int[0];

	public LootSimulator()
	{
		FillDesiredTestConditions();
		FillItemExclusions();
	}

	private void FillItemExclusions()
	{
		List<int> innerDimensions = new List<int>();
		innerDimensions.AddRange(from tuple in ItemID.Sets.IsAPickup.Select((bool state, int index) => new { index, state })
			where tuple.state
			select tuple.index);
		innerDimensions.AddRange(from tuple in ItemID.Sets.CommonCoin.Select((bool state, int index) => new { index, state })
			where tuple.state
			select tuple.index);
		_excludedItemIds = innerDimensions.ToArray();
	}

	private void FillDesiredTestConditions()
	{
		_neededTestConditions.AddRange(new List<ISimulationConditionSetter>
		{
			SimulationConditionSetters.MidDay,
			SimulationConditionSetters.MidNight,
			SimulationConditionSetters.HardMode,
			SimulationConditionSetters.ExpertMode,
			SimulationConditionSetters.ExpertAndHardMode,
			SimulationConditionSetters.WindyExpertHardmodeEndgameBloodMoonNight,
			SimulationConditionSetters.WindyExpertHardmodeEndgameEclipseMorning,
			SimulationConditionSetters.SlimeStaffTest,
			SimulationConditionSetters.LuckyCoinTest
		});
	}

	public void Run()
	{
		int timesMultiplier = 10000;
		SetCleanSlateWorldConditions();
		string text = "";
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int i = -65; i < NPCID.Count; i++)
		{
			if (TryGettingLootFor(i, timesMultiplier, out var outputText))
			{
				text = text + outputText + "\n\n";
			}
		}
		stopwatch.Stop();
		text += $"\nSimulation Took {(float)stopwatch.ElapsedMilliseconds / 1000f} seconds to complete.\n";
		Platform.Get<IClipboard>().set_Value(text);
	}

	private void SetCleanSlateWorldConditions()
	{
		Main.dayTime = true;
		Main.time = 27000.0;
		Main.hardMode = false;
		Main.GameMode = 0;
		NPC.downedMechBoss1 = false;
		NPC.downedMechBoss2 = false;
		NPC.downedMechBoss3 = false;
		NPC.downedMechBossAny = false;
		NPC.downedPlantBoss = false;
		Main._shouldUseWindyDayMusic = false;
		Main._shouldUseStormMusic = false;
		Main.eclipse = false;
		Main.bloodMoon = false;
	}

	private bool TryGettingLootFor(int npcNetId, int timesMultiplier, out string outputText)
	{
		SimulatorInfo textSnippet = new SimulatorInfo();
		NPC result = new NPC();
		result.SetDefaults(npcNetId);
		textSnippet.npcVictim = result;
		LootSimulationItemCounter lootSimulationItemCounter = (textSnippet.itemCounter = new LootSimulationItemCounter());
		foreach (ISimulationConditionSetter neededTestCondition in _neededTestConditions)
		{
			neededTestCondition.Setup(textSnippet);
			int num = neededTestCondition.GetTimesToRunMultiplier(textSnippet) * timesMultiplier;
			for (int i = 0; i < num; i++)
			{
				result.NPCLoot();
			}
			lootSimulationItemCounter.IncreaseTimesAttempted(num, textSnippet.runningExpertMode);
			neededTestCondition.TearDown(textSnippet);
			SetCleanSlateWorldConditions();
		}
		lootSimulationItemCounter.Exclude(_excludedItemIds.ToArray());
		string text = lootSimulationItemCounter.PrintCollectedItems(expert: false);
		string text2 = lootSimulationItemCounter.PrintCollectedItems(expert: true);
		string name = NPCID.Search.GetName(npcNetId);
		string text3 = $"FindEntryByNPCID(NPCID.{name})";
		if (text.Length > 0)
		{
			text3 = $"{text3}\n.AddDropsNormalMode({text})";
		}
		if (text2.Length > 0)
		{
			text3 = $"{text3}\n.AddDropsExpertMode({text2})";
		}
		text3 = (outputText = text3 + ";");
		if (text.Length <= 0)
		{
			return text2.Length > 0;
		}
		return true;
	}
}
