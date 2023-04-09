using System;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.Utilities;

namespace Terraria;

public class Lighting
{
	private const float DEFAULT_GLOBAL_BRIGHTNESS = 1.2f;

	private const float BLIND_GLOBAL_BRIGHTNESS = 1f;

	[Old]
	public static int OffScreenTiles = 45;

	private static LightMode _mode = LightMode.Color;

	private static readonly LightingEngine NewEngine = new LightingEngine();

	private static readonly LegacyLighting LegacyEngine = new LegacyLighting(Main.Camera);

	private static ILightingEngine _activeEngine;

	public static float GlobalBrightness { get; set; }

	public static LightMode Mode
	{
		get
		{
			return _mode;
		}
		set
		{
			_mode = value;
			switch (_mode)
			{
			case LightMode.Color:
				_activeEngine = NewEngine;
				LegacyEngine.Mode = 0;
				OffScreenTiles = 35;
				break;
			case LightMode.White:
				_activeEngine = LegacyEngine;
				LegacyEngine.Mode = 1;
				break;
			case LightMode.Retro:
				_activeEngine = LegacyEngine;
				LegacyEngine.Mode = 2;
				break;
			case LightMode.Trippy:
				_activeEngine = LegacyEngine;
				LegacyEngine.Mode = 3;
				break;
			}
			Main.renderCount = 0;
			Main.renderNow = false;
		}
	}

	public static bool NotRetro
	{
		get
		{
			if (Mode != LightMode.Retro)
			{
				return Mode != LightMode.Trippy;
			}
			return false;
		}
	}

	public static bool UsingNewLighting => Mode == LightMode.Color;

	public static bool UpdateEveryFrame
	{
		get
		{
			if (Main.LightingEveryFrame && !Main.RenderTargetsRequired)
			{
				return !NotRetro;
			}
			return false;
		}
	}

	public static void Initialize()
	{
		GlobalBrightness = 1.2f;
		NewEngine.Rebuild();
		LegacyEngine.Rebuild();
		if (_activeEngine == null)
		{
			Mode = LightMode.Color;
		}
	}

	public static void LightTiles(int firstX, int lastX, int firstY, int lastY)
	{
		Main.render = true;
		UpdateGlobalBrightness();
		_activeEngine.ProcessArea(new Rectangle(firstX, firstY, lastX - firstX, lastY - firstY));
	}

	private static void UpdateGlobalBrightness()
	{
		GlobalBrightness = 1.2f;
		if (Main.player[Main.myPlayer].blind)
		{
			GlobalBrightness = 1f;
		}
	}

	public static float Brightness(int x, int y)
	{
		Vector3 playerFileData = _activeEngine.GetColor(x, y);
		return GlobalBrightness * (playerFileData.X + playerFileData.Y + playerFileData.Z) / 3f;
	}

	public static Vector3 GetSubLight(Vector2 position)
	{
		Vector2 num = position / 16f - new Vector2(0.5f, 0.5f);
		Vector2 vector = new Vector2(num.X % 1f, num.Y % 1f);
		int num2 = (int)num.X;
		int num3 = (int)num.Y;
		Vector3 color = _activeEngine.GetColor(num2, num3);
		Vector3 color2 = _activeEngine.GetColor(num2 + 1, num3);
		Vector3 color3 = _activeEngine.GetColor(num2, num3 + 1);
		Vector3 color4 = _activeEngine.GetColor(num2 + 1, num3 + 1);
		Vector3 value = Vector3.Lerp(color, color2, vector.X);
		Vector3 value2 = Vector3.Lerp(color3, color4, vector.X);
		return Vector3.Lerp(value, value2, vector.Y);
	}

	public static void AddLight(Vector2 position, Vector3 rgb)
	{
		AddLight((int)(position.X / 16f), (int)(position.Y / 16f), rgb.X, rgb.Y, rgb.Z);
	}

	public static void AddLight(Vector2 position, float r, float g, float b)
	{
		AddLight((int)(position.X / 16f), (int)(position.Y / 16f), r, g, b);
	}

	public static void AddLight(int i, int j, int torchID, float lightAmount)
	{
		TorchID.TorchColor(torchID, out var color, out var G, out var B);
		_activeEngine.AddLight(i, j, new Vector3(color * lightAmount, G * lightAmount, B * lightAmount));
	}

	public static void AddLight(Vector2 position, int torchID)
	{
		TorchID.TorchColor(torchID, out var i, out var G, out var B);
		AddLight((int)position.X / 16, (int)position.Y / 16, i, G, B);
	}

