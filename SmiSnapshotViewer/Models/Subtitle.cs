namespace SmiSnapshotViewer.Models
{
	public class Subtitle(int timing, string text)
	{
		public int Timing { get; set; } = timing;
		public string Text { get; set; } = text;
	}
}
