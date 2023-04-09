using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;

namespace Terraria.GameContent.Bestiary;

public class BestiaryEntry
{
	public IEntryIcon Icon;

	public IBestiaryUICollectionInfoProvider UIInfoProvider;

	public List<IBestiaryInfoElement> Info { get; private set; }

	public BestiaryEntry()
	{
		Info = new List<IBestiaryInfoElement>();
	}

	public static BestiaryEntry Enemy(int npcNetId)
	{
		NPC result = ContentSamples.NpcsByNetId[npcNetId];
		List<IBestiaryInfoElement> list = new List<IBestiaryInfoElement>
		{
			new NPCNetIdBestiaryInfoElement(npcNetId),
			new NamePlateInfoElement(Lang.GetNPCName(npcNetId).Key, npcNetId),
			new NPCPortraitInfoElement(ContentSamples.NpcBestiaryRarityStars[npcNetId]),
			new NPCKillCounterInfoElement(npcNetId)
		};
		list.Add(new NPCStatsReportInfoElement(npcNetId));
		if (result.rarity != 0)
		{
			list.Add(new RareSpawnBestiaryInfoElement(result.rarity));
		}
		IBestiaryUICollectionInfoProvider uIInfoProvider;
		if (result.boss || NPCID.Sets.ShouldBeCountedAsBoss[result.type])
		{
			list.Add(new BossBestiaryInfoElement());
			uIInfoProvider = new CommonEnemyUICollectionInfoProvider(result.GetBestiaryCreditId(), quickUnlock: true);
		}
		else
		{
			uIInfoProvider = new CommonEnemyUICollectionInfoProvider(result.GetBestiaryCreditId(), quickUnlock: false);
		}
		string key = Lang.GetNPCName(result.netID).Key;
		key = key.Replace("NPCName.", "");
		string text = "Bestiary_FlavorText.npc_" + key;
		if (Language.Exists(text))
		{
			list.Add(new FlavorTextBestiaryInfoElement(text));
		}
		return new BestiaryEntry
		{
			Icon = new UnlockableNPCEntryIcon(npcNetId),
			Info = list,
			UIInfoProvider = uIInfoProvider
		};
	}

	public static BestiaryEntry TownNPC(int npcNetId)
	{
		NPC num = ContentSamples.NpcsByNetId[npcNetId];
		List<IBestiaryInfoElement> num2 = new List<IBestiaryInfoElement>
		{
			new NPCNetIdBestiaryInfoElement(npcNetId),
			new NamePlateInfoElement(Lang.GetNPCName(npcNetId).Key, npcNetId),
			new NPCPortraitInfoElement(ContentSamples.NpcBestiaryRarityStars[npcNetId]),
			new NPCKillCounterInfoElement(npcNetId)
		};
		string dropRate = Lang.GetNPCName(num.netID).Key;
		dropRate = dropRate.Replace("NPCName.", "");
		string i = "Bestiary_FlavorText.npc_" + dropRate;
		if (Language.Exists(i))
		{
			num2.Add(new FlavorTextBestiaryInfoElement(i));
		}
		return new BestiaryEntry
		{
			Icon = new UnlockableNPCEntryIcon(npcNetId),
			Info = num2,
			UIInfoProvider = new TownNPCUICollectionInfoProvider(num.GetBestiaryCreditId())
		};
	}

	public static BestiaryEntry Critter(int npcNetId)
	{
		NPC nPC = ContentSamples.NpcsByNetId[npcNetId];
		List<IBestiaryInfoElement> list = new List<IBestiaryInfoElement>
		{
			new NPCNetIdBestiaryInfoElement(npcNetId),
			new NamePlateInfoElement(Lang.GetNPCName(npcNetId).Key, npcNetId),
			new NPCPortraitInfoElement(ContentSamples.NpcBestiaryRarityStars[npcNetId]),
			new NPCKillCounterInfoElement(npcNetId)
		};
		string key = Lang.GetNPCName(nPC.netID).Key;
		key = key.Replace("NPCName.", "");
		string text = "Bestiary_FlavorText.npc_" + key;
		if (Language.Exists(text))
		{
			list.Add(new FlavorTextBestiaryInfoElement(text));
		}
		return new BestiaryEntry
		{
			Icon = new UnlockableNPCEntryIcon(npcNetId),
			Info = list,
			UIInfoProvider = new CritterUICollectionInfoProvider(nPC.GetBestiaryCreditId())
		};
	}

	public static BestiaryEntry Biome(string nameLanguageKey, string texturePath, Func<bool> unlockCondition)
	{
		return new BestiaryEntry
		{
			Icon = new CustomEntryIcon(nameLanguageKey, texturePath, unlockCondition),
			Info = new List<IBestiaryInfoElement>()
		};
	}

	public void AddTags(params IBestiaryInfoElement[] elements)
	{
		Info.AddRange(elements);
	}
}
