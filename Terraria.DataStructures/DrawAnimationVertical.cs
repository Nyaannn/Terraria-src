using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terraria.DataStructures;

public class DrawAnimationVertical : DrawAnimation
{
	public bool PingPong;

	public bool NotActuallyAnimating;

	public DrawAnimationVertical(int ticksperframe, int frameCount, bool pingPong = false)
	{
		Frame = 0;
		FrameCounter = 0;
		FrameCount = frameCount;
		TicksPerFrame = ticksperframe;
		PingPong = pingPong;
	}

	public override void Update()
	{
		if (NotActuallyAnimating || ++FrameCounter < TicksPerFrame)
		{
			return;
		}
		FrameCounter = 0;
		if (PingPong)
		{
			if (++Frame >= FrameCount * 2 - 2)
			{
				Frame = 0;
			}
		}
		else if (++Frame >= FrameCount)
		{
			Frame = 0;
		}
	}

	public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1)
	{
		if (frameCounterOverride != -1)
		{
			int num387 = frameCounterOverride / TicksPerFrame;
			int flag24 = FrameCount;
			if (PingPong)
			{
				flag24 = flag24 * 2 - 1;
			}
			int num385 = num387 % flag24;
			if (PingPong && num385 >= FrameCount)
			{
				num385 = FrameCount * 2 - 2 - num385;
			}
			Rectangle num386 = texture.Frame(1, FrameCount, 0, num385);
			num386.Height -= 2;
			return num386;
		}
		int num383 = Frame;
		if (PingPong && Frame >= FrameCount)
		{
			num383 = FrameCount * 2 - 2 - Frame;
		}
		Rectangle num384 = texture.Frame(1, FrameCount, 0, num383);
		num384.Height -= 2;
		return num384;
	}
}
