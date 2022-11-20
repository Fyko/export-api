import { credentials } from "@grpc/grpc-js";
import { ExporterClient } from "../dist/client";
import {
  CreateExportRequest,
  CreateExportResponse,
  ExportFormat,
} from "../dist/types";
import { writeFile } from "fs/promises";

const client = new ExporterClient(
  `localhost:${process.env.PORT}`,
  credentials.createInsecure()
);

void (async () => {
  const request = new CreateExportRequest();
  request.setChannelId(process.env.DISCORD_CHANNEL!);
  request.setToken(process.env.DISCORD_TOKEN!);
  request.setExportFormat(ExportFormat.HTMLDARK);

  return new Promise(async (res, rej) => {
    const stream = client.createExport(request);

    const chunks: (string | Uint8Array)[] = [];
    let progress = 0;
    stream.on("data", (response: CreateExportResponse) => {
      const p = response.getProgress();
      if (p && p > progress) {
        progress = p;
        console.log((p * 100).toFixed() + "%");
      }

      const data = response.getData();
      const inner = data?.getData();
      if (inner) {
        console.log(`Inner exists!`);
        chunks.push(inner);
      }
    });

    stream.on("end", async () => {
      await writeFile("./foo.html", chunks);
      return res(void 0);
    });

    stream.on("error", rej);
  });
})();
