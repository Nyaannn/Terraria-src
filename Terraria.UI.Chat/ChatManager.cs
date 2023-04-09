using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;

namespace Terraria.UI.Chat;

public static class ChatManager
{
	public static class Regexes
	{
		public static readonly Regex Format = new Regex("(?<!\\\\)\\[(?<tag>[a-zA-Z]{1,10})(\\/(?<options>[^:]+))?:(?<text>.+?)(?<!\\\\)\\]", RegexOptions.Compiled);
	}

	public static readonly ChatCommandProcessor Commands = new ChatCommandProcessor();

	private static ConcurrentDictionary<string, ITagHandler> _handlers = new ConcurrentDictionary<string, ITagHandler>();

	public static readonly Vector2[] ShadowDirections = new Vector2[4]
	{
		-Vector2.UnitX,
		Vector2.UnitX,
		-Vector2.UnitY,
		Vector2.UnitY
	};

	public static Color WaveColor(Color color)
	{
		float num = (float)(int)Main.mouseTextColor / 255f;
		color = Color.Lerp(color, Color.Black, 1f - num);
		color.A = Main.mouseTextColor;
		return color;
	}

	public static void ConvertNormalSnippets(TextSnippet[] snippets)
	{
		for (int i = 0; i < snippets.Length; i++)
		{
			TextSnippet textSnippet = snippets[i];
			if (snippets[i].GetType() == typeof(TextSnippet))
			{
				PlainTagHandler.PlainSnippet plainSnippet = (PlainTagHandler.PlainSnippet)(snippets[i] = new PlainTagHandler.PlainSnippet(textSnippet.Text, textSnippet.Color, textSnippet.Scale));
			}
		}
	}

	public static void Register<T>(params string[] names) where T : ITagHandler, new()
	{
		T val = new T();
		for (int i = 0; i < names.Length; i++)
		{
			_handlers[names[i].ToLower()] = val;
		}
	}

	private static ITagHandler GetHandler(string tagName)
	{
		string key = tagName.ToLower();
		if (_handlers.ContainsKey(key))
		{
			return _handlers[key];
		}
		return null;
	}

	public static List<TextSnippet> ParseMessage(string text, Color baseColor)
	{
		text = text.Replace("\r", "");
		MatchCollection matchCollection = Regexes.Format.Matches(text);
		List<TextSnippet> list = new List<TextSnippet>();
		int num = 0;
		foreach (Match item in matchCollection)
		{
			if (item.Index > num)
			{
				list.Add(new TextSnippet(text.Substring(num, item.Index - num), baseColor));
			}
			num = item.Index + item.Length;
			string value2 = item.Groups["tag"].Value;
			string value = item.Groups["text"].Value;
			string writeInformation = item.Groups["options"].Value;
			ITagHandler handler = GetHandler(value2);
			if (handler != null)
			{
				list.Add(handler.Parse(value, baseColor, writeInformation));
				list[list.Count - 1].TextOriginal = item.ToString();
			}
			else
			{
				list.Add(new TextSnippet(value, baseColor));
			}
		}
		if (text.Length > num)
		{
			list.Add(new TextSnippet(text.Substring(num, text.Length - num), baseColor));
		}
		return list;
	}

	public static bool AddChatText(DynamicSpriteFont font, string text, Vector2 baseScale)
	{
		int num = 470;
		num = Main.screenWidth - 330;
		if (GetStringSize(font, Main.chatText + text, baseScale).X > (float)num)
		{
			return false;
		}
		Main.chatText += text;
		return true;
	}

	public static Vector2 GetStringSize(DynamicSpriteFont font, string text, Vector2 baseScale, float maxWidth = -1f)
	{
		TextSnippet[] snippets = ParseMessage(text, Color.White).ToArray();
		return GetStringSize(font, snippets, baseScale, maxWidth);
	}

