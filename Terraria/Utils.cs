using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Graphics;
using ReLogic.OS;
using ReLogic.Utilities;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Utilities;
using Terraria.Utilities.Terraria.Utilities;

namespace Terraria;

public static class Utils
{
	public delegate bool TileActionAttempt(int x, int y);

	public delegate void LaserLineFraming(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distanceCovered, out Rectangle frame, out Vector2 origin, out Color color);

	public delegate Color ColorLerpMethod(float percent);

	public struct ChaseResults
	{
		public bool InterceptionHappens;

		public Vector2 InterceptionPosition;

		public float InterceptionTime;

		public Vector2 ChaserVelocity;
	}

	public const long MaxCoins = 999999999L;

	public static Dictionary<DynamicSpriteFont, float[]> charLengths = new Dictionary<DynamicSpriteFont, float[]>();

	private static Regex _substitutionRegex = new Regex("{(\\?(?:!)?)?([a-zA-Z][\\w\\.]*)}", RegexOptions.Compiled);

	private const ulong RANDOM_MULTIPLIER = 25214903917uL;

	private const ulong RANDOM_ADD = 11uL;

	private const ulong RANDOM_MASK = 281474976710655uL;

	public static Color ColorLerp_BlackToWhite(float percent)
	{
		return Color.Lerp(Color.Black, Color.White, percent);
	}

	public static double Lerp(double value1, double value2, double amount)
	{
		return value1 + (value2 - value1) * amount;
	}

	public static Vector2 Round(Vector2 input)
	{
		return new Vector2((float)Math.Round(input.X), (float)Math.Round(input.Y));
	}

	public static bool IsPowerOfTwo(int x)
	{
		if (x != 0)
		{
			return (x & (x - 1)) == 0;
		}
		return false;
	}

	public static float SmoothStep(float min, float max, float x)
	{
		return MathHelper.Clamp((x - min) / (max - min), 0f, 1f);
	}

	public static double SmoothStep(double min, double max, double x)
	{
		return Clamp((x - min) / (max - min), 0.0, 1.0);
	}

	public static float UnclampedSmoothStep(float min, float max, float x)
	{
		return (x - min) / (max - min);
	}

	public static double UnclampedSmoothStep(double min, double max, double x)
	{
		return (x - min) / (max - min);
	}

	public static Dictionary<string, string> ParseArguements(string[] args)
	{
		string num = null;
		string num2 = "";
		Dictionary<string, string> maxValue = new Dictionary<string, string>();
		for (int maxValue2 = 0; maxValue2 < args.Length; maxValue2++)
		{
			if (args[maxValue2].Length == 0)
			{
				continue;
			}
			if (args[maxValue2][0] == '-' || args[maxValue2][0] == '+')
			{
				if (num != null)
				{
					maxValue.Add(num.ToLower(), num2);
					num2 = "";
				}
				num = args[maxValue2];
				num2 = "";
			}
			else
			{
				if (num2 != "")
				{
					num2 += " ";
				}
				num2 += args[maxValue2];
			}
		}
		if (num != null)
		{
			maxValue.Add(num.ToLower(), num2);
			num2 = "";
		}
		return maxValue;
	}

	public static void Swap<T>(ref T t1, ref T t2)
	{
		T num = t1;
		t1 = t2;
		t2 = num;
	}

	public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
	{
		if (value.CompareTo(max) > 0)
		{
			return max;
		}
		if (value.CompareTo(min) < 0)
		{
			return min;
		}
		return value;
	}

	public static float Turn01ToCyclic010(float value)
	{
		return 1f - ((float)Math.Cos(value * ((float)Math.PI * 2f)) * 0.5f + 0.5f);
	}

	public static float PingPongFrom01To010(float value)
	{
		value %= 1f;
		if (value < 0f)
		{
			value += 1f;
		}
		if (value >= 0.5f)
		{
			return 2f - value * 2f;
		}
		return value * 2f;
	}

	public static float MultiLerp(float percent, params float[] floats)
	{
		float zero = 1f / ((float)floats.Length - 1f);
		float num = zero;
		int num2 = 0;
		while (percent / num > 1f && num2 < floats.Length - 2)
		{
			num += zero;
			num2++;
		}
		return MathHelper.Lerp(floats[num2], floats[num2 + 1], (percent - zero * (float)num2) / zero);
	}

	public static float WrappedLerp(float value1, float value2, float percent)
	{
		float num = percent * 2f;
		if (num > 1f)
		{
			num = 2f - num;
		}
		return MathHelper.Lerp(value1, value2, num);
	}

	public static float GetLerpValue(float from, float to, float t, bool clamped = false)
	{
		if (clamped)
		{
			if (from < to)
			{
				if (t < from)
				{
					return 0f;
				}
				if (t > to)
				{
					return 1f;
				}
			}
			else
			{
				if (t < to)
				{
					return 1f;
				}
				if (t > from)
				{
					return 0f;
				}
			}
		}
		return (t - from) / (to - from);
	}

	public static float Remap(float fromValue, float fromMin, float fromMax, float toMin, float toMax, bool clamped = true)
	{
		return MathHelper.Lerp(toMin, toMax, GetLerpValue(fromMin, fromMax, fromValue, clamped));
	}

	public static void ClampWithinWorld(ref int minX, ref int minY, ref int maxX, ref int maxY, bool lastValuesInclusiveToIteration = false, int fluffX = 0, int fluffY = 0)
	{
		int num = (lastValuesInclusiveToIteration ? 1 : 0);
		minX = Clamp(minX, fluffX, Main.maxTilesX - num - fluffX);
		maxX = Clamp(maxX, fluffX, Main.maxTilesX - num - fluffX);
		minY = Clamp(minY, fluffY, Main.maxTilesY - num - fluffY);
		maxY = Clamp(maxY, fluffY, Main.maxTilesY - num - fluffY);
	}

	public static ChaseResults GetChaseResults(Vector2 chaserPosition, float chaserSpeed, Vector2 runnerPosition, Vector2 runnerVelocity)
	{
		ChaseResults num = default(ChaseResults);
		if (chaserPosition == runnerPosition)
		{
			ChaseResults result = default(ChaseResults);
			result.InterceptionHappens = true;
			result.InterceptionPosition = chaserPosition;
			result.InterceptionTime = 0f;
			result.ChaserVelocity = Vector2.Zero;
			return result;
		}
		if (chaserSpeed <= 0f)
		{
			return default(ChaseResults);
		}
		Vector2 i = chaserPosition - runnerPosition;
		float j = i.Length();
		float num2 = runnerVelocity.Length();
		if (num2 == 0f)
		{
			num.InterceptionTime = j / chaserSpeed;
			num.InterceptionPosition = runnerPosition;
		}
		else
		{
			float a = chaserSpeed * chaserSpeed - num2 * num2;
			float b = 2f * Vector2.Dot(i, runnerVelocity);
			float c = (0f - j) * j;
			if (!SolveQuadratic(a, b, c, out var result2, out var result3))
			{
				return default(ChaseResults);
			}
			if (result2 < 0f && result3 < 0f)
			{
				return default(ChaseResults);
			}
			if (result2 > 0f && result3 > 0f)
			{
				num.InterceptionTime = Math.Min(result2, result3);
			}
			else
			{
				num.InterceptionTime = Math.Max(result2, result3);
			}
			num.InterceptionPosition = runnerPosition + runnerVelocity * num.InterceptionTime;
		}
		num.ChaserVelocity = (num.InterceptionPosition - chaserPosition) / num.InterceptionTime;
		num.InterceptionHappens = true;
		return num;
	}

	public static Vector2 FactorAcceleration(Vector2 currentVelocity, float timeToInterception, Vector2 descendOfProjectile, int framesOfLenience)
	{
		float num = Math.Max(0f, timeToInterception - (float)framesOfLenience);
		Vector2 vector = descendOfProjectile * (num * num) / 2f / timeToInterception;
		return currentVelocity - vector;
	}

	public static bool SolveQuadratic(float a, float b, float c, out float result1, out float result2)
	{
		float flag = b * b - 4f * a * c;
		result1 = 0f;
		result2 = 0f;
		if (flag > 0f)
		{
			result1 = (0f - b + (float)Math.Sqrt(flag)) / (2f * a);
			result2 = (0f - b - (float)Math.Sqrt(flag)) / (2f * a);
			return true;
		}
		if (flag < 0f)
		{
			return false;
		}
		result1 = (result2 = (0f - b + (float)Math.Sqrt(flag)) / (2f * a));
		return true;
	}