	public static void AddLight(int i, int j, float r, float g, float b)
	{
		if (!Main.gamePaused && Main.netMode != 2)
		{
			_activeEngine.AddLight(i, j, new Vector3(r, g, b));
		}
	}

	public static void NextLightMode()
	{
		Mode++;
		if (!Enum.IsDefined(typeof(LightMode), Mode))
		{
			Mode = LightMode.White;
		}
		Clear();
	}

	public static void Clear()
	{
		_activeEngine.Clear();
	}

	public static Color GetColor(Point tileCoords)
	{
		if (Main.gameMenu)
		{
			return Color.White;
		}
		return new Color(_activeEngine.GetColor(tileCoords.X, tileCoords.Y) * GlobalBrightness);
	}

	public static Color GetColor(Point tileCoords, Color originalColor)
	{
		if (Main.gameMenu)
		{
			return originalColor;
		}
		return new Color(_activeEngine.GetColor(tileCoords.X, tileCoords.Y) * originalColor.ToVector3());
	}

	public static Color GetColor(int x, int y, Color oldColor)
	{
		if (Main.gameMenu)
		{
			return oldColor;
		}
		return new Color(_activeEngine.GetColor(x, y) * oldColor.ToVector3());
	}

	public static Color GetColorClamped(int x, int y, Color oldColor)
	{
		if (Main.gameMenu)
		{
			return oldColor;
		}
		Vector3 canSpawn = _activeEngine.GetColor(x, y);
		canSpawn = Vector3.Min(Vector3.One, canSpawn);
		return new Color(canSpawn * oldColor.ToVector3());
	}

	public static Color GetColor(int x, int y)
	{
		if (Main.gameMenu)
		{
			return Color.White;
		}
		Color num = default(Color);
		Vector3 color = _activeEngine.GetColor(x, y);
		float num2 = GlobalBrightness * 255f;
		int num3 = (int)(color.X * num2);
		int width = (int)(color.Y * num2);
		int vector = (int)(color.Z * num2);
		if (num3 > 255)
		{
			num3 = 255;
		}
		if (width > 255)
		{
			width = 255;
		}
		if (vector > 255)
		{
			vector = 255;
		}
		vector <<= 16;
		width <<= 8;
		num.PackedValue = (uint)(num3 | width | vector) | 0xFF000000u;
		return num;
	}

	public static void GetColor9Slice(int centerX, int centerY, ref Color[] slices)
	{
		int item = 0;
		for (int num = centerX - 1; num <= centerX + 1; num++)
		{
			for (int num2 = centerY - 1; num2 <= centerY + 1; num2++)
			{
				Vector3 nPCEntityToPlayerInventorySettings = _activeEngine.GetColor(num, num2);
				int item2 = (int)(255f * nPCEntityToPlayerInventorySettings.X * GlobalBrightness);
				int item3 = (int)(255f * nPCEntityToPlayerInventorySettings.Y * GlobalBrightness);
				int num3 = (int)(255f * nPCEntityToPlayerInventorySettings.Z * GlobalBrightness);
				if (item2 > 255)
				{
					item2 = 255;
				}
				if (item3 > 255)
				{
					item3 = 255;
				}
				if (num3 > 255)
				{
					num3 = 255;
				}
				num3 <<= 16;
				item3 <<= 8;
				slices[item].PackedValue = (uint)(item2 | item3 | num3) | 0xFF000000u;
				item += 3;
			}
			item -= 8;
		}
	}

	public static void GetColor9Slice(int x, int y, ref Vector3[] slices)
	{
		slices[0] = _activeEngine.GetColor(x - 1, y - 1) * GlobalBrightness;
		slices[3] = _activeEngine.GetColor(x - 1, y) * GlobalBrightness;
		slices[6] = _activeEngine.GetColor(x - 1, y + 1) * GlobalBrightness;
		slices[1] = _activeEngine.GetColor(x, y - 1) * GlobalBrightness;
		slices[4] = _activeEngine.GetColor(x, y) * GlobalBrightness;
		slices[7] = _activeEngine.GetColor(x, y + 1) * GlobalBrightness;
		slices[2] = _activeEngine.GetColor(x + 1, y - 1) * GlobalBrightness;
		slices[5] = _activeEngine.GetColor(x + 1, y) * GlobalBrightness;
		slices[8] = _activeEngine.GetColor(x + 1, y + 1) * GlobalBrightness;
	}

