using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Social;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.GameContent.UI.States;

public class UIWorldCreation : UIState
{
	private enum WorldSizeId
	{
		Small,
		Medium,
		Large
	}

	private enum WorldDifficultyId
	{
		Normal,
		Expert,
		Master,
		Creative
	}

	private enum WorldEvilId
	{
		Random,
		Corruption,
		Crimson
	}

	private WorldSizeId _optionSize;

	private WorldDifficultyId _optionDifficulty;

	private WorldEvilId _optionEvil;

	private string _optionwWorldName;

	private string _optionSeed;

	private UICharacterNameButton _namePlate;

	private UICharacterNameButton _seedPlate;

	private UIWorldCreationPreview _previewPlate;

	private GroupOptionButton<WorldSizeId>[] _sizeButtons;

	private GroupOptionButton<WorldDifficultyId>[] _difficultyButtons;

	private GroupOptionButton<WorldEvilId>[] _evilButtons;

	private UIText _descriptionText;

	public const int MAX_NAME_LENGTH = 27;

	public const int MAX_SEED_LENGTH = 40;

	public UIWorldCreation()
	{
		BuildPage();
	}

	private void BuildPage()
	{
		int x = 18;
		RemoveAllChildren();
		UIElement choices = new UIElement
		{
			Width = StyleDimension.FromPixels(500f),
			Height = StyleDimension.FromPixels(434f + (float)x),
			Top = StyleDimension.FromPixels(170f - (float)x),
			HAlign = 0.5f,
			VAlign = 0f
		};
		choices.SetPadding(0f);
		Append(choices);
		UIPanel uIPanel = new UIPanel
		{
			Width = StyleDimension.FromPercent(1f),
			Height = StyleDimension.FromPixels(280 + x),
			Top = StyleDimension.FromPixels(50f),
			BackgroundColor = new Color(33, 43, 79) * 0.8f
		};
		uIPanel.SetPadding(0f);
		choices.Append(uIPanel);
		MakeBackAndCreatebuttons(choices);
		UIElement room = new UIElement
		{
			Top = StyleDimension.FromPixelsAndPercent(0f, 0f),
			Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
			Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
			HAlign = 1f
		};
		room.SetPadding(0f);
		room.PaddingTop = 8f;
		room.PaddingBottom = 12f;
		uIPanel.Append(room);
		MakeInfoMenu(room);
	}

