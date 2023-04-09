using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.GameContent.UI.States;

public class UIWorkshopSelectWorldToPublish : UIState, IHaveBackButtonCommand
{
	private UIList _entryList;

	private UITextPanel<LocalizedText> _backPanel;

	private UIPanel _containerPanel;

	private UIScrollbar _scrollbar;

	private bool _isScrollbarAttached;

	private UIState _menuToGoBackTo;

	private bool skipDraw;

	public UIWorkshopSelectWorldToPublish(UIState menuToGoBackTo)
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
		UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.WorkshopSelectWorldToPublishMenuTitle"), 0.8f, large: true);
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

	private void PopulateEntries()
	{
		Main.LoadWorlds();
		_entryList.Clear();
		IOrderedEnumerable<WorldFileData> orderedEnumerable = from x in new List<WorldFileData>(Main.WorldList)
			orderby x.IsFavorite descending, x.Name, x.GetFileName()
			select x;
		_entryList.Clear();
		int length = 0;
		foreach (WorldFileData num2 in orderedEnumerable)
		{
			_entryList.Add(new UIWorkshopPublishWorldListItem(this, num2, length++));
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
		int wall = 3000;
		UILinkPointNavigator.SetPosition(wall, _backPanel.GetInnerDimensions().ToRectangle().Center.ToVector2());
		int list = wall;
		UILinkPoint i = UILinkPointNavigator.Points[list];
		i.Unlink();
		i.Right = list;
		float j = 1f / Main.UIScale;
		Rectangle clippingRectangle = _containerPanel.GetClippingRectangle(spriteBatch);
		Vector2 num = clippingRectangle.TopLeft() * j;
		Vector2 num2 = clippingRectangle.BottomRight() * j;
		List<SnapPoint> num3 = GetSnapPoints();
		for (int m = 0; m < num3.Count; m++)
		{
			if (!num3[m].Position.Between(num, num2))
			{
				num3.Remove(num3[m]);
				m--;
			}
		}
		int k = 1;
		SnapPoint[,] l = new SnapPoint[_entryList.Count, k];
		foreach (SnapPoint item in num3.Where((SnapPoint a) => a.Name == "Publish"))
		{
			l[item.Id, 0] = item;
		}
		list = wall + 1;
		int[] array = new int[_entryList.Count];
		for (int n = 0; n < array.Length; n++)
		{
			array[n] = -1;
		}
		for (int num4 = 0; num4 < k; num4++)
		{
			int num5 = -1;
			for (int num6 = 0; num6 < l.GetLength(0); num6++)
			{
				if (l[num6, num4] != null)
				{
					i = UILinkPointNavigator.Points[list];
					i.Unlink();
					UILinkPointNavigator.SetPosition(list, l[num6, num4].Position);
					if (num5 != -1)
					{
						i.Up = num5;
						UILinkPointNavigator.Points[num5].Down = list;
					}
					if (array[num6] != -1)
					{
						i.Left = array[num6];
						UILinkPointNavigator.Points[array[num6]].Right = list;
					}
					i.Down = wall;
					if (num4 == 0)
					{
						UILinkPointNavigator.Points[wall].Up = (UILinkPointNavigator.Points[wall + 1].Up = list);
					}
					num5 = list;
					array[num6] = list;
					UILinkPointNavigator.Shortcuts.FANCYUI_HIGHEST_INDEX = list;
					list++;
				}
			}
		}
		if (PlayerInput.UsingGamepadUI && _entryList.Count == 0 && UILinkPointNavigator.CurrentPoint > 3000)
		{
			UILinkPointNavigator.ChangePoint(3000);
		}
	}
}
