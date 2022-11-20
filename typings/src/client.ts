export * from '../generated/export_grpc_pb';

import { CallOptions, ClientOptions, credentials } from "@grpc/grpc-js";
import { EventEmitter } from 'events';
import type TypedEmitter from "typed-emitter";
import { ExporterClient } from '../generated/export_grpc_pb';
import { CreateExportRequest, CreateExportResponse, ExportFormat } from './types';

export function createExporterClient(
	address: string,
	creds = credentials.createInsecure(),
	options?: ClientOptions
): ExporterClient {
	return new ExporterClient(address, creds, options);
}

export type ExportEvents = {
	error: (err: Error) => void;
	progress: (progress: number) => void;
	chunk: (chunk: string | Uint8Array) => void;
	done: (messageCount: number, file: Buffer) => void;
}

/**
 * Create an EventEmitter 
 * @param client - The client to use to make the request
 * @param data - The data to provide to the request
 * @param options - Other gRPC options
 * @returns An EventEmitter
 * @see [Examples: High Level](https://fyko.github.io/export-api/docs/api-versions/gRPC#high-level)
 */
export function createExport(
	client: ExporterClient,
	data: CreateExportData,
	options?: Partial<CallOptions>
) {
	const request = createExportRequest(data);
	const stream = client.createExport(request, options);

	const chunks: Uint8Array[] = [];

	let progress = 0;
	let messageCount: number | undefined = 0;
	const emitter = new EventEmitter() as TypedEmitter<ExportEvents>;

	stream.on('data', (response: CreateExportResponse) => {
		// if `response.progress` is present
		const p = response.getProgress();
		if (p && p > progress) {
			progress = p;
			emitter.emit('progress', progress);
		}

		// if finally sending the file itself, push to chunk array
		const data = response.getData();
		const count = data?.getMessageCount();
		if (count) {
			messageCount = count;
		}

		const inner = data?.getData();
		const isUint8Array = (x: unknown): x is Uint8Array => x instanceof Uint8Array;
		if (isUint8Array(inner)) {
			chunks.push(inner)
		}
	});

	stream.on("end", async () => {
		stream.destroy();
		return emitter.emit("done", messageCount ?? 0, Buffer.concat(chunks));
	});

	stream.on("error", (err) => {
		return emitter.emit('error', err);
	});

	return emitter;
}

/**
 * Turns a stream into a promise that resolves with the message count and file buffer.
 * @param emitter - The emitter returned by `createExport`
 * @returns - A tuple of the message count and file buffer (in that order)
 */
export function promisifyExportResult(emitter: TypedEmitter<ExportEvents>) {
	return new Promise<[number, Buffer]>((resolve, reject) => {
		emitter.on("done", (count, file) => resolve([count, file]));
		emitter.on("error", (err) => reject(err));
	});
}

/**
 * The data to provide to the export request
 */
export interface CreateExportData {
	/**
	 * The id of the channel to export
	 */
	channelId: string;
	/**
	 * The bot token for performing requests
	 */
	token: string;
	/**
	 * The format to export the channel as, defaults to PlainText
	 */
	exportFormat?: ExportFormat;
	/**
	 * The [date format](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) for dates in exported files, defaults to dd-MMM-yy hh:mm tt
	 */
	dateFormat?: string;
	/**
	 * Only include messages sent after this date
	 */
	after?: string;
	/**
	 * Only include messages sent before this date
	 */
	before?: string;
}

/**
 * 
 * @param data - The data to provide to the request
 * @returns A fresh `CreateExportRequest`
 */
function createExportRequest(data: CreateExportData) {
	const request = new CreateExportRequest();

	request.setChannelId(data.channelId);
	request.setToken(data.token);

	if (data.exportFormat)
		request.setExportFormat(data.exportFormat);
	if (data.dateFormat)
		request.setDateFormat(data.dateFormat);
	if (data.after)
		request.setAfter(data.after);
	if (data.before)
		request.setBefore(data.before);

	return request;
}
