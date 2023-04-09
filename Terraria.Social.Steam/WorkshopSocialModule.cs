using System.Collections.Generic;
using System.IO;
using System.Linq;
using Steamworks;
using Terraria.DataStructures;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Social.Base;

namespace Terraria.Social.Steam;

public class WorkshopSocialModule : Terraria.Social.Base.WorkshopSocialModule
{
	private WorkshopHelper.UGCBased.Downloader _downloader;

	private WorkshopHelper.UGCBased.PublishedItemsFinder _publishedItems;

	private List<WorkshopHelper.UGCBased.APublisherInstance> _publisherInstances;

	private string _contentBaseFolder;

	public override void Initialize()
	{
		base.Branding = new WorkshopBranding
		{
			ResourcePackBrand = ResourcePack.BrandingType.SteamWorkshop
		};
		_publisherInstances = new List<WorkshopHelper.UGCBased.APublisherInstance>();
		base.ProgressReporter = new WorkshopProgressReporter(_publisherInstances);
		base.SupportedTags = new SupportedWorkshopTags();
		_contentBaseFolder = Main.SavePath + Path.DirectorySeparatorChar + "Workshop";
		_downloader = WorkshopHelper.UGCBased.Downloader.Create();
		_publishedItems = WorkshopHelper.UGCBased.PublishedItemsFinder.Create();
		WorkshopIssueReporter workshopIssueReporter = new WorkshopIssueReporter();
		workshopIssueReporter.OnNeedToOpenUI += _issueReporter_OnNeedToOpenUI;
		workshopIssueReporter.OnNeedToNotifyUI += _issueReporter_OnNeedToNotifyUI;
		base.IssueReporter = workshopIssueReporter;
		UIWorkshopHub.OnWorkshopHubMenuOpened += RefreshSubscriptionsAndPublishings;
	}

	private void _issueReporter_OnNeedToNotifyUI()
	{
		Main.IssueReporterIndicator.AttemptLettingPlayerKnow();
		Main.WorkshopPublishingIndicator.Hide();
	}

	private void _issueReporter_OnNeedToOpenUI()
	{
		Main.OpenReportsMenu();
	}

	public override void Shutdown()
	{
	}

	public override void LoadEarlyContent()
	{
		RefreshSubscriptionsAndPublishings();
	}

	private void RefreshSubscriptionsAndPublishings()
	{
		_downloader.Refresh(base.IssueReporter);
		_publishedItems.Refresh();
	}

	public override List<string> GetListOfSubscribedWorldPaths()
	{
		return _downloader.WorldPaths.Select((string folderPath) => folderPath + Path.DirectorySeparatorChar + "world.wld").ToList();
	}

	public override List<string> GetListOfSubscribedResourcePackPaths()
	{
		return _downloader.ResourcePackPaths;
	}

	public override bool TryGetPath(string pathEnd, out string fullPathFound)
	{
		fullPathFound = null;
		string text = _downloader.ResourcePackPaths.FirstOrDefault((string x) => x.EndsWith(pathEnd));
		if (text == null)
		{
			return false;
		}
		fullPathFound = text;
		return true;
	}

	private void Forget(WorkshopHelper.UGCBased.APublisherInstance instance)
	{
		_publisherInstances.Remove(instance);
		RefreshSubscriptionsAndPublishings();
	}

	public override void PublishWorld(WorldFileData world, WorkshopItemPublishSettings settings)
	{
		string i = world.Name;
		string j = GetTextForWorld(world);
		string[] k = settings.GetUsedTagsInternalNames();
		string l = GetTemporaryFolderPath() + world.GetFileName(includeExtension: false);
		if (MakeTemporaryFolder(l))
		{
			WorkshopHelper.UGCBased.WorldPublisherInstance projectile = new WorkshopHelper.UGCBased.WorldPublisherInstance(world);
			_publisherInstances.Add(projectile);
			projectile.PublishContent(_publishedItems, base.IssueReporter, Forget, i, j, l, settings.PreviewImagePath, settings.Publicity, k);
		}
	}

