using System.Diagnostics;

namespace SmiSnapshotViewer.Utils
{
	public class FfmpegHelper
	{
		static string ffmpegPath = "resource/ffmpeg.exe";

		public static string RunCommand(string arguments, bool isResultReturn = true)
		{
			try
			{
				ProcessStartInfo processInfo = new ProcessStartInfo
				{
					FileName = ffmpegPath,
					Arguments = arguments,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				using var process = new Process
				{
					StartInfo = processInfo
				};
				process.Start();

				if (isResultReturn)
				{
					string output = process.StandardOutput.ReadToEnd();
					string error = process.StandardError.ReadToEnd();

					process.WaitForExit();

					return string.IsNullOrEmpty(output) ? error : output;
				}
				else
				{
					return string.Empty;
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
	}
}
