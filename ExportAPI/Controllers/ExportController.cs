using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DiscordChatExporter.Domain.Discord;
using DiscordChatExporter.Domain.Exceptions;
using DiscordChatExporter.Domain.Exporting;
using DiscordChatExporter.Domain.Discord.Models;
using Newtonsoft.Json;

namespace ExportAPI.Controllers
{
	[ApiController]
	[Route("v1/export")]
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

		internal static string GetPath(string channelId)
		{
			var nowhex = DateTime.Now.Ticks.ToString("X2");
			return $"/tmp/exports/{nowhex}/{channelId}.html";
		}

		[HttpPost]
		public async Task<Stream> Post(ExportOptions options)
		{
			var channelId = options.ChannelId;

			var token = new AuthToken(AuthTokenType.Bot, options.Token);
			var client = new DiscordClient(token);
			Channel channel;
			try
			{
				channel = await client.GetChannelAsync(channelId);
			}
			catch (DiscordChatExporterException e)
			{
				var isUnauthorized = e.Message.Contains("Authentication");
				var content = isUnauthorized ? "Invalid Discord token provided." : "Please provide a valid channel";

				Response.ContentType = "application/json";
				Response.StatusCode = isUnauthorized ? 401 : 409;
				return GenerateStreamFromString("{ \"error\": \"" + content + "\" }");
			}

			var guild = await client.GetGuildAsync(channel.GuildId);

			using var req = new HttpRequestMessage(HttpMethod.Get, new Uri("https://discord.com/api/users/@me"));
			req.Headers.Authorization = token.GetAuthorizationHeader();
			var res = await _httpclient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
			var text = await res.Content.ReadAsStringAsync();
			var me = JsonConvert.DeserializeObject<User>(text);

			_logger.LogInformation($"[{me.FullName} ({me.Id})] Exporting #{channel.Name} ({channel.Id}) within {guild.Name} ({guild.Id})");
			var path = GetPath(channel.Id);

			var request = new ExportRequest(
				guild,
				channel,
				path,
				ExportFormat.HtmlDark,
				null,
				null,
				null,
				false,
				"dd-MMM-yy hh:mm tt"
			);

			var exporter = new ChannelExporter(client);

			await exporter.ExportChannelAsync(request);

			var stream = new FileStream(path, FileMode.Open);

			Response.ContentType = "text/html; charset=UTF-8";
			Response.StatusCode = 200;

			deleteFile(path);

			return stream;
		}

		async internal void deleteFile(string path) {
			await Task.Delay(TimeSpan.FromSeconds(30));
			System.IO.File.Delete(path);
			_logger.LogInformation($"Deleted {path}");
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
		public string Token { get; set; }
		public string ChannelId { get; set; }
	}
}