	private string GetTextForWorld(WorldFileData world)
	{
		string collisionPoint = "This is \"";
		collisionPoint += world.Name;
		string width = "";
		width = world.WorldSizeX switch
		{
			4200 => "small", 
			6400 => "medium", 
			8400 => "large", 
			_ => "custom", 
		};
		string height = "";
		height = world.GameMode switch
		{
			3 => "journey", 
			0 => "classic", 
			1 => "expert", 
			2 => "master", 
			_ => "custom", 
		};
		collisionPoint = collisionPoint + "\", a " + width.ToLower() + " " + height.ToLower() + " world";
		collisionPoint = collisionPoint + " infected by the " + (world.HasCorruption ? "corruption" : "crimson");
		if (world.IsHardMode)
		{
			collisionPoint += ", in hardmode";
		}
		return collisionPoint + ".";
	}

	public override void PublishResourcePack(ResourcePack resourcePack, WorkshopItemPublishSettings settings)
	{
		if (resourcePack.IsCompressed)
		{
			base.IssueReporter.ReportInstantUploadProblem("Workshop.ReportIssue_CannotPublishZips");
			return;
		}
		string vector = resourcePack.Name;
		string position = resourcePack.Description;
		if (string.IsNullOrWhiteSpace(position))
		{
			position = "";
		}
		string[] tile = settings.GetUsedTagsInternalNames();
		string vector2 = resourcePack.FullPath;
		WorkshopHelper.UGCBased.ResourcePackPublisherInstance num = new WorkshopHelper.UGCBased.ResourcePackPublisherInstance(resourcePack);
		_publisherInstances.Add(num);
		num.PublishContent(_publishedItems, base.IssueReporter, Forget, vector, position, vector2, settings.PreviewImagePath, settings.Publicity, tile);
	}

	private string GetTemporaryFolderPath()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ulong point = SteamUser.GetSteamID().m_SteamID;
		return _contentBaseFolder + Path.DirectorySeparatorChar + point + Path.DirectorySeparatorChar;
	}

	private bool MakeTemporaryFolder(string temporaryFolderPath)
	{
		bool tile = true;
		if (!Utils.TryCreatingDirectory(temporaryFolderPath))
		{
			base.IssueReporter.ReportDelayedUploadProblem("Workshop.ReportIssue_CouldNotCreateTemporaryFolder!");
			tile = false;
		}
		return tile;
	}

	public override void ImportDownloadedWorldToLocalSaves(WorldFileData world, string newFileName = null, string newDisplayName = null)
	{
		Main.menuMode = 10;
		world.CopyToLocal(newFileName, newDisplayName);
	}

	public List<IssueReport> GetReports()
	{
		List<IssueReport> list = new List<IssueReport>();
		if (base.IssueReporter != null)
		{
			list.AddRange(base.IssueReporter.GetReports());
		}
		return list;
	}

	public override bool TryGetInfoForWorld(WorldFileData world, out FoundWorkshopEntryInfo info)
	{
		info = null;
		string text = GetTemporaryFolderPath() + world.GetFileName(includeExtension: false);
		if (!Directory.Exists(text))
		{
			return false;
		}
		if (AWorkshopEntry.TryReadingManifest(text + Path.DirectorySeparatorChar + "workshop.json", out info))
		{
			return true;
		}
		return false;
	}

	public override bool TryGetInfoForResourcePack(ResourcePack resourcePack, out FoundWorkshopEntryInfo info)
	{
		info = null;
		string fullPath = resourcePack.FullPath;
		if (!Directory.Exists(fullPath))
		{
			return false;
		}
		if (AWorkshopEntry.TryReadingManifest(fullPath + Path.DirectorySeparatorChar + "workshop.json", out info))
		{
			return true;
		}
		return false;
	}
}
