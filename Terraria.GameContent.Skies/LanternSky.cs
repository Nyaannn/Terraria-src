using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace Terraria.GameContent.Skies;

public class LanternSky : CustomSky
{
	private struct Lantern
	{
		private const int MAX_FRAMES_X = 3;

		public int Variant;

		public int TimeUntilFloat;

		public int TimeUntilFloatMax;

		private Texture2D _texture;

		public Vector2 Position;

		public float Depth;

		public float Rotation;

		public int FrameHeight;

		public int FrameWidth;

		public float Speed;

		public bool Active;

		public Texture2D Texture
		{
			get
			{
				return _texture;
			}
			set
			{
				_texture = value;
				FrameWidth = value.Width / 3;
				FrameHeight = value.Height;
			}
		}

		public float FloatAdjustedSpeed => Speed * ((float)TimeUntilFloat / (float)TimeUntilFloatMax);

		public Rectangle GetSourceRectangle()
		{
			return new Rectangle(FrameWidth * Variant, 0, FrameWidth, FrameHeight);
		}
	}

	private bool _active;

	private bool _leaving;

	private float _opacity;

	private Asset<Texture2D> _texture;

	private Lantern[] _lanterns;

	private UnifiedRandom _random = new UnifiedRandom();

	private int _lanternsDrawing;

	private const float slowDown = 0.5f;

	public override void OnLoad()
	{
		_texture = TextureAssets.Extra[134];
		GenerateLanterns(onlyMissing: false);
	}

	private void GenerateLanterns(bool onlyMissing)
	{
		if (!onlyMissing)
		{
			_lanterns = new Lantern[Main.maxTilesY / 4];
		}
		for (int i = 0; i < _lanterns.Length; i++)
		{
			if (!onlyMissing || !_lanterns[i].Active)
			{
				int num = (int)((double)Main.screenPosition.Y * 0.7 - (double)Main.screenHeight);
				int minValue = (int)((double)num - Main.worldSurface * 16.0);
				_lanterns[i].Position = new Vector2(_random.Next(0, Main.maxTilesX) * 16, _random.Next(minValue, num));
				ResetLantern(i);
				_lanterns[i].Active = true;
			}
		}
		_lanternsDrawing = _lanterns.Length;
	}

	public void ResetLantern(int i)
	{
		_lanterns[i].Depth = (1f - (float)i / (float)_lanterns.Length) * 4.4f + 1.6f;
		_lanterns[i].Speed = -1.5f - 2.5f * (float)_random.NextDouble();
		_lanterns[i].Texture = _texture.get_Value();
		_lanterns[i].Variant = _random.Next(3);
		_lanterns[i].TimeUntilFloat = (int)((float)(2000 + _random.Next(1200)) * 2f);
		_lanterns[i].TimeUntilFloatMax = _lanterns[i].TimeUntilFloat;
	}

	public override void Update(GameTime gameTime)
	{
		if (Main.gamePaused || !Main.hasFocus)
		{
			return;
		}
		_opacity = Utils.Clamp(_opacity + (float)LanternNight.LanternsUp.ToDirectionInt() * 0.01f, 0f, 1f);
		for (int list = 0; list < _lanterns.Length; list++)
		{
			if (!_lanterns[list].Active)
			{
				continue;
			}
			float num = Main.windSpeedCurrent;
			if (num == 0f)
			{
				num = 0.1f;
			}
			float num2 = (float)Math.Sin(_lanterns[list].Position.X / 120f) * 0.5f;
			_lanterns[list].Position.Y += num2 * 0.5f;
			_lanterns[list].Position.Y += _lanterns[list].FloatAdjustedSpeed * 0.5f;
			_lanterns[list].Position.X += (0.1f + num) * (3f - _lanterns[list].Speed) * 0.5f * ((float)list / (float)_lanterns.Length + 1.5f) / 2.5f;
			_lanterns[list].Rotation = num2 * (float)((!(num < 0f)) ? 1 : (-1)) * 0.5f;
			_lanterns[list].TimeUntilFloat = Math.Max(0, _lanterns[list].TimeUntilFloat - 1);
			if (_lanterns[list].Position.Y < 300f)
			{
				if (!_leaving)
				{
					ResetLantern(list);
					_lanterns[list].Position = new Vector2(_random.Next(0, Main.maxTilesX) * 16, (float)Main.worldSurface * 16f + 1600f);
				}
				else
				{
					_lanterns[list].Active = false;
					_lanternsDrawing--;
				}
			}
		}
		_active = true;
	}

