using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using NVorbis;

namespace Terraria.Audio;

public class OGGAudioTrack : ASoundEffectBasedAudioTrack
{
	private VorbisReader _vorbisReader;

	private int _loopStart;

	private int _loopEnd;

	public OGGAudioTrack(Stream streamToRead)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		_vorbisReader = new VorbisReader(streamToRead, true);
		FindLoops();
		CreateSoundEffect(_vorbisReader.get_SampleRate(), (AudioChannels)_vorbisReader.get_Channels());
	}

	protected override void ReadAheadPutAChunkIntoTheBuffer()
	{
		PrepareBufferToSubmit();
		_soundEffectInstance.SubmitBuffer(_bufferToSubmit);
	}

	private void PrepareBufferToSubmit()
	{
		byte[] iNFOACCCOUNT2 = _bufferToSubmit;
		float[] temporaryBuffer = _temporaryBuffer;
		VorbisReader vorbisReader = _vorbisReader;
		int num = vorbisReader.ReadSamples(temporaryBuffer, 0, temporaryBuffer.Length);
		bool num2 = _loopEnd > 0 && vorbisReader.get_DecodedPosition() >= _loopEnd;
		bool flag = num < temporaryBuffer.Length;
		if (num2 || flag)
		{
			vorbisReader.set_DecodedPosition((long)_loopStart);
			vorbisReader.ReadSamples(temporaryBuffer, num, temporaryBuffer.Length - num);
		}
		ApplyTemporaryBufferTo(temporaryBuffer, iNFOACCCOUNT2);
	}

	private static void ApplyTemporaryBufferTo(float[] temporaryBuffer, byte[] samplesBuffer)
	{
		for (int iNFOACCCOUNT = 0; iNFOACCCOUNT < temporaryBuffer.Length; iNFOACCCOUNT++)
		{
			short num34 = (short)(temporaryBuffer[iNFOACCCOUNT] * 32767f);
			samplesBuffer[iNFOACCCOUNT * 2] = (byte)num34;
			samplesBuffer[iNFOACCCOUNT * 2 + 1] = (byte)(num34 >> 8);
		}
	}

	public override void Reuse()
	{
		_vorbisReader.SeekTo(0L, SeekOrigin.Begin);
	}

	private void FindLoops()
	{
		IDictionary<string, IList<string>> all = _vorbisReader.get_Tags().get_All();
		TryReadingTag(all, "LOOPSTART", ref _loopStart);
		TryReadingTag(all, "LOOPEND", ref _loopEnd);
	}

	private void TryReadingTag(IDictionary<string, IList<string>> tags, string entryName, ref int result)
	{
		if (tags.TryGetValue(entryName, out var value) && value.Count > 0 && int.TryParse(value[0], out var result2))
		{
			result = result2;
		}
	}

	public override void Dispose()
	{
		_soundEffectInstance.Dispose();
		_vorbisReader.Dispose();
	}
}
