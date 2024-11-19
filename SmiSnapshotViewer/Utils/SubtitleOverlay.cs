using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;

using FontStyle = System.Drawing.FontStyle;

namespace SmiSnapshotViewer.Utils
{
	public class SubtitleOverlay
	{
		public static void AddSubtitleToImages(Dictionary<int, string> frameSubtitleMap, string imageDir, string outputDir, string fontPath = "", string fontFamily = "맑은 고딕", float fontSize = 32)
		{
			var font = new Font(new FontFamily(fontFamily), fontSize, FontStyle.Bold);
			if (!string.IsNullOrEmpty(fontPath) && System.IO.File.Exists(fontPath))
			{
				var pfc = new PrivateFontCollection();
				pfc.AddFontFile(fontPath);
				font = new Font(pfc.Families[0], fontSize, FontStyle.Bold);
			}

			foreach (var frameSubtitle in frameSubtitleMap)
			{
				string inputImagePath = System.IO.Path.Combine(imageDir, $"{frameSubtitle.Key:D4}.png");
				string outputImagePath = System.IO.Path.Combine(outputDir, $"{frameSubtitle.Key:D4}.png");

				if (System.IO.File.Exists(inputImagePath))
				{
					using var image = Image.FromFile(inputImagePath);
					using var g = Graphics.FromImage(image);
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

					var text = frameSubtitle.Value;
					var textBrush = new SolidBrush(Color.White);
					var outlineBrush = new SolidBrush(Color.Black);
					var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

					var rect = new Rectangle(0, image.Height - 135, image.Width, 120);

					for (int dx = -1; dx <= 1; dx++)
					{
						for (int dy = -1; dy <= 1; dy++)
						{
							if (dx == 0 && dy == 0) continue;
							var offsetRect = new Rectangle(rect.X + dx, rect.Y + dy, rect.Width, rect.Height);
							g.DrawString(text, font, outlineBrush, offsetRect, format);
						}
					}

					g.DrawString(text, font, textBrush, rect, format);

					image.Save(outputImagePath, ImageFormat.Png);
				}
			}
		}
	}
}
