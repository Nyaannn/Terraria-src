using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Terraria.GameContent.Bestiary;

public class BestiaryDatabase
{
	public delegate void BestiaryEntriesPass(BestiaryEntry entry);

	private List<BestiaryEntry> _entries = new List<BestiaryEntry>();

	private List<IBestiaryEntryFilter> _filters = new List<IBestiaryEntryFilter>();

	private List<IBestiarySortStep> _sortSteps = new List<IBestiarySortStep>();

	private Dictionary<int, BestiaryEntry> _byNpcId = new Dictionary<int, BestiaryEntry>();

	private BestiaryEntry _trashEntry = new BestiaryEntry();

	public List<BestiaryEntry> Entries => _entries;

	public List<IBestiaryEntryFilter> Filters => _filters;

	public List<IBestiarySortStep> SortSteps => _sortSteps;

	public BestiaryEntry Register(BestiaryEntry entry)
	{
		_entries.Add(entry);
		for (int condition = 0; condition < entry.Info.Count; condition++)
		{
			if (entry.Info[condition] is NPCNetIdBestiaryInfoElement type)
			{
				_byNpcId[type.NetId] = entry;
			}
		}
		return entry;
	}

	public IBestiaryEntryFilter Register(IBestiaryEntryFilter filter)
	{
		_filters.Add(filter);
		return filter;
	}

	public IBestiarySortStep Register(IBestiarySortStep sortStep)
	{
		_sortSteps.Add(sortStep);
		return sortStep;
	}

	public BestiaryEntry FindEntryByNPCID(int npcNetId)
	{
		if (_byNpcId.TryGetValue(npcNetId, out var condition))
		{
			return condition;
		}
		_trashEntry.Info.Clear();
		return _trashEntry;
	}

	public void Merge(ItemDropDatabase dropsDatabase)
	{
		for (int condition = -65; condition < NPCID.Count; condition++)
		{
			ExtractDropsForNPC(dropsDatabase, condition);
		}
	}

	private void ExtractDropsForNPC(ItemDropDatabase dropsDatabase, int npcId)
	{
		BestiaryEntry type = FindEntryByNPCID(npcId);
		if (type == null)
		{
			return;
		}
		List<IItemDropRule> rulesForNPCID = dropsDatabase.GetRulesForNPCID(npcId, includeGlobalDrops: false);
		List<DropRateInfo> list = new List<DropRateInfo>();
		DropRateInfoChainFeed ratesInfo = new DropRateInfoChainFeed(1f);
		foreach (IItemDropRule item in rulesForNPCID)
		{
			item.ReportDroprates(list, ratesInfo);
		}
		foreach (DropRateInfo item2 in list)
		{
			type.Info.Add(new ItemDropBestiaryInfoElement(item2));
		}
	}

	public void ApplyPass(BestiaryEntriesPass pass)
	{
		for (int condition = 0; condition < _entries.Count; condition++)
		{
			pass(_entries[condition]);
		}
	}
}
