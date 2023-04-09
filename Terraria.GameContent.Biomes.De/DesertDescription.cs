using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.WorldBuilding;

namespace Terraria.GameContent.Biomes.Desert;

public class DesertDescription
{
	public static readonly DesertDescription Invalid = new DesertDescription
	{
		IsValid = false
	};

	private static readonly Vector2D DefaultBlockScale = new Vector2D(4.0, 2.0);

	private const int SCAN_PADDING = 5;

	public Rectangle CombinedArea { get; private set; }

	public Rectangle Desert { get; private set; }

	public Rectangle Hive { get; private set; }

	public Vector2D BlockScale { get; private set; }

	public int BlockColumnCount { get; private set; }

	public int BlockRowCount { get; private set; }

	public bool IsValid { get; private set; }

	public SurfaceMap Surface { get; private set; }

	private DesertDescription()
	{
	}

	public void UpdateSurfaceMap()
	{
		Surface = SurfaceMap.FromArea(CombinedArea.Left - 5, CombinedArea.Width + 10);
	}

	public static DesertDescription CreateFromPlacement(Point origin)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		Vector2D num = DefaultBlockScale;
		double num2 = (double)Main.maxTilesX / 4200.0;
		int num3 = (int)(80.0 * num2);
		int num4 = (int)((WorldGen.genRand.NextDouble() * 0.5 + 1.5) * 170.0 * num2);
		if (WorldGen.remixWorldGen)
		{
			num4 = (int)(340.0 * num2);
		}
		int num5 = (int)(num.X * (double)num3);
		int num6 = (int)(num.Y * (double)num4);
		origin.X -= num5 / 2;
		SurfaceMap surfaceMap = SurfaceMap.FromArea(origin.X - 5, num5 + 10);
		if (RowHasInvalidTiles(origin.X, surfaceMap.Bottom, num5))
		{
			return Invalid;
		}
		int num7 = (int)(surfaceMap.Average + (double)surfaceMap.Bottom) / 2;
		origin.Y = num7 + WorldGen.genRand.Next(40, 60);
		int num8 = 0;
		if (Main.tenthAnniversaryWorld)
		{
			num8 = (int)(20.0 * num2);
		}
		return new DesertDescription
		{
			CombinedArea = new Rectangle(origin.X, num7, num5, origin.Y + num6 - num7),
			Hive = new Rectangle(origin.X, origin.Y + num8, num5, num6 - num8),
			Desert = new Rectangle(origin.X, num7, num5, origin.Y + num6 / 2 - num7 + num8),
			BlockScale = num,
			BlockColumnCount = num3,
			BlockRowCount = num4,
			Surface = surfaceMap,
			IsValid = true
		};
	}

	private static bool RowHasInvalidTiles(int startX, int startY, int width)
	{
		if (GenVars.skipDesertTileCheck)
		{
			return false;
		}
		for (int i = startX; i < startX + width; i++)
		{
			switch (Main.tile[i, startY].type)
			{
			case 59:
			case 60:
				return true;
			case 147:
			case 161:
				return true;
			}
		}
		return false;
	}
}
