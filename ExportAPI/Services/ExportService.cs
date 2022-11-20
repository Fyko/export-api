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
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace ExportAPI.Services;

public class ExporterService : Exporter.ExporterBase
{
	private readonly ILogger _logger;

	private const int ChunkSize = 1024 * 32; // 32 KB

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

	internal void deleteFile(string path)
	{
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

	public override async Task CreateExport(CreateExportRequest options, IServerStreamWriter<CreateExportResponse> responseStream, ServerCallContext context)
	{
		var ef = options.ExportFormat;
		var exportFormat = (DiscordChatExporter.Core.Exporting.ExportFormat)ef;

		var parsed = Snowflake.TryParse(options.ChannelId);
		var channelId = parsed ?? Snowflake.Zero;

		var client = new DiscordClient(options.Token);
		client._resolvedTokenKind = TokenKind.Bot;
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

		var exporter = new ChannelExporter(client);

		_logger.LogInformation("Starting export");
		var progress = new Progress<Percentage>(p => responseStream.WriteAsync(new CreateExportResponse { Progress = p.Value }));
		var messageCount = await exporter.ExportChannelAsync(request, progress);
		_logger.LogInformation("Finished exporting");


		var buffer = new byte[ChunkSize];
		await using var readStream = File.OpenRead(path);
		while (true)
		{
			var count = await readStream.ReadAsync(buffer);

			if (count == 0)
			{
				break;
			}

			Console.WriteLine("Sending file data chunk of length " + count);
			await responseStream.WriteAsync(new CreateExportResponse
			{
				Data = new ExportComplete {
					MessageCount = messageCount,
					Data = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, count))
				}
			});
		}

		deleteFile(path);
	}
}
