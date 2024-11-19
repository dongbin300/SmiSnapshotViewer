using SmiSnapshotViewer.Models;

using System.IO;
using System.Text.RegularExpressions;

namespace SmiSnapshotViewer.Utils
{
	public class SmiHelper
	{
		public static Smi ParseSmi(string smiFileName)
		{
			var smi = new Smi();

			var data = File.ReadAllText(smiFileName);

			var syncRegex = new Regex(@"<SYNC Start=(\d+)><P Class=KRCC>\s*([\s\S]*?)\s*(?=<SYNC|$)", RegexOptions.IgnoreCase);
			var matches = syncRegex.Matches(data);

			foreach (Match match in matches)
			{
				if (match.Success)
				{
					var startTime = int.Parse(match.Groups[1].Value);
					var subtitle = match.Groups[2].Value.Replace("<br>", " ").Replace("&nbsp;", "").Trim();

					if (string.IsNullOrWhiteSpace(subtitle))
					{
						continue;
					}

					smi.Subtitles.Add(new Subtitle(startTime, subtitle));
				}
			}

			return smi;
		}
	}
}
