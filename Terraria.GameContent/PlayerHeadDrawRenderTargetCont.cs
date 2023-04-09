using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Terraria.GameContent;

public class PlayerHeadDrawRenderTargetContent : AnOutlinedDrawRenderTargetContent
{
	private Player _player;

	private readonly List<DrawData> _drawData = new List<DrawData>();

	private readonly List<int> _dust = new List<int>();

	private readonly List<int> _gore = new List<int>();

	public void UsePlayer(Player player)
	{
		_player = player;
	}

	internal override void DrawTheContent(SpriteBatch spriteBatch)
	{
		if (_player != null && !_player.ShouldNotDraw)
		{
			_drawData.Clear();
			_dust.Clear();
			_gore.Clear();
			PlayerDrawHeadSet xPositionStart = default(PlayerDrawHeadSet);
			xPositionStart.BoringSetup(_player, _drawData, _dust, _gore, width / 2, height / 2, 1f, 1f);
			PlayerDrawHeadLayers.DrawPlayer_00_BackHelmet(ref xPositionStart);
			PlayerDrawHeadLayers.DrawPlayer_01_FaceSkin(ref xPositionStart);
			PlayerDrawHeadLayers.DrawPlayer_02_DrawArmorWithFullHair(ref xPositionStart);
			PlayerDrawHeadLayers.DrawPlayer_03_HelmetHair(ref xPositionStart);
			PlayerDrawHeadLayers.DrawPlayer_04_HatsWithFullHair(ref xPositionStart);
			PlayerDrawHeadLayers.DrawPlayer_05_TallHats(ref xPositionStart);
			PlayerDrawHeadLayers.DrawPlayer_06_NormalHats(ref xPositionStart);
			PlayerDrawHeadLayers.DrawPlayer_07_JustHair(ref xPositionStart);
			PlayerDrawHeadLayers.DrawPlayer_08_FaceAcc(ref xPositionStart);
			PlayerDrawHeadLayers.DrawPlayer_RenderAllLayers(ref xPositionStart);
		}
	}
}
