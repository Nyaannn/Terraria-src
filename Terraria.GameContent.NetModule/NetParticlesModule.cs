using System.IO;
using Terraria.GameContent.Drawing;
using Terraria.Net;

namespace Terraria.GameContent.NetModules;

public class NetParticlesModule : NetModule
{
	public static NetPacket Serialize(ParticleOrchestraType particleType, ParticleOrchestraSettings settings)
	{
		NetPacket num = NetModule.CreatePacket<NetParticlesModule>(22);
		num.Writer.Write((byte)particleType);
		settings.Serialize(num.Writer);
		return num;
	}

	public override bool Deserialize(BinaryReader reader, int userId)
	{
		ParticleOrchestraType particleOrchestraType = (ParticleOrchestraType)reader.ReadByte();
		ParticleOrchestraSettings settings = default(ParticleOrchestraSettings);
		settings.DeserializeFrom(reader);
		if (Main.netMode == 2)
		{
			NetManager.Instance.Broadcast(Serialize(particleOrchestraType, settings), userId);
		}
		else
		{
			ParticleOrchestrator.SpawnParticlesDirect(particleOrchestraType, settings);
		}
		return true;
	}
}
