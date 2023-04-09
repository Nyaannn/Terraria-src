using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Terraria.Social.WeGame;

public class WeGameHelper
{
	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	private static extern void OutputDebugString(string message);

	public static void WriteDebugString(string format, params object[] args)
	{
		_ = "[WeGame] - " + format;
	}

	public static string Serialize<T>(T data)
	{
		DataContractJsonSerializer key = new DataContractJsonSerializer(typeof(T));
		using MemoryStream memoryStream = new MemoryStream();
		key.WriteObject((Stream)memoryStream, (object)data);
		memoryStream.Position = 0L;
		using StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8);
		return streamReader.ReadToEnd();
	}

	public static void UnSerialize<T>(string str, out T data)
	{
		using MemoryStream smartCursorKey = new MemoryStream(Encoding.Unicode.GetBytes(str));
		DataContractJsonSerializer smartSelectKey = new DataContractJsonSerializer(typeof(T));
		data = (T)smartSelectKey.ReadObject((Stream)smartCursorKey);
	}
}