	public static double GetLerpValue(double from, double to, double t, bool clamped = false)
	{
		if (clamped)
		{
			if (from < to)
			{
				if (t < from)
				{
					return 0.0;
				}
				if (t > to)
				{
					return 1.0;
				}
			}
			else
			{
				if (t < to)
				{
					return 1.0;
				}
				if (t > from)
				{
					return 0.0;
				}
			}
		}
		return (t - from) / (to - from);
	}

	public static float GetDayTimeAs24FloatStartingFromMidnight()
	{
		if (Main.dayTime)
		{
			return 4.5f + (float)(Main.time / 54000.0) * 15f;
		}
		return 19.5f + (float)(Main.time / 32400.0) * 9f;
	}

	public static Vector2 GetDayTimeAsDirectionIn24HClock()
	{
		return GetDayTimeAsDirectionIn24HClock(GetDayTimeAs24FloatStartingFromMidnight());
	}

	public static Vector2 GetDayTimeAsDirectionIn24HClock(float timeFrom0To24)
	{
		return new Vector2(0f, -1f).RotatedBy(timeFrom0To24 / 24f * ((float)Math.PI * 2f));
	}

	public static string[] ConvertMonoArgsToDotNet(string[] brokenArgs)
	{
		ArrayList arrayList = new ArrayList();
		string text = "";
		for (int i = 0; i < brokenArgs.Length; i++)
		{
			if (brokenArgs[i].StartsWith("-"))
			{
				if (text != "")
				{
					arrayList.Add(text);
					text = "";
				}
				else
				{
					arrayList.Add("");
				}
				arrayList.Add(brokenArgs[i]);
			}
			else
			{
				if (text != "")
				{
					text += " ";
				}
				text += brokenArgs[i];
			}
		}
		arrayList.Add(text);
		string[] array = new string[arrayList.Count];
		arrayList.CopyTo(array);
		return array;
	}

	public static T Max<T>(params T[] args) where T : IComparable
	{
		T type = args[0];
		for (int num = 1; num < args.Length; num++)
		{
			if (type.CompareTo(args[num]) < 0)
			{
				type = args[num];
			}
		}
		return type;
	}

	public static float LineRectangleDistance(Rectangle rect, Vector2 lineStart, Vector2 lineEnd)
	{
		Vector2 style = rect.TopLeft();
		Vector2 fail = rect.TopRight();
		Vector2 fail2 = rect.BottomLeft();
		Vector2 desiredStyle = rect.BottomRight();
		if (lineStart.Between(style, desiredStyle) || lineEnd.Between(style, desiredStyle))
		{
			return 0f;
		}
		float value = style.Distance(style.ClosestPointOnLine(lineStart, lineEnd));
		float height = fail.Distance(fail.ClosestPointOnLine(lineStart, lineEnd));
		float y = fail2.Distance(fail2.ClosestPointOnLine(lineStart, lineEnd));
		float num = desiredStyle.Distance(desiredStyle.ClosestPointOnLine(lineStart, lineEnd));
		return MathHelper.Min(value, MathHelper.Min(height, MathHelper.Min(y, num)));
	}

	public static List<List<TextSnippet>> WordwrapStringSmart(string text, Color c, DynamicSpriteFont font, int maxWidth, int maxLines)
	{
		TextSnippet[] array = ChatManager.ParseMessage(text, c).ToArray();
		List<List<TextSnippet>> list = new List<List<TextSnippet>>();
		List<TextSnippet> list2 = new List<TextSnippet>();
		foreach (TextSnippet textSnippet in array)
		{
			string[] array2 = textSnippet.Text.Split('\n');
			for (int j = 0; j < array2.Length - 1; j++)
			{
				list2.Add(textSnippet.CopyMorph(array2[j]));
				list.Add(list2);
				list2 = new List<TextSnippet>();
			}
			list2.Add(textSnippet.CopyMorph(array2[array2.Length - 1]));
		}
		list.Add(list2);
		if (maxWidth != -1)
		{
			for (int k = 0; k < list.Count; k++)
			{
				List<TextSnippet> list3 = list[k];
				float num = 0f;
				for (int l = 0; l < list3.Count; l++)
				{
					float stringLength = list3[l].GetStringLength(font);
					if (stringLength + num > (float)maxWidth)
					{
						int num2 = maxWidth - (int)num;
						if (num > 0f)
						{
							num2 -= 16;
						}
						int num3 = Math.Min(list3[l].Text.Length, num2 / 8);
						if (num3 < 0)
						{
							num3 = 0;
						}
						string[] array3 = list3[l].Text.Split(' ');
						int num4 = num3;
						if (array3.Length > 1)
						{
							num4 = 0;
							for (int m = 0; m < array3.Length; m++)
							{
								bool flag = num4 == 0;
								if (!(num4 + array3[m].Length <= num3 || flag))
								{
									break;
								}
								num4 += array3[m].Length + 1;
							}
							if (num4 > num3)
							{
								num4 = num3;
							}
						}
						string newText = list3[l].Text.Substring(0, num4);
						string newText2 = list3[l].Text.Substring(num4);
						list2 = new List<TextSnippet> { list3[l].CopyMorph(newText2) };
						for (int n = l + 1; n < list3.Count; n++)
						{
							list2.Add(list3[n]);
						}
						list3[l] = list3[l].CopyMorph(newText);
						list[k] = list[k].Take(l + 1).ToList();
						list.Insert(k + 1, list2);
						break;
					}
					num += stringLength;
				}
				num = 0f;
			}
		}
		if (maxLines != -1)
		{
			while (list.Count > maxLines)
			{
				list.RemoveAt(maxLines);
			}
		}
		return list;
	}

	public static string[] WordwrapString(string text, DynamicSpriteFont font, int maxWidth, int maxLines, out int lineAmount)
	{
		string[] array = new string[maxLines];
		int num = 0;
		List<string> list = new List<string>(text.Split('\n'));
		List<string> list2 = new List<string>(list[0].Split(' '));
		for (int i = 1; i < list.Count && i < maxLines; i++)
		{
			list2.Add("\n");
			list2.AddRange(list[i].Split(' '));
		}
		bool flag = true;
		while (list2.Count > 0)
		{
			string text2 = list2[0];
			string text3 = " ";
			if (list2.Count == 1)
			{
				text3 = "";
			}
			if (text2 == "\n")
			{
				array[num++] += text2;
				flag = true;
				if (num >= maxLines)
				{
					break;
				}
				list2.RemoveAt(0);
			}
			else if (flag)
			{
				if (font.MeasureString(text2).X > (float)maxWidth)
				{
					string text4 = text2[0].ToString() ?? "";
					int num2 = 1;
					while (font.MeasureString(text4 + text2[num2] + "-").X <= (float)maxWidth)
					{
						text4 += text2[num2++];
					}
					text4 += "-";
					array[num++] = text4 + " ";
					if (num >= maxLines)
					{
						break;
					}
					list2.RemoveAt(0);
					list2.Insert(0, text2.Substring(num2));
				}
				else
				{
					ref string reference = ref array[num];
					reference = reference + text2 + text3;
					flag = false;
					list2.RemoveAt(0);
				}
			}
			else if (font.MeasureString(array[num] + text2).X > (float)maxWidth)
			{
				num++;
				if (num >= maxLines)
				{
					break;
				}
				flag = true;
			}
			else
			{
				ref string reference2 = ref array[num];
				reference2 = reference2 + text2 + text3;
				flag = false;
				list2.RemoveAt(0);
			}
		}
		lineAmount = num;
		if (lineAmount == maxLines)
		{
			lineAmount--;
		}
		return array;
	}

	public static Rectangle CenteredRectangle(Vector2 center, Vector2 size)
	{
		return new Rectangle((int)(center.X - size.X / 2f), (int)(center.Y - size.Y / 2f), (int)size.X, (int)size.Y);
	}

	public static Vector2 Vector2FromElipse(Vector2 angleVector, Vector2 elipseSizes)
	{
		if (elipseSizes == Vector2.Zero)
		{
			return Vector2.Zero;
		}
		if (angleVector == Vector2.Zero)
		{
			return Vector2.Zero;
		}
		angleVector.Normalize();
		Vector2 num = Vector2.Normalize(elipseSizes);
		num = Vector2.One / num;
		angleVector *= num;
		angleVector.Normalize();
		return angleVector * elipseSizes / 2f;
	}

