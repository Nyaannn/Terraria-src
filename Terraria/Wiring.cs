using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;

namespace Terraria;

public static class Wiring
{
	public static bool blockPlayerTeleportationForOneIteration;

	public static bool running;

	private static Dictionary<Point16, bool> _wireSkip;

	private static DoubleStack<Point16> _wireList;

	private static DoubleStack<byte> _wireDirectionList;

	private static Dictionary<Point16, byte> _toProcess;

	private static Queue<Point16> _GatesCurrent;

	private static Queue<Point16> _LampsToCheck;

	private static Queue<Point16> _GatesNext;

	private static Dictionary<Point16, bool> _GatesDone;

	private static Dictionary<Point16, byte> _PixelBoxTriggers;

	private static Vector2[] _teleport;

	private const int MaxPump = 20;

	private static int[] _inPumpX;

	private static int[] _inPumpY;

	private static int _numInPump;

	private static int[] _outPumpX;

	private static int[] _outPumpY;

	private static int _numOutPump;

	private const int MaxMech = 1000;

	private static int[] _mechX;

	private static int[] _mechY;

	private static int _numMechs;

	private static int[] _mechTime;

	private static int _currentWireColor;

	private static int CurrentUser = 255;

	public static void SetCurrentUser(int plr = -1)
	{
		if (plr < 0 || plr > 255)
		{
			plr = 255;
		}
		if (Main.netMode == 0)
		{
			plr = Main.myPlayer;
		}
		CurrentUser = plr;
	}

	public static void Initialize()
	{
		_wireSkip = new Dictionary<Point16, bool>();
		_wireList = new DoubleStack<Point16>();
		_wireDirectionList = new DoubleStack<byte>();
		_toProcess = new Dictionary<Point16, byte>();
		_GatesCurrent = new Queue<Point16>();
		_GatesNext = new Queue<Point16>();
		_GatesDone = new Dictionary<Point16, bool>();
		_LampsToCheck = new Queue<Point16>();
		_PixelBoxTriggers = new Dictionary<Point16, byte>();
		_inPumpX = new int[20];
		_inPumpY = new int[20];
		_outPumpX = new int[20];
		_outPumpY = new int[20];
		_teleport = new Vector2[2]
		{
			Vector2.One * -1f,
			Vector2.One * -1f
		};
		_mechX = new int[1000];
		_mechY = new int[1000];
		_mechTime = new int[1000];
	}

	public static void SkipWire(int x, int y)
	{
		_wireSkip[new Point16(x, y)] = true;
	}

	public static void SkipWire(Point16 point)
	{
		_wireSkip[point] = true;
	}

	public static void ClearAll()
	{
		for (int i = 0; i < 20; i++)
		{
			_inPumpX[i] = 0;
			_inPumpY[i] = 0;
			_outPumpX[i] = 0;
			_outPumpY[i] = 0;
		}
		_numInPump = 0;
		_numOutPump = 0;
		for (int j = 0; j < 1000; j++)
		{
			_mechTime[j] = 0;
			_mechX[j] = 0;
			_mechY[j] = 0;
		}
		_numMechs = 0;
	}

	public static void UpdateMech()
	{
		SetCurrentUser();
		for (int tileSafely = _numMechs - 1; tileSafely >= 0; tileSafely--)
		{
			_mechTime[tileSafely]--;
			int tileSafely2 = _mechX[tileSafely];
			int wall = _mechY[tileSafely];
			if (!WorldGen.InWorld(tileSafely2, wall, 1))
			{
				_numMechs--;
			}
			else
			{
				Tile i = Main.tile[tileSafely2, wall];
				if (i == null)
				{
					_numMechs--;
				}
				else
				{
					if (i.active() && i.type == 144)
					{
						if (i.frameY == 0)
						{
							_mechTime[tileSafely] = 0;
						}
						else
						{
							int num = i.frameX / 18;
							switch (num)
							{
							case 0:
								num = 60;
								break;
							case 1:
								num = 180;
								break;
							case 2:
								num = 300;
								break;
							case 3:
								num = 30;
								break;
							case 4:
								num = 15;
								break;
							}
							if (Math.IEEERemainder(_mechTime[tileSafely], num) == 0.0)
							{
								_mechTime[tileSafely] = 18000;
								TripWire(_mechX[tileSafely], _mechY[tileSafely], 1, 1);
							}
						}
					}
					if (_mechTime[tileSafely] <= 0)
					{
						if (i.active() && i.type == 144)
						{
							i.frameY = 0;
							NetMessage.SendTileSquare(-1, _mechX[tileSafely], _mechY[tileSafely]);
						}
						if (i.active() && i.type == 411)
						{
							int num2 = i.frameX % 36 / 18;
							int num3 = i.frameY % 36 / 18;
							int num4 = _mechX[tileSafely] - num2;
							int num5 = _mechY[tileSafely] - num3;
							int num6 = 36;
							if (Main.tile[num4, num5].frameX >= 36)
							{
								num6 = -36;
							}
							for (int j = num4; j < num4 + 2; j++)
							{
								for (int k = num5; k < num5 + 2; k++)
								{
									if (WorldGen.InWorld(j, k, 1))
									{
										Tile tile = Main.tile[j, k];
										if (tile != null)
										{
											tile.frameX = (short)(tile.frameX + num6);
										}
									}
								}
							}
							NetMessage.SendTileSquare(-1, num4, num5, 2, 2);
						}
						for (int l = tileSafely; l < _numMechs; l++)
						{
							_mechX[l] = _mechX[l + 1];
							_mechY[l] = _mechY[l + 1];
							_mechTime[l] = _mechTime[l + 1];
						}
						_numMechs--;
					}
				}
			}
		}
	}

	public static void HitSwitch(int i, int j)
	{
		if (!WorldGen.InWorld(i, j) || Main.tile[i, j] == null)
		{
			return;
		}
		if (Main.tile[i, j].type == 135 || Main.tile[i, j].type == 314 || Main.tile[i, j].type == 423 || Main.tile[i, j].type == 428 || Main.tile[i, j].type == 442 || Main.tile[i, j].type == 476)
		{
			SoundEngine.PlaySound(28, i * 16, j * 16, 0);
			TripWire(i, j, 1, 1);
		}
		else if (Main.tile[i, j].type == 440)
		{
			SoundEngine.PlaySound(28, i * 16 + 16, j * 16 + 16, 0);
			TripWire(i, j, 3, 3);
		}
		else if (Main.tile[i, j].type == 136)
		{
			if (Main.tile[i, j].frameY == 0)
			{
				Main.tile[i, j].frameY = 18;
			}
			else
			{
				Main.tile[i, j].frameY = 0;
			}
			SoundEngine.PlaySound(28, i * 16, j * 16, 0);
			TripWire(i, j, 1, 1);
		}
		else if (Main.tile[i, j].type == 443)
		{
			GeyserTrap(i, j);
		}
		else if (Main.tile[i, j].type == 144)
		{
			if (Main.tile[i, j].frameY == 0)
			{
				Main.tile[i, j].frameY = 18;
				if (Main.netMode != 1)
				{
					CheckMech(i, j, 18000);
				}
			}
			else
			{
				Main.tile[i, j].frameY = 0;
			}
			SoundEngine.PlaySound(28, i * 16, j * 16, 0);
		}
		else if (Main.tile[i, j].type == 441 || Main.tile[i, j].type == 468)
		{
			int tileSafely = Main.tile[i, j].frameX / 18 * -1;
			int tileSafely2 = Main.tile[i, j].frameY / 18 * -1;
			tileSafely %= 4;
			if (tileSafely < -1)
			{
				tileSafely += 2;
			}
			tileSafely += i;
			tileSafely2 += j;
			SoundEngine.PlaySound(28, i * 16, j * 16, 0);
			TripWire(tileSafely, tileSafely2, 2, 2);
		}
		else if (Main.tile[i, j].type == 467)
		{
			if (Main.tile[i, j].frameX / 36 == 4)
			{
				int tileSafely3 = Main.tile[i, j].frameX / 18 * -1;
				int flag = Main.tile[i, j].frameY / 18 * -1;
				tileSafely3 %= 4;
				if (tileSafely3 < -1)
				{
					tileSafely3 += 2;
				}
				tileSafely3 += i;
				flag += j;
				SoundEngine.PlaySound(28, i * 16, j * 16, 0);
				TripWire(tileSafely3, flag, 2, 2);
			}
		}
		else
		{
			if (Main.tile[i, j].type != 132 && Main.tile[i, j].type != 411)
			{
				return;
			}
			short num2 = 36;
			int num3 = Main.tile[i, j].frameX / 18 * -1;
			int num4 = Main.tile[i, j].frameY / 18 * -1;
			num3 %= 4;
			if (num3 < -1)
			{
				num3 += 2;
				num2 = -36;
			}
			num3 += i;
			num4 += j;
			if (Main.netMode != 1 && Main.tile[num3, num4].type == 411)
			{
				CheckMech(num3, num4, 60);
			}
			for (int k = num3; k < num3 + 2; k++)
			{
				for (int l = num4; l < num4 + 2; l++)
				{
					if (Main.tile[k, l].type == 132 || Main.tile[k, l].type == 411)
					{
						Main.tile[k, l].frameX += num2;
					}
				}
			}
			WorldGen.TileFrame(num3, num4);
			SoundEngine.PlaySound(28, i * 16, j * 16, 0);
			TripWire(num3, num4, 2, 2);
		}
	}

	public static void PokeLogicGate(int lampX, int lampY)
	{
		if (Main.netMode != 1)
		{
			_LampsToCheck.Enqueue(new Point16(lampX, lampY));
			LogicGatePass();
		}
	}

	public static bool Actuate(int i, int j)
	{
		Tile num = Main.tile[i, j];
		if (!num.actuator())
		{
			return false;
		}
		if (num.inActive())
		{
			ReActive(i, j);
		}
		else
		{
			DeActive(i, j);
		}
		return true;
	}

	public static void ActuateForced(int i, int j)
	{
		if (Main.tile[i, j].inActive())
		{
			ReActive(i, j);
		}
		else
		{
			DeActive(i, j);
		}
	}

	public static void MassWireOperation(Point ps, Point pe, Player master)
	{
		int num = 0;
		int i = 0;
		for (int j = 0; j < 58; j++)
		{
			if (master.inventory[j].type == 530)
			{
				num += master.inventory[j].stack;
			}
			if (master.inventory[j].type == 849)
			{
				i += master.inventory[j].stack;
			}
		}
		int num3 = num;
		int num2 = i;
		MassWireOperationInner(master, ps, pe, master.Center, master.direction == 1, ref num, ref i);
		int num4 = num3 - num;
		int num5 = num2 - i;
		if (Main.netMode == 2)
		{
			NetMessage.SendData(110, master.whoAmI, -1, null, 530, num4, master.whoAmI);
			NetMessage.SendData(110, master.whoAmI, -1, null, 849, num5, master.whoAmI);
			return;
		}
		for (int k = 0; k < num4; k++)
		{
			master.ConsumeItem(530);
		}
		for (int l = 0; l < num5; l++)
		{
			master.ConsumeItem(849);
		}
	}

