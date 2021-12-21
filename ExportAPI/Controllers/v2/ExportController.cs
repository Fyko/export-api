using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DiscordChatExporter.Core.Discord;
using DiscordChatExporter.Core.Exceptions;
using DiscordChatExporter.Core.Exporting;
using DiscordChatExporter.Core.Exporting.Partitioning;
using DiscordChatExporter.Core.Exporting.Filtering;
using DiscordChatExporter.Core.Discord.Data;

namespace ExportAPI.Controllers
{
	[ApiController]
	[Route("v2/export")]
	public class ExportController : ControllerBase
	{
		private readonly ILogger<ExportController> _logger;

		private readonly HttpClient _httpclient = new HttpClient();

		public ExportController(ILogger<ExportController> logger)
		{
			_logger = logger;

			if (!Directory.Exists("/tmp/exports"))
			{
				Directory.CreateDirectory("/tmp/exports");
			}

			var nowhex = DateTime.Now.Ticks.ToString("X2");
			if (!Directory.Exists($"/tmp/exports/{nowhex}"))
			{
				Directory.CreateDirectory($"/tmp/exports/{nowhex}");
			}
		}

		internal static string GetPath(string channelId, ExportFormat exportType)
		{
			var nowhex = DateTime.Now.Ticks.ToString("X2");
			return $"/tmp/exports/{nowhex}/{channelId}.{exportType.GetFileExtension()}";
		}

		[HttpPost]
		public async Task<Stream> Post(ExportOptions options)
		{
			Stream SendJsonError(string error, int status)
			{
				Response.ContentType = "application/json";
				Response.StatusCode = status;
				return GenerateStreamFromString("{ \"error\": \"" + error + "\" }");
			}

			if (!Enum.IsDefined(typeof(ExportFormat), options.export_format))
			{
				return SendJsonError($"An export format with the id '{options.export_format}' was not found.", 400);
			}

			var parsed = Snowflake.TryParse(options.channel_id);
			var channelId = parsed ?? Snowflake.Zero;

			var token = new AuthToken(AuthTokenKind.Bot, options.token);
			var client = new DiscordClient(token);
			Channel channel;
			try
			{
				channel = await client.GetChannelAsync(channelId);
			}
			catch (DiscordChatExporterException e)
			{
				if (e.Message.Contains("Authentication")) return SendJsonError("An invalid Discord token was provided.", 401);
				if (e.Message.Contains("Requested resource does not exist")) return SendJsonError("A channel with the provided ID was not found.", 404);
				return SendJsonError($"An unknown error occurred: {e.Message}", 500);
			}

			var guild = await client.GetGuildAsync(channel.GuildId);

			using var req = new HttpRequestMessage(HttpMethod.Get, new Uri("https://discord.com/api/v8/users/@me"));
			req.Headers.Authorization = token.GetAuthenticationHeader();
			var res = await _httpclient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
			var text = await res.Content.ReadAsStringAsync();
			var me = DiscordChatExporter.Core.Discord.Data.User.Parse(JsonDocument.Parse(text).RootElement.Clone());

			var path = GetPath(channel.Id.ToString(), options.export_format);
			_logger.LogInformation($"[{me.FullName} ({me.Id})] Exporting #{channel.Name} ({channel.Id}) within {guild.Name} ({guild.Id}) to {path}");
			var request = new ExportRequest(
				guild,
				channel,
				path,
				options.export_format,
				Snowflake.TryParse(options.after),
				Snowflake.TryParse(options.before),
				PartitionLimit.Null,
				MessageFilter.Null,
				false,
				false,
				options.date_format
			);

			var exporter = new ChannelExporter(client);

			_logger.LogInformation("Starting export");
			await exporter.ExportChannelAsync(request);
			_logger.LogInformation("Finished exporting");
			var stream = new FileStream(path, FileMode.Open);

			var ext = options.export_format.GetFileExtension();
			if (ext == "txt") ext = "plain";
			Response.ContentType = $"text/{ext}; charset=UTF-8";
			Response.StatusCode = 200;

			deleteFile(path);

			return stream;
		}

		async internal void deleteFile(string path)
		{
			await Task.Delay(TimeSpan.FromSeconds(30));
			var exists = System.IO.File.Exists(path);
			if (exists)
			{
				try
				{
					System.IO.File.Delete(path);
					_logger.LogInformation($"Deleted {path}");
				}
				catch { }
			}

		}

		internal Stream GenerateStreamFromString(string s)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}

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