	public static Vector2 GetStringSize(DynamicSpriteFont font, TextSnippet[] snippets, Vector2 baseScale, float maxWidth = -1f)
	{
		Vector2 vec = new Vector2(Main.mouseX, Main.mouseY);
		Vector2 zero = Vector2.Zero;
		Vector2 vector = zero;
		Vector2 result = vector;
		float x = font.MeasureString(" ").X;
		float num = 1f;
		float num2 = 0f;
		foreach (TextSnippet textSnippet in snippets)
		{
			textSnippet.Update();
			num = textSnippet.Scale;
			if (textSnippet.UniqueDraw(justCheckingString: true, out var size, null))
			{
				vector.X += size.X * baseScale.X * num;
				result.X = Math.Max(result.X, vector.X);
				result.Y = Math.Max(result.Y, vector.Y + size.Y);
				continue;
			}
			string[] array = textSnippet.Text.Split('\n');
			string[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				string[] array3 = array2[j].Split(' ');
				for (int k = 0; k < array3.Length; k++)
				{
					if (k != 0)
					{
						vector.X += x * baseScale.X * num;
					}
					if (maxWidth > 0f)
					{
						float num3 = font.MeasureString(array3[k]).X * baseScale.X * num;
						if (vector.X - zero.X + num3 > maxWidth)
						{
							vector.X = zero.X;
							vector.Y += (float)font.get_LineSpacing() * num2 * baseScale.Y;
							result.Y = Math.Max(result.Y, vector.Y);
							num2 = 0f;
						}
					}
					if (num2 < num)
					{
						num2 = num;
					}
					Vector2 vector2 = font.MeasureString(array3[k]);
					vec.Between(vector, vector + vector2);
					vector.X += vector2.X * baseScale.X * num;
					result.X = Math.Max(result.X, vector.X);
					result.Y = Math.Max(result.Y, vector.Y + vector2.Y);
				}
				if (array.Length > 1)
				{
					vector.X = zero.X;
					vector.Y += (float)font.get_LineSpacing() * num2 * baseScale.Y;
					result.Y = Math.Max(result.Y, vector.Y);
					num2 = 0f;
				}
			}
		}
		return result;
	}

