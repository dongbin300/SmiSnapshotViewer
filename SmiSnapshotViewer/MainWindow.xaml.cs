using Microsoft.Win32;

using SmiSnapshotViewer.Utils;

using System.IO;
using System.Text;
using System.Windows;

namespace SmiSnapshotViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			WaitTextBox.Text = Settings1.Default.Wait;
			FontFamilyTextBox.Text = Settings1.Default.FontFamily;
			FontSizeTextBox.Text = Settings1.Default.FontSize;
		}

		private void VideoFileSelectButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();

			if (dialog.ShowDialog() ?? false)
			{
				VideoFileNameTextBox.Text = dialog.FileName;
			}
		}

		private void ExtractButton_Click(object sender, RoutedEventArgs e)
		{
			Settings1.Default.Wait = WaitTextBox.Text;
			Settings1.Default.FontFamily = FontFamilyTextBox.Text;
			Settings1.Default.FontSize = FontSizeTextBox.Text;
			Settings1.Default.Save();

			var inputVideoFile = VideoFileNameTextBox.Text;
			var inputSmiFile = VideoFileNameTextBox.Text.Replace(".mp4", ".smi").Replace(".avi", ".smi").Replace(".flv", ".smi");
			var inputVideoDirectory = Path.GetDirectoryName(inputVideoFile) ?? default!;
			var outputImageDirectory = Path.Combine(inputVideoDirectory, "result");
			var outputImageDirectory2 = Path.Combine(inputVideoDirectory, "result2");
			var fps = 1m;

			if (!Directory.Exists(outputImageDirectory))
			{
				Directory.CreateDirectory(outputImageDirectory);
			}
			if (!Directory.Exists(outputImageDirectory2))
			{
				Directory.CreateDirectory(outputImageDirectory2);
			}

			/* FPS 가져오기 */
			var info = FfmpegHelper.RunCommand($"-i \"{inputVideoFile}\"");
			var fpsString = info.Split(',').Where(x => x.Contains("fps"));
			if (fpsString.Any())
			{
				fps = decimal.Parse(fpsString.ElementAt(0).Replace("fps", "").Trim());
			}

			/* SMI 파싱 및 프레임이미지와 매핑 */
			var smi = SmiHelper.ParseSmi(inputSmiFile);
			var frameSubtitleMap = new Dictionary<int, string>();
			for (var i = 0; i < smi.Subtitles.Count; i++)
			{
				frameSubtitleMap.Add(i + 1, smi.Subtitles[i].Text);
			}

			/* 프레임 이미지 추출 */
			var builder = new StringBuilder();
			builder.Append("select='");
			List<int> frameNums = [];
			foreach (var subtitle in smi.Subtitles)
			{
				var frameNum = (int)(subtitle.Timing / 1000 * fps);
				frameNums.Add(frameNum);
			}
			builder.Append(string.Join("+", frameNums.Select(x => $"eq(n,{x})")));
			builder.Append('\'');
			var command = $"-i \"{inputVideoFile}\" -vf \"{builder}\" -vsync vfr \"{outputImageDirectory}/%04d.png\"";
			var result = FfmpegHelper.RunCommand(command, false);

			/* 추출하는 동안 대기 */
			Thread.Sleep(int.Parse(WaitTextBox.Text));

			/* 자막 오버레잉 */
			SubtitleOverlay.AddSubtitleToImages(frameSubtitleMap, outputImageDirectory, outputImageDirectory2, "", FontFamilyTextBox.Text, float.Parse(FontSizeTextBox.Text));

			MessageBox.Show("Complete!");
		}
	}
}