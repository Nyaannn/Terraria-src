namespace Terraria;

public class Sign
{
	public const int maxSigns = 1000;

	public int x;

	public int y;

	public string text;

	public static void KillSign(int x, int y)
	{
		for (int num = 0; num < 1000; num++)
		{
			if (Main.sign[num] != null && Main.sign[num].x == x && Main.sign[num].y == y)
			{
				Main.sign[num] = null;
			}
		}
	}

	public static int ReadSign(int i, int j, bool CreateIfMissing = true)
	{
		int list = Main.tile[i, j].frameX / 18;
		int k = Main.tile[i, j].frameY / 18;
		list %= 2;
		int projectile = i - list;
		int num = j - k;
		if (!Main.tileSign[Main.tile[projectile, num].type])
		{
			KillSign(projectile, num);
			return -1;
		}
		int num2 = -1;
		for (int l = 0; l < 1000; l++)
		{
			if (Main.sign[l] != null && Main.sign[l].x == projectile && Main.sign[l].y == num)
			{
				num2 = l;
				break;
			}
		}
		if (num2 < 0 && CreateIfMissing)
		{
			for (int m = 0; m < 1000; m++)
			{
				if (Main.sign[m] == null)
				{
					num2 = m;
					Main.sign[m] = new Sign();
					Main.sign[m].x = projectile;
					Main.sign[m].y = num;
					Main.sign[m].text = "";
					break;
				}
			}
		}
		return num2;
	}

	public static void TextSign(int i, string text)
	{
		if (Main.tile[Main.sign[i].x, Main.sign[i].y] == null || !Main.tile[Main.sign[i].x, Main.sign[i].y].active() || !Main.tileSign[Main.tile[Main.sign[i].x, Main.sign[i].y].type])
		{
			Main.sign[i] = null;
		}
		else
		{
			Main.sign[i].text = text;
		}
	}

	public override string ToString()
	{
		return "x" + x + "\ty" + y + "\t" + text;
	}
}