	public static void DrawColorCodedStringShadow(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
	{
		for (int i = 0; i < ShadowDirections.Length; i++)
		{
			DrawColorCodedString(spriteBatch, font, snippets, position + ShadowDirections[i] * spread, baseColor, rotation, origin, baseScale, out var _, maxWidth, ignoreColors: true);
		}
	}

	public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth, bool ignoreColors = false)
	{
		int num = -1;
		Vector2 num2 = new Vector2(Main.mouseX, Main.mouseY);
		Vector2 i = position;
		Vector2 progress2 = i;
		float x = font.MeasureString(" ").X;
		Color color = baseColor;
		float num3 = 1f;
		float num4 = 0f;
		for (int j = 0; j < snippets.Length; j++)
		{
			TextSnippet textSnippet = snippets[j];
			textSnippet.Update();
			if (!ignoreColors)
			{
				color = textSnippet.GetVisibleColor();
			}
			num3 = textSnippet.Scale;
			if (textSnippet.UniqueDraw(justCheckingString: false, out var size, spriteBatch, i, color, num3))
			{
				if (num2.Between(i, i + size))
				{
					num = j;
				}
				i.X += size.X * baseScale.X * num3;
				progress2.X = Math.Max(progress2.X, i.X);
				continue;
			}
			string[] array = textSnippet.Text.Split('\n');
			array = Regex.Split(textSnippet.Text, "(\n)");
			bool flag = true;
			foreach (string text in array)
			{
				string[] array2 = Regex.Split(text, "( )");
				array2 = text.Split(' ');
				if (text == "\n")
				{
					i.Y += (float)font.get_LineSpacing() * num4 * baseScale.Y;
					i.X = position.X;
					progress2.Y = Math.Max(progress2.Y, i.Y);
					num4 = 0f;
					flag = false;
					continue;
				}
				for (int l = 0; l < array2.Length; l++)
				{
					if (l != 0)
					{
						i.X += x * baseScale.X * num3;
					}
					if (maxWidth > 0f)
					{
						float num5 = font.MeasureString(array2[l]).X * baseScale.X * num3;
						if (i.X - position.X + num5 > maxWidth)
						{
							i.X = position.X;
							i.Y += (float)font.get_LineSpacing() * num4 * baseScale.Y;
							progress2.Y = Math.Max(progress2.Y, i.Y);
							num4 = 0f;
						}
					}
					if (num4 < num3)
					{
						num4 = num3;
					}
					DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, font, array2[l], i, color, rotation, origin, baseScale * textSnippet.Scale * num3, SpriteEffects.None, 0f);
					Vector2 vector = font.MeasureString(array2[l]);
					if (num2.Between(i, i + vector))
					{
						num = j;
					}
					i.X += vector.X * baseScale.X * num3;
					progress2.X = Math.Max(progress2.X, i.X);
				}
				if (array.Length > 1 && flag)
				{
					i.Y += (float)font.get_LineSpacing() * num4 * baseScale.Y;
					i.X = position.X;
					progress2.Y = Math.Max(progress2.Y, i.Y);
					num4 = 0f;
				}
				flag = true;
			}
		}
		hoveredSnippet = num;
		return progress2;
	}

	public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth = -1f, float spread = 2f)
	{
		DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black, rotation, origin, baseScale, maxWidth, spread);
		return DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, out hoveredSnippet, maxWidth);
	}

	public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, float rotation, Color color, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth = -1f, float spread = 2f)
	{
		DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black, rotation, origin, baseScale, maxWidth, spread);
		return DrawColorCodedString(spriteBatch, font, snippets, position, color, rotation, origin, baseScale, out hoveredSnippet, maxWidth, ignoreColors: true);
	}

	public static void DrawColorCodedStringShadow(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
	{
		for (int i = 0; i < ShadowDirections.Length; i++)
		{
			DrawColorCodedString(spriteBatch, font, text, position + ShadowDirections[i] * spread, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors: true);
		}
	}

	public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false)
	{
		Vector2 vector = position;
		Vector2 result = vector;
		string[] array = text.Split('\n');
		float x = font.MeasureString(" ").X;
		Color color = baseColor;
		float num = 1f;
		float num2 = 0f;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Split(':');
			foreach (string text2 in array3)
			{
				if (text2.StartsWith("sss"))
				{
					if (text2.StartsWith("sss1"))
					{
						if (!ignoreColors)
						{
							color = Color.Red;
						}
					}
					else if (text2.StartsWith("sss2"))
					{
						if (!ignoreColors)
						{
							color = Color.Blue;
						}
					}
					else if (text2.StartsWith("sssr") && !ignoreColors)
					{
						color = Color.White;
					}
					continue;
				}
				string[] array4 = text2.Split(' ');
				for (int k = 0; k < array4.Length; k++)
				{
					if (k != 0)
					{
						vector.X += x * baseScale.X * num;
					}
					if (maxWidth > 0f)
					{
						float num3 = font.MeasureString(array4[k]).X * baseScale.X * num;
						if (vector.X - position.X + num3 > maxWidth)
						{
							vector.X = position.X;
							vector.Y += (float)font.get_LineSpacing() * num2 * baseScale.Y;
							result.Y = Math.Max(result.Y, vector.Y);
							num2 = 0f;
						}
					}
					if (num2 < num)
					{
						num2 = num;
					}
					DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, font, array4[k], vector, color, rotation, origin, baseScale * num, SpriteEffects.None, 0f);
					vector.X += font.MeasureString(array4[k]).X * baseScale.X * num;
					result.X = Math.Max(result.X, vector.X);
				}
			}
			vector.X = position.X;
			vector.Y += (float)font.get_LineSpacing() * num2 * baseScale.Y;
			result.Y = Math.Max(result.Y, vector.Y);
			num2 = 0f;
		}
		return result;
	}

	public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
	{
		TextSnippet[] snippets = ParseMessage(text, baseColor).ToArray();
		ConvertNormalSnippets(snippets);
		DrawColorCodedStringShadow(spriteBatch, font, snippets, position, new Color(0, 0, 0, baseColor.A), rotation, origin, baseScale, maxWidth, spread);
		int hoveredSnippet;
		return DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, out hoveredSnippet, maxWidth);
	}
}