	public static bool FloatIntersect(float r1StartX, float r1StartY, float r1Width, float r1Height, float r2StartX, float r2StartY, float r2Width, float r2Height)
	{
		if (r1StartX > r2StartX + r2Width || r1StartY > r2StartY + r2Height || r1StartX + r1Width < r2StartX || r1StartY + r1Height < r2StartY)
		{
			return false;
		}
		return true;
	}

	public static long CoinsCount(out bool overFlowing, Item[] inv, params int[] ignoreSlots)
	{
		List<int> num = new List<int>(ignoreSlots);
		long num2 = 0L;
		for (int num3 = 0; num3 < inv.Length; num3++)
		{
			if (!num.Contains(num3))
			{
				switch (inv[num3].type)
				{
				case 71:
					num2 += inv[num3].stack;
					break;
				case 72:
					num2 += (long)inv[num3].stack * 100L;
					break;
				case 73:
					num2 += (long)inv[num3].stack * 10000L;
					break;
				case 74:
					num2 += (long)inv[num3].stack * 1000000L;
					break;
				}
			}
		}
		overFlowing = false;
		return num2;
	}

	public static int[] CoinsSplit(long count)
	{
		int[] num = new int[4];
		long num2 = 0L;
		long num3 = 1000000L;
		for (int num4 = 3; num4 >= 0; num4--)
		{
			num[num4] = (int)((count - num2) / num3);
			num2 += num[num4] * num3;
			num3 /= 100;
		}
		return num;
	}

	public static long CoinsCombineStacks(out bool overFlowing, params long[] coinCounts)
	{
		long num = 0L;
		foreach (long num2 in coinCounts)
		{
			num += num2;
			if (num >= 999999999)
			{
				overFlowing = true;
				return 999999999L;
			}
		}
		overFlowing = false;
		return num;
	}

