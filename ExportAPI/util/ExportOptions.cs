using DiscordChatExporter.Core.Exporting;

namespace ExportAPI.Util
{
	public class ExportOptions
	{
		public string token { get; set; }
		public string channel_id { get; set; }

		public string after { get; set; }

		public string before { get; set; }

		public ExportFormat export_format { get; set; } = ExportFormat.HtmlDark;

		public string date_format { get; set; } = "dd-MMM-yy hh:mm tt";
	}
}