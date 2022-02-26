import { credentials } from '@grpc/grpc-js';
import { ExporterClient } from '../dist';
import { CreateExportRequest, CreateExportResponse, ExportFormat } from '../dist/export_pb';
import { writeFile } from 'fs/promises';

const client = new ExporterClient(
    `localhost:${process.env.PORT}`,
    credentials.createInsecure(),
);

void (async () => {
	const request = new CreateExportRequest();
	request.setChannelId(process.env.DISCORD_CHANNEL!);
	request.setToken(process.env.DISCORD_TOKEN!);
	request.setExportFormat(ExportFormat.HTMLDARK);

	return new Promise(async (res, rej) => {
		const stream = client.createExport(request);


		const chunks: (string | Uint8Array)[] = [];
		stream.on('data', (response: CreateExportResponse) => {
			const progress = response.getProgress()
			if (progress) console.log(progress);
			
			const data = response.getData();
			const inner = data?.getData();
			if (inner) {
				console.log(`Inner exists!`); 
				chunks.push(inner);
			}
		});

		stream.on('end', async () => {
			await writeFile('./foo.html', chunks);
			return res(void 0);
		});

        stream.on('error', rej);
	});
})();