	private static bool CheckMech(int i, int j, int time)
	{
		for (int num = 0; num < _numMechs; num++)
		{
			if (_mechX[num] == i && _mechY[num] == j)
			{
				return false;
			}
		}
		if (_numMechs < 999)
		{
			_mechX[_numMechs] = i;
			_mechY[_numMechs] = j;
			_mechTime[_numMechs] = time;
			_numMechs++;
			return true;
		}
		return false;
	}

	private static void XferWater()
	{
		for (int num = 0; num < _numInPump; num++)
		{
			int type = _inPumpX[num];
			int num2 = _inPumpY[num];
			int tileSafely = Main.tile[type, num2].liquid;
			if (tileSafely <= 0)
			{
				continue;
			}
			byte b = Main.tile[type, num2].liquidType();
			for (int i = 0; i < _numOutPump; i++)
			{
				int num3 = _outPumpX[i];
				int num4 = _outPumpY[i];
				int liquid = Main.tile[num3, num4].liquid;
				if (liquid >= 255)
				{
					continue;
				}
				byte b2 = Main.tile[num3, num4].liquidType();
				if (liquid == 0)
				{
					b2 = b;
				}
				if (b2 == b)
				{
					int num5 = tileSafely;
					if (num5 + liquid > 255)
					{
						num5 = 255 - liquid;
					}
					Main.tile[num3, num4].liquid += (byte)num5;
					Main.tile[type, num2].liquid -= (byte)num5;
					tileSafely = Main.tile[type, num2].liquid;
					Main.tile[num3, num4].liquidType(b);
					WorldGen.SquareTileFrame(num3, num4);
					if (Main.tile[type, num2].liquid == 0)
					{
						Main.tile[type, num2].liquidType(0);
						WorldGen.SquareTileFrame(type, num2);
						break;
					}
				}
			}
			WorldGen.SquareTileFrame(type, num2);
		}
	}

	private static void TripWire(int left, int top, int width, int height)
	{
		if (Main.netMode == 1)
		{
			return;
		}
		running = true;
		if (_wireList.Count != 0)
		{
			_wireList.Clear(quickClear: true);
		}
		if (_wireDirectionList.Count != 0)
		{
			_wireDirectionList.Clear(quickClear: true);
		}
		Vector2[] result = new Vector2[8];
		int tile = 0;
		for (int num2 = left; num2 < left + width; num2++)
		{
			for (int num3 = top; num3 < top + height; num3++)
			{
				Point16 num = new Point16(num2, num3);
				Tile point = Main.tile[num2, num3];
				if (point != null && point.wire())
				{
					_wireList.PushBack(num);
				}
			}
		}
		_teleport[0].X = -1f;
		_teleport[0].Y = -1f;
		_teleport[1].X = -1f;
		_teleport[1].Y = -1f;
		if (_wireList.Count > 0)
		{
			_numInPump = 0;
			_numOutPump = 0;
			HitWire(_wireList, 1);
			if (_numInPump > 0 && _numOutPump > 0)
			{
				XferWater();
			}
		}
		result[tile++] = _teleport[0];
		result[tile++] = _teleport[1];
		for (int num4 = left; num4 < left + width; num4++)
		{
			for (int i = top; i < top + height; i++)
			{
				Point16 num = new Point16(num4, i);
				Tile tile2 = Main.tile[num4, i];
				if (tile2 != null && tile2.wire2())
				{
					_wireList.PushBack(num);
				}
			}
		}
		_teleport[0].X = -1f;
		_teleport[0].Y = -1f;
		_teleport[1].X = -1f;
		_teleport[1].Y = -1f;
		if (_wireList.Count > 0)
		{
			_numInPump = 0;
			_numOutPump = 0;
			HitWire(_wireList, 2);
			if (_numInPump > 0 && _numOutPump > 0)
			{
				XferWater();
			}
		}
		result[tile++] = _teleport[0];
		result[tile++] = _teleport[1];
		_teleport[0].X = -1f;
		_teleport[0].Y = -1f;
		_teleport[1].X = -1f;
		_teleport[1].Y = -1f;
		for (int tile3 = left; tile3 < left + width; tile3++)
		{
			for (int tile4 = top; tile4 < top + height; tile4++)
			{
				Point16 num = new Point16(tile3, tile4);
				Tile style2 = Main.tile[tile3, tile4];
				if (style2 != null && style2.wire3())
				{
					_wireList.PushBack(num);
				}
			}
		}
		if (_wireList.Count > 0)
		{
			_numInPump = 0;
			_numOutPump = 0;
			HitWire(_wireList, 3);
			if (_numInPump > 0 && _numOutPump > 0)
			{
				XferWater();
			}
		}
		result[tile++] = _teleport[0];
		result[tile++] = _teleport[1];
		_teleport[0].X = -1f;
		_teleport[0].Y = -1f;
		_teleport[1].X = -1f;
		_teleport[1].Y = -1f;
		for (int type = left; type < left + width; type++)
		{
			for (int direction = top; direction < top + height; direction++)
			{
				Point16 num = new Point16(type, direction);
				Tile j = Main.tile[type, direction];
				if (j != null && j.wire4())
				{
					_wireList.PushBack(num);
				}
			}
		}
		if (_wireList.Count > 0)
		{
			_numInPump = 0;
			_numOutPump = 0;
			HitWire(_wireList, 4);
			if (_numInPump > 0 && _numOutPump > 0)
			{
				XferWater();
			}
		}
		result[tile++] = _teleport[0];
		result[tile++] = _teleport[1];
		running = false;
		for (int k = 0; k < 8; k += 2)
		{
			_teleport[0] = result[k];
			_teleport[1] = result[k + 1];
			if (_teleport[0].X >= 0f && _teleport[1].X >= 0f)
			{
				Teleport();
			}
		}
		PixelBoxPass();
		LogicGatePass();
	}

	private static void PixelBoxPass()
	{
		foreach (KeyValuePair<Point16, byte> num in _PixelBoxTriggers)
		{
			if (num.Value == 3)
			{
				Tile tile2 = Main.tile[num.Key.X, num.Key.Y];
				tile2.frameX = (short)((tile2.frameX != 18) ? 18 : 0);
				NetMessage.SendTileSquare(-1, num.Key.X, num.Key.Y);
			}
		}
		_PixelBoxTriggers.Clear();
	}

	private static void LogicGatePass()
	{
		if (_GatesCurrent.Count != 0)
		{
			return;
		}
		_GatesDone.Clear();
		while (_LampsToCheck.Count > 0)
		{
			while (_LampsToCheck.Count > 0)
			{
				Point16 point = _LampsToCheck.Dequeue();
				CheckLogicGate(point.X, point.Y);
			}
			while (_GatesNext.Count > 0)
			{
				Utils.Swap(ref _GatesCurrent, ref _GatesNext);
				while (_GatesCurrent.Count > 0)
				{
					Point16 key = _GatesCurrent.Peek();
					if (_GatesDone.TryGetValue(key, out var value) && value)
					{
						_GatesCurrent.Dequeue();
						continue;
					}
					_GatesDone.Add(key, value: true);
					TripWire(key.X, key.Y, 1, 1);
					_GatesCurrent.Dequeue();
				}
			}
		}
		_GatesDone.Clear();
		if (blockPlayerTeleportationForOneIteration)
		{
			blockPlayerTeleportationForOneIteration = false;
		}
	}

	private static void CheckLogicGate(int lampX, int lampY)
	{
		if (!WorldGen.InWorld(lampX, lampY, 1))
		{
			return;
		}
		for (int i = lampY; i < Main.maxTilesY; i++)
		{
			Tile tile = Main.tile[lampX, i];
			if (!tile.active())
			{
				break;
			}
			if (tile.type == 420)
			{
				_GatesDone.TryGetValue(new Point16(lampX, i), out var value);
				int num = tile.frameY / 18;
				bool flag = tile.frameX == 18;
				bool flag2 = tile.frameX == 36;
				if (num < 0)
				{
					break;
				}
				int num2 = 0;
				int num3 = 0;
				bool flag3 = false;
				for (int num4 = i - 1; num4 > 0; num4--)
				{
					Tile tile2 = Main.tile[lampX, num4];
					if (!tile2.active() || tile2.type != 419)
					{
						break;
					}
					if (tile2.frameX == 36)
					{
						flag3 = true;
						break;
					}
					num2++;
					num3 += (tile2.frameX == 18).ToInt();
				}
				bool flag4 = false;
				switch (num)
				{
				default:
					return;
				case 0:
					flag4 = num2 == num3;
					break;
				case 2:
					flag4 = num2 != num3;
					break;
				case 1:
					flag4 = num3 > 0;
					break;
				case 3:
					flag4 = num3 == 0;
					break;
				case 4:
					flag4 = num3 == 1;
					break;
				case 5:
					flag4 = num3 != 1;
					break;
				}
				bool flag5 = !flag3 && flag2;
				bool flag6 = false;
				if (flag3 && Framing.GetTileSafely(lampX, lampY).frameX == 36)
				{
					flag6 = true;
				}
				if (!(flag4 != flag || flag5 || flag6))
				{
					break;
				}
				_ = tile.frameX % 18 / 18;
				tile.frameX = (short)(18 * flag4.ToInt());
				if (flag3)
				{
					tile.frameX = 36;
				}
				SkipWire(lampX, i);
				WorldGen.SquareTileFrame(lampX, i);
				NetMessage.SendTileSquare(-1, lampX, i);
				bool flag7 = !flag3 || flag6;
				if (flag6)
				{
					if (num3 == 0 || num2 == 0)
					{
						flag7 = false;
					}
					flag7 = Main.rand.NextFloat() < (float)num3 / (float)num2;
				}
				if (flag5)
				{
					flag7 = false;
				}
				if (flag7)
				{
					if (!value)
					{
						_GatesNext.Enqueue(new Point16(lampX, i));
						break;
					}
					Vector2 position = new Vector2(lampX, i) * 16f - new Vector2(10f);
					Utils.PoofOfSmoke(position);
					NetMessage.SendData(106, -1, -1, null, (int)position.X, position.Y);
				}
				break;
			}
			if (tile.type != 419)
			{
				break;
			}
		}
	}