	public static void GetCornerColors(int centerX, int centerY, out VertexColors vertices, float scale = 1f)
	{
		vertices = default(VertexColors);
		Vector3 color = _activeEngine.GetColor(centerX, centerY);
		Vector3 color2 = _activeEngine.GetColor(centerX, centerY - 1);
		Vector3 color3 = _activeEngine.GetColor(centerX, centerY + 1);
		Vector3 color4 = _activeEngine.GetColor(centerX - 1, centerY);
		Vector3 color5 = _activeEngine.GetColor(centerX + 1, centerY);
		Vector3 color6 = _activeEngine.GetColor(centerX - 1, centerY - 1);
		Vector3 color7 = _activeEngine.GetColor(centerX + 1, centerY - 1);
		Vector3 color8 = _activeEngine.GetColor(centerX - 1, centerY + 1);
		Vector3 color9 = _activeEngine.GetColor(centerX + 1, centerY + 1);
		float num = GlobalBrightness * scale * 63.75f;
		int num2 = (int)((color2.X + color6.X + color4.X + color.X) * num);
		int num3 = (int)((color2.Y + color6.Y + color4.Y + color.Y) * num);
		int num4 = (int)((color2.Z + color6.Z + color4.Z + color.Z) * num);
		if (num2 > 255)
		{
			num2 = 255;
		}
		if (num3 > 255)
		{
			num3 = 255;
		}
		if (num4 > 255)
		{
			num4 = 255;
		}
		num3 <<= 8;
		num4 <<= 16;
		vertices.TopLeftColor.PackedValue = (uint)(num2 | num3 | num4) | 0xFF000000u;
		num2 = (int)((color2.X + color7.X + color5.X + color.X) * num);
		num3 = (int)((color2.Y + color7.Y + color5.Y + color.Y) * num);
		num4 = (int)((color2.Z + color7.Z + color5.Z + color.Z) * num);
		if (num2 > 255)
		{
			num2 = 255;
		}
		if (num3 > 255)
		{
			num3 = 255;
		}
		if (num4 > 255)
		{
			num4 = 255;
		}
		num3 <<= 8;
		num4 <<= 16;
		vertices.TopRightColor.PackedValue = (uint)(num2 | num3 | num4) | 0xFF000000u;
		num2 = (int)((color3.X + color8.X + color4.X + color.X) * num);
		num3 = (int)((color3.Y + color8.Y + color4.Y + color.Y) * num);
		num4 = (int)((color3.Z + color8.Z + color4.Z + color.Z) * num);
		if (num2 > 255)
		{
			num2 = 255;
		}
		if (num3 > 255)
		{
			num3 = 255;
		}
		if (num4 > 255)
		{
			num4 = 255;
		}
		num3 <<= 8;
		num4 <<= 16;
		vertices.BottomLeftColor.PackedValue = (uint)(num2 | num3 | num4) | 0xFF000000u;
		num2 = (int)((color3.X + color9.X + color5.X + color.X) * num);
		num3 = (int)((color3.Y + color9.Y + color5.Y + color.Y) * num);
		num4 = (int)((color3.Z + color9.Z + color5.Z + color.Z) * num);
		if (num2 > 255)
		{
			num2 = 255;
		}
		if (num3 > 255)
		{
			num3 = 255;
		}
		if (num4 > 255)
		{
			num4 = 255;
		}
		num3 <<= 8;
		num4 <<= 16;
		vertices.BottomRightColor.PackedValue = (uint)(num2 | num3 | num4) | 0xFF000000u;
	}

