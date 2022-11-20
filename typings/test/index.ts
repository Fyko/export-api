import { writeFile } from "fs/promises";
import { createExport, createExporterClient, promisifyExportResult } from "../src/client";
import {
  ExportFormat
} from "../src/types";

const client = createExporterClient(`localhost:${process.env.PORT}`);

void (async () => {
  const stream = createExport(client, {
    channelId: process.env.DISCORD_CHANNEL!,
    token: process.env.DISCORD_TOKEN!,
    exportFormat: ExportFormat.HTMLDARK,
  });

  stream.on("progress", (progress) => 
    console.log(`progress: ${progress}`));

  const [count, file] = await promisifyExportResult(stream);

  console.log(`export created with ${count} messages (${file.byteLength} bytes)`);
  await writeFile("./foo.html", file);
})();