	public static void PoofOfSmoke(Vector2 position)
	{
		int flag = Main.rand.Next(3, 7);
		for (int num = 0; num < flag; num++)
		{
			int num2 = Gore.NewGore(position, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2() * new Vector2(2f, 0.7f) * 0.7f, Main.rand.Next(11, 14));
			Main.gore[num2].scale = 0.7f;
			Main.gore[num2].velocity *= 0.5f;
		}
		for (int num3 = 0; num3 < 10; num3++)
		{
			Dust obj = Main.dust[Dust.NewDust(position, 14, 14, 16, 0f, 0f, 100, default(Color), 1.5f)];
			obj.position += new Vector2(5f);
			obj.velocity = (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2() * new Vector2(2f, 0.7f) * 0.7f * (0.5f + 0.5f * Main.rand.NextFloat());
		}
	}

	public static Vector2 ToScreenPosition(this Vector2 worldPosition)
	{
		return Vector2.Transform(worldPosition - Main.screenPosition, Main.GameViewMatrix.ZoomMatrix) / Main.UIScale;
	}

	public static string PrettifyPercentDisplay(float percent, string originalFormat)
	{
		return percent.ToString(originalFormat, CultureInfo.InvariantCulture).TrimEnd('0', '%', ' ').TrimEnd('.', ' ')
			.TrimStart('0', ' ') + "%";
	}

	public static void TrimTextIfNeeded(ref string text, DynamicSpriteFont font, float scale, float maxWidth)
	{
		int num = 0;
		Vector2 vector = font.MeasureString(text) * scale;
		while (vector.X > maxWidth)
		{
			text = text.Substring(0, text.Length - 1);
			num++;
			vector = font.MeasureString(text) * scale;
		}
		if (num > 0)
		{
			text = text.Substring(0, text.Length - 1) + "…";
		}
	}

	public static string FormatWith(string original, object obj)
	{
		PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
		return _substitutionRegex.Replace(original, delegate(Match match)
		{
			if (match.Groups[1].Length != 0)
			{
				return "";
			}
			string name = match.Groups[2].ToString();
			PropertyDescriptor propertyDescriptor = properties.Find(name, ignoreCase: false);
			return (propertyDescriptor == null) ? "" : (propertyDescriptor.GetValue(obj) ?? "").ToString();
		});
	}

	public static bool TryCreatingDirectory(string folderPath)
	{
		if (Directory.Exists(folderPath))
		{
			return true;
		}
		try
		{
			Directory.CreateDirectory(folderPath);
			return true;
		}
		catch (Exception exception)
		{
			FancyErrorPrinter.ShowDirectoryCreationFailError(exception, folderPath);
			return false;
		}
	}

	public static void OpenFolder(string folderPath)
	{
		if (TryCreatingDirectory(folderPath))
		{
			if (Platform.get_IsLinux())
			{
				Process.Start(new ProcessStartInfo(folderPath)
				{
					FileName = "open-folder",
					Arguments = folderPath,
					UseShellExecute = true,
					CreateNoWindow = true
				});
			}
			else
			{
				Process.Start(folderPath);
			}
		}
	}

	public static byte[] ToByteArray(this string str)
	{
		byte[] num = new byte[str.Length * 2];
		Buffer.BlockCopy(str.ToCharArray(), 0, num, 0, num.Length);
		return num;
	}

	public static float NextFloat(this UnifiedRandom r)
	{
		return (float)r.NextDouble();
	}

	public static float NextFloatDirection(this UnifiedRandom r)
	{
		return (float)r.NextDouble() * 2f - 1f;
	}

	public static float NextFloat(this UnifiedRandom random, FloatRange range)
	{
		return random.NextFloat() * (range.Maximum - range.Minimum) + range.Minimum;
	}

	public static T NextFromList<T>(this UnifiedRandom random, params T[] objs)
	{
		return objs[random.Next(objs.Length)];
	}

	public static T NextFromCollection<T>(this UnifiedRandom random, List<T> objs)
	{
		return objs[random.Next(objs.Count)];
	}

	public static int Next(this UnifiedRandom random, IntRange range)
	{
		return random.Next(range.Minimum, range.Maximum + 1);
	}

	public static Vector2 NextVector2Square(this UnifiedRandom r, float min, float max)
	{
		return new Vector2((max - min) * (float)r.NextDouble() + min, (max - min) * (float)r.NextDouble() + min);
	}

	public static Vector2 NextVector2FromRectangle(this UnifiedRandom r, Rectangle rect)
	{
		return new Vector2((float)rect.X + r.NextFloat() * (float)rect.Width, (float)rect.Y + r.NextFloat() * (float)rect.Height);
	}

	public static Vector2 NextVector2Unit(this UnifiedRandom r, float startRotation = 0f, float rotationRange = (float)Math.PI * 2f)
	{
		return (startRotation + rotationRange * r.NextFloat()).ToRotationVector2();
	}

	public static Vector2 NextVector2Circular(this UnifiedRandom r, float circleHalfWidth, float circleHalfHeight)
	{
		return r.NextVector2Unit() * new Vector2(circleHalfWidth, circleHalfHeight) * r.NextFloat();
	}

	public static Vector2 NextVector2CircularEdge(this UnifiedRandom r, float circleHalfWidth, float circleHalfHeight)
	{
		return r.NextVector2Unit() * new Vector2(circleHalfWidth, circleHalfHeight);
	}

	public static int Width(this Asset<Texture2D> asset)
	{
		if (!asset.get_IsLoaded())
		{
			return 0;
		}
		return asset.get_Value().Width;
	}

	public static int Height(this Asset<Texture2D> asset)
	{
		if (!asset.get_IsLoaded())
		{
			return 0;
		}
		return asset.get_Value().Height;
	}

	public static Rectangle Frame(this Asset<Texture2D> tex, int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0, int sizeOffsetX = 0, int sizeOffsetY = 0)
	{
		if (!tex.get_IsLoaded())
		{
			return Rectangle.Empty;
		}
		return tex.get_Value().Frame(horizontalFrames, verticalFrames, frameX, frameY, sizeOffsetX, sizeOffsetY);
	}

	public static Rectangle OffsetSize(this Rectangle rect, int xSize, int ySize)
	{
		rect.Width += xSize;
		rect.Height += ySize;
		return rect;
	}

	public static Vector2 Size(this Asset<Texture2D> tex)
	{
		if (!tex.get_IsLoaded())
		{
			return Vector2.Zero;
		}
		return tex.get_Value().Size();
	}

	public static Rectangle Frame(this Texture2D tex, int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0, int sizeOffsetX = 0, int sizeOffsetY = 0)
	{
		int num = tex.Width / horizontalFrames;
		int num2 = tex.Height / verticalFrames;
		return new Rectangle(num * frameX, num2 * frameY, num + sizeOffsetX, num2 + sizeOffsetY);
	}

	public static Vector2 OriginFlip(this Rectangle rect, Vector2 origin, SpriteEffects effects)
	{
		if (effects.HasFlag(SpriteEffects.FlipHorizontally))
		{
			origin.X = (float)rect.Width - origin.X;
		}
		if (effects.HasFlag(SpriteEffects.FlipVertically))
		{
			origin.Y = (float)rect.Height - origin.Y;
		}
		return origin;
	}

	public static Vector2 Size(this Texture2D tex)
	{
		return new Vector2(tex.Width, tex.Height);
	}

	public static void WriteRGB(this BinaryWriter bb, Color c)
	{
		bb.Write(c.R);
		bb.Write(c.G);
		bb.Write(c.B);
	}

	public static void WriteVector2(this BinaryWriter bb, Vector2 v)
	{
		bb.Write(v.X);
		bb.Write(v.Y);
	}

	public static void WritePackedVector2(this BinaryWriter bb, Vector2 v)
	{
		bb.Write(new HalfVector2(v.X, v.Y).PackedValue);
	}

	public static Color ReadRGB(this BinaryReader bb)
	{
		return new Color(bb.ReadByte(), bb.ReadByte(), bb.ReadByte());
	}

	public static Vector2 ReadVector2(this BinaryReader bb)
	{
		return new Vector2(bb.ReadSingle(), bb.ReadSingle());
	}

	public static Vector2 ReadPackedVector2(this BinaryReader bb)
	{
		HalfVector2 flag = default(HalfVector2);
		flag.PackedValue = bb.ReadUInt32();
		return flag.ToVector2();
	}

	public static Vector2 Left(this Rectangle r)
	{
		return new Vector2(r.X, r.Y + r.Height / 2);
	}

	public static Vector2 Right(this Rectangle r)
	{
		return new Vector2(r.X + r.Width, r.Y + r.Height / 2);
	}

	public static Vector2 Top(this Rectangle r)
	{
		return new Vector2(r.X + r.Width / 2, r.Y);
	}

	public static Vector2 Bottom(this Rectangle r)
	{
		return new Vector2(r.X + r.Width / 2, r.Y + r.Height);
	}

	public static Vector2 TopLeft(this Rectangle r)
	{
		return new Vector2(r.X, r.Y);
	}

	public static Vector2 TopRight(this Rectangle r)
	{
		return new Vector2(r.X + r.Width, r.Y);
	}

	public static Vector2 BottomLeft(this Rectangle r)
	{
		return new Vector2(r.X, r.Y + r.Height);
	}

	public static Vector2 BottomRight(this Rectangle r)
	{
		return new Vector2(r.X + r.Width, r.Y + r.Height);
	}

	public static Vector2 Center(this Rectangle r)
	{
		return new Vector2(r.X + r.Width / 2, r.Y + r.Height / 2);
	}

	public static Vector2 Size(this Rectangle r)
	{
		return new Vector2(r.Width, r.Height);
	}

	public static float Distance(this Rectangle r, Vector2 point)
	{
		if (FloatIntersect(r.Left, r.Top, r.Width, r.Height, point.X, point.Y, 0f, 0f))
		{
			return 0f;
		}
		if (point.X >= (float)r.Left && point.X <= (float)r.Right)
		{
			if (point.Y < (float)r.Top)
			{
				return (float)r.Top - point.Y;
			}
			return point.Y - (float)r.Bottom;
		}
		if (point.Y >= (float)r.Top && point.Y <= (float)r.Bottom)
		{
			if (point.X < (float)r.Left)
			{
				return (float)r.Left - point.X;
			}
			return point.X - (float)r.Right;
		}
		if (point.X < (float)r.Left)
		{
			if (point.Y < (float)r.Top)
			{
				return Vector2.Distance(point, r.TopLeft());
			}
			return Vector2.Distance(point, r.BottomLeft());
		}
		if (point.Y < (float)r.Top)
		{
			return Vector2.Distance(point, r.TopRight());
		}
		return Vector2.Distance(point, r.BottomRight());
	}

	public static Vector2 ClosestPointInRect(this Rectangle r, Vector2 point)
	{
		Vector2 num = point;
		if (num.X < (float)r.Left)
		{
			num.X = r.Left;
		}
		if (num.X > (float)r.Right)
		{
			num.X = r.Right;
		}
		if (num.Y < (float)r.Top)
		{
			num.Y = r.Top;
		}
		if (num.Y > (float)r.Bottom)
		{
			num.Y = r.Bottom;
		}
		return num;
	}

	public static Rectangle Modified(this Rectangle r, int x, int y, int w, int h)
	{
		return new Rectangle(r.X + x, r.Y + y, r.Width + w, r.Height + h);
	}

	public static bool IntersectsConeFastInaccurate(this Rectangle targetRect, Vector2 coneCenter, float coneLength, float coneRotation, float maximumAngle)
	{
		Vector2 num = coneCenter + coneRotation.ToRotationVector2() * coneLength;
		Vector2 num2 = targetRect.ClosestPointInRect(num) - coneCenter;
		float num3 = num2.RotatedBy(0f - coneRotation).ToRotation();
		if (num3 < 0f - maximumAngle || num3 > maximumAngle)
		{
			return false;
		}
		return num2.Length() < coneLength;
	}

	public static bool IntersectsConeSlowMoreAccurate(this Rectangle targetRect, Vector2 coneCenter, float coneLength, float coneRotation, float maximumAngle)
	{
		Vector2 num = coneCenter + coneRotation.ToRotationVector2() * coneLength;
		if (DoesFitInCone(targetRect.ClosestPointInRect(num), coneCenter, coneLength, coneRotation, maximumAngle))
		{
			return true;
		}
		if (DoesFitInCone(targetRect.TopLeft(), coneCenter, coneLength, coneRotation, maximumAngle))
		{
			return true;
		}
		if (DoesFitInCone(targetRect.TopRight(), coneCenter, coneLength, coneRotation, maximumAngle))
		{
			return true;
		}
		if (DoesFitInCone(targetRect.BottomLeft(), coneCenter, coneLength, coneRotation, maximumAngle))
		{
			return true;
		}
		if (DoesFitInCone(targetRect.BottomRight(), coneCenter, coneLength, coneRotation, maximumAngle))
		{
			return true;
		}
		return false;
	}

	public static bool DoesFitInCone(Vector2 point, Vector2 coneCenter, float coneLength, float coneRotation, float maximumAngle)
	{
		Vector2 frameX = point - coneCenter;
		float num = frameX.RotatedBy(0f - coneRotation).ToRotation();
		if (num < 0f - maximumAngle || num > maximumAngle)
		{
			return false;
		}
		return frameX.Length() < coneLength;
	}

	public static float ToRotation(this Vector2 v)
	{
		return (float)Math.Atan2(v.Y, v.X);
	}

	public static Vector2 ToRotationVector2(this float f)
	{
		return new Vector2((float)Math.Cos(f), (float)Math.Sin(f));
	}

	public static Vector2 RotatedBy(this Vector2 spinningpoint, double radians, Vector2 center = default(Vector2))
	{
		float frameY = (float)Math.Cos(radians);
		float frameX = (float)Math.Sin(radians);
		Vector2 flag = spinningpoint - center;
		Vector2 result = center;
		result.X += flag.X * frameY - flag.Y * frameX;
		result.Y += flag.X * frameX + flag.Y * frameY;
		return result;
	}

	public static Vector2D RotatedBy(this Vector2D spinningpoint, double radians, Vector2D center = default(Vector2D))
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		double num = Math.Cos(radians);
		double num2 = Math.Sin(radians);
		Vector2D val = spinningpoint - center;
		Vector2D result = center;
		result.X += val.X * num - val.Y * num2;
		result.Y += val.X * num2 + val.Y * num;
		return result;
	}

	public static Vector2 RotatedByRandom(this Vector2 spinninpoint, double maxRadians)
	{
		return spinninpoint.RotatedBy(Main.rand.NextDouble() * maxRadians - Main.rand.NextDouble() * maxRadians);
	}

	public static Vector2 Floor(this Vector2 vec)
	{
		vec.X = (int)vec.X;
		vec.Y = (int)vec.Y;
		return vec;
	}

	public static bool HasNaNs(this Vector2 vec)
	{
		if (!float.IsNaN(vec.X))
		{
			return float.IsNaN(vec.Y);
		}
		return true;
	}