	public static void GetColor4Slice(int centerX, int centerY, ref Color[] slices)
	{
		Vector3 item = _activeEngine.GetColor(centerX, centerY - 1);
		Vector3 item2 = _activeEngine.GetColor(centerX, centerY + 1);
		Vector3 number = _activeEngine.GetColor(centerX - 1, centerY);
		Vector3 color = _activeEngine.GetColor(centerX + 1, centerY);
		float num = item.X + item.Y + item.Z;
		float num2 = item2.X + item2.Y + item2.Z;
		float num3 = color.X + color.Y + color.Z;
		float num4 = number.X + number.Y + number.Z;
		if (num >= num4)
		{
			int num5 = (int)(255f * number.X * GlobalBrightness);
			int num6 = (int)(255f * number.Y * GlobalBrightness);
			int num7 = (int)(255f * number.Z * GlobalBrightness);
			if (num5 > 255)
			{
				num5 = 255;
			}
			if (num6 > 255)
			{
				num6 = 255;
			}
			if (num7 > 255)
			{
				num7 = 255;
			}
			slices[0] = new Color((byte)num5, (byte)num6, (byte)num7, 255);
		}
		else
		{
			int num8 = (int)(255f * item.X * GlobalBrightness);
			int num9 = (int)(255f * item.Y * GlobalBrightness);
			int num10 = (int)(255f * item.Z * GlobalBrightness);
			if (num8 > 255)
			{
				num8 = 255;
			}
			if (num9 > 255)
			{
				num9 = 255;
			}
			if (num10 > 255)
			{
				num10 = 255;
			}
			slices[0] = new Color((byte)num8, (byte)num9, (byte)num10, 255);
		}
		if (num >= num3)
		{
			int num11 = (int)(255f * color.X * GlobalBrightness);
			int num12 = (int)(255f * color.Y * GlobalBrightness);
			int num13 = (int)(255f * color.Z * GlobalBrightness);
			if (num11 > 255)
			{
				num11 = 255;
			}
			if (num12 > 255)
			{
				num12 = 255;
			}
			if (num13 > 255)
			{
				num13 = 255;
			}
			slices[1] = new Color((byte)num11, (byte)num12, (byte)num13, 255);
		}
		else
		{
			int num14 = (int)(255f * item.X * GlobalBrightness);
			int num15 = (int)(255f * item.Y * GlobalBrightness);
			int num16 = (int)(255f * item.Z * GlobalBrightness);
			if (num14 > 255)
			{
				num14 = 255;
			}
			if (num15 > 255)
			{
				num15 = 255;
			}
			if (num16 > 255)
			{
				num16 = 255;
			}
			slices[1] = new Color((byte)num14, (byte)num15, (byte)num16, 255);
		}
		if (num2 >= num4)
		{
			int num17 = (int)(255f * number.X * GlobalBrightness);
			int num18 = (int)(255f * number.Y * GlobalBrightness);
			int num19 = (int)(255f * number.Z * GlobalBrightness);
			if (num17 > 255)
			{
				num17 = 255;
			}
			if (num18 > 255)
			{
				num18 = 255;
			}
			if (num19 > 255)
			{
				num19 = 255;
			}
			slices[2] = new Color((byte)num17, (byte)num18, (byte)num19, 255);
		}
		else
		{
			int num20 = (int)(255f * item2.X * GlobalBrightness);
			int num21 = (int)(255f * item2.Y * GlobalBrightness);
			int num22 = (int)(255f * item2.Z * GlobalBrightness);
			if (num20 > 255)
			{
				num20 = 255;
			}
			if (num21 > 255)
			{
				num21 = 255;
			}
			if (num22 > 255)
			{
				num22 = 255;
			}
			slices[2] = new Color((byte)num20, (byte)num21, (byte)num22, 255);
		}
		if (num2 >= num3)
		{
			int num23 = (int)(255f * color.X * GlobalBrightness);
			int num24 = (int)(255f * color.Y * GlobalBrightness);
			int num25 = (int)(255f * color.Z * GlobalBrightness);
			if (num23 > 255)
			{
				num23 = 255;
			}
			if (num24 > 255)
			{
				num24 = 255;
			}
			if (num25 > 255)
			{
				num25 = 255;
			}
			slices[3] = new Color((byte)num23, (byte)num24, (byte)num25, 255);
		}
		else
		{
			int num26 = (int)(255f * item2.X * GlobalBrightness);
			int num27 = (int)(255f * item2.Y * GlobalBrightness);
			int num28 = (int)(255f * item2.Z * GlobalBrightness);
			if (num26 > 255)
			{
				num26 = 255;
			}
			if (num27 > 255)
			{
				num27 = 255;
			}
			if (num28 > 255)
			{
				num28 = 255;
			}
			slices[3] = new Color((byte)num26, (byte)num27, (byte)num28, 255);
		}
	}

	public static void GetColor4Slice(int x, int y, ref Vector3[] slices)
	{
		Vector3 num = _activeEngine.GetColor(x, y - 1);
		Vector3 color = _activeEngine.GetColor(x, y + 1);
		Vector3 color2 = _activeEngine.GetColor(x - 1, y);
		Vector3 color3 = _activeEngine.GetColor(x + 1, y);
		float num2 = num.X + num.Y + num.Z;
		float num3 = color.X + color.Y + color.Z;
		float num4 = color3.X + color3.Y + color3.Z;
		float num5 = color2.X + color2.Y + color2.Z;
		if (num2 >= num5)
		{
			slices[0] = color2 * GlobalBrightness;
		}
		else
		{
			slices[0] = num * GlobalBrightness;
		}
		if (num2 >= num4)
		{
			slices[1] = color3 * GlobalBrightness;
		}
		else
		{
			slices[1] = num * GlobalBrightness;
		}
		if (num3 >= num5)
		{
			slices[2] = color2 * GlobalBrightness;
		}
		else
		{
			slices[2] = color * GlobalBrightness;
		}
		if (num3 >= num4)
		{
			slices[3] = color3 * GlobalBrightness;
		}
		else
		{
			slices[3] = color * GlobalBrightness;
		}
	}
}
