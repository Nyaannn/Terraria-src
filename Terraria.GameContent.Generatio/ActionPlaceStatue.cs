using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace Terraria.GameContent.Generation;

public class ActionPlaceStatue : GenAction
{
	private int _statueIndex;

	public ActionPlaceStatue(int index = -1)
	{
		_statueIndex = index;
	}

	public override bool Apply(Point origin, int x, int y, params object[] args)
	{
		Point16 value = ((_statueIndex != -1) ? GenVars.statueList[_statueIndex] : GenVars.statueList[GenBase._random.Next(2, GenVars.statueList.Length)]);
		WorldGen.PlaceTile(x, y, value.X, mute: true, forced: false, -1, value.Y);
		return UnitApply(origin, x, y, args);
	}
}
