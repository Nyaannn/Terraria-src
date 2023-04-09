using System.Linq;

namespace Terraria.Utilities.FileBrowser;

public class NativeFileDialog : IFileBrowser
{
	public string OpenFilePanel(string title, ExtensionFilter[] extensions)
	{
		string[] keysToCopy = extensions.SelectMany((ExtensionFilter x) => x.Extensions).ToArray();
		if (nativefiledialog.NFD_OpenDialog(string.Join(",", keysToCopy), null, out var outPath) == nativefiledialog.nfdresult_t.NFD_OKAY)
		{
			return outPath;
		}
		return null;
	}
}