	private void MakeInfoMenu(UIElement parentContainer)
	{
		UIElement uIElement = new UIElement
		{
			Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
			Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
			HAlign = 0.5f,
			VAlign = 0f
		};
		uIElement.SetPadding(10f);
		uIElement.PaddingBottom = 0f;
		uIElement.PaddingTop = 0f;
		parentContainer.Append(uIElement);
		float item = 0f;
		float num = 44f;
		float shapeData = 88f + num;
		float i = num;
		GroupOptionButton<bool> groupOptionButton = new GroupOptionButton<bool>(option: true, null, Language.GetText("UI.WorldCreationRandomizeNameDescription"), Color.White, "Images/UI/WorldCreation/IconRandomName")
		{
			Width = StyleDimension.FromPixelsAndPercent(40f, 0f),
			Height = new StyleDimension(40f, 0f),
			HAlign = 0f,
			Top = StyleDimension.FromPixelsAndPercent(item, 0f),
			ShowHighlightWhenSelected = false
		};
		groupOptionButton.OnLeftMouseDown += ClickRandomizeName;
		groupOptionButton.OnMouseOver += ShowOptionDescription;
		groupOptionButton.OnMouseOut += ClearOptionDescription;
		groupOptionButton.SetSnapPoint("RandomizeName", 0);
		uIElement.Append(groupOptionButton);
		UICharacterNameButton uICharacterNameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), Language.GetText("UI.WorldCreationNameEmpty"), Language.GetText("UI.WorldDescriptionName"))
		{
			Width = StyleDimension.FromPixelsAndPercent(0f - shapeData, 1f),
			HAlign = 0f,
			Left = new StyleDimension(i, 0f),
			Top = StyleDimension.FromPixelsAndPercent(item, 0f)
		};
		uICharacterNameButton.OnLeftMouseDown += Click_SetName;
		uICharacterNameButton.OnMouseOver += ShowOptionDescription;
		uICharacterNameButton.OnMouseOut += ClearOptionDescription;
		uICharacterNameButton.SetSnapPoint("Name", 0);
		uIElement.Append(uICharacterNameButton);
		_namePlate = uICharacterNameButton;
		item += uICharacterNameButton.GetDimensions().Height + 4f;
		GroupOptionButton<bool> groupOptionButton2 = new GroupOptionButton<bool>(option: true, null, Language.GetText("UI.WorldCreationRandomizeSeedDescription"), Color.White, "Images/UI/WorldCreation/IconRandomSeed")
		{
			Width = StyleDimension.FromPixelsAndPercent(40f, 0f),
			Height = new StyleDimension(40f, 0f),
			HAlign = 0f,
			Top = StyleDimension.FromPixelsAndPercent(item, 0f),
			ShowHighlightWhenSelected = false
		};
		groupOptionButton2.OnLeftMouseDown += ClickRandomizeSeed;
		groupOptionButton2.OnMouseOver += ShowOptionDescription;
		groupOptionButton2.OnMouseOut += ClearOptionDescription;
		groupOptionButton2.SetSnapPoint("RandomizeSeed", 0);
		uIElement.Append(groupOptionButton2);
		UICharacterNameButton uICharacterNameButton2 = new UICharacterNameButton(Language.GetText("UI.WorldCreationSeed"), Language.GetText("UI.WorldCreationSeedEmpty"), Language.GetText("UI.WorldDescriptionSeed"))
		{
			Width = StyleDimension.FromPixelsAndPercent(0f - shapeData, 1f),
			HAlign = 0f,
			Left = new StyleDimension(i, 0f),
			Top = StyleDimension.FromPixelsAndPercent(item, 0f),
			DistanceFromTitleToOption = 29f
		};
		uICharacterNameButton2.OnLeftMouseDown += Click_SetSeed;
		uICharacterNameButton2.OnMouseOver += ShowOptionDescription;
		uICharacterNameButton2.OnMouseOut += ClearOptionDescription;
		uICharacterNameButton2.SetSnapPoint("Seed", 0);
		uIElement.Append(uICharacterNameButton2);
		_seedPlate = uICharacterNameButton2;
		UIWorldCreationPreview uIWorldCreationPreview = new UIWorldCreationPreview
		{
			Width = StyleDimension.FromPixels(84f),
			Height = StyleDimension.FromPixels(84f),
			HAlign = 1f,
			VAlign = 0f
		};
		uIElement.Append(uIWorldCreationPreview);
		_previewPlate = uIWorldCreationPreview;
		item += uICharacterNameButton2.GetDimensions().Height + 10f;
		AddHorizontalSeparator(uIElement, item + 2f);
		float usableWidthPercent = 1f;
		AddWorldSizeOptions(uIElement, item, ClickSizeOption, "size", usableWidthPercent);
		item += 48f;
		AddHorizontalSeparator(uIElement, item);
		AddWorldDifficultyOptions(uIElement, item, ClickDifficultyOption, "difficulty", usableWidthPercent);
		item += 48f;
		AddHorizontalSeparator(uIElement, item);
		AddWorldEvilOptions(uIElement, item, ClickEvilOption, "evil", usableWidthPercent);
		item += 48f;
		AddHorizontalSeparator(uIElement, item);
		AddDescriptionPanel(uIElement, item, "desc");
		SetDefaultOptions();
	}

	private static void AddHorizontalSeparator(UIElement Container, float accumualtedHeight)
	{
		UIHorizontalSeparator list = new UIHorizontalSeparator
		{
			Width = StyleDimension.FromPercent(1f),
			Top = StyleDimension.FromPixels(accumualtedHeight - 8f),
			Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
		};
		Container.Append(list);
	}

	private void SetDefaultOptions()
	{
		AssignRandomWorldName();
		AssignRandomWorldSeed();
		UpdateInputFields();
		GroupOptionButton<WorldSizeId>[] sizeButtons = _sizeButtons;
		for (int item = 0; item < sizeButtons.Length; item++)
		{
			sizeButtons[item].SetCurrentOption(WorldSizeId.Medium);
		}
		_optionSize = WorldSizeId.Medium;
		if (Main.ActivePlayerFileData.Player.difficulty == 3)
		{
			GroupOptionButton<WorldDifficultyId>[] difficultyButtons = _difficultyButtons;
			for (int item = 0; item < difficultyButtons.Length; item++)
			{
				difficultyButtons[item].SetCurrentOption(WorldDifficultyId.Creative);
			}
			_optionDifficulty = WorldDifficultyId.Creative;
			UpdatePreviewPlate();
		}
		else
		{
			GroupOptionButton<WorldDifficultyId>[] difficultyButtons = _difficultyButtons;
			for (int item = 0; item < difficultyButtons.Length; item++)
			{
				difficultyButtons[item].SetCurrentOption(WorldDifficultyId.Normal);
			}
		}
		GroupOptionButton<WorldEvilId>[] evilButtons = _evilButtons;
		for (int item = 0; item < evilButtons.Length; item++)
		{
			evilButtons[item].SetCurrentOption(WorldEvilId.Random);
		}
	}

	private void AddDescriptionPanel(UIElement container, float accumulatedHeight, string tagGroup)
	{
		float list = 0f;
		UISlicedImage uISlicedImage = new UISlicedImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight", (AssetRequestMode)1))
		{
			HAlign = 0.5f,
			VAlign = 1f,
			Width = StyleDimension.FromPixelsAndPercent((0f - list) * 2f, 1f),
			Left = StyleDimension.FromPixels(0f - list),
			Height = StyleDimension.FromPixelsAndPercent(40f, 0f),
			Top = StyleDimension.FromPixels(2f)
		};
		uISlicedImage.SetSliceDepths(10);
		uISlicedImage.Color = Color.LightGray * 0.7f;
		container.Append(uISlicedImage);
		UIText room = new UIText(Language.GetText("UI.WorldDescriptionDefault"), 0.82f)
		{
			HAlign = 0f,
			VAlign = 0f,
			Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
			Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
			Top = StyleDimension.FromPixelsAndPercent(5f, 0f)
		};
		room.PaddingLeft = 20f;
		room.PaddingRight = 20f;
		room.PaddingTop = 6f;
		uISlicedImage.Append(room);
		_descriptionText = room;
	}

	private void AddWorldSizeOptions(UIElement container, float accumualtedHeight, MouseEvent clickEvent, string tagGroup, float usableWidthPercent)
	{
		WorldSizeId[] array = new WorldSizeId[3]
		{
			WorldSizeId.Small,
			WorldSizeId.Medium,
			WorldSizeId.Large
		};
		LocalizedText[] array2 = new LocalizedText[3]
		{
			Lang.menu[92],
			Lang.menu[93],
			Lang.menu[94]
		};
		LocalizedText[] array3 = new LocalizedText[3]
		{
			Language.GetText("UI.WorldDescriptionSizeSmall"),
			Language.GetText("UI.WorldDescriptionSizeMedium"),
			Language.GetText("UI.WorldDescriptionSizeLarge")
		};
		Color[] array4 = new Color[3]
		{
			Color.Cyan,
			Color.Lerp(Color.Cyan, Color.LimeGreen, 0.5f),
			Color.LimeGreen
		};
		string[] array5 = new string[3] { "Images/UI/WorldCreation/IconSizeSmall", "Images/UI/WorldCreation/IconSizeMedium", "Images/UI/WorldCreation/IconSizeLarge" };
		GroupOptionButton<WorldSizeId>[] array6 = new GroupOptionButton<WorldSizeId>[array.Length];
		for (int i = 0; i < array6.Length; i++)
		{
			GroupOptionButton<WorldSizeId> groupOptionButton = new GroupOptionButton<WorldSizeId>(array[i], array2[i], array3[i], array4[i], array5[i], 1f, 1f, 16f);
			groupOptionButton.Width = StyleDimension.FromPixelsAndPercent(-4 * (array6.Length - 1), 1f / (float)array6.Length * usableWidthPercent);
			groupOptionButton.Left = StyleDimension.FromPercent(1f - usableWidthPercent);
			groupOptionButton.HAlign = (float)i / (float)(array6.Length - 1);
			groupOptionButton.Top.Set(accumualtedHeight, 0f);
			groupOptionButton.OnLeftMouseDown += clickEvent;
			groupOptionButton.OnMouseOver += ShowOptionDescription;
			groupOptionButton.OnMouseOut += ClearOptionDescription;
			groupOptionButton.SetSnapPoint(tagGroup, i);
			container.Append(groupOptionButton);
			array6[i] = groupOptionButton;
		}
		_sizeButtons = array6;
	}

	private void AddWorldDifficultyOptions(UIElement container, float accumualtedHeight, MouseEvent clickEvent, string tagGroup, float usableWidthPercent)
	{
		WorldDifficultyId[] list = new WorldDifficultyId[4]
		{
			WorldDifficultyId.Creative,
			WorldDifficultyId.Normal,
			WorldDifficultyId.Expert,
			WorldDifficultyId.Master
		};
		LocalizedText[] topRoom = new LocalizedText[4]
		{
			Language.GetText("UI.Creative"),
			Language.GetText("UI.Normal"),
			Language.GetText("UI.Expert"),
			Language.GetText("UI.Master")
		};
		LocalizedText[] bottomRoom = new LocalizedText[4]
		{
			Language.GetText("UI.WorldDescriptionCreative"),
			Language.GetText("UI.WorldDescriptionNormal"),
			Language.GetText("UI.WorldDescriptionExpert"),
			Language.GetText("UI.WorldDescriptionMaster")
		};
		Color[] exitX = new Color[4]
		{
			Main.creativeModeColor,
			Color.White,
			Main.mcColor,
			Main.hcColor
		};
		string[] array = new string[4] { "Images/UI/WorldCreation/IconDifficultyCreative", "Images/UI/WorldCreation/IconDifficultyNormal", "Images/UI/WorldCreation/IconDifficultyExpert", "Images/UI/WorldCreation/IconDifficultyMaster" };
		GroupOptionButton<WorldDifficultyId>[] array2 = new GroupOptionButton<WorldDifficultyId>[list.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			GroupOptionButton<WorldDifficultyId> groupOptionButton = new GroupOptionButton<WorldDifficultyId>(list[i], topRoom[i], bottomRoom[i], exitX[i], array[i], 1f, 1f, 16f);
			groupOptionButton.Width = StyleDimension.FromPixelsAndPercent(-1 * (array2.Length - 1), 1f / (float)array2.Length * usableWidthPercent);
			groupOptionButton.Left = StyleDimension.FromPercent(1f - usableWidthPercent);
			groupOptionButton.HAlign = (float)i / (float)(array2.Length - 1);
			groupOptionButton.Top.Set(accumualtedHeight, 0f);
			groupOptionButton.OnLeftMouseDown += clickEvent;
			groupOptionButton.OnMouseOver += ShowOptionDescription;
			groupOptionButton.OnMouseOut += ClearOptionDescription;
			groupOptionButton.SetSnapPoint(tagGroup, i);
			container.Append(groupOptionButton);
			array2[i] = groupOptionButton;
		}
		_difficultyButtons = array2;
	}

	private void AddWorldEvilOptions(UIElement container, float accumualtedHeight, MouseEvent clickEvent, string tagGroup, float usableWidthPercent)
	{
		WorldEvilId[] array = new WorldEvilId[3]
		{
			WorldEvilId.Random,
			WorldEvilId.Corruption,
			WorldEvilId.Crimson
		};
		LocalizedText[] item = new LocalizedText[3]
		{
			Lang.misc[103],
			Lang.misc[101],
			Lang.misc[102]
		};
		LocalizedText[] array2 = new LocalizedText[3]
		{
			Language.GetText("UI.WorldDescriptionEvilRandom"),
			Language.GetText("UI.WorldDescriptionEvilCorrupt"),
			Language.GetText("UI.WorldDescriptionEvilCrimson")
		};
		Color[] array3 = new Color[3]
		{
			Color.White,
			Color.MediumPurple,
			Color.IndianRed
		};
		string[] array4 = new string[3] { "Images/UI/WorldCreation/IconEvilRandom", "Images/UI/WorldCreation/IconEvilCorruption", "Images/UI/WorldCreation/IconEvilCrimson" };
		GroupOptionButton<WorldEvilId>[] array5 = new GroupOptionButton<WorldEvilId>[array.Length];
		for (int i = 0; i < array5.Length; i++)
		{
			GroupOptionButton<WorldEvilId> groupOptionButton = new GroupOptionButton<WorldEvilId>(array[i], item[i], array2[i], array3[i], array4[i], 1f, 1f, 16f);
			groupOptionButton.Width = StyleDimension.FromPixelsAndPercent(-4 * (array5.Length - 1), 1f / (float)array5.Length * usableWidthPercent);
			groupOptionButton.Left = StyleDimension.FromPercent(1f - usableWidthPercent);
			groupOptionButton.HAlign = (float)i / (float)(array5.Length - 1);
			groupOptionButton.Top.Set(accumualtedHeight, 0f);
			groupOptionButton.OnLeftMouseDown += clickEvent;
			groupOptionButton.OnMouseOver += ShowOptionDescription;
			groupOptionButton.OnMouseOut += ClearOptionDescription;
			groupOptionButton.SetSnapPoint(tagGroup, i);
			container.Append(groupOptionButton);
			array5[i] = groupOptionButton;
		}
		_evilButtons = array5;
	}

	private void ClickRandomizeName(UIMouseEvent evt, UIElement listeningElement)
	{
		AssignRandomWorldName();
		UpdateInputFields();
		UpdateSliders();
		UpdatePreviewPlate();
	}

	private void ClickRandomizeSeed(UIMouseEvent evt, UIElement listeningElement)
	{
		AssignRandomWorldSeed();
		UpdateInputFields();
		UpdateSliders();
		UpdatePreviewPlate();
	}

	private void ClickSizeOption(UIMouseEvent evt, UIElement listeningElement)
	{
		GroupOptionButton<WorldSizeId> result = (GroupOptionButton<WorldSizeId>)listeningElement;
		_optionSize = result.OptionValue;
		GroupOptionButton<WorldSizeId>[] sizeButtons = _sizeButtons;
		for (int i = 0; i < sizeButtons.Length; i++)
		{
			sizeButtons[i].SetCurrentOption(result.OptionValue);
		}
		UpdatePreviewPlate();
	}

	private void ClickDifficultyOption(UIMouseEvent evt, UIElement listeningElement)
	{
		GroupOptionButton<WorldDifficultyId> flag = (GroupOptionButton<WorldDifficultyId>)listeningElement;
		_optionDifficulty = flag.OptionValue;
		GroupOptionButton<WorldDifficultyId>[] difficultyButtons = _difficultyButtons;
		for (int room = 0; room < difficultyButtons.Length; room++)
		{
			difficultyButtons[room].SetCurrentOption(flag.OptionValue);
		}
		UpdatePreviewPlate();
	}

	private void ClickEvilOption(UIMouseEvent evt, UIElement listeningElement)
	{
		GroupOptionButton<WorldEvilId> flag2 = (GroupOptionButton<WorldEvilId>)listeningElement;
		_optionEvil = flag2.OptionValue;
		GroupOptionButton<WorldEvilId>[] flag = _evilButtons;
		for (int i = 0; i < flag.Length; i++)
		{
			flag[i].SetCurrentOption(flag2.OptionValue);
		}
		UpdatePreviewPlate();
	}

	private void UpdatePreviewPlate()
	{
		_previewPlate.UpdateOption((byte)_optionDifficulty, (byte)_optionEvil, (byte)_optionSize);
	}

	private void UpdateSliders()
	{
		GroupOptionButton<WorldSizeId>[] sizeButtons = _sizeButtons;
		for (int i = 0; i < sizeButtons.Length; i++)
		{
			sizeButtons[i].SetCurrentOption(_optionSize);
		}
		GroupOptionButton<WorldDifficultyId>[] difficultyButtons = _difficultyButtons;
		for (int i = 0; i < difficultyButtons.Length; i++)
		{
			difficultyButtons[i].SetCurrentOption(_optionDifficulty);
		}
		GroupOptionButton<WorldEvilId>[] evilButtons = _evilButtons;
		for (int i = 0; i < evilButtons.Length; i++)
		{
			evilButtons[i].SetCurrentOption(_optionEvil);
		}
	}

	public void ShowOptionDescription(UIMouseEvent evt, UIElement listeningElement)
	{
		LocalizedText list = null;
		if (listeningElement is GroupOptionButton<WorldSizeId> houseType)
		{
			list = houseType.Description;
		}
		if (listeningElement is GroupOptionButton<WorldDifficultyId> groupOptionButton)
		{
			list = groupOptionButton.Description;
		}
		if (listeningElement is GroupOptionButton<WorldEvilId> groupOptionButton2)
		{
			list = groupOptionButton2.Description;
		}
		if (listeningElement is UICharacterNameButton uICharacterNameButton)
		{
			list = uICharacterNameButton.Description;
		}
		if (listeningElement is GroupOptionButton<bool> groupOptionButton3)
		{
			list = groupOptionButton3.Description;
		}
		if (list != null)
		{
			_descriptionText.SetText(list);
		}
	}

	public void ClearOptionDescription(UIMouseEvent evt, UIElement listeningElement)
	{
		_descriptionText.SetText(Language.GetText("UI.WorldDescriptionDefault"));
	}

	private void MakeBackAndCreatebuttons(UIElement outerContainer)
	{
		UITextPanel<LocalizedText> result = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, large: true)
		{
			Width = StyleDimension.FromPixelsAndPercent(-10f, 0.5f),
			Height = StyleDimension.FromPixels(50f),
			VAlign = 1f,
			HAlign = 0f,
			Top = StyleDimension.FromPixels(-45f)
		};
		result.OnMouseOver += FadedMouseOver;
		result.OnMouseOut += FadedMouseOut;
		result.OnLeftMouseDown += Click_GoBack;
		result.SetSnapPoint("Back", 0);
		outerContainer.Append(result);
		UITextPanel<LocalizedText> flag = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, large: true)
		{
			Width = StyleDimension.FromPixelsAndPercent(-10f, 0.5f),
			Height = StyleDimension.FromPixels(50f),
			VAlign = 1f,
			HAlign = 1f,
			Top = StyleDimension.FromPixels(-45f)
		};
		flag.OnMouseOver += FadedMouseOver;
		flag.OnMouseOut += FadedMouseOut;
		flag.OnLeftMouseDown += Click_NamingAndCreating;
		flag.SetSnapPoint("Create", 0);
		outerContainer.Append(flag);
	}

	private void Click_GoBack(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(11);
		Main.OpenWorldSelectUI();
	}

	private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(12);
		((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
		((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
	}

	private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
	{
		((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.8f;
		((UIPanel)evt.Target).BorderColor = Color.Black;
	}

	private void Click_SetName(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(10);
		Main.clrInput();
		UIVirtualKeyboard dictionary = new UIVirtualKeyboard(Lang.menu[48].Value, "", OnFinishedSettingName, GoBackHere, 0, allowEmpty: true);
		dictionary.SetMaxInputLength(27);
		Main.MenuUI.SetState(dictionary);
	}

	private void Click_SetSeed(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(10);
		Main.clrInput();
		UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Language.GetTextValue("UI.EnterSeed"), "", OnFinishedSettingSeed, GoBackHere, 0, allowEmpty: true);
		uIVirtualKeyboard.SetMaxInputLength(40);
		Main.MenuUI.SetState(uIVirtualKeyboard);
	}

	private void Click_NamingAndCreating(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(10);
		if (string.IsNullOrEmpty(_optionwWorldName))
		{
			_optionwWorldName = "";
			Main.clrInput();
			UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Lang.menu[48].Value, "", OnFinishedNamingAndCreating, GoBackHere);
			uIVirtualKeyboard.SetMaxInputLength(27);
			Main.MenuUI.SetState(uIVirtualKeyboard);
		}
		else
		{
			FinishCreatingWorld();
		}
	}

	private void OnFinishedSettingName(string name)
	{
		_optionwWorldName = name.Trim();
		UpdateInputFields();
		GoBackHere();
	}

	private void UpdateInputFields()
	{
		_namePlate.SetContents(_optionwWorldName);
		_namePlate.Recalculate();
		_namePlate.TrimDisplayIfOverElementDimensions(27);
		_namePlate.Recalculate();
		_seedPlate.SetContents(_optionSeed);
		_seedPlate.Recalculate();
		_seedPlate.TrimDisplayIfOverElementDimensions(40);
		_seedPlate.Recalculate();
	}

	private void OnFinishedSettingSeed(string seed)
	{
		_optionSeed = seed.Trim();
		ProcessSeed(out var processedSeed);
		_optionSeed = processedSeed;
		UpdateInputFields();
		UpdateSliders();
		UpdatePreviewPlate();
		GoBackHere();
	}

	private void GoBackHere()
	{
		Main.MenuUI.SetState(this);
	}

	private void OnFinishedNamingAndCreating(string name)
	{
		OnFinishedSettingName(name);
		FinishCreatingWorld();
	}

	private void FinishCreatingWorld()
	{
		ProcessSeed(out var processedSeed);
		switch (_optionSize)
		{
		case WorldSizeId.Small:
			Main.maxTilesX = 4200;
			Main.maxTilesY = 1200;
			break;
		case WorldSizeId.Medium:
			Main.maxTilesX = 6400;
			Main.maxTilesY = 1800;
			break;
		case WorldSizeId.Large:
			Main.maxTilesX = 8400;
			Main.maxTilesY = 2400;
			break;
		}
		WorldGen.setWorldSize();
		switch (_optionDifficulty)
		{
		case WorldDifficultyId.Creative:
			Main.GameMode = 3;
			break;
		case WorldDifficultyId.Normal:
			Main.GameMode = 0;
			break;
		case WorldDifficultyId.Expert:
			Main.GameMode = 1;
			break;
		case WorldDifficultyId.Master:
			Main.GameMode = 2;
			break;
		}
		switch (_optionEvil)
		{
		case WorldEvilId.Random:
			WorldGen.WorldGenParam_Evil = -1;
			break;
		case WorldEvilId.Corruption:
			WorldGen.WorldGenParam_Evil = 0;
			break;
		case WorldEvilId.Crimson:
			WorldGen.WorldGenParam_Evil = 1;
			break;
		}
		Main.ActiveWorldFileData = WorldFile.CreateMetadata(Main.worldName = _optionwWorldName.Trim(), SocialAPI.Cloud != null && SocialAPI.Cloud.EnabledByDefault, Main.GameMode);
		if (processedSeed.Length == 0)
		{
			Main.ActiveWorldFileData.SetSeedToRandom();
		}
		else
		{
			Main.ActiveWorldFileData.SetSeed(processedSeed);
		}
		Main.menuMode = 10;
		WorldGen.CreateNewWorld();
	}

	public static void ProcessSpecialWorldSeeds(string processedSeed)
	{
		WorldGen.noTrapsWorldGen = false;
		WorldGen.notTheBees = false;
		WorldGen.getGoodWorldGen = false;
		WorldGen.tenthAnniversaryWorldGen = false;
		WorldGen.dontStarveWorldGen = false;
		WorldGen.tempRemixWorldGen = false;
		WorldGen.tempTenthAnniversaryWorldGen = false;
		WorldGen.everythingWorldGen = false;
		if (processedSeed.ToLower() == "no traps" || processedSeed.ToLower() == "notraps")
		{
			WorldGen.noTrapsWorldGen = true;
		}
		if (processedSeed.ToLower() == "not the bees" || processedSeed.ToLower() == "not the bees!" || processedSeed.ToLower() == "notthebees")
		{
			WorldGen.notTheBees = true;
		}
		if (processedSeed.ToLower() == "for the worthy" || processedSeed.ToLower() == "fortheworthy")
		{
			WorldGen.getGoodWorldGen = true;
		}
		if (processedSeed.ToLower() == "don't dig up" || processedSeed.ToLower() == "dont dig up" || processedSeed.ToLower() == "dontdigup")
		{
			WorldGen.tempRemixWorldGen = true;
		}
		if (processedSeed.ToLower() == "celebrationmk10")
		{
			WorldGen.tempTenthAnniversaryWorldGen = true;
		}
		if (processedSeed.ToLower() == "constant" || processedSeed.ToLower() == "theconstant" || processedSeed.ToLower() == "the constant" || processedSeed.ToLower() == "eye4aneye" || processedSeed.ToLower() == "eyeforaneye")
		{
			WorldGen.dontStarveWorldGen = true;
		}
		if (processedSeed.ToLower() == "get fixed boi" || processedSeed.ToLower() == "getfixedboi")
		{
			WorldGen.noTrapsWorldGen = true;
			WorldGen.notTheBees = true;
			WorldGen.getGoodWorldGen = true;
			WorldGen.tempTenthAnniversaryWorldGen = true;
			WorldGen.dontStarveWorldGen = true;
			WorldGen.tempRemixWorldGen = true;
			WorldGen.everythingWorldGen = true;
		}
	}

	private void ProcessSeed(out string processedSeed)
	{
		processedSeed = _optionSeed;
		ProcessSpecialWorldSeeds(processedSeed);
		string[] array = _optionSeed.Split('.');
		if (array.Length != 4)
		{
			return;
		}
		if (int.TryParse(array[0], out var result))
		{
			switch (result)
			{
			case 1:
				_optionSize = WorldSizeId.Small;
				break;
			case 2:
				_optionSize = WorldSizeId.Medium;
				break;
			case 3:
				_optionSize = WorldSizeId.Large;
				break;
			}
		}
		if (int.TryParse(array[1], out result))
		{
			switch (result)
			{
			case 1:
				_optionDifficulty = WorldDifficultyId.Normal;
				break;
			case 2:
				_optionDifficulty = WorldDifficultyId.Expert;
				break;
			case 3:
				_optionDifficulty = WorldDifficultyId.Master;
				break;
			case 4:
				_optionDifficulty = WorldDifficultyId.Creative;
				break;
			}
		}
		if (int.TryParse(array[2], out result))
		{
			switch (result)
			{
			case 1:
				_optionEvil = WorldEvilId.Corruption;
				break;
			case 2:
				_optionEvil = WorldEvilId.Crimson;
				break;
			}
		}
		processedSeed = array[3];
	}

	private void AssignRandomWorldName()
	{
		do
		{
			LocalizedText localizedText = Language.SelectRandom(Lang.CreateDialogFilter("RandomWorldName_Composition."));
			LocalizedText localizedText2 = Language.SelectRandom(Lang.CreateDialogFilter("RandomWorldName_Adjective."));
			LocalizedText localizedText3 = Language.SelectRandom(Lang.CreateDialogFilter("RandomWorldName_Location."));
			LocalizedText localizedText4 = Language.SelectRandom(Lang.CreateDialogFilter("RandomWorldName_Noun."));
			var obj = new
			{
				Adjective = localizedText2.Value,
				Location = localizedText3.Value,
				Noun = localizedText4.Value
			};
			_optionwWorldName = localizedText.FormatWith(obj);
			if (Main.rand.Next(10000) == 0)
			{
				_optionwWorldName = Language.GetTextValue("SpecialWorldName.TheConstant");
			}
		}
		while (_optionwWorldName.Length > 27);
	}

	private void AssignRandomWorldSeed()
	{
		_optionSeed = Main.rand.Next().ToString();
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
		SetupGamepadPoints(spriteBatch);
	}

	private void SetupGamepadPoints(SpriteBatch spriteBatch)
	{
		UILinkPointNavigator.Shortcuts.BackButtonCommand = 1;
		int num = 3000;
		List<SnapPoint> snapPoints = GetSnapPoints();
		SnapPoint snapPoint = null;
		SnapPoint snapPoint2 = null;
		SnapPoint snapPoint3 = null;
		SnapPoint snapPoint4 = null;
		SnapPoint snapPoint5 = null;
		SnapPoint snapPoint6 = null;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			SnapPoint snapPoint7 = snapPoints[i];
			switch (snapPoint7.Name)
			{
			case "Back":
				snapPoint = snapPoint7;
				break;
			case "Create":
				snapPoint2 = snapPoint7;
				break;
			case "Name":
				snapPoint3 = snapPoint7;
				break;
			case "Seed":
				snapPoint4 = snapPoint7;
				break;
			case "RandomizeName":
				snapPoint5 = snapPoint7;
				break;
			case "RandomizeSeed":
				snapPoint6 = snapPoint7;
				break;
			}
		}
		List<SnapPoint> snapGroup = GetSnapGroup(snapPoints, "size");
		List<SnapPoint> snapGroup2 = GetSnapGroup(snapPoints, "difficulty");
		List<SnapPoint> snapGroup3 = GetSnapGroup(snapPoints, "evil");
		UILinkPointNavigator.SetPosition(num, snapPoint.Position);
		UILinkPoint room = UILinkPointNavigator.Points[num];
		room.Unlink();
		UILinkPoint uILinkPoint = room;
		num++;
		UILinkPointNavigator.SetPosition(num, snapPoint2.Position);
		room = UILinkPointNavigator.Points[num];
		room.Unlink();
		UILinkPoint uILinkPoint2 = room;
		num++;
		UILinkPointNavigator.SetPosition(num, snapPoint5.Position);
		room = UILinkPointNavigator.Points[num];
		room.Unlink();
		UILinkPoint uILinkPoint3 = room;
		num++;
		UILinkPointNavigator.SetPosition(num, snapPoint3.Position);
		room = UILinkPointNavigator.Points[num];
		room.Unlink();
		UILinkPoint uILinkPoint4 = room;
		num++;
		UILinkPointNavigator.SetPosition(num, snapPoint6.Position);
		room = UILinkPointNavigator.Points[num];
		room.Unlink();
		UILinkPoint uILinkPoint5 = room;
		num++;
		UILinkPointNavigator.SetPosition(num, snapPoint4.Position);
		room = UILinkPointNavigator.Points[num];
		room.Unlink();
		UILinkPoint uILinkPoint6 = room;
		num++;
		UILinkPoint[] array = new UILinkPoint[snapGroup.Count];
		for (int j = 0; j < snapGroup.Count; j++)
		{
			UILinkPointNavigator.SetPosition(num, snapGroup[j].Position);
			room = UILinkPointNavigator.Points[num];
			room.Unlink();
			array[j] = room;
			num++;
		}
		UILinkPoint[] array2 = new UILinkPoint[snapGroup2.Count];
		for (int k = 0; k < snapGroup2.Count; k++)
		{
			UILinkPointNavigator.SetPosition(num, snapGroup2[k].Position);
			room = UILinkPointNavigator.Points[num];
			room.Unlink();
			array2[k] = room;
			num++;
		}
		UILinkPoint[] array3 = new UILinkPoint[snapGroup3.Count];
		for (int l = 0; l < snapGroup3.Count; l++)
		{
			UILinkPointNavigator.SetPosition(num, snapGroup3[l].Position);
			room = UILinkPointNavigator.Points[num];
			room.Unlink();
			array3[l] = room;
			num++;
		}
		LoopHorizontalLineLinks(array);
		LoopHorizontalLineLinks(array2);
		EstablishUpDownRelationship(array, array2);
		for (int m = 0; m < array.Length; m++)
		{
			array[m].Up = uILinkPoint6.ID;
		}
		if (true)
		{
			LoopHorizontalLineLinks(array3);
			EstablishUpDownRelationship(array2, array3);
			for (int n = 0; n < array3.Length; n++)
			{
				array3[n].Down = uILinkPoint.ID;
			}
			array3[array3.Length - 1].Down = uILinkPoint2.ID;
			uILinkPoint2.Up = array3[array3.Length - 1].ID;
			uILinkPoint.Up = array3[0].ID;
		}
		else
		{
			for (int num2 = 0; num2 < array2.Length; num2++)
			{
				array2[num2].Down = uILinkPoint.ID;
			}
			array2[array2.Length - 1].Down = uILinkPoint2.ID;
			uILinkPoint2.Up = array2[array2.Length - 1].ID;
			uILinkPoint.Up = array2[0].ID;
		}
		uILinkPoint2.Left = uILinkPoint.ID;
		uILinkPoint.Right = uILinkPoint2.ID;
		uILinkPoint4.Down = uILinkPoint6.ID;
		uILinkPoint4.Left = uILinkPoint3.ID;
		uILinkPoint3.Right = uILinkPoint4.ID;
		uILinkPoint6.Up = uILinkPoint4.ID;
		uILinkPoint6.Down = array[0].ID;
		uILinkPoint6.Left = uILinkPoint5.ID;
		uILinkPoint5.Right = uILinkPoint6.ID;
		uILinkPoint5.Up = uILinkPoint3.ID;
		uILinkPoint5.Down = array[0].ID;
		uILinkPoint3.Down = uILinkPoint5.ID;
	}

	private void EstablishUpDownRelationship(UILinkPoint[] topSide, UILinkPoint[] bottomSide)
	{
		int num = Math.Max(topSide.Length, bottomSide.Length);
		for (int i = 0; i < num; i++)
		{
			int num2 = Math.Min(i, topSide.Length - 1);
			int num3 = Math.Min(i, bottomSide.Length - 1);
			topSide[num2].Down = bottomSide[num3].ID;
			bottomSide[num3].Up = topSide[num2].ID;
		}
	}

	private void LoopHorizontalLineLinks(UILinkPoint[] pointsLine)
	{
		for (int i = 1; i < pointsLine.Length - 1; i++)
		{
			pointsLine[i - 1].Right = pointsLine[i].ID;
			pointsLine[i].Left = pointsLine[i - 1].ID;
			pointsLine[i].Right = pointsLine[i + 1].ID;
			pointsLine[i + 1].Left = pointsLine[i].ID;
		}
	}

	private List<SnapPoint> GetSnapGroup(List<SnapPoint> ptsOnPage, string groupName)
	{
		List<SnapPoint> list = ptsOnPage.Where((SnapPoint a) => a.Name == groupName).ToList();
		list.Sort(SortPoints);
		return list;
	}

	private int SortPoints(SnapPoint a, SnapPoint b)
	{
		return a.Id.CompareTo(b.Id);
	}
}