	public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
	{
		if (Main.gameMenu && _active)
		{
			_active = false;
			_leaving = false;
			for (int i = 0; i < _lanterns.Length; i++)
			{
				_lanterns[i].Active = false;
			}
		}
		if ((double)Main.screenPosition.Y > Main.worldSurface * 16.0 || Main.gameMenu || _opacity <= 0f)
		{
			return;
		}
		int value = -1;
		int value2 = 0;
		for (int nPC = 0; nPC < _lanterns.Length; nPC++)
		{
			float topLeft = _lanterns[nPC].Depth;
			if (value == -1 && topLeft < maxDepth)
			{
				value = nPC;
			}
			if (topLeft <= minDepth)
			{
				break;
			}
			value2 = nPC;
		}
		if (value == -1)
		{
			return;
		}
		Vector2 vector = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
		Rectangle vector2 = new Rectangle(-1000, -1000, 4000, 4000);
		for (int j = value; j < value2; j++)
		{
			if (_lanterns[j].Active)
			{
				Color color = new Color(250, 120, 60, 120);
				float num = 1f;
				if (_lanterns[j].Depth > 5f)
				{
					num = 0.3f;
				}
				else if ((double)_lanterns[j].Depth > 4.5)
				{
					num = 0.4f;
				}
				else if (_lanterns[j].Depth > 4f)
				{
					num = 0.5f;
				}
				else if ((double)_lanterns[j].Depth > 3.5)
				{
					num = 0.6f;
				}
				else if (_lanterns[j].Depth > 3f)
				{
					num = 0.7f;
				}
				else if ((double)_lanterns[j].Depth > 2.5)
				{
					num = 0.8f;
				}
				else if (_lanterns[j].Depth > 2f)
				{
					num = 0.9f;
				}
				color = new Color((int)((float)(int)color.R * num), (int)((float)(int)color.G * num), (int)((float)(int)color.B * num), (int)((float)(int)color.A * num));
				Vector2 vector3 = new Vector2(1f / _lanterns[j].Depth, 0.9f / _lanterns[j].Depth);
				vector3 *= 1.2f;
				Vector2 position = _lanterns[j].Position;
				position = (position - vector) * vector3 + vector - Main.screenPosition;
				position.X = (position.X + 500f) % 4000f;
				if (position.X < 0f)
				{
					position.X += 4000f;
				}
				position.X -= 500f;
				if (vector2.Contains((int)position.X, (int)position.Y))
				{
					DrawLantern(spriteBatch, _lanterns[j], color, vector3, position, num);
				}
			}
		}
	}

	private void DrawLantern(SpriteBatch spriteBatch, Lantern lantern, Color opacity, Vector2 depthScale, Vector2 position, float alpha)
	{
		float y = (Main.GlobalTimeWrappedHourly % 6f / 6f * ((float)Math.PI * 2f)).ToRotationVector2().Y;
		float num = y * 0.2f + 0.8f;
		Color color = new Color(255, 255, 255, 0) * _opacity * alpha * num * 0.4f;
		for (float num2 = 0f; num2 < 1f; num2 += 1f / 3f)
		{
			Vector2 vector = new Vector2(0f, 2f).RotatedBy((float)Math.PI * 2f * num2 + lantern.Rotation) * y;
			spriteBatch.Draw(lantern.Texture, position + vector, lantern.GetSourceRectangle(), color, lantern.Rotation, lantern.GetSourceRectangle().Size() / 2f, depthScale.X * 2f, SpriteEffects.None, 0f);
		}
		spriteBatch.Draw(lantern.Texture, position, lantern.GetSourceRectangle(), opacity * _opacity, lantern.Rotation, lantern.GetSourceRectangle().Size() / 2f, depthScale.X * 2f, SpriteEffects.None, 0f);
	}

	public override void Activate(Vector2 position, params object[] args)
	{
		if (_active)
		{
			_leaving = false;
			GenerateLanterns(onlyMissing: true);
		}
		else
		{
			GenerateLanterns(onlyMissing: false);
			_active = true;
			_leaving = false;
		}
	}

	public override void Deactivate(params object[] args)
	{
		_leaving = true;
	}

	public override bool IsActive()
	{
		return _active;
	}

	public override void Reset()
	{
		_active = false;
	}
}