	private static void HitWire(DoubleStack<Point16> next, int wireType)
	{
		_wireDirectionList.Clear(quickClear: true);
		for (int num = 0; num < next.Count; num++)
		{
			Point16 point = next.PopFront();
			SkipWire(point);
			_toProcess.Add(point, 4);
			next.PushBack(point);
			_wireDirectionList.PushBack(0);
		}
		_currentWireColor = wireType;
		while (next.Count > 0)
		{
			Point16 key = next.PopFront();
			int num2 = _wireDirectionList.PopFront();
			int x = key.X;
			int y = key.Y;
			if (!_wireSkip.ContainsKey(key))
			{
				HitWireSingle(x, y);
			}
			for (int i = 0; i < 4; i++)
			{
				int num3;
				int num4;
				switch (i)
				{
				case 0:
					num3 = x;
					num4 = y + 1;
					break;
				case 1:
					num3 = x;
					num4 = y - 1;
					break;
				case 2:
					num3 = x + 1;
					num4 = y;
					break;
				case 3:
					num3 = x - 1;
					num4 = y;
					break;
				default:
					num3 = x;
					num4 = y + 1;
					break;
				}
				if (num3 < 2 || num3 >= Main.maxTilesX - 2 || num4 < 2 || num4 >= Main.maxTilesY - 2)
				{
					continue;
				}
				Tile tile = Main.tile[num3, num4];
				if (tile == null)
				{
					continue;
				}
				Tile tile2 = Main.tile[x, y];
				if (tile2 == null)
				{
					continue;
				}
				byte b = 3;
				if (tile.type == 424 || tile.type == 445)
				{
					b = 0;
				}
				if (tile2.type == 424)
				{
					switch (tile2.frameX / 18)
					{
					case 0:
						if (i != num2)
						{
							continue;
						}
						break;
					case 1:
						if ((num2 != 0 || i != 3) && (num2 != 3 || i != 0) && (num2 != 1 || i != 2) && (num2 != 2 || i != 1))
						{
							continue;
						}
						break;
					case 2:
						if ((num2 != 0 || i != 2) && (num2 != 2 || i != 0) && (num2 != 1 || i != 3) && (num2 != 3 || i != 1))
						{
							continue;
						}
						break;
					}
				}
				if (tile2.type == 445)
				{
					if (i != num2)
					{
						continue;
					}
					if (_PixelBoxTriggers.ContainsKey(key))
					{
						_PixelBoxTriggers[key] |= (byte)((!(i == 0 || i == 1)) ? 1 : 2);
					}
					else
					{
						_PixelBoxTriggers[key] = (byte)((!(i == 0 || i == 1)) ? 1u : 2u);
					}
				}
				if (wireType switch
				{
					1 => tile.wire() ? 1 : 0, 
					2 => tile.wire2() ? 1 : 0, 
					3 => tile.wire3() ? 1 : 0, 
					4 => tile.wire4() ? 1 : 0, 
					_ => 0, 
				} == 0)
				{
					continue;
				}
				Point16 point2 = new Point16(num3, num4);
				if (_toProcess.TryGetValue(point2, out var value))
				{
					value = (byte)(value - 1);
					if (value == 0)
					{
						_toProcess.Remove(point2);
					}
					else
					{
						_toProcess[point2] = value;
					}
					continue;
				}
				next.PushBack(point2);
				_wireDirectionList.PushBack((byte)i);
				if (b > 0)
				{
					_toProcess.Add(point2, b);
				}
			}
		}
		_wireSkip.Clear();
		_toProcess.Clear();
	}

	public static IEntitySource GetProjectileSource(int sourceTileX, int sourceTileY)
	{
		return new EntitySource_Wiring(sourceTileX, sourceTileY);
	}

	public static IEntitySource GetNPCSource(int sourceTileX, int sourceTileY)
	{
		return new EntitySource_Wiring(sourceTileX, sourceTileY);
	}

	public static IEntitySource GetItemSource(int sourceTileX, int sourceTileY)
	{
		return new EntitySource_Wiring(sourceTileX, sourceTileY);
	}

