using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.GameContent.UI.States;

public class UIWorkshopSelectResourcePackToPublish : UIState, IHaveBackButtonCommand
{
	private UIList _entryList;

	private UITextPanel<LocalizedText> _backPanel;

	private UIPanel _containerPanel;

	private UIScrollbar _scrollbar;

	private bool _isScrollbarAttached;

	private UIState _menuToGoBackTo;

	private List<ResourcePack> _entries = new List<ResourcePack>();

	private bool skipDraw;

	public UIWorkshopSelectResourcePackToPublish(UIState menuToGoBackTo)
	{
		_menuToGoBackTo = menuToGoBackTo;
	}

	public override void OnInitialize()
	{
		UIElement uIElement = new UIElement();
		uIElement.Width.Set(0f, 0.8f);
		uIElement.MaxWidth.Set(650f, 0f);
		uIElement.Top.Set(220f, 0f);
		uIElement.Height.Set(-220f, 1f);
		uIElement.HAlign = 0.5f;
		UIPanel uIPanel = new UIPanel();
		uIPanel.Width.Set(0f, 1f);
		uIPanel.Height.Set(-110f, 1f);
		uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
		uIElement.Append(uIPanel);
		_containerPanel = uIPanel;
		_entryList = new UIList();
		_entryList.Width.Set(0f, 1f);
		_entryList.Height.Set(0f, 1f);
		_entryList.ListPadding = 5f;
		uIPanel.Append(_entryList);
		_scrollbar = new UIScrollbar();
		_scrollbar.SetView(100f, 1000f);
		_scrollbar.Height.Set(0f, 1f);
		_scrollbar.HAlign = 1f;
		_entryList.SetScrollbar(_scrollbar);
		UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.WorkshopSelectResourcePackToPublishMenuTitle"), 0.8f, large: true);
		uITextPanel.HAlign = 0.5f;
		uITextPanel.Top.Set(-40f, 0f);
		uITextPanel.SetPadding(15f);
		uITextPanel.BackgroundColor = new Color(73, 94, 171);
		uIElement.Append(uITextPanel);
		UITextPanel<LocalizedText> uITextPanel2 = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, large: true);
		uITextPanel2.Width.Set(-10f, 0.5f);
		uITextPanel2.Height.Set(50f, 0f);
		uITextPanel2.VAlign = 1f;
		uITextPanel2.HAlign = 0.5f;
		uITextPanel2.Top.Set(-45f, 0f);
		uITextPanel2.OnMouseOver += FadedMouseOver;
		uITextPanel2.OnMouseOut += FadedMouseOut;
		uITextPanel2.OnLeftClick += GoBackClick;
		uIElement.Append(uITextPanel2);
		_backPanel = uITextPanel2;
		Append(uIElement);
	}

	public override void Recalculate()
	{
		if (_scrollbar != null)
		{
			if (_isScrollbarAttached && !_scrollbar.CanScroll)
			{
				_containerPanel.RemoveChild(_scrollbar);
				_isScrollbarAttached = false;
				_entryList.Width.Set(0f, 1f);
			}
			else if (!_isScrollbarAttached && _scrollbar.CanScroll)
			{
				_containerPanel.Append(_scrollbar);
				_isScrollbarAttached = true;
				_entryList.Width.Set(-25f, 1f);
			}
		}
		base.Recalculate();
	}

	private void GoBackClick(UIMouseEvent evt, UIElement listeningElement)
	{
		HandleBackButtonUsage();
	}

	public void HandleBackButtonUsage()
	{
		SoundEngine.PlaySound(11);
		Main.MenuUI.SetState(_menuToGoBackTo);
	}

	private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(12);
		((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
		((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
	}

	private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
	{
		((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.7f;
		((UIPanel)evt.Target).BorderColor = Color.Black;
	}

	public override void OnActivate()
	{
		PopulateEntries();
		if (PlayerInput.UsingGamepadUI)
		{
			UILinkPointNavigator.ChangePoint(3000 + ((_entryList.Count != 0) ? 1 : 0));
		}
	}

	public void PopulateEntries()
	{
		_entries.Clear();
		IOrderedEnumerable<ResourcePack> height = from x in AssetInitializer.CreatePublishableResourcePacksList(Main.instance.Services).AllPacks
			where x.Branding == ResourcePack.BrandingType.None
			orderby x.IsCompressed
			select x;
		_entries.AddRange(height);
		_entryList.Clear();
		int height2 = 0;
		foreach (ResourcePack random2 in _entries)
		{
			_entryList.Add(new UIWorkshopPublishResourcePackListItem(this, random2, height2++, !random2.IsCompressed));
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		if (skipDraw)
		{
			skipDraw = false;
			return;
		}
		base.Draw(spriteBatch);
		SetupGamepadPoints(spriteBatch);
	}

	private void SetupGamepadPoints(SpriteBatch spriteBatch)
	{
		UILinkPointNavigator.Shortcuts.BackButtonCommand = 7;
		int startPoint = 3000;
		UILinkPointNavigator.SetPosition(startPoint, _backPanel.GetInnerDimensions().ToRectangle().Center.ToVector2());
		int endPoint = startPoint;
		UILinkPoint point = UILinkPointNavigator.Points[endPoint];
		point.Unlink();
		point.Right = endPoint;
		float num = 1f / Main.UIScale;
		Rectangle clippingRectangle = _containerPanel.GetClippingRectangle(spriteBatch);
		Vector2 minValue = clippingRectangle.TopLeft() * num;
		Vector2 point2 = clippingRectangle.BottomRight() * num;
		List<SnapPoint> point3 = GetSnapPoints();
		for (int i = 0; i < point3.Count; i++)
		{
			if (!point3[i].Position.Between(minValue, point2))
			{
				point3.Remove(point3[i]);
				i--;
			}
		}
		int num2 = 1;
		SnapPoint[,] array = new SnapPoint[_entryList.Count, num2];
		foreach (SnapPoint item in point3.Where((SnapPoint a) => a.Name == "Publish"))
		{
			array[item.Id, 0] = item;
		}
		endPoint = startPoint + 1;
		int[] array2 = new int[_entryList.Count];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = -1;
		}
		for (int k = 0; k < num2; k++)
		{
			int num3 = -1;
			for (int l = 0; l < array.GetLength(0); l++)
			{
				if (array[l, k] != null)
				{
					point = UILinkPointNavigator.Points[endPoint];
					point.Unlink();
					UILinkPointNavigator.SetPosition(endPoint, array[l, k].Position);
					if (num3 != -1)
					{
						point.Up = num3;
						UILinkPointNavigator.Points[num3].Down = endPoint;
					}
					if (array2[l] != -1)
					{
						point.Left = array2[l];
						UILinkPointNavigator.Points[array2[l]].Right = endPoint;
					}
					point.Down = startPoint;
					if (k == 0)
					{
						UILinkPointNavigator.Points[startPoint].Up = (UILinkPointNavigator.Points[startPoint + 1].Up = endPoint);
					}
					num3 = endPoint;
					array2[l] = endPoint;
					UILinkPointNavigator.Shortcuts.FANCYUI_HIGHEST_INDEX = endPoint;
					endPoint++;
				}
			}
		}
		if (PlayerInput.UsingGamepadUI && _entryList.Count == 0 && UILinkPointNavigator.CurrentPoint > 3000)
		{
			UILinkPointNavigator.ChangePoint(3000);
		}
	}
}