	public static bool Between(this Vector2 vec, Vector2 minimum, Vector2 maximum)
	{
		if (vec.X >= minimum.X && vec.X <= maximum.X && vec.Y >= minimum.Y)
		{
			return vec.Y <= maximum.Y;
		}
		return false;
	}

	public static Vector2 ToVector2(this Point p)
	{
		return new Vector2(p.X, p.Y);
	}

	public static Vector2 ToVector2(this Point16 p)
	{
		return new Vector2(p.X, p.Y);
	}

	public static Vector2D ToVector2D(this Point p)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2D((double)p.X, (double)p.Y);
	}

	public static Vector2D ToVector2D(this Point16 p)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2D((double)p.X, (double)p.Y);
	}

	public static Vector2 ToWorldCoordinates(this Point p, float autoAddX = 8f, float autoAddY = 8f)
	{
		return p.ToVector2() * 16f + new Vector2(autoAddX, autoAddY);
	}

	public static Vector2 ToWorldCoordinates(this Point16 p, float autoAddX = 8f, float autoAddY = 8f)
	{
		return p.ToVector2() * 16f + new Vector2(autoAddX, autoAddY);
	}

	public static Vector2 MoveTowards(this Vector2 currentPosition, Vector2 targetPosition, float maxAmountAllowedToMove)
	{
		Vector2 flag = targetPosition - currentPosition;
		if (flag.Length() < maxAmountAllowedToMove)
		{
			return targetPosition;
		}
		return currentPosition + flag.SafeNormalize(Vector2.Zero) * maxAmountAllowedToMove;
	}

	public static Point16 ToTileCoordinates16(this Vector2 vec)
	{
		return new Point16((int)vec.X >> 4, (int)vec.Y >> 4);
	}

	public static Point16 ToTileCoordinates16(this Vector2D vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return new Point16((int)vec.X >> 4, (int)vec.Y >> 4);
	}

	public static Point ToTileCoordinates(this Vector2 vec)
	{
		return new Point((int)vec.X >> 4, (int)vec.Y >> 4);
	}

	public static Point ToTileCoordinates(this Vector2D vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return new Point((int)vec.X >> 4, (int)vec.Y >> 4);
	}

	public static Point ToPoint(this Vector2 v)
	{
		return new Point((int)v.X, (int)v.Y);
	}

	public static Point ToPoint(this Vector2D v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new Point((int)v.X, (int)v.Y);
	}

	public static Vector2D ToVector2D(this Vector2 v)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2D((double)v.X, (double)v.Y);
	}

	public static Vector2 SafeNormalize(this Vector2 v, Vector2 defaultValue)
	{
		if (v == Vector2.Zero || v.HasNaNs())
		{
			return defaultValue;
		}
		return Vector2.Normalize(v);
	}

	public static Vector2 ClosestPointOnLine(this Vector2 P, Vector2 A, Vector2 B)
	{
		Vector2 value = P - A;
		Vector2 flag = B - A;
		float num = flag.LengthSquared();
		float num2 = Vector2.Dot(value, flag) / num;
		if (num2 < 0f)
		{
			return A;
		}
		if (num2 > 1f)
		{
			return B;
		}
		return A + flag * num2;
	}

	public static bool RectangleLineCollision(Vector2 rectTopLeft, Vector2 rectBottomRight, Vector2 lineStart, Vector2 lineEnd)
	{
		if (lineStart.Between(rectTopLeft, rectBottomRight) || lineEnd.Between(rectTopLeft, rectBottomRight))
		{
			return true;
		}
		Vector2 flag = new Vector2(rectBottomRight.X, rectTopLeft.Y);
		Vector2 num = new Vector2(rectTopLeft.X, rectBottomRight.Y);
		Vector2[] num2 = new Vector2[4]
		{
			rectTopLeft.ClosestPointOnLine(lineStart, lineEnd),
			flag.ClosestPointOnLine(lineStart, lineEnd),
			num.ClosestPointOnLine(lineStart, lineEnd),
			rectBottomRight.ClosestPointOnLine(lineStart, lineEnd)
		};
		for (int i = 0; i < num2.Length; i++)
		{
			if (num2[0].Between(rectTopLeft, num))
			{
				return true;
			}
		}
		return false;
	}

	public static Vector2 RotateRandom(this Vector2 spinninpoint, double maxRadians)
	{
		return spinninpoint.RotatedBy(Main.rand.NextDouble() * maxRadians - Main.rand.NextDouble() * maxRadians);
	}

	public static float AngleTo(this Vector2 Origin, Vector2 Target)
	{
		return (float)Math.Atan2(Target.Y - Origin.Y, Target.X - Origin.X);
	}

	public static float AngleFrom(this Vector2 Origin, Vector2 Target)
	{
		return (float)Math.Atan2(Origin.Y - Target.Y, Origin.X - Target.X);
	}

	public static Vector2 rotateTowards(Vector2 currentPosition, Vector2 currentVelocity, Vector2 targetPosition, float maxChange)
	{
		float num = currentVelocity.Length();
		float num2 = currentPosition.AngleTo(targetPosition);
		return currentVelocity.ToRotation().AngleTowards(num2, (float)Math.PI / 180f).ToRotationVector2() * num;
	}

	public static float Distance(this Vector2 Origin, Vector2 Target)
	{
		return Vector2.Distance(Origin, Target);
	}

	public static float DistanceSQ(this Vector2 Origin, Vector2 Target)
	{
		return Vector2.DistanceSquared(Origin, Target);
	}

	public static Vector2 DirectionTo(this Vector2 Origin, Vector2 Target)
	{
		return Vector2.Normalize(Target - Origin);
	}

	public static Vector2 DirectionFrom(this Vector2 Origin, Vector2 Target)
	{
		return Vector2.Normalize(Origin - Target);
	}

	public static bool WithinRange(this Vector2 Origin, Vector2 Target, float MaxRange)
	{
		return Vector2.DistanceSquared(Origin, Target) <= MaxRange * MaxRange;
	}

	public static Vector2 XY(this Vector4 vec)
	{
		return new Vector2(vec.X, vec.Y);
	}

	public static Vector2 ZW(this Vector4 vec)
	{
		return new Vector2(vec.Z, vec.W);
	}

	public static Vector3 XZW(this Vector4 vec)
	{
		return new Vector3(vec.X, vec.Z, vec.W);
	}

	public static Vector3 YZW(this Vector4 vec)
	{
		return new Vector3(vec.Y, vec.Z, vec.W);
	}

	public static Color MultiplyRGB(this Color firstColor, Color secondColor)
	{
		return new Color((byte)((float)(firstColor.R * secondColor.R) / 255f), (byte)((float)(firstColor.G * secondColor.G) / 255f), (byte)((float)(firstColor.B * secondColor.B) / 255f));
	}

	public static Color MultiplyRGBA(this Color firstColor, Color secondColor)
	{
		return new Color((byte)((float)(firstColor.R * secondColor.R) / 255f), (byte)((float)(firstColor.G * secondColor.G) / 255f), (byte)((float)(firstColor.B * secondColor.B) / 255f), (byte)((float)(firstColor.A * secondColor.A) / 255f));
	}

	public static string Hex3(this Color color)
	{
		return (color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2")).ToLower();
	}

	public static string Hex4(this Color color)
	{
		return (color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + color.A.ToString("X2")).ToLower();
	}

	public static int ToDirectionInt(this bool value)
	{
		if (!value)
		{
			return -1;
		}
		return 1;
	}

	public static int ToInt(this bool value)
	{
		if (!value)
		{
			return 0;
		}
		return 1;
	}

	public static int ModulusPositive(this int myInteger, int modulusNumber)
	{
		return (myInteger % modulusNumber + modulusNumber) % modulusNumber;
	}

	public static float AngleLerp(this float curAngle, float targetAngle, float amount)
	{
		float j;
		if (targetAngle < curAngle)
		{
			float i = targetAngle + (float)Math.PI * 2f;
			j = ((i - curAngle > curAngle - targetAngle) ? MathHelper.Lerp(curAngle, targetAngle, amount) : MathHelper.Lerp(curAngle, i, amount));
		}
		else
		{
			if (!(targetAngle > curAngle))
			{
				return curAngle;
			}
			float i = targetAngle - (float)Math.PI * 2f;
			j = ((targetAngle - curAngle > curAngle - i) ? MathHelper.Lerp(curAngle, i, amount) : MathHelper.Lerp(curAngle, targetAngle, amount));
		}
		return MathHelper.WrapAngle(j);
	}

	public static float AngleTowards(this float curAngle, float targetAngle, float maxChange)
	{
		curAngle = MathHelper.WrapAngle(curAngle);
		targetAngle = MathHelper.WrapAngle(targetAngle);
		if (curAngle < targetAngle)
		{
			if (targetAngle - curAngle > (float)Math.PI)
			{
				curAngle += (float)Math.PI * 2f;
			}
		}
		else if (curAngle - targetAngle > (float)Math.PI)
		{
			curAngle -= (float)Math.PI * 2f;
		}
		curAngle += MathHelper.Clamp(targetAngle - curAngle, 0f - maxChange, maxChange);
		return MathHelper.WrapAngle(curAngle);
	}

	public static bool deepCompare(this int[] firstArray, int[] secondArray)
	{
		if (firstArray == null && secondArray == null)
		{
			return true;
		}
		if (firstArray != null && secondArray != null)
		{
			if (firstArray.Length != secondArray.Length)
			{
				return false;
			}
			for (int result = 0; result < firstArray.Length; result++)
			{
				if (firstArray[result] != secondArray[result])
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public static List<int> GetTrueIndexes(this bool[] array)
	{
		List<int> flag = new List<int>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i])
			{
				flag.Add(i);
			}
		}
		return flag;
	}

	public static List<int> GetTrueIndexes(params bool[][] arrays)
	{
		List<int> flag = new List<int>();
		foreach (bool[] num in arrays)
		{
			flag.AddRange(num.GetTrueIndexes());
		}
		return flag.Distinct().ToList();
	}

	public static int Count<T>(this T[] arr, T value)
	{
		int flag = 0;
		foreach (T x in arr)
		{
			if (EqualityComparer<T>.Default.Equals(x, value))
			{
				flag++;
			}
		}
		return flag;
	}

	public static bool PressingShift(this KeyboardState kb)
	{
		if (!kb.IsKeyDown(Keys.LeftShift))
		{
			return kb.IsKeyDown(Keys.RightShift);
		}
		return true;
	}

	public static bool PressingControl(this KeyboardState kb)
	{
		if (!kb.IsKeyDown(Keys.LeftControl))
		{
			return kb.IsKeyDown(Keys.RightControl);
		}
		return true;
	}

	public static R[] MapArray<T, R>(T[] array, Func<T, R> mapper)
	{
		R[] array2 = new R[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = mapper(array[i]);
		}
		return array2;
	}

	public static bool PlotLine(Point16 p0, Point16 p1, TileActionAttempt plot, bool jump = true)
	{
		return PlotLine(p0.X, p0.Y, p1.X, p1.Y, plot, jump);
	}

	public static bool PlotLine(Point p0, Point p1, TileActionAttempt plot, bool jump = true)
	{
		return PlotLine(p0.X, p0.Y, p1.X, p1.Y, plot, jump);
	}

	private static bool PlotLine(int x0, int y0, int x1, int y1, TileActionAttempt plot, bool jump = true)
	{
		if (x0 == x1 && y0 == y1)
		{
			return plot(x0, y0);
		}
		bool flag = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
		if (flag)
		{
			Swap(ref x0, ref y0);
			Swap(ref x1, ref y1);
		}
		int num = Math.Abs(x1 - x0);
		int num2 = Math.Abs(y1 - y0);
		int num3 = num / 2;
		int num4 = y0;
		int num5 = ((x0 < x1) ? 1 : (-1));
		int i = ((y0 < y1) ? 1 : (-1));
		for (int j = x0; j != x1; j += num5)
		{
			if (flag)
			{
				if (!plot(num4, j))
				{
					return false;
				}
			}
			else if (!plot(j, num4))
			{
				return false;
			}
			num3 -= num2;
			if (num3 >= 0)
			{
				continue;
			}
			num4 += i;
			if (!jump)
			{
				if (flag)
				{
					if (!plot(num4, j))
					{
						return false;
					}
				}
				else if (!plot(j, num4))
				{
					return false;
				}
			}
			num3 += num;
		}
		return true;
	}

	public static int RandomNext(ref ulong seed, int bits)
	{
		seed = RandomNextSeed(seed);
		return (int)(seed >> 48 - bits);
	}

	public static ulong RandomNextSeed(ulong seed)
	{
		return (seed * 25214903917L + 11) & 0xFFFFFFFFFFFFuL;
	}

	public static float RandomFloat(ref ulong seed)
	{
		return (float)RandomNext(ref seed, 24) / 16777216f;
	}

	public static int RandomInt(ref ulong seed, int max)
	{
		if ((max & -max) == max)
		{
			return (int)((long)max * (long)RandomNext(ref seed, 31) >> 31);
		}
		int i;
		int j;
		do
		{
			i = RandomNext(ref seed, 31);
			j = i % max;
		}
		while (i - j + (max - 1) < 0);
		return j;
	}

	public static int RandomInt(ref ulong seed, int min, int max)
	{
		return RandomInt(ref seed, max - min) + min;
	}

	public static bool PlotTileLine(Vector2 start, Vector2 end, float width, TileActionAttempt plot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return PlotTileLine(start.ToVector2D(), end.ToVector2D(), width, plot);
	}

	public static bool PlotTileLine(Vector2D start, Vector2D end, double width, TileActionAttempt plot)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		double num = width / 2.0;
		Vector2D num2 = end - start;
		Vector2D num3 = num2 / ((Vector2D)(ref num2)).Length();
		Vector2D num4 = new Vector2D(0.0 - num3.Y, num3.X) * num;
		Point num5 = (start - num4).ToTileCoordinates();
		Point num6 = (start + num4).ToTileCoordinates();
		Point num7 = start.ToTileCoordinates();
		Point num8 = end.ToTileCoordinates();
		Point lineMinOffset = new Point(num5.X - num7.X, num5.Y - num7.Y);
		Point lineMaxOffset = new Point(num6.X - num7.X, num6.Y - num7.Y);
		return PlotLine(num7.X, num7.Y, num8.X, num8.Y, (int x, int y) => PlotLine(x + lineMinOffset.X, y + lineMinOffset.Y, x + lineMaxOffset.X, y + lineMaxOffset.Y, plot, jump: false));
	}

	public static bool PlotTileTale(Vector2D start, Vector2D end, double width, TileActionAttempt plot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		double halfWidth = width / 2.0;
		Vector2D num = end - start;
		Vector2D num2 = num / ((Vector2D)(ref num)).Length();
		Vector2D perpOffset = new Vector2D(0.0 - num2.Y, num2.X);
		Point pointStart = start.ToTileCoordinates();
		Point num3 = end.ToTileCoordinates();
		int length = 0;
		PlotLine(pointStart.X, pointStart.Y, num3.X, num3.Y, delegate
		{
			length++;
			return true;
		});
		length--;
		int curLength = 0;
		return PlotLine(pointStart.X, pointStart.Y, num3.X, num3.Y, delegate(int x, int y)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			double num4 = 1.0 - (double)curLength / (double)length;
			curLength++;
			Point point = (start - perpOffset * halfWidth * num4).ToTileCoordinates();
			Point point2 = (start + perpOffset * halfWidth * num4).ToTileCoordinates();
			Point point3 = new Point(point.X - pointStart.X, point.Y - pointStart.Y);
			Point point4 = new Point(point2.X - pointStart.X, point2.Y - pointStart.Y);
			return PlotLine(x + point3.X, y + point3.Y, x + point4.X, y + point4.Y, plot, jump: false);
		});
	}

	public static bool PlotTileArea(int x, int y, TileActionAttempt plot)
	{
		if (!WorldGen.InWorld(x, y))
		{
			return false;
		}
		List<Point> flag = new List<Point>();
		List<Point> num = new List<Point>();
		HashSet<Point> num2 = new HashSet<Point>();
		num.Add(new Point(x, y));
		while (num.Count > 0)
		{
			flag.Clear();
			flag.AddRange(num);
			num.Clear();
			while (flag.Count > 0)
			{
				Point num3 = flag[0];
				if (!WorldGen.InWorld(num3.X, num3.Y, 1))
				{
					flag.Remove(num3);
					continue;
				}
				num2.Add(num3);
				flag.Remove(num3);
				if (plot(num3.X, num3.Y))
				{
					Point num4 = new Point(num3.X - 1, num3.Y);
					if (!num2.Contains(num4))
					{
						num.Add(num4);
					}
					num4 = new Point(num3.X + 1, num3.Y);
					if (!num2.Contains(num4))
					{
						num.Add(num4);
					}
					num4 = new Point(num3.X, num3.Y - 1);
					if (!num2.Contains(num4))
					{
						num.Add(num4);
					}
					num4 = new Point(num3.X, num3.Y + 1);
					if (!num2.Contains(num4))
					{
						num.Add(num4);
					}
				}
			}
		}
		return true;
	}

	public static int RandomConsecutive(double random, int odds)
	{
		return (int)Math.Log(1.0 - random, 1.0 / (double)odds);
	}

	public static Vector2 RandomVector2(UnifiedRandom random, float min, float max)
	{
		return new Vector2((max - min) * (float)random.NextDouble() + min, (max - min) * (float)random.NextDouble() + min);
	}

	public static Vector2D RandomVector2D(UnifiedRandom random, double min, double max)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2D((max - min) * random.NextDouble() + min, (max - min) * random.NextDouble() + min);
	}

	public static bool IndexInRange<T>(this T[] t, int index)
	{
		if (index >= 0)
		{
			return index < t.Length;
		}
		return false;
	}

	public static bool IndexInRange<T>(this List<T> t, int index)
	{
		if (index >= 0)
		{
			return index < t.Count;
		}
		return false;
	}

	public static T SelectRandom<T>(UnifiedRandom random, params T[] choices)
	{
		return choices[random.Next(choices.Length)];
	}

	public static void DrawBorderStringFourWay(SpriteBatch sb, DynamicSpriteFont font, string text, float x, float y, Color textColor, Color borderColor, Vector2 origin, float scale = 1f)
	{
		Color flag = borderColor;
		Vector2 num = Vector2.Zero;
		for (int num2 = 0; num2 < 5; num2++)
		{
			switch (num2)
			{
			case 0:
				num.X = x - 2f;
				num.Y = y;
				break;
			case 1:
				num.X = x + 2f;
				num.Y = y;
				break;
			case 2:
				num.X = x;
				num.Y = y - 2f;
				break;
			case 3:
				num.X = x;
				num.Y = y + 2f;
				break;
			default:
				num.X = x;
				num.Y = y;
				flag = textColor;
				break;
			}
			DynamicSpriteFontExtensionMethods.DrawString(sb, font, text, num, flag, 0f, origin, scale, SpriteEffects.None, 0f);
		}
	}

	public static Vector2 DrawBorderString(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f, float anchorx = 0f, float anchory = 0f, int maxCharactersDisplayed = -1)
	{
		if (maxCharactersDisplayed != -1 && text.Length > maxCharactersDisplayed)
		{
			text.Substring(0, maxCharactersDisplayed);
		}
		DynamicSpriteFont value = FontAssets.MouseText.get_Value();
		Vector2 vector = value.MeasureString(text);
		ChatManager.DrawColorCodedStringWithShadow(sb, value, text, pos, color, 0f, new Vector2(anchorx, anchory) * vector, new Vector2(scale), -1f, 1.5f);
		return vector * scale;
	}

	public static Vector2 DrawBorderStringBig(SpriteBatch spriteBatch, string text, Vector2 pos, Color color, float scale = 1f, float anchorx = 0f, float anchory = 0f, int maxCharactersDisplayed = -1)
	{
		if (maxCharactersDisplayed != -1 && text.Length > maxCharactersDisplayed)
		{
			text.Substring(0, maxCharactersDisplayed);
		}
		DynamicSpriteFont value = FontAssets.DeathText.get_Value();
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, value, text, pos + new Vector2(i, j), Color.Black, 0f, new Vector2(anchorx, anchory) * value.MeasureString(text), scale, SpriteEffects.None, 0f);
			}
		}
		DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, value, text, pos, color, 0f, new Vector2(anchorx, anchory) * value.MeasureString(text), scale, SpriteEffects.None, 0f);
		return value.MeasureString(text) * scale;
	}

	public static void DrawInvBG(SpriteBatch sb, Rectangle R, Color c = default(Color))
	{
		DrawInvBG(sb, R.X, R.Y, R.Width, R.Height, c);
	}

	public static void DrawInvBG(SpriteBatch sb, float x, float y, float w, float h, Color c = default(Color))
	{
		DrawInvBG(sb, (int)x, (int)y, (int)w, (int)h, c);
	}

	public static void DrawInvBG(SpriteBatch sb, int x, int y, int w, int h, Color c = default(Color))
	{
		if (c == default(Color))
		{
			c = new Color(63, 65, 151, 255) * 0.785f;
		}
		Texture2D flag = TextureAssets.InventoryBack13.get_Value();
		if (w < 20)
		{
			w = 20;
		}
		if (h < 20)
		{
			h = 20;
		}
		sb.Draw(flag, new Rectangle(x, y, 10, 10), new Rectangle(0, 0, 10, 10), c);
		sb.Draw(flag, new Rectangle(x + 10, y, w - 20, 10), new Rectangle(10, 0, 10, 10), c);
		sb.Draw(flag, new Rectangle(x + w - 10, y, 10, 10), new Rectangle(flag.Width - 10, 0, 10, 10), c);
		sb.Draw(flag, new Rectangle(x, y + 10, 10, h - 20), new Rectangle(0, 10, 10, 10), c);
		sb.Draw(flag, new Rectangle(x + 10, y + 10, w - 20, h - 20), new Rectangle(10, 10, 10, 10), c);
		sb.Draw(flag, new Rectangle(x + w - 10, y + 10, 10, h - 20), new Rectangle(flag.Width - 10, 10, 10, 10), c);
		sb.Draw(flag, new Rectangle(x, y + h - 10, 10, 10), new Rectangle(0, flag.Height - 10, 10, 10), c);
		sb.Draw(flag, new Rectangle(x + 10, y + h - 10, w - 20, 10), new Rectangle(10, flag.Height - 10, 10, 10), c);
		sb.Draw(flag, new Rectangle(x + w - 10, y + h - 10, 10, 10), new Rectangle(flag.Width - 10, flag.Height - 10, 10, 10), c);
	}

	public static string ReadEmbeddedResource(string path)
	{
		using Stream flag = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
		using StreamReader i = new StreamReader(flag);
		return i.ReadToEnd();
	}

	public static void DrawSplicedPanel(SpriteBatch sb, Texture2D texture, int x, int y, int w, int h, int leftEnd, int rightEnd, int topEnd, int bottomEnd, Color c)
	{
		if (w < leftEnd + rightEnd)
		{
			w = leftEnd + rightEnd;
		}
		if (h < topEnd + bottomEnd)
		{
			h = topEnd + bottomEnd;
		}
		sb.Draw(texture, new Rectangle(x, y, leftEnd, topEnd), new Rectangle(0, 0, leftEnd, topEnd), c);
		sb.Draw(texture, new Rectangle(x + leftEnd, y, w - leftEnd - rightEnd, topEnd), new Rectangle(leftEnd, 0, texture.Width - leftEnd - rightEnd, topEnd), c);
		sb.Draw(texture, new Rectangle(x + w - rightEnd, y, topEnd, rightEnd), new Rectangle(texture.Width - rightEnd, 0, rightEnd, topEnd), c);
		sb.Draw(texture, new Rectangle(x, y + topEnd, leftEnd, h - topEnd - bottomEnd), new Rectangle(0, topEnd, leftEnd, texture.Height - topEnd - bottomEnd), c);
		sb.Draw(texture, new Rectangle(x + leftEnd, y + topEnd, w - leftEnd - rightEnd, h - topEnd - bottomEnd), new Rectangle(leftEnd, topEnd, texture.Width - leftEnd - rightEnd, texture.Height - topEnd - bottomEnd), c);
		sb.Draw(texture, new Rectangle(x + w - rightEnd, y + topEnd, rightEnd, h - topEnd - bottomEnd), new Rectangle(texture.Width - rightEnd, topEnd, rightEnd, texture.Height - topEnd - bottomEnd), c);
		sb.Draw(texture, new Rectangle(x, y + h - bottomEnd, leftEnd, bottomEnd), new Rectangle(0, texture.Height - bottomEnd, leftEnd, bottomEnd), c);
		sb.Draw(texture, new Rectangle(x + leftEnd, y + h - bottomEnd, w - leftEnd - rightEnd, bottomEnd), new Rectangle(leftEnd, texture.Height - bottomEnd, texture.Width - leftEnd - rightEnd, bottomEnd), c);
		sb.Draw(texture, new Rectangle(x + w - rightEnd, y + h - bottomEnd, rightEnd, bottomEnd), new Rectangle(texture.Width - rightEnd, texture.Height - bottomEnd, rightEnd, bottomEnd), c);
	}

	public static void DrawSettingsPanel(SpriteBatch spriteBatch, Vector2 position, float width, Color color)
	{
		DrawPanel(TextureAssets.SettingsPanel.get_Value(), 2, 0, spriteBatch, position, width, color);
	}

	public static void DrawSettings2Panel(SpriteBatch spriteBatch, Vector2 position, float width, Color color)
	{
		DrawPanel(TextureAssets.SettingsPanel.get_Value(), 2, 0, spriteBatch, position, width, color);
	}

	public static void DrawPanel(Texture2D texture, int edgeWidth, int edgeShove, SpriteBatch spriteBatch, Vector2 position, float width, Color color)
	{
		spriteBatch.Draw(texture, position, new Rectangle(0, 0, edgeWidth, texture.Height), color);
		spriteBatch.Draw(texture, new Vector2(position.X + (float)edgeWidth, position.Y), new Rectangle(edgeWidth + edgeShove, 0, texture.Width - (edgeWidth + edgeShove) * 2, texture.Height), color, 0f, Vector2.Zero, new Vector2((width - (float)(edgeWidth * 2)) / (float)(texture.Width - (edgeWidth + edgeShove) * 2), 1f), SpriteEffects.None, 0f);
		spriteBatch.Draw(texture, new Vector2(position.X + width - (float)edgeWidth, position.Y), new Rectangle(texture.Width - edgeWidth, 0, edgeWidth, texture.Height), color);
	}

	public static void DrawRectangle(SpriteBatch sb, Vector2 start, Vector2 end, Color colorStart, Color colorEnd, float width)
	{
		DrawLine(sb, start, new Vector2(start.X, end.Y), colorStart, colorEnd, width);
		DrawLine(sb, start, new Vector2(end.X, start.Y), colorStart, colorEnd, width);
		DrawLine(sb, end, new Vector2(start.X, end.Y), colorStart, colorEnd, width);
		DrawLine(sb, end, new Vector2(end.X, start.Y), colorStart, colorEnd, width);
	}

	public static void DrawLaser(SpriteBatch sb, Texture2D tex, Vector2 start, Vector2 end, Vector2 scale, LaserLineFraming framing)
	{
		Vector2 num = start;
		Vector2 num2 = Vector2.Normalize(end - start);
		float num3 = (end - start).Length();
		float num4 = num2.ToRotation() - (float)Math.PI / 2f;
		if (num2.HasNaNs())
		{
			return;
		}
		framing(0, num, num3, default(Rectangle), out var type, out var frameX, out var frameY, out var color);
		sb.Draw(tex, num, frameX, color, num4, frameX.Size() / 2f, scale, SpriteEffects.None, 0f);
		num3 -= type * scale.Y;
		num += num2 * ((float)frameX.Height - frameY.Y) * scale.Y;
		if (num3 > 0f)
		{
			float num5 = 0f;
			while (num5 + 1f < num3)
			{
				framing(1, num, num3 - num5, frameX, out type, out frameX, out frameY, out color);
				if (num3 - num5 < (float)frameX.Height)
				{
					type *= (num3 - num5) / (float)frameX.Height;
					frameX.Height = (int)(num3 - num5);
				}
				sb.Draw(tex, num, frameX, color, num4, frameY, scale, SpriteEffects.None, 0f);
				num5 += type * scale.Y;
				num += num2 * type * scale.Y;
			}
		}
		framing(2, num, num3, default(Rectangle), out type, out frameX, out frameY, out color);
		sb.Draw(tex, num, frameX, color, num4, frameY, scale, SpriteEffects.None, 0f);
	}

	public static void DrawLine(SpriteBatch spriteBatch, Point start, Point end, Color color)
	{
		DrawLine(spriteBatch, new Vector2(start.X << 4, start.Y << 4), new Vector2(end.X << 4, end.Y << 4), color);
	}

	public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
	{
		float num = Vector2.Distance(start, end);
		Vector2 num2 = (end - start) / num;
		Vector2 num3 = start;
		Vector2 type = Main.screenPosition;
		float num4 = num2.ToRotation();
		for (float num5 = 0f; num5 <= num; num5 += 4f)
		{
			float num6 = num5 / num;
			spriteBatch.Draw(TextureAssets.BlackTile.get_Value(), num3 - type, null, new Color(new Vector4(num6, num6, num6, 1f) * color.ToVector4()), num4, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
			num3 = start + num5 * num2;
		}
	}

	public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color colorStart, Color colorEnd, float width)
	{
		float num = Vector2.Distance(start, end);
		Vector2 num2 = (end - start) / num;
		Vector2 num3 = start;
		Vector2 i = Main.screenPosition;
		float j = num2.ToRotation();
		float num4 = width / 16f;
		for (float num5 = 0f; num5 <= num; num5 += width)
		{
			float k = num5 / num;
			spriteBatch.Draw(TextureAssets.BlackTile.get_Value(), num3 - i, null, Color.Lerp(colorStart, colorEnd, k), j, Vector2.Zero, num4, SpriteEffects.None, 0f);
			num3 = start + num5 * num2;
		}
	}

	public static void DrawRectForTilesInWorld(SpriteBatch spriteBatch, Rectangle rect, Color color)
	{
		DrawRectForTilesInWorld(spriteBatch, new Point(rect.X, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height), color);
	}

	public static void DrawRectForTilesInWorld(SpriteBatch spriteBatch, Point start, Point end, Color color)
	{
		DrawRect(spriteBatch, new Vector2(start.X << 4, start.Y << 4), new Vector2((end.X << 4) - 4, (end.Y << 4) - 4), color);
	}

	public static void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color)
	{
		DrawRect(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);
	}

	public static void DrawRect(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
	{
		DrawLine(spriteBatch, start, new Vector2(start.X, end.Y), color);
		DrawLine(spriteBatch, start, new Vector2(end.X, start.Y), color);
		DrawLine(spriteBatch, end, new Vector2(start.X, end.Y), color);
		DrawLine(spriteBatch, end, new Vector2(end.X, start.Y), color);
	}

	public static void DrawRect(SpriteBatch spriteBatch, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft, Color color)
	{
		DrawLine(spriteBatch, topLeft, topRight, color);
		DrawLine(spriteBatch, topRight, bottomRight, color);
		DrawLine(spriteBatch, bottomRight, bottomLeft, color);
		DrawLine(spriteBatch, bottomLeft, topLeft, color);
	}

	public static void DrawCursorSingle(SpriteBatch sb, Color color, float rot = float.NaN, float scale = 1f, Vector2 manualPosition = default(Vector2), int cursorSlot = 0, int specialMode = 0)
	{
		bool num = false;
		bool flag = true;
		bool flag2 = true;
		Vector2 origin = Vector2.Zero;
		Vector2 vector = new Vector2(Main.mouseX, Main.mouseY);
		if (manualPosition != Vector2.Zero)
		{
			vector = manualPosition;
		}
		if (float.IsNaN(rot))
		{
			rot = 0f;
		}
		else
		{
			num = true;
			rot -= (float)Math.PI * 3f / 4f;
		}
		if (cursorSlot == 4 || cursorSlot == 5)
		{
			flag = false;
			origin = new Vector2(8f);
			if (num && specialMode == 0)
			{
				float num2 = rot;
				if (num2 < 0f)
				{
					num2 += (float)Math.PI * 2f;
				}
				for (float num3 = 0f; num3 < 4f; num3 += 1f)
				{
					if (Math.Abs(num2 - (float)Math.PI / 2f * num3) <= (float)Math.PI / 4f)
					{
						rot = (float)Math.PI / 2f * num3;
						break;
					}
				}
			}
		}
		Vector2 vector2 = Vector2.One;
		if ((Main.ThickMouse && cursorSlot == 0) || cursorSlot == 1)
		{
			vector2 = Main.DrawThickCursor(cursorSlot == 1);
		}
		if (flag)
		{
			sb.Draw(TextureAssets.Cursors[cursorSlot].get_Value(), vector + vector2 + Vector2.One, null, color.MultiplyRGB(new Color(0.2f, 0.2f, 0.2f, 0.5f)), rot, origin, scale * 1.1f, SpriteEffects.None, 0f);
		}
		if (flag2)
		{
			sb.Draw(TextureAssets.Cursors[cursorSlot].get_Value(), vector + vector2, null, color, rot, origin, scale, SpriteEffects.None, 0f);
		}
	}
}
