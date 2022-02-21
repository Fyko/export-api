using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ExportAPI.Util;
using DiscordChatExporter.Core.Discord;
using DiscordChatExporter.Core.Exceptions;
using DiscordChatExporter.Core.Exporting;
using DiscordChatExporter.Core.Exporting.Partitioning;
using DiscordChatExporter.Core.Exporting.Filtering;
using DiscordChatExporter.Core.Discord.Data;

namespace ExportAPI.Controllers
{
	[ApiController]
	[Route("v3/export")]
	public class WebSocketController : ControllerBase
	{
		private readonly ILogger<WebSocketController> _logger;

		public WebSocketController(ILogger<WebSocketController> logger)
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

		[HttpGet()]
		public async Task Get()
		{
			if (HttpContext.WebSockets.IsWebSocketRequest)
			{
				using WebSocket webSocket = await
								   HttpContext.WebSockets.AcceptWebSocketAsync();
				await Export(HttpContext, webSocket);
			}
			else
			{
				HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			}
		}

		private async Task Export(HttpContext httpContext, WebSocket webSocket)
		{
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			var str = System.Text.Encoding.Default.GetString(buffer);
			var ser = JsonConvert.DeserializeObject<ExportOptions>(str);
			_logger.LogInformation($"[{ser.channel_id}]");

			while (!result.CloseStatus.HasValue)
			{
				await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}
	}
}