using System;
using System.Drawing;
using System.IO;
using System.Xml;

namespace GenerateEmotePack
{
	class Program
	{
		static void Main(string[] args)
		{
			var plist = new XmlDocument();
			plist.Load("SA-Emoticons.AdiumEmoticonSet\\Emoticons.plist");

			// What the christ is this format. Well, whatever.
			var listKeys = plist.SelectNodes("/plist/dict/dict/key");
			var listValues = plist.SelectNodes("/plist/dict/dict/dict");

			if (listKeys.Count == 0)
				throw new InvalidDataException("Empty Emoticons.plist file.");

			if (listKeys.Count != listValues.Count)
				throw new InvalidDataException("Broken Emoticons.plist file.");

			using (var writer = new StreamWriter("sa_phpbb.pak"))
			{
				for (int i = 0; i < listKeys.Count; i++)
				{
					var filename = listKeys[i].InnerText;
					var metadata = listValues[i];

					var triggerText = metadata.SelectSingleNode("array/string").InnerText;
					var description = metadata.SelectSingleNode("string").InnerText;

					// Just in case, remove any apostrophes to reduce chance of string escape fuckups.
					description = description.Replace("'", "");

					// phpBB length limits
					if (description.Length > 50)
						description = description.Substring(0, 50);

					Console.WriteLine("Found {0} that is triggered by {1}, described as {2}", filename, triggerText, description);

					using (var image = Image.FromFile(Path.Combine("SA-Emoticons.AdiumEmoticonSet", filename)))
					{
						// filename, width, height, display on posting page (1 for yes, 0 for no), emotion, smilie code
						// Also comma at the end of each line, just because!
						// 'icon_e_surprised.gif', '15', '17', '1', 'Surprised', ':eek:', 
						writer.WriteLine($"'{filename}', '{image.Width}', '{image.Height}', '0', '{description}', '{triggerText}',");
					}
				}
			}
		}
	}
}
