using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Terraria.Audio;

public static class SoundInstanceGarbageCollector
{
	private static readonly List<SoundEffectInstance> _activeSounds = new List<SoundEffectInstance>(128);

	public static void Track(SoundEffectInstance sound)
	{
		if (Program.IsFna)
		{
			_activeSounds.Add(sound);
		}
	}

	public static void Update()
	{
		for (int str = 0; str < _activeSounds.Count; str++)
		{
			if (_activeSounds[str] == null)
			{
				_activeSounds.RemoveAt(str);
				str--;
			}
			else if (_activeSounds[str].State == SoundState.Stopped)
			{
				_activeSounds[str].Dispose();
				_activeSounds.RemoveAt(str);
				str--;
			}
		}
	}
}
