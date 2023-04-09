using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;

namespace Terraria.Graphics.Shaders;

public class MiscShaderData : ShaderData
{
	private Vector3 _uColor = Vector3.One;

	private Vector3 _uSecondaryColor = Vector3.One;

	private float _uSaturation = 1f;

	private float _uOpacity = 1f;

	private Asset<Texture2D> _uImage0;

	private Asset<Texture2D> _uImage1;

	private Asset<Texture2D> _uImage2;

	private bool _useProjectionMatrix;

	private Vector4 _shaderSpecificData = Vector4.Zero;

	private SamplerState _customSamplerState;

	public MiscShaderData(Ref<Effect> shader, string passName)
		: base(shader, passName)
	{
	}

	public virtual void Apply(DrawData? drawData = null)
	{
		base.Shader.Parameters["uColor"].SetValue(_uColor);
		base.Shader.Parameters["uSaturation"].SetValue(_uSaturation);
		base.Shader.Parameters["uSecondaryColor"].SetValue(_uSecondaryColor);
		base.Shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
		base.Shader.Parameters["uOpacity"].SetValue(_uOpacity);
		base.Shader.Parameters["uShaderSpecificData"].SetValue(_shaderSpecificData);
		if (drawData.HasValue)
		{
			DrawData num = drawData.Value;
			Vector4 num2 = Vector4.Zero;
			if (drawData.Value.sourceRect.HasValue)
			{
				num2 = new Vector4(num.sourceRect.Value.X, num.sourceRect.Value.Y, num.sourceRect.Value.Width, num.sourceRect.Value.Height);
			}
			base.Shader.Parameters["uSourceRect"].SetValue(num2);
			base.Shader.Parameters["uWorldPosition"].SetValue(Main.screenPosition + num.position);
			base.Shader.Parameters["uImageSize0"].SetValue(new Vector2(num.texture.Width, num.texture.Height));
		}
		else
		{
			base.Shader.Parameters["uSourceRect"].SetValue(new Vector4(0f, 0f, 4f, 4f));
		}
		SamplerState value = SamplerState.LinearWrap;
		if (_customSamplerState != null)
		{
			value = _customSamplerState;
		}
		if (_uImage0 != null)
		{
			Main.graphics.GraphicsDevice.Textures[0] = _uImage0.get_Value();
			Main.graphics.GraphicsDevice.SamplerStates[0] = value;
			base.Shader.Parameters["uImageSize0"].SetValue(new Vector2(_uImage0.Width(), _uImage0.Height()));
		}
		if (_uImage1 != null)
		{
			Main.graphics.GraphicsDevice.Textures[1] = _uImage1.get_Value();
			Main.graphics.GraphicsDevice.SamplerStates[1] = value;
			base.Shader.Parameters["uImageSize1"].SetValue(new Vector2(_uImage1.Width(), _uImage1.Height()));
		}
		if (_uImage2 != null)
		{
			Main.graphics.GraphicsDevice.Textures[2] = _uImage2.get_Value();
			Main.graphics.GraphicsDevice.SamplerStates[2] = value;
			base.Shader.Parameters["uImageSize2"].SetValue(new Vector2(_uImage2.Width(), _uImage2.Height()));
		}
		_ = _useProjectionMatrix;
		base.Apply();
	}

	public MiscShaderData UseColor(float r, float g, float b)
	{
		return UseColor(new Vector3(r, g, b));
	}

	public MiscShaderData UseColor(Color color)
	{
		return UseColor(color.ToVector3());
	}

	public MiscShaderData UseColor(Vector3 color)
	{
		_uColor = color;
		return this;
	}

	public MiscShaderData UseSamplerState(SamplerState state)
	{
		_customSamplerState = state;
		return this;
	}

	public MiscShaderData UseImage0(string path)
	{
		_uImage0 = Main.Assets.Request<Texture2D>(path, (AssetRequestMode)1);
		return this;
	}

	public MiscShaderData UseImage1(string path)
	{
		_uImage1 = Main.Assets.Request<Texture2D>(path, (AssetRequestMode)1);
		return this;
	}

	public MiscShaderData UseImage2(string path)
	{
		_uImage2 = Main.Assets.Request<Texture2D>(path, (AssetRequestMode)1);
		return this;
	}

	private static bool IsPowerOfTwo(int n)
	{
		return (int)Math.Ceiling(Math.Log(n) / Math.Log(2.0)) == (int)Math.Floor(Math.Log(n) / Math.Log(2.0));
	}

	public MiscShaderData UseOpacity(float alpha)
	{
		_uOpacity = alpha;
		return this;
	}

	public MiscShaderData UseSecondaryColor(float r, float g, float b)
	{
		return UseSecondaryColor(new Vector3(r, g, b));
	}

	public MiscShaderData UseSecondaryColor(Color color)
	{
		return UseSecondaryColor(color.ToVector3());
	}

	public MiscShaderData UseSecondaryColor(Vector3 color)
	{
		_uSecondaryColor = color;
		return this;
	}

	public MiscShaderData UseProjectionMatrix(bool doUse)
	{
		_useProjectionMatrix = doUse;
		return this;
	}

	public MiscShaderData UseSaturation(float saturation)
	{
		_uSaturation = saturation;
		return this;
	}

	public virtual MiscShaderData GetSecondaryShader(Entity entity)
	{
		return this;
	}

	public MiscShaderData UseShaderSpecificData(Vector4 specificData)
	{
		_shaderSpecificData = specificData;
		return this;
	}
}
