using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ExportAPI.Proto;
using DiscordChatExporter.Core.Discord;
using DiscordChatExporter.Core.Exceptions;
using DiscordChatExporter.Core.Exporting;
using DiscordChatExporter.Core.Exporting.Partitioning;
using DiscordChatExporter.Core.Exporting.Filtering;
using DiscordChatExporter.Core.Discord.Data;
using Gress;

namespace ExportAPI.Services;

public class ExporterService : Exporter.ExporterBase
{
	private readonly ILogger _logger;

	public ExporterService(ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<ExporterService>();

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

	internal static string GetPath(string channelId, DiscordChatExporter.Core.Exporting.ExportFormat exportType)
	{
		var nowhex = DateTime.Now.Ticks.ToString("X2");
		return $"/tmp/exports/{nowhex}/{channelId}.{exportType.GetFileExtension()}";
	}

	public override async Task CreateExport(CreateExportRequest options, IServerStreamWriter<CreateExportResponse> responseStream, ServerCallContext context)
	{

		var ef = options.ExportFormat;
		var exportFormat = (DiscordChatExporter.Core.Exporting.ExportFormat)ef;

		var parsed = Snowflake.TryParse(options.ChannelId);
		var channelId = parsed ?? Snowflake.Zero;

		var client = new DiscordClient(options.Token);
		client._tokenKind = TokenKind.Bot;
		Channel channel;
		try
		{
			channel = await client.GetChannelAsync(channelId);
		}
		catch (DiscordChatExporterException e)
		{
			if (e.Message.Contains("Authentication"))
			{
				throw new RpcException(new Status(StatusCode.PermissionDenied, "An invalid Discord token was provided."));
			}
			if (e.Message.Contains("Requested resource does not exist"))
			{
				throw new RpcException(new Status(StatusCode.NotFound, "A channel with the provided ID was not found."));
			}
			throw new RpcException(new Status(StatusCode.Unknown, $"An unknown error occurred: {e.Message}"));
		}

		var guild = await client.GetGuildAsync(channel.GuildId);
		var res = await client.GetJsonResponseAsync("users/@me");
		var me = DiscordChatExporter.Core.Discord.Data.User.Parse(res);

		var path = GetPath(channel.Id.ToString(), exportFormat);
		_logger.LogInformation($"[{me.FullName} ({me.Id})] Exporting #{channel.Name} ({channel.Id}) within {guild.Name} ({guild.Id}) to {path}");
		var request = new ExportRequest(
			guild,
			channel,
			path,
			exportFormat,
			Snowflake.TryParse(options.After),
			Snowflake.TryParse(options.Before),
			PartitionLimit.Null,
			MessageFilter.Null,
			false,
			false,
			options.DateFormat
		);

		var progress = new Progress<double>(p => responseStream.WriteAsync(new CreateExportResponse { Progress = p }));

		var exporter = new ChannelExporter(client);

		_logger.LogInformation("Starting export");
		var messageCount = await exporter.ExportChannelAsync(request, progress);
		_logger.LogInformation("Finished exporting");
		var stream = new FileStream(path, FileMode.Open);

		var ext = exportFormat.GetFileExtension();
		if (ext == "txt") ext = "plain";
		// Response.ContentType = $"text/{ext}; charset=UTF-8";
		// Response.Headers.Add("X-Message-Count", messageCount.ToString());
		// Response.StatusCode = 200;

		// deleteFile(path);

		// await responseStream.WriteAsync(new CreateExportResponse{ Data = ByteString.FromStream(stream) });
		await responseStream.WriteAsync(new CreateExportResponse { Data = new ExportComplete { MessageCount = messageCount, Path = path } });
	}
}
