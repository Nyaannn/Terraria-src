using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Animations;
using Terraria.GameContent.Skies.CreditsRoll;
using Terraria.Graphics.Effects;

namespace Terraria.GameContent.Skies;

public class CreditsRollSky : CustomSky
{
	private int _endTime;

	private int _currentTime;

	private CreditsRollComposer _composer = new CreditsRollComposer();

	private List<IAnimationSegment> _segmentsInGame = new List<IAnimationSegment>();

	private List<IAnimationSegment> _segmentsInMainMenu = new List<IAnimationSegment>();

	private bool _isActive;

	private bool _wantsToBeSeen;

	private float _opacity;

	public int AmountOfTimeNeededForFullPlay => _endTime;

	public CreditsRollSky()
	{
		EnsureSegmentsAreMade();
	}

	public override void Update(GameTime gameTime)
	{
		if (Main.gamePaused || !Main.hasFocus)
		{
			return;
		}
		_currentTime++;
		float tileSafely = 1f / 120f;
		if (Main.gameMenu)
		{
			tileSafely = 1f / 15f;
		}
		_opacity = MathHelper.Clamp(_opacity + tileSafely * (float)_wantsToBeSeen.ToDirectionInt(), 0f, 1f);
		if (_opacity == 0f && !_wantsToBeSeen)
		{
			_isActive = false;
			return;
		}
		bool point = true;
		if (!Main.CanPlayCreditsRoll())
		{
			point = false;
		}
		if (_currentTime >= _endTime)
		{
			point = false;
		}
		if (!point)
		{
			SkyManager.Instance.Deactivate("CreditsRoll");
		}
	}

	public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
	{
		float xLeftEnd = 1f;
		if (!(xLeftEnd < minDepth) && !(xLeftEnd > maxDepth))
		{
			Vector2 xRightEnd = Main.ScreenSize.ToVector2() / 2f;
			if (Main.gameMenu)
			{
				xRightEnd.Y = 300f;
			}
			GameAnimationSegment gameAnimationSegment = default(GameAnimationSegment);
			gameAnimationSegment.SpriteBatch = spriteBatch;
			gameAnimationSegment.AnchorPositionOnScreen = xRightEnd;
			gameAnimationSegment.TimeInAnimation = _currentTime;
			gameAnimationSegment.DisplayOpacity = _opacity;
			GameAnimationSegment num2 = gameAnimationSegment;
			List<IAnimationSegment> list = _segmentsInGame;
			if (Main.gameMenu)
			{
				list = _segmentsInMainMenu;
			}
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Draw(ref num2);
			}
		}
	}

	public override bool IsActive()
	{
		return _isActive;
	}

	public override void Reset()
	{
		_currentTime = 0;
		EnsureSegmentsAreMade();
		_isActive = false;
		_wantsToBeSeen = false;
	}

	public override void Activate(Vector2 position, params object[] args)
	{
		_isActive = true;
		_wantsToBeSeen = true;
		if (_opacity == 0f)
		{
			EnsureSegmentsAreMade();
			_currentTime = 0;
		}
	}

	private void EnsureSegmentsAreMade()
	{
		if (_segmentsInMainMenu.Count <= 0 || _segmentsInGame.Count <= 0)
		{
			_segmentsInGame.Clear();
			_composer.FillSegments(_segmentsInGame, out _endTime, inGame: true);
			_segmentsInMainMenu.Clear();
			_composer.FillSegments(_segmentsInMainMenu, out _endTime, inGame: false);
		}
	}

	public override void Deactivate(params object[] args)
	{
		_wantsToBeSeen = false;
	}
}
