using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terraria.GameContent;

public abstract class AnOutlinedDrawRenderTargetContent : ARenderTargetContentByRequest
{
	protected int width = 84;

	protected int height = 84;

	public Color _borderColor = Color.White;

	private EffectPass _coloringShader;

	private RenderTarget2D _helperTarget;

	public void UseColor(Color color)
	{
		_borderColor = color;
	}

	protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
	{
		Effect num = Main.pixelShader;
		if (_coloringShader == null)
		{
			_coloringShader = num.CurrentTechnique.Passes["ColorOnly"];
		}
		new Rectangle(0, 0, width, height);
		PrepareARenderTarget_AndListenToEvents(ref _target, device, width, height, RenderTargetUsage.PreserveContents);
		PrepareARenderTarget_WithoutListeningToEvents(ref _helperTarget, device, width, height, RenderTargetUsage.DiscardContents);
		device.SetRenderTarget(_helperTarget);
		device.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
		DrawTheContent(spriteBatch);
		spriteBatch.End();
		device.SetRenderTarget(_target);
		device.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
		_coloringShader.Apply();
		int uIElement = 2;
		int uIPanel = uIElement * 2;
		for (int uIElement2 = -uIPanel; uIElement2 <= uIPanel; uIElement2 += uIElement)
		{
			for (int uIElement3 = -uIPanel; uIElement3 <= uIPanel; uIElement3 += uIElement)
			{
				if (Math.Abs(uIElement2) + Math.Abs(uIElement3) == uIPanel)
				{
					spriteBatch.Draw(_helperTarget, new Vector2(uIElement2, uIElement3), Color.Black);
				}
			}
		}
		uIPanel = uIElement;
		for (int i = -uIPanel; i <= uIPanel; i += uIElement)
		{
			for (int j = -uIPanel; j <= uIPanel; j += uIElement)
			{
				if (Math.Abs(i) + Math.Abs(j) == uIPanel)
				{
					spriteBatch.Draw(_helperTarget, new Vector2(i, j), _borderColor);
				}
			}
		}
		num.CurrentTechnique.Passes[0].Apply();
		spriteBatch.Draw(_helperTarget, Vector2.Zero, Color.White);
		spriteBatch.End();
		device.SetRenderTarget(null);
		_wasPrepared = true;
	}

	internal abstract void DrawTheContent(SpriteBatch spriteBatch);
}