	private static void HitWireSingle(int i, int j)
	{
		Tile vector = Main.tile[i, j];
		bool? type = null;
		bool damage = true;
		int num = vector.type;
		if (vector.actuator())
		{
			ActuateForced(i, j);
		}
		if (!vector.active())
		{
			return;
		}
		switch (num)
		{
		case 144:
			HitSwitch(i, j);
			WorldGen.SquareTileFrame(i, j);
			NetMessage.SendTileSquare(-1, i, j);
			break;
		case 421:
			if (!vector.actuator())
			{
				vector.type = 422;
				WorldGen.SquareTileFrame(i, j);
				NetMessage.SendTileSquare(-1, i, j);
			}
			break;
		case 422:
			if (!vector.actuator())
			{
				vector.type = 421;
				WorldGen.SquareTileFrame(i, j);
				NetMessage.SendTileSquare(-1, i, j);
			}
			break;
		}
		if (num >= 255 && num <= 268)
		{
			if (!vector.actuator())
			{
				if (num >= 262)
				{
					vector.type -= 7;
				}
				else
				{
					vector.type += 7;
				}
				WorldGen.SquareTileFrame(i, j);
				NetMessage.SendTileSquare(-1, i, j);
			}
			return;
		}
		if (num == 419)
		{
			int num2 = 18;
			if (vector.frameX >= num2)
			{
				num2 = -num2;
			}
			if (vector.frameX == 36)
			{
				num2 = 0;
			}
			SkipWire(i, j);
			vector.frameX = (short)(vector.frameX + num2);
			WorldGen.SquareTileFrame(i, j);
			NetMessage.SendTileSquare(-1, i, j);
			_LampsToCheck.Enqueue(new Point16(i, j));
			return;
		}
		if (num == 406)
		{
			int num3 = vector.frameX % 54 / 18;
			int num4 = vector.frameY % 54 / 18;
			int num5 = i - num3;
			int num6 = j - num4;
			int num7 = 54;
			if (Main.tile[num5, num6].frameY >= 108)
			{
				num7 = -108;
			}
			for (int k = num5; k < num5 + 3; k++)
			{
				for (int l = num6; l < num6 + 3; l++)
				{
					SkipWire(k, l);
					Main.tile[k, l].frameY = (short)(Main.tile[k, l].frameY + num7);
				}
			}
			NetMessage.SendTileSquare(-1, num5 + 1, num6 + 1, 3);
			return;
		}
		if (num == 452)
		{
			int num8 = vector.frameX % 54 / 18;
			int num9 = vector.frameY % 54 / 18;
			int num10 = i - num8;
			int num11 = j - num9;
			int num12 = 54;
			if (Main.tile[num10, num11].frameX >= 54)
			{
				num12 = -54;
			}
			for (int m = num10; m < num10 + 3; m++)
			{
				for (int n = num11; n < num11 + 3; n++)
				{
					SkipWire(m, n);
					Main.tile[m, n].frameX = (short)(Main.tile[m, n].frameX + num12);
				}
			}
			NetMessage.SendTileSquare(-1, num10 + 1, num11 + 1, 3);
			return;
		}
		if (num == 411)
		{
			int num13 = vector.frameX % 36 / 18;
			int num14 = vector.frameY % 36 / 18;
			int num15 = i - num13;
			int num16 = j - num14;
			int num17 = 36;
			if (Main.tile[num15, num16].frameX >= 36)
			{
				num17 = -36;
			}
			for (int num18 = num15; num18 < num15 + 2; num18++)
			{
				for (int num19 = num16; num19 < num16 + 2; num19++)
				{
					SkipWire(num18, num19);
					Main.tile[num18, num19].frameX = (short)(Main.tile[num18, num19].frameX + num17);
				}
			}
			NetMessage.SendTileSquare(-1, num15, num16, 2, 2);
			return;
		}
		if (num == 356)
		{
			int num20 = vector.frameX % 36 / 18;
			int num21 = vector.frameY % 54 / 18;
			int num22 = i - num20;
			int num23 = j - num21;
			for (int num24 = num22; num24 < num22 + 2; num24++)
			{
				for (int num25 = num23; num25 < num23 + 3; num25++)
				{
					SkipWire(num24, num25);
				}
			}
			if (!Main.fastForwardTimeToDawn && Main.sundialCooldown == 0)
			{
				Main.Sundialing();
			}
			NetMessage.SendTileSquare(-1, num22, num23, 2, 2);
			return;
		}
		if (num == 663)
		{
			int num26 = vector.frameX % 36 / 18;
			int num27 = vector.frameY % 54 / 18;
			int num28 = i - num26;
			int num29 = j - num27;
			for (int num30 = num28; num30 < num28 + 2; num30++)
			{
				for (int num31 = num29; num31 < num29 + 3; num31++)
				{
					SkipWire(num30, num31);
				}
			}
			if (!Main.fastForwardTimeToDusk && Main.moondialCooldown == 0)
			{
				Main.Moondialing();
			}
			NetMessage.SendTileSquare(-1, num28, num29, 2, 2);
			return;
		}
		if (num == 425)
		{
			int num32 = vector.frameX % 36 / 18;
			int num33 = vector.frameY % 36 / 18;
			int num34 = i - num32;
			int num35 = j - num33;
			for (int num36 = num34; num36 < num34 + 2; num36++)
			{
				for (int num37 = num35; num37 < num35 + 2; num37++)
				{
					SkipWire(num36, num37);
				}
			}
			if (Main.AnnouncementBoxDisabled)
			{
				return;
			}
			Color pink = Color.Pink;
			int num38 = Sign.ReadSign(num34, num35, CreateIfMissing: false);
			if (num38 == -1 || Main.sign[num38] == null || string.IsNullOrWhiteSpace(Main.sign[num38].text))
			{
				return;
			}
			if (Main.AnnouncementBoxRange == -1)
			{
				if (Main.netMode == 0)
				{
					Main.NewTextMultiline(Main.sign[num38].text, force: false, pink, 460);
				}
				else if (Main.netMode == 2)
				{
					NetMessage.SendData(107, -1, -1, NetworkText.FromLiteral(Main.sign[num38].text), 255, (int)pink.R, (int)pink.G, (int)pink.B, 460);
				}
			}
			else if (Main.netMode == 0)
			{
				if (Main.player[Main.myPlayer].Distance(new Vector2(num34 * 16 + 16, num35 * 16 + 16)) <= (float)Main.AnnouncementBoxRange)
				{
					Main.NewTextMultiline(Main.sign[num38].text, force: false, pink, 460);
				}
			}
			else
			{
				if (Main.netMode != 2)
				{
					return;
				}
				for (int num39 = 0; num39 < 255; num39++)
				{
					if (Main.player[num39].active && Main.player[num39].Distance(new Vector2(num34 * 16 + 16, num35 * 16 + 16)) <= (float)Main.AnnouncementBoxRange)
					{
						NetMessage.SendData(107, num39, -1, NetworkText.FromLiteral(Main.sign[num38].text), 255, (int)pink.R, (int)pink.G, (int)pink.B, 460);
					}
				}
			}
			return;
		}
		if (num == 405)
		{
			ToggleFirePlace(i, j, vector, type, damage);
			return;
		}
		if (num == 209)
		{
			int num40 = vector.frameX % 72 / 18;
			int num41 = vector.frameY % 54 / 18;
			int num42 = i - num40;
			int num43 = j - num41;
			int num44 = vector.frameY / 54;
			int num45 = vector.frameX / 72;
			int num46 = -1;
			if (num40 == 1 || num40 == 2)
			{
				num46 = num41;
			}
			int num47 = 0;
			if (num40 == 3)
			{
				num47 = -54;
			}
			if (num40 == 0)
			{
				num47 = 54;
			}
			if (num44 >= 8 && num47 > 0)
			{
				num47 = 0;
			}
			if (num44 == 0 && num47 < 0)
			{
				num47 = 0;
			}
			bool flag = false;
			if (num47 != 0)
			{
				for (int num48 = num42; num48 < num42 + 4; num48++)
				{
					for (int num49 = num43; num49 < num43 + 3; num49++)
					{
						SkipWire(num48, num49);
						Main.tile[num48, num49].frameY = (short)(Main.tile[num48, num49].frameY + num47);
					}
				}
				flag = true;
			}
			if ((num45 == 3 || num45 == 4) && (num46 == 0 || num46 == 1))
			{
				num47 = ((num45 == 3) ? 72 : (-72));
				for (int num50 = num42; num50 < num42 + 4; num50++)
				{
					for (int num51 = num43; num51 < num43 + 3; num51++)
					{
						SkipWire(num50, num51);
						Main.tile[num50, num51].frameX = (short)(Main.tile[num50, num51].frameX + num47);
					}
				}
				flag = true;
			}
			if (flag)
			{
				NetMessage.SendTileSquare(-1, num42, num43, 4, 3);
			}
			if (num46 != -1)
			{
				bool flag2 = true;
				if ((num45 == 3 || num45 == 4) && num46 < 2)
				{
					flag2 = false;
				}
				if (CheckMech(num42, num43, 30) && flag2)
				{
					WorldGen.ShootFromCannon(num42, num43, num44, num45 + 1, 0, 0f, CurrentUser, fromWire: true);
				}
			}
			return;
		}
		if (num == 212)
		{
			int num52 = vector.frameX % 54 / 18;
			int num53 = vector.frameY % 54 / 18;
			int num54 = i - num52;
			int num55 = j - num53;
			int num56 = vector.frameX / 54;
			int num57 = -1;
			if (num52 == 1)
			{
				num57 = num53;
			}
			int num58 = 0;
			if (num52 == 0)
			{
				num58 = -54;
			}
			if (num52 == 2)
			{
				num58 = 54;
			}
			if (num56 >= 1 && num58 > 0)
			{
				num58 = 0;
			}
			if (num56 == 0 && num58 < 0)
			{
				num58 = 0;
			}
			bool flag3 = false;
			if (num58 != 0)
			{
				for (int num59 = num54; num59 < num54 + 3; num59++)
				{
					for (int num60 = num55; num60 < num55 + 3; num60++)
					{
						SkipWire(num59, num60);
						Main.tile[num59, num60].frameX = (short)(Main.tile[num59, num60].frameX + num58);
					}
				}
				flag3 = true;
			}
			if (flag3)
			{
				NetMessage.SendTileSquare(-1, num54, num55, 3, 3);
			}
			if (num57 != -1 && CheckMech(num54, num55, 10))
			{
				float num61 = 12f + (float)Main.rand.Next(450) * 0.01f;
				float num62 = Main.rand.Next(85, 105);
				float num63 = Main.rand.Next(-35, 11);
				int type2 = 166;
				int damage2 = 0;
				float knockBack = 0f;
				Vector2 vector2 = new Vector2((num54 + 2) * 16 - 8, (num55 + 2) * 16 - 8);
				if (vector.frameX / 54 == 0)
				{
					num62 *= -1f;
					vector2.X -= 12f;
				}
				else
				{
					vector2.X += 12f;
				}
				float num64 = num62;
				float num65 = num63;
				float num66 = (float)Math.Sqrt(num64 * num64 + num65 * num65);
				num66 = num61 / num66;
				num64 *= num66;
				num65 *= num66;
				Projectile.NewProjectile(GetProjectileSource(num54, num55), vector2.X, vector2.Y, num64, num65, type2, damage2, knockBack, CurrentUser);
			}
			return;
		}
		if (num == 215)
		{
			ToggleCampFire(i, j, vector, type, damage);
			return;
		}
		if (num == 130)
		{
			if (Main.tile[i, j - 1] == null || !Main.tile[i, j - 1].active() || (!TileID.Sets.BasicChest[Main.tile[i, j - 1].type] && !TileID.Sets.BasicChestFake[Main.tile[i, j - 1].type] && Main.tile[i, j - 1].type != 88))
			{
				vector.type = 131;
				WorldGen.SquareTileFrame(i, j);
				NetMessage.SendTileSquare(-1, i, j);
			}
			return;
		}
		if (num == 131)
		{
			vector.type = 130;
			WorldGen.SquareTileFrame(i, j);
			NetMessage.SendTileSquare(-1, i, j);
			return;
		}
		if (num == 387 || num == 386)
		{
			bool value = num == 387;
			int num67 = WorldGen.ShiftTrapdoor(i, j, playerAbove: true).ToInt();
			if (num67 == 0)
			{
				num67 = -WorldGen.ShiftTrapdoor(i, j, playerAbove: false).ToInt();
			}
			if (num67 != 0)
			{
				NetMessage.SendData(19, -1, -1, null, 3 - value.ToInt(), i, j, num67);
			}
			return;
		}
		if (num == 389 || num == 388)
		{
			bool flag4 = num == 389;
			WorldGen.ShiftTallGate(i, j, flag4);
			NetMessage.SendData(19, -1, -1, null, 4 + flag4.ToInt(), i, j);
			return;
		}
		if (num == 11)
		{
			if (WorldGen.CloseDoor(i, j, forced: true))
			{
				NetMessage.SendData(19, -1, -1, null, 1, i, j);
			}
			return;
		}
		if (num == 10)
		{
			int num68 = 1;
			if (Main.rand.Next(2) == 0)
			{
				num68 = -1;
			}
			if (!WorldGen.OpenDoor(i, j, num68))
			{
				if (WorldGen.OpenDoor(i, j, -num68))
				{
					NetMessage.SendData(19, -1, -1, null, 0, i, j, -num68);
				}
			}
			else
			{
				NetMessage.SendData(19, -1, -1, null, 0, i, j, num68);
			}
			return;
		}
		if (num == 216)
		{
			WorldGen.LaunchRocket(i, j, fromWiring: true);
			SkipWire(i, j);
			return;
		}
		if (num == 497 || (num == 15 && vector.frameY / 40 == 1) || (num == 15 && vector.frameY / 40 == 20))
		{
			int num69 = j - vector.frameY % 40 / 18;
			SkipWire(i, num69);
			SkipWire(i, num69 + 1);
			if (CheckMech(i, num69, 60))
			{
				Projectile.NewProjectile(GetProjectileSource(i, num69), i * 16 + 8, num69 * 16 + 12, 0f, 0f, 733, 0, 0f, Main.myPlayer);
			}
			return;
		}
		switch (num)
		{
		case 335:
		{
			int num157 = j - vector.frameY / 18;
			int num158 = i - vector.frameX / 18;
			SkipWire(num158, num157);
			SkipWire(num158, num157 + 1);
			SkipWire(num158 + 1, num157);
			SkipWire(num158 + 1, num157 + 1);
			if (CheckMech(num158, num157, 30))
			{
				WorldGen.LaunchRocketSmall(num158, num157, fromWiring: true);
			}
			break;
		}
		case 338:
		{
			int num76 = j - vector.frameY / 18;
			int num77 = i - vector.frameX / 18;
			SkipWire(num77, num76);
			SkipWire(num77, num76 + 1);
			if (!CheckMech(num77, num76, 30))
			{
				break;
			}
			bool flag5 = false;
			for (int num78 = 0; num78 < 1000; num78++)
			{
				if (Main.projectile[num78].active && Main.projectile[num78].aiStyle == 73 && Main.projectile[num78].ai[0] == (float)num77 && Main.projectile[num78].ai[1] == (float)num76)
				{
					flag5 = true;
					break;
				}
			}
			if (!flag5)
			{
				int type3 = 419 + Main.rand.Next(4);
				Projectile.NewProjectile(GetProjectileSource(num77, num76), num77 * 16 + 8, num76 * 16 + 2, 0f, 0f, type3, 0, 0f, Main.myPlayer, num77, num76);
			}
			break;
		}
		case 235:
		{
			int num108 = i - vector.frameX / 18;
			if (vector.wall == 87 && (double)j > Main.worldSurface && !NPC.downedPlantBoss)
			{
				break;
			}
			if (_teleport[0].X == -1f)
			{
				_teleport[0].X = num108;
				_teleport[0].Y = j;
				if (vector.halfBrick())
				{
					_teleport[0].Y += 0.5f;
				}
			}
			else if (_teleport[0].X != (float)num108 || _teleport[0].Y != (float)j)
			{
				_teleport[1].X = num108;
				_teleport[1].Y = j;
				if (vector.halfBrick())
				{
					_teleport[1].Y += 0.5f;
				}
			}
			break;
		}
		case 4:
			ToggleTorch(i, j, vector, type);
			break;
		case 429:
		{
			int num79 = Main.tile[i, j].frameX / 18;
			bool flag6 = num79 % 2 >= 1;
			bool flag7 = num79 % 4 >= 2;
			bool flag8 = num79 % 8 >= 4;
			bool flag9 = num79 % 16 >= 8;
			bool flag10 = false;
			short num80 = 0;
			switch (_currentWireColor)
			{
			case 1:
				num80 = 18;
				flag10 = !flag6;
				break;
			case 2:
				num80 = 72;
				flag10 = !flag8;
				break;
			case 3:
				num80 = 36;
				flag10 = !flag7;
				break;
			case 4:
				num80 = 144;
				flag10 = !flag9;
				break;
			}
			if (flag10)
			{
				vector.frameX += num80;
			}
			else
			{
				vector.frameX -= num80;
			}
			NetMessage.SendTileSquare(-1, i, j);
			break;
		}
		case 149:
			ToggleHolidayLight(i, j, vector, type);
			break;
		case 244:
		{
			int num132;
			for (num132 = vector.frameX / 18; num132 >= 3; num132 -= 3)
			{
			}
			int num133;
			for (num133 = vector.frameY / 18; num133 >= 3; num133 -= 3)
			{
			}
			int num134 = i - num132;
			int num135 = j - num133;
			int num136 = 54;
			if (Main.tile[num134, num135].frameX >= 54)
			{
				num136 = -54;
			}
			for (int num137 = num134; num137 < num134 + 3; num137++)
			{
				for (int num138 = num135; num138 < num135 + 2; num138++)
				{
					SkipWire(num137, num138);
					Main.tile[num137, num138].frameX = (short)(Main.tile[num137, num138].frameX + num136);
				}
			}
			NetMessage.SendTileSquare(-1, num134, num135, 3, 2);
			break;
		}
		case 565:
		{
			int num99;
			for (num99 = vector.frameX / 18; num99 >= 2; num99 -= 2)
			{
			}
			int num100;
			for (num100 = vector.frameY / 18; num100 >= 2; num100 -= 2)
			{
			}
			int num101 = i - num99;
			int num102 = j - num100;
			int num103 = 36;
			if (Main.tile[num101, num102].frameX >= 36)
			{
				num103 = -36;
			}
			for (int num104 = num101; num104 < num101 + 2; num104++)
			{
				for (int num105 = num102; num105 < num102 + 2; num105++)
				{
					SkipWire(num104, num105);
					Main.tile[num104, num105].frameX = (short)(Main.tile[num104, num105].frameX + num103);
				}
			}
			NetMessage.SendTileSquare(-1, num101, num102, 2, 2);
			break;
		}
		case 42:
			ToggleHangingLantern(i, j, vector, type, damage);
			break;
		case 93:
			ToggleLamp(i, j, vector, type, damage);
			break;
		case 95:
		case 100:
		case 126:
		case 173:
		case 564:
			Toggle2x2Light(i, j, vector, type, damage);
			break;
		case 593:
		{
			SkipWire(i, j);
			short num106 = (short)((Main.tile[i, j].frameX != 0) ? (-18) : 18);
			Main.tile[i, j].frameX += num106;
			if (Main.netMode == 2)
			{
				NetMessage.SendTileSquare(-1, i, j, 1, 1);
			}
			int num107 = ((num106 > 0) ? 4 : 3);
			Animation.NewTemporaryAnimation(num107, 593, i, j);
			NetMessage.SendTemporaryAnimation(-1, num107, 593, i, j);
			break;
		}
		case 594:
		{
			int num81;
			for (num81 = vector.frameY / 18; num81 >= 2; num81 -= 2)
			{
			}
			num81 = j - num81;
			int num82 = vector.frameX / 18;
			if (num82 > 1)
			{
				num82 -= 2;
			}
			num82 = i - num82;
			SkipWire(num82, num81);
			SkipWire(num82, num81 + 1);
			SkipWire(num82 + 1, num81);
			SkipWire(num82 + 1, num81 + 1);
			short num83 = (short)((Main.tile[num82, num81].frameX != 0) ? (-36) : 36);
			for (int num84 = 0; num84 < 2; num84++)
			{
				for (int num85 = 0; num85 < 2; num85++)
				{
					Main.tile[num82 + num84, num81 + num85].frameX += num83;
				}
			}
			if (Main.netMode == 2)
			{
				NetMessage.SendTileSquare(-1, num82, num81, 2, 2);
			}
			int num86 = ((num83 > 0) ? 4 : 3);
			Animation.NewTemporaryAnimation(num86, 594, num82, num81);
			NetMessage.SendTemporaryAnimation(-1, num86, 594, num82, num81);
			break;
		}
		case 34:
			ToggleChandelier(i, j, vector, type, damage);
			break;
		case 314:
			if (CheckMech(i, j, 5))
			{
				Minecart.FlipSwitchTrack(i, j);
			}
			break;
		case 33:
		case 49:
		case 174:
		case 372:
		case 646:
			ToggleCandle(i, j, vector, type);
			break;
		case 92:
			ToggleLampPost(i, j, vector, type, damage);
			break;
		case 137:
		{
			int num139 = vector.frameY / 18;
			Vector2 vector4 = Vector2.Zero;
			float speedX = 0f;
			float speedY = 0f;
			int num140 = 0;
			int damage4 = 0;
			switch (num139)
			{
			case 0:
			case 1:
			case 2:
			case 5:
				if (CheckMech(i, j, 200))
				{
					int num148 = ((vector.frameX == 0) ? (-1) : ((vector.frameX == 18) ? 1 : 0));
					int num149 = ((vector.frameX >= 36) ? ((vector.frameX >= 72) ? 1 : (-1)) : 0);
					vector4 = new Vector2(i * 16 + 8 + 10 * num148, j * 16 + 8 + 10 * num149);
					float num150 = 3f;
					if (num139 == 0)
					{
						num140 = 98;
						damage4 = 20;
						num150 = 12f;
					}
					if (num139 == 1)
					{
						num140 = 184;
						damage4 = 40;
						num150 = 12f;
					}
					if (num139 == 2)
					{
						num140 = 187;
						damage4 = 40;
						num150 = 5f;
					}
					if (num139 == 5)
					{
						num140 = 980;
						damage4 = 30;
						num150 = 12f;
					}
					speedX = (float)num148 * num150;
					speedY = (float)num149 * num150;
				}
				break;
			case 3:
			{
				if (!CheckMech(i, j, 300))
				{
					break;
				}
				int num143 = 200;
				for (int num144 = 0; num144 < 1000; num144++)
				{
					if (Main.projectile[num144].active && Main.projectile[num144].type == num140)
					{
						float num145 = (new Vector2(i * 16 + 8, j * 18 + 8) - Main.projectile[num144].Center).Length();
						num143 = ((!(num145 < 50f)) ? ((!(num145 < 100f)) ? ((!(num145 < 200f)) ? ((!(num145 < 300f)) ? ((!(num145 < 400f)) ? ((!(num145 < 500f)) ? ((!(num145 < 700f)) ? ((!(num145 < 900f)) ? ((!(num145 < 1200f)) ? (num143 - 1) : (num143 - 2)) : (num143 - 3)) : (num143 - 4)) : (num143 - 5)) : (num143 - 6)) : (num143 - 8)) : (num143 - 10)) : (num143 - 15)) : (num143 - 50));
					}
				}
				if (num143 > 0)
				{
					num140 = 185;
					damage4 = 40;
					int num146 = 0;
					int num147 = 0;
					switch (vector.frameX / 18)
					{
					case 0:
					case 1:
						num146 = 0;
						num147 = 1;
						break;
					case 2:
						num146 = 0;
						num147 = -1;
						break;
					case 3:
						num146 = -1;
						num147 = 0;
						break;
					case 4:
						num146 = 1;
						num147 = 0;
						break;
					}
					speedX = (float)(4 * num146) + (float)Main.rand.Next(-20 + ((num146 == 1) ? 20 : 0), 21 - ((num146 == -1) ? 20 : 0)) * 0.05f;
					speedY = (float)(4 * num147) + (float)Main.rand.Next(-20 + ((num147 == 1) ? 20 : 0), 21 - ((num147 == -1) ? 20 : 0)) * 0.05f;
					vector4 = new Vector2(i * 16 + 8 + 14 * num146, j * 16 + 8 + 14 * num147);
				}
				break;
			}
			case 4:
				if (CheckMech(i, j, 90))
				{
					int num141 = 0;
					int num142 = 0;
					switch (vector.frameX / 18)
					{
					case 0:
					case 1:
						num141 = 0;
						num142 = 1;
						break;
					case 2:
						num141 = 0;
						num142 = -1;
						break;
					case 3:
						num141 = -1;
						num142 = 0;
						break;
					case 4:
						num141 = 1;
						num142 = 0;
						break;
					}
					speedX = 8 * num141;
					speedY = 8 * num142;
					damage4 = 60;
					num140 = 186;
					vector4 = new Vector2(i * 16 + 8 + 18 * num141, j * 16 + 8 + 18 * num142);
				}
				break;
			}
			switch (num139)
			{
			case -10:
				if (CheckMech(i, j, 200))
				{
					int num155 = -1;
					if (vector.frameX != 0)
					{
						num155 = 1;
					}
					speedX = 12 * num155;
					damage4 = 20;
					num140 = 98;
					vector4 = new Vector2(i * 16 + 8, j * 16 + 7);
					vector4.X += 10 * num155;
					vector4.Y += 2f;
				}
				break;
			case -9:
				if (CheckMech(i, j, 200))
				{
					int num151 = -1;
					if (vector.frameX != 0)
					{
						num151 = 1;
					}
					speedX = 12 * num151;
					damage4 = 40;
					num140 = 184;
					vector4 = new Vector2(i * 16 + 8, j * 16 + 7);
					vector4.X += 10 * num151;
					vector4.Y += 2f;
				}
				break;
			case -8:
				if (CheckMech(i, j, 200))
				{
					int num156 = -1;
					if (vector.frameX != 0)
					{
						num156 = 1;
					}
					speedX = 5 * num156;
					damage4 = 40;
					num140 = 187;
					vector4 = new Vector2(i * 16 + 8, j * 16 + 7);
					vector4.X += 10 * num156;
					vector4.Y += 2f;
				}
				break;
			case -7:
			{
				if (!CheckMech(i, j, 300))
				{
					break;
				}
				num140 = 185;
				int num152 = 200;
				for (int num153 = 0; num153 < 1000; num153++)
				{
					if (Main.projectile[num153].active && Main.projectile[num153].type == num140)
					{
						float num154 = (new Vector2(i * 16 + 8, j * 18 + 8) - Main.projectile[num153].Center).Length();
						num152 = ((!(num154 < 50f)) ? ((!(num154 < 100f)) ? ((!(num154 < 200f)) ? ((!(num154 < 300f)) ? ((!(num154 < 400f)) ? ((!(num154 < 500f)) ? ((!(num154 < 700f)) ? ((!(num154 < 900f)) ? ((!(num154 < 1200f)) ? (num152 - 1) : (num152 - 2)) : (num152 - 3)) : (num152 - 4)) : (num152 - 5)) : (num152 - 6)) : (num152 - 8)) : (num152 - 10)) : (num152 - 15)) : (num152 - 50));
					}
				}
				if (num152 > 0)
				{
					speedX = (float)Main.rand.Next(-20, 21) * 0.05f;
					speedY = 4f + (float)Main.rand.Next(0, 21) * 0.05f;
					damage4 = 40;
					vector4 = new Vector2(i * 16 + 8, j * 16 + 16);
					vector4.Y += 6f;
					Projectile.NewProjectile(GetProjectileSource(i, j), (int)vector4.X, (int)vector4.Y, speedX, speedY, num140, damage4, 2f, Main.myPlayer);
				}
				break;
			}
			case -6:
				if (CheckMech(i, j, 90))
				{
					speedX = 0f;
					speedY = 8f;
					damage4 = 60;
					num140 = 186;
					vector4 = new Vector2(i * 16 + 8, j * 16 + 16);
					vector4.Y += 10f;
				}
				break;
			}
			if (num140 != 0)
			{
				Projectile.NewProjectile(GetProjectileSource(i, j), (int)vector4.X, (int)vector4.Y, speedX, speedY, num140, damage4, 2f, Main.myPlayer);
			}
			break;
		}
		case 443:
			GeyserTrap(i, j);
			break;
		case 531:
		{
			int num127 = vector.frameX / 36;
			int num128 = vector.frameY / 54;
			int num129 = i - (vector.frameX - num127 * 36) / 18;
			int num130 = j - (vector.frameY - num128 * 54) / 18;
			if (CheckMech(num129, num130, 900))
			{
				Vector2 vector3 = new Vector2(num129 + 1, num130) * 16f;
				vector3.Y += 28f;
				int num131 = 99;
				int damage3 = 70;
				float knockBack2 = 10f;
				if (num131 != 0)
				{
					Projectile.NewProjectile(GetProjectileSource(num129, num130), (int)vector3.X, (int)vector3.Y, 0f, 0f, num131, damage3, knockBack2, Main.myPlayer);
				}
			}
			break;
		}
		case 35:
		case 139:
			WorldGen.SwitchMB(i, j);
			break;
		case 207:
			WorldGen.SwitchFountain(i, j);
			break;
		case 410:
		case 480:
		case 509:
		case 657:
		case 658:
			WorldGen.SwitchMonolith(i, j);
			break;
		case 455:
			BirthdayParty.ToggleManualParty();
			break;
		case 141:
			WorldGen.KillTile(i, j, fail: false, effectOnly: false, noItem: true);
			NetMessage.SendTileSquare(-1, i, j);
			Projectile.NewProjectile(GetProjectileSource(i, j), i * 16 + 8, j * 16 + 8, 0f, 0f, 108, 500, 10f, Main.myPlayer);
			break;
		case 210:
			WorldGen.ExplodeMine(i, j, fromWiring: true);
			break;
		case 142:
		case 143:
		{
			int num93 = j - vector.frameY / 18;
			int num94 = vector.frameX / 18;
			if (num94 > 1)
			{
				num94 -= 2;
			}
			num94 = i - num94;
			SkipWire(num94, num93);
			SkipWire(num94, num93 + 1);
			SkipWire(num94 + 1, num93);
			SkipWire(num94 + 1, num93 + 1);
			if (num == 142)
			{
				for (int num95 = 0; num95 < 4; num95++)
				{
					if (_numInPump >= 19)
					{
						break;
					}
					int num96;
					int num97;
					switch (num95)
					{
					case 0:
						num96 = num94;
						num97 = num93 + 1;
						break;
					case 1:
						num96 = num94 + 1;
						num97 = num93 + 1;
						break;
					case 2:
						num96 = num94;
						num97 = num93;
						break;
					default:
						num96 = num94 + 1;
						num97 = num93;
						break;
					}
					_inPumpX[_numInPump] = num96;
					_inPumpY[_numInPump] = num97;
					_numInPump++;
				}
				break;
			}
			for (int num98 = 0; num98 < 4; num98++)
			{
				if (_numOutPump >= 19)
				{
					break;
				}
				int num96;
				int num97;
				switch (num98)
				{
				case 0:
					num96 = num94;
					num97 = num93 + 1;
					break;
				case 1:
					num96 = num94 + 1;
					num97 = num93 + 1;
					break;
				case 2:
					num96 = num94;
					num97 = num93;
					break;
				default:
					num96 = num94 + 1;
					num97 = num93;
					break;
				}
				_outPumpX[_numOutPump] = num96;
				_outPumpY[_numOutPump] = num97;
				_numOutPump++;
			}
			break;
		}
		case 105:
		{
			int num109 = j - vector.frameY / 18;
			int num110 = vector.frameX / 18;
			int num111 = 0;
			while (num110 >= 2)
			{
				num110 -= 2;
				num111++;
			}
			num110 = i - num110;
			num110 = i - vector.frameX % 36 / 18;
			num109 = j - vector.frameY % 54 / 18;
			int num112 = vector.frameY / 54;
			num112 %= 3;
			num111 = vector.frameX / 36 + num112 * 55;
			SkipWire(num110, num109);
			SkipWire(num110, num109 + 1);
			SkipWire(num110, num109 + 2);
			SkipWire(num110 + 1, num109);
			SkipWire(num110 + 1, num109 + 1);
			SkipWire(num110 + 1, num109 + 2);
			int num113 = num110 * 16 + 16;
			int num114 = (num109 + 3) * 16;
			int num115 = -1;
			int num116 = -1;
			bool flag11 = true;
			bool flag12 = false;
			switch (num111)
			{
			case 5:
				num116 = 73;
				break;
			case 13:
				num116 = 24;
				break;
			case 30:
				num116 = 6;
				break;
			case 35:
				num116 = 2;
				break;
			case 51:
				num116 = Utils.SelectRandom(Main.rand, new short[2] { 299, 538 });
				break;
			case 52:
				num116 = 356;
				break;
			case 53:
				num116 = 357;
				break;
			case 54:
				num116 = Utils.SelectRandom(Main.rand, new short[2] { 355, 358 });
				break;
			case 55:
				num116 = Utils.SelectRandom(Main.rand, new short[2] { 367, 366 });
				break;
			case 56:
				num116 = Utils.SelectRandom(Main.rand, new short[5] { 359, 359, 359, 359, 360 });
				break;
			case 57:
				num116 = 377;
				break;
			case 58:
				num116 = 300;
				break;
			case 59:
				num116 = Utils.SelectRandom(Main.rand, new short[2] { 364, 362 });
				break;
			case 60:
				num116 = 148;
				break;
			case 61:
				num116 = 361;
				break;
			case 62:
				num116 = Utils.SelectRandom(Main.rand, new short[3] { 487, 486, 485 });
				break;
			case 63:
				num116 = 164;
				flag11 &= NPC.MechSpawn(num113, num114, 165);
				break;
			case 64:
				num116 = 86;
				flag12 = true;
				break;
			case 65:
				num116 = 490;
				break;
			case 66:
				num116 = 82;
				break;
			case 67:
				num116 = 449;
				break;
			case 68:
				num116 = 167;
				break;
			case 69:
				num116 = 480;
				break;
			case 70:
				num116 = 48;
				break;
			case 71:
				num116 = Utils.SelectRandom(Main.rand, new short[3] { 170, 180, 171 });
				flag12 = true;
				break;
			case 72:
				num116 = 481;
				break;
			case 73:
				num116 = 482;
				break;
			case 74:
				num116 = 430;
				break;
			case 75:
				num116 = 489;
				break;
			case 76:
				num116 = 611;
				break;
			case 77:
				num116 = 602;
				break;
			case 78:
				num116 = Utils.SelectRandom(Main.rand, new short[6] { 595, 596, 599, 597, 600, 598 });
				break;
			case 79:
				num116 = Utils.SelectRandom(Main.rand, new short[2] { 616, 617 });
				break;
			case 80:
				num116 = Utils.SelectRandom(Main.rand, new short[2] { 671, 672 });
				break;
			case 81:
				num116 = 673;
				break;
			case 82:
				num116 = Utils.SelectRandom(Main.rand, new short[2] { 674, 675 });
				break;
			}
			if (num116 != -1 && CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, num116) && flag11)
			{
				if (!flag12 || !Collision.SolidTiles(num110 - 2, num110 + 3, num109, num109 + 2))
				{
					num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114, num116);
				}
				else
				{
					Vector2 position = new Vector2(num113 - 4, num114 - 22) - new Vector2(10f);
					Utils.PoofOfSmoke(position);
					NetMessage.SendData(106, -1, -1, null, (int)position.X, position.Y);
				}
			}
			if (num115 <= -1)
			{
				switch (num111)
				{
				case 4:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 1))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114 - 12, 1);
					}
					break;
				case 7:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 49))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113 - 4, num114 - 6, 49);
					}
					break;
				case 8:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 55))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114 - 12, 55);
					}
					break;
				case 9:
				{
					int type4 = 46;
					if (BirthdayParty.PartyIsUp)
					{
						type4 = 540;
					}
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, type4))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114 - 12, type4);
					}
					break;
				}
				case 10:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 21))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114, 21);
					}
					break;
				case 16:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 42))
					{
						if (!Collision.SolidTiles(num110 - 1, num110 + 1, num109, num109 + 1))
						{
							num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114 - 12, 42);
							break;
						}
						Vector2 position3 = new Vector2(num113 - 4, num114 - 22) - new Vector2(10f);
						Utils.PoofOfSmoke(position3);
						NetMessage.SendData(106, -1, -1, null, (int)position3.X, position3.Y);
					}
					break;
				case 18:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 67))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114 - 12, 67);
					}
					break;
				case 23:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 63))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114 - 12, 63);
					}
					break;
				case 27:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 85))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113 - 9, num114, 85);
					}
					break;
				case 28:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 74))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114 - 12, Utils.SelectRandom(Main.rand, new short[3] { 74, 297, 298 }));
					}
					break;
				case 34:
				{
					for (int num125 = 0; num125 < 2; num125++)
					{
						for (int num126 = 0; num126 < 3; num126++)
						{
							Tile tile = Main.tile[num110 + num125, num109 + num126];
							tile.type = 349;
							tile.frameX = (short)(num125 * 18 + 216);
							tile.frameY = (short)(num126 * 18);
						}
					}
					Animation.NewTemporaryAnimation(0, 349, num110, num109);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, num110, num109, 2, 3);
					}
					break;
				}
				case 42:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 58))
					{
						num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114 - 12, 58);
					}
					break;
				case 37:
					if (CheckMech(num110, num109, 600) && Item.MechSpawn(num113, num114, 58) && Item.MechSpawn(num113, num114, 1734) && Item.MechSpawn(num113, num114, 1867))
					{
						Item.NewItem(GetItemSource(num113, num114), num113, num114 - 16, 0, 0, 58);
					}
					break;
				case 50:
					if (CheckMech(num110, num109, 30) && NPC.MechSpawn(num113, num114, 65))
					{
						if (!Collision.SolidTiles(num110 - 2, num110 + 3, num109, num109 + 2))
						{
							num115 = NPC.NewNPC(GetNPCSource(num110, num109), num113, num114 - 12, 65);
							break;
						}
						Vector2 position2 = new Vector2(num113 - 4, num114 - 22) - new Vector2(10f);
						Utils.PoofOfSmoke(position2);
						NetMessage.SendData(106, -1, -1, null, (int)position2.X, position2.Y);
					}
					break;
				case 2:
					if (CheckMech(num110, num109, 600) && Item.MechSpawn(num113, num114, 184) && Item.MechSpawn(num113, num114, 1735) && Item.MechSpawn(num113, num114, 1868))
					{
						Item.NewItem(GetItemSource(num113, num114), num113, num114 - 16, 0, 0, 184);
					}
					break;
				case 17:
					if (CheckMech(num110, num109, 600) && Item.MechSpawn(num113, num114, 166))
					{
						Item.NewItem(GetItemSource(num113, num114), num113, num114 - 20, 0, 0, 166);
					}
					break;
				case 40:
				{
					if (!CheckMech(num110, num109, 300))
					{
						break;
					}
					int num121 = 50;
					int[] array2 = new int[num121];
					int num122 = 0;
					for (int num123 = 0; num123 < 200; num123++)
					{
						if (Main.npc[num123].active && (Main.npc[num123].type == 17 || Main.npc[num123].type == 19 || Main.npc[num123].type == 22 || Main.npc[num123].type == 38 || Main.npc[num123].type == 54 || Main.npc[num123].type == 107 || Main.npc[num123].type == 108 || Main.npc[num123].type == 142 || Main.npc[num123].type == 160 || Main.npc[num123].type == 207 || Main.npc[num123].type == 209 || Main.npc[num123].type == 227 || Main.npc[num123].type == 228 || Main.npc[num123].type == 229 || Main.npc[num123].type == 368 || Main.npc[num123].type == 369 || Main.npc[num123].type == 550 || Main.npc[num123].type == 441 || Main.npc[num123].type == 588))
						{
							array2[num122] = num123;
							num122++;
							if (num122 >= num121)
							{
								break;
							}
						}
					}
					if (num122 > 0)
					{
						int num124 = array2[Main.rand.Next(num122)];
						Main.npc[num124].position.X = num113 - Main.npc[num124].width / 2;
						Main.npc[num124].position.Y = num114 - Main.npc[num124].height - 1;
						NetMessage.SendData(23, -1, -1, null, num124);
					}
					break;
				}
				case 41:
				{
					if (!CheckMech(num110, num109, 300))
					{
						break;
					}
					int num117 = 50;
					int[] array = new int[num117];
					int num118 = 0;
					for (int num119 = 0; num119 < 200; num119++)
					{
						if (Main.npc[num119].active && (Main.npc[num119].type == 18 || Main.npc[num119].type == 20 || Main.npc[num119].type == 124 || Main.npc[num119].type == 178 || Main.npc[num119].type == 208 || Main.npc[num119].type == 353 || Main.npc[num119].type == 633 || Main.npc[num119].type == 663))
						{
							array[num118] = num119;
							num118++;
							if (num118 >= num117)
							{
								break;
							}
						}
					}
					if (num118 > 0)
					{
						int num120 = array[Main.rand.Next(num118)];
						Main.npc[num120].position.X = num113 - Main.npc[num120].width / 2;
						Main.npc[num120].position.Y = num114 - Main.npc[num120].height - 1;
						NetMessage.SendData(23, -1, -1, null, num120);
					}
					break;
				}
				}
			}
			if (num115 >= 0)
			{
				Main.npc[num115].value = 0f;
				Main.npc[num115].npcSlots = 0f;
				Main.npc[num115].SpawnedFromStatue = true;
				Main.npc[num115].CanBeReplacedByOtherNPCs = true;
			}
			break;
		}
		case 349:
		{
			int num87 = vector.frameY / 18;
			num87 %= 3;
			int num88 = j - num87;
			int num89;
			for (num89 = vector.frameX / 18; num89 >= 2; num89 -= 2)
			{
			}
			num89 = i - num89;
			SkipWire(num89, num88);
			SkipWire(num89, num88 + 1);
			SkipWire(num89, num88 + 2);
			SkipWire(num89 + 1, num88);
			SkipWire(num89 + 1, num88 + 1);
			SkipWire(num89 + 1, num88 + 2);
			short num90 = (short)((Main.tile[num89, num88].frameX != 0) ? (-216) : 216);
			for (int num91 = 0; num91 < 2; num91++)
			{
				for (int num92 = 0; num92 < 3; num92++)
				{
					Main.tile[num89 + num91, num88 + num92].frameX += num90;
				}
			}
			if (Main.netMode == 2)
			{
				NetMessage.SendTileSquare(-1, num89, num88, 2, 3);
			}
			Animation.NewTemporaryAnimation((num90 <= 0) ? 1 : 0, 349, num89, num88);
			break;
		}
		case 506:
		{
			int num70 = vector.frameY / 18;
			num70 %= 3;
			int num71 = j - num70;
			int num72;
			for (num72 = vector.frameX / 18; num72 >= 2; num72 -= 2)
			{
			}
			num72 = i - num72;
			SkipWire(num72, num71);
			SkipWire(num72, num71 + 1);
			SkipWire(num72, num71 + 2);
			SkipWire(num72 + 1, num71);
			SkipWire(num72 + 1, num71 + 1);
			SkipWire(num72 + 1, num71 + 2);
			short num73 = (short)((Main.tile[num72, num71].frameX >= 72) ? (-72) : 72);
			for (int num74 = 0; num74 < 2; num74++)
			{
				for (int num75 = 0; num75 < 3; num75++)
				{
					Main.tile[num72 + num74, num71 + num75].frameX += num73;
				}
			}
			if (Main.netMode == 2)
			{
				NetMessage.SendTileSquare(-1, num72, num71, 2, 3);
			}
			break;
		}
		case 546:
			vector.type = 557;
			WorldGen.SquareTileFrame(i, j);
			NetMessage.SendTileSquare(-1, i, j);
			break;
		case 557:
			vector.type = 546;
			WorldGen.SquareTileFrame(i, j);
			NetMessage.SendTileSquare(-1, i, j);
			break;
		}
	}

	public static void ToggleHolidayLight(int i, int j, Tile tileCache, bool? forcedStateWhereTrueIsOn)
	{
		bool tile = tileCache.frameX >= 54;
		if (!forcedStateWhereTrueIsOn.HasValue || !forcedStateWhereTrueIsOn.Value != tile)
		{
			if (tileCache.frameX < 54)
			{
				tileCache.frameX += 54;
			}
			else
			{
				tileCache.frameX -= 54;
			}
			NetMessage.SendTileSquare(-1, i, j);
		}
	}

	public static void ToggleHangingLantern(int i, int j, Tile tileCache, bool? forcedStateWhereTrueIsOn, bool doSkipWires)
	{
		int blockDamaged;
		for (blockDamaged = tileCache.frameY / 18; blockDamaged >= 2; blockDamaged -= 2)
		{
		}
		int num = j - blockDamaged;
		short num2 = 18;
		if (tileCache.frameX > 0)
		{
			num2 = -18;
		}
		bool flag = tileCache.frameX > 0;
		if (!forcedStateWhereTrueIsOn.HasValue || !forcedStateWhereTrueIsOn.Value != flag)
		{
			Main.tile[i, num].frameX += num2;
			Main.tile[i, num + 1].frameX += num2;
			if (doSkipWires)
			{
				SkipWire(i, num);
				SkipWire(i, num + 1);
			}
			NetMessage.SendTileSquare(-1, i, j, 1, 2);
		}
	}

	public static void Toggle2x2Light(int i, int j, Tile tileCache, bool? forcedStateWhereTrueIsOn, bool doSkipWires)
	{
		int tile;
		for (tile = tileCache.frameY / 18; tile >= 2; tile -= 2)
		{
		}
		tile = j - tile;
		int tile2 = tileCache.frameX / 18;
		if (tile2 > 1)
		{
			tile2 -= 2;
		}
		tile2 = i - tile2;
		short type = 36;
		if (Main.tile[tile2, tile].frameX > 0)
		{
			type = -36;
		}
		bool num = Main.tile[tile2, tile].frameX > 0;
		if (!forcedStateWhereTrueIsOn.HasValue || !forcedStateWhereTrueIsOn.Value != num)
		{
			Main.tile[tile2, tile].frameX += type;
			Main.tile[tile2, tile + 1].frameX += type;
			Main.tile[tile2 + 1, tile].frameX += type;
			Main.tile[tile2 + 1, tile + 1].frameX += type;
			if (doSkipWires)
			{
				SkipWire(tile2, tile);
				SkipWire(tile2 + 1, tile);
				SkipWire(tile2, tile + 1);
				SkipWire(tile2 + 1, tile + 1);
			}
			NetMessage.SendTileSquare(-1, tile2, tile, 2, 2);
		}
	}

	public static void ToggleLampPost(int i, int j, Tile tileCache, bool? forcedStateWhereTrueIsOn, bool doSkipWires)
	{
		int tile = j - tileCache.frameY / 18;
		short tile2 = 18;
		if (tileCache.frameX > 0)
		{
			tile2 = -18;
		}
		bool flag = tileCache.frameX > 0;
		if (forcedStateWhereTrueIsOn.HasValue && !forcedStateWhereTrueIsOn.Value == flag)
		{
			return;
		}
		for (int k = tile; k < tile + 6; k++)
		{
			Main.tile[i, k].frameX += tile2;
			if (doSkipWires)
			{
				SkipWire(i, k);
			}
		}
		NetMessage.SendTileSquare(-1, i, tile, 1, 6);
	}

	public static void ToggleTorch(int i, int j, Tile tileCache, bool? forcedStateWhereTrueIsOn)
	{
		bool tile = tileCache.frameX >= 66;
		if (!forcedStateWhereTrueIsOn.HasValue || !forcedStateWhereTrueIsOn.Value != tile)
		{
			if (tileCache.frameX < 66)
			{
				tileCache.frameX += 66;
			}
			else
			{
				tileCache.frameX -= 66;
			}
			NetMessage.SendTileSquare(-1, i, j);
		}
	}

	public static void ToggleCandle(int i, int j, Tile tileCache, bool? forcedStateWhereTrueIsOn)
	{
		short tile = 18;
		if (tileCache.frameX > 0)
		{
			tile = -18;
		}
		bool num = tileCache.frameX > 0;
		if (!forcedStateWhereTrueIsOn.HasValue || !forcedStateWhereTrueIsOn.Value != num)
		{
			tileCache.frameX += tile;
			NetMessage.SendTileSquare(-1, i, j, 3);
		}
	}

	public static void ToggleLamp(int i, int j, Tile tileCache, bool? forcedStateWhereTrueIsOn, bool doSkipWires)
	{
		int tile;
		for (tile = tileCache.frameY / 18; tile >= 3; tile -= 3)
		{
		}
		tile = j - tile;
		short num = 18;
		if (tileCache.frameX > 0)
		{
			num = -18;
		}
		bool k = tileCache.frameX > 0;
		if (!forcedStateWhereTrueIsOn.HasValue || !forcedStateWhereTrueIsOn.Value != k)
		{
			Main.tile[i, tile].frameX += num;
			Main.tile[i, tile + 1].frameX += num;
			Main.tile[i, tile + 2].frameX += num;
			if (doSkipWires)
			{
				SkipWire(i, tile);
				SkipWire(i, tile + 1);
				SkipWire(i, tile + 2);
			}
			NetMessage.SendTileSquare(-1, i, tile, 1, 3);
		}
	}

	public static void ToggleChandelier(int i, int j, Tile tileCache, bool? forcedStateWhereTrueIsOn, bool doSkipWires)
	{
		int tileSafely;
		for (tileSafely = tileCache.frameY / 18; tileSafely >= 3; tileSafely -= 3)
		{
		}
		int num = j - tileSafely;
		int k = tileCache.frameX % 108 / 18;
		if (k > 2)
		{
			k -= 3;
		}
		k = i - k;
		short num2 = 54;
		if (Main.tile[k, num].frameX % 108 > 0)
		{
			num2 = -54;
		}
		bool flag = Main.tile[k, num].frameX % 108 > 0;
		if (forcedStateWhereTrueIsOn.HasValue && !forcedStateWhereTrueIsOn.Value == flag)
		{
			return;
		}
		for (int l = k; l < k + 3; l++)
		{
			for (int m = num; m < num + 3; m++)
			{
				Main.tile[l, m].frameX += num2;
				if (doSkipWires)
				{
					SkipWire(l, m);
				}
			}
		}
		NetMessage.SendTileSquare(-1, k + 1, num + 1, 3);
	}

	public static void ToggleCampFire(int i, int j, Tile tileCache, bool? forcedStateWhereTrueIsOn, bool doSkipWires)
	{
		int num = tileCache.frameX % 54 / 18;
		int num2 = tileCache.frameY % 36 / 18;
		int num3 = i - num;
		int num4 = j - num2;
		bool flag = Main.tile[num3, num4].frameY >= 36;
		if (forcedStateWhereTrueIsOn.HasValue && !forcedStateWhereTrueIsOn.Value == flag)
		{
			return;
		}
		int num5 = 36;
		if (Main.tile[num3, num4].frameY >= 36)
		{
			num5 = -36;
		}
		for (int k = num3; k < num3 + 3; k++)
		{
			for (int l = num4; l < num4 + 2; l++)
			{
				if (doSkipWires)
				{
					SkipWire(k, l);
				}
				Main.tile[k, l].frameY = (short)(Main.tile[k, l].frameY + num5);
			}
		}
		NetMessage.SendTileSquare(-1, num3, num4, 3, 2);
	}

	public static void ToggleFirePlace(int i, int j, Tile theBlock, bool? forcedStateWhereTrueIsOn, bool doSkipWires)
	{
		int type = theBlock.frameX % 54 / 18;
		int flag = theBlock.frameY % 36 / 18;
		int num = i - type;
		int num2 = j - flag;
		bool flag2 = Main.tile[num, num2].frameX >= 54;
		if (forcedStateWhereTrueIsOn.HasValue && !forcedStateWhereTrueIsOn.Value == flag2)
		{
			return;
		}
		int num3 = 54;
		if (Main.tile[num, num2].frameX >= 54)
		{
			num3 = -54;
		}
		for (int k = num; k < num + 3; k++)
		{
			for (int l = num2; l < num2 + 2; l++)
			{
				if (doSkipWires)
				{
					SkipWire(k, l);
				}
				Main.tile[k, l].frameX = (short)(Main.tile[k, l].frameX + num3);
			}
		}
		NetMessage.SendTileSquare(-1, num, num2, 3, 2);
	}

	private static void GeyserTrap(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.type != 443)
		{
			return;
		}
		int num = tile.frameX / 36;
		int num2 = i - (tile.frameX - num * 36) / 18;
		if (CheckMech(num2, j, 200))
		{
			Vector2 zero = Vector2.Zero;
			Vector2 zero2 = Vector2.Zero;
			int num3 = 654;
			int damage = 20;
			if (num < 2)
			{
				zero = new Vector2(num2 + 1, j) * 16f;
				zero2 = new Vector2(0f, -8f);
			}
			else
			{
				zero = new Vector2(num2 + 1, j + 1) * 16f;
				zero2 = new Vector2(0f, 8f);
			}
			if (num3 != 0)
			{
				Projectile.NewProjectile(GetProjectileSource(num2, j), (int)zero.X, (int)zero.Y, zero2.X, zero2.Y, num3, damage, 2f, Main.myPlayer);
			}
		}
	}

	private static void Teleport()
	{
		if (_teleport[0].X < _teleport[1].X + 3f && _teleport[0].X > _teleport[1].X - 3f && _teleport[0].Y > _teleport[1].Y - 3f && _teleport[0].Y < _teleport[1].Y)
		{
			return;
		}
		Rectangle[] i = new Rectangle[2];
		i[0].X = (int)(_teleport[0].X * 16f);
		i[0].Width = 48;
		i[0].Height = 48;
		i[0].Y = (int)(_teleport[0].Y * 16f - (float)i[0].Height);
		i[1].X = (int)(_teleport[1].X * 16f);
		i[1].Width = 48;
		i[1].Height = 48;
		i[1].Y = (int)(_teleport[1].Y * 16f - (float)i[1].Height);
		for (int j = 0; j < 2; j++)
		{
			Vector2 k = new Vector2(i[1].X - i[0].X, i[1].Y - i[0].Y);
			if (j == 1)
			{
				k = new Vector2(i[0].X - i[1].X, i[0].Y - i[1].Y);
			}
			if (!blockPlayerTeleportationForOneIteration)
			{
				for (int l = 0; l < 255; l++)
				{
					if (Main.player[l].active && !Main.player[l].dead && !Main.player[l].teleporting && TeleporterHitboxIntersects(i[j], Main.player[l].Hitbox))
					{
						Vector2 vector = Main.player[l].position + k;
						Main.player[l].teleporting = true;
						if (Main.netMode == 2)
						{
							RemoteClient.CheckSection(l, vector);
						}
						Main.player[l].Teleport(vector);
						if (Main.netMode == 2)
						{
							NetMessage.SendData(65, -1, -1, null, 0, l, vector.X, vector.Y);
						}
					}
				}
			}
			for (int m = 0; m < 200; m++)
			{
				if (Main.npc[m].active && !Main.npc[m].teleporting && Main.npc[m].lifeMax > 5 && !Main.npc[m].boss && !Main.npc[m].noTileCollide)
				{
					int type = Main.npc[m].type;
					if (!NPCID.Sets.TeleportationImmune[type] && TeleporterHitboxIntersects(i[j], Main.npc[m].Hitbox))
					{
						Main.npc[m].teleporting = true;
						Main.npc[m].Teleport(Main.npc[m].position + k);
					}
				}
			}
		}
		for (int n = 0; n < 255; n++)
		{
			Main.player[n].teleporting = false;
		}
		for (int num = 0; num < 200; num++)
		{
			Main.npc[num].teleporting = false;
		}
	}

	private static bool TeleporterHitboxIntersects(Rectangle teleporter, Rectangle entity)
	{
		Rectangle rectangle = Rectangle.Union(teleporter, entity);
		if (rectangle.Width <= teleporter.Width + entity.Width)
		{
			return rectangle.Height <= teleporter.Height + entity.Height;
		}
		return false;
	}

	private static void DeActive(int i, int j)
	{
		if (!Main.tile[i, j].active() || (Main.tile[i, j].type == 226 && (double)j > Main.worldSurface && !NPC.downedPlantBoss))
		{
			return;
		}
		bool flag = Main.tileSolid[Main.tile[i, j].type] && !TileID.Sets.NotReallySolid[Main.tile[i, j].type];
		ushort type = Main.tile[i, j].type;
		if (type == 314 || (uint)(type - 386) <= 3u || type == 476)
		{
			flag = false;
		}
		if (flag && (!Main.tile[i, j - 1].active() || (!TileID.Sets.BasicChest[Main.tile[i, j - 1].type] && Main.tile[i, j - 1].type != 26 && Main.tile[i, j - 1].type != 77 && Main.tile[i, j - 1].type != 88 && Main.tile[i, j - 1].type != 470 && Main.tile[i, j - 1].type != 475 && Main.tile[i, j - 1].type != 237 && Main.tile[i, j - 1].type != 597 && WorldGen.CanKillTile(i, j))))
		{
			Main.tile[i, j].inActive(inActive: true);
			WorldGen.SquareTileFrame(i, j, resetFrame: false);
			if (Main.netMode != 1)
			{
				NetMessage.SendTileSquare(-1, i, j);
			}
		}
	}

	private static void ReActive(int i, int j)
	{
		Main.tile[i, j].inActive(inActive: false);
		WorldGen.SquareTileFrame(i, j, resetFrame: false);
		if (Main.netMode != 1)
		{
			NetMessage.SendTileSquare(-1, i, j);
		}
	}

	private static void MassWireOperationInner(Player user, Point ps, Point pe, Vector2 dropPoint, bool dir, ref int wireCount, ref int actuatorCount)
	{
		Math.Abs(ps.X - pe.X);
		Math.Abs(ps.Y - pe.Y);
		int num = Math.Sign(pe.X - ps.X);
		int num2 = Math.Sign(pe.Y - ps.Y);
		WiresUI.Settings.MultiToolMode toolMode = WiresUI.Settings.ToolMode;
		Point pt = default(Point);
		bool flag = false;
		Item.StartCachingType(530);
		Item.StartCachingType(849);
		bool flag2 = dir;
		int num3;
		int num4;
		int num5;
		if (flag2)
		{
			pt.X = ps.X;
			num3 = ps.Y;
			num4 = pe.Y;
			num5 = num2;
		}
		else
		{
			pt.Y = ps.Y;
			num3 = ps.X;
			num4 = pe.X;
			num5 = num;
		}
		for (int i = num3; i != num4; i += num5)
		{
			if (flag)
			{
				break;
			}
			if (flag2)
			{
				pt.Y = i;
			}
			else
			{
				pt.X = i;
			}
			bool? flag3 = MassWireOperationStep(user, pt, toolMode, ref wireCount, ref actuatorCount);
			if (flag3.HasValue && !flag3.Value)
			{
				flag = true;
				break;
			}
		}
		if (flag2)
		{
			pt.Y = pe.Y;
			num3 = ps.X;
			num4 = pe.X;
			num5 = num;
		}
		else
		{
			pt.X = pe.X;
			num3 = ps.Y;
			num4 = pe.Y;
			num5 = num2;
		}
		for (int j = num3; j != num4; j += num5)
		{
			if (flag)
			{
				break;
			}
			if (!flag2)
			{
				pt.Y = j;
			}
			else
			{
				pt.X = j;
			}
			bool? flag4 = MassWireOperationStep(user, pt, toolMode, ref wireCount, ref actuatorCount);
			if (flag4.HasValue && !flag4.Value)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			MassWireOperationStep(user, pe, toolMode, ref wireCount, ref actuatorCount);
		}
		EntitySource_ByItemSourceId reason = new EntitySource_ByItemSourceId(user, 5);
		Item.DropCache(reason, dropPoint, Vector2.Zero, 530);
		Item.DropCache(reason, dropPoint, Vector2.Zero, 849);
	}

	private static bool? MassWireOperationStep(Player user, Point pt, WiresUI.Settings.MultiToolMode mode, ref int wiresLeftToConsume, ref int actuatorsLeftToConstume)
	{
		if (!WorldGen.InWorld(pt.X, pt.Y, 1))
		{
			return null;
		}
		Tile tile = Main.tile[pt.X, pt.Y];
		if (tile == null)
		{
			return null;
		}
		if (user != null && !user.CanDoWireStuffHere(pt.X, pt.Y))
		{
			return null;
		}
		if (!mode.HasFlag(WiresUI.Settings.MultiToolMode.Cutter))
		{
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Red) && !tile.wire())
			{
				if (wiresLeftToConsume <= 0)
				{
					return false;
				}
				wiresLeftToConsume--;
				WorldGen.PlaceWire(pt.X, pt.Y);
				NetMessage.SendData(17, -1, -1, null, 5, pt.X, pt.Y);
			}
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Green) && !tile.wire3())
			{
				if (wiresLeftToConsume <= 0)
				{
					return false;
				}
				wiresLeftToConsume--;
				WorldGen.PlaceWire3(pt.X, pt.Y);
				NetMessage.SendData(17, -1, -1, null, 12, pt.X, pt.Y);
			}
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Blue) && !tile.wire2())
			{
				if (wiresLeftToConsume <= 0)
				{
					return false;
				}
				wiresLeftToConsume--;
				WorldGen.PlaceWire2(pt.X, pt.Y);
				NetMessage.SendData(17, -1, -1, null, 10, pt.X, pt.Y);
			}
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Yellow) && !tile.wire4())
			{
				if (wiresLeftToConsume <= 0)
				{
					return false;
				}
				wiresLeftToConsume--;
				WorldGen.PlaceWire4(pt.X, pt.Y);
				NetMessage.SendData(17, -1, -1, null, 16, pt.X, pt.Y);
			}
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Actuator) && !tile.actuator())
			{
				if (actuatorsLeftToConstume <= 0)
				{
					return false;
				}
				actuatorsLeftToConstume--;
				WorldGen.PlaceActuator(pt.X, pt.Y);
				NetMessage.SendData(17, -1, -1, null, 8, pt.X, pt.Y);
			}
		}
		if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Cutter))
		{
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Red) && tile.wire() && WorldGen.KillWire(pt.X, pt.Y))
			{
				NetMessage.SendData(17, -1, -1, null, 6, pt.X, pt.Y);
			}
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Green) && tile.wire3() && WorldGen.KillWire3(pt.X, pt.Y))
			{
				NetMessage.SendData(17, -1, -1, null, 13, pt.X, pt.Y);
			}
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Blue) && tile.wire2() && WorldGen.KillWire2(pt.X, pt.Y))
			{
				NetMessage.SendData(17, -1, -1, null, 11, pt.X, pt.Y);
			}
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Yellow) && tile.wire4() && WorldGen.KillWire4(pt.X, pt.Y))
			{
				NetMessage.SendData(17, -1, -1, null, 17, pt.X, pt.Y);
			}
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Actuator) && tile.actuator() && WorldGen.KillActuator(pt.X, pt.Y))
			{
				NetMessage.SendData(17, -1, -1, null, 9, pt.X, pt.Y);
			}
		}
		return true;
	}
}
