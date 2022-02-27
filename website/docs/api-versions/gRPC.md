---
id: gRPC
title: gRPC (reccomended)
sidebar_position: 0
---

v3 of the Export API introduces a gRPC API. One of the major upsides of using the gRPC protocol is a stream of progress updates and finally streaming the file back to the client.

The `@fyko/export-api` package includes a typed JavaScript client and will be referenced in this part of the documentation.

## Protobuf Definition

:::tip Server Reflection
The gRPC service has [server reflection](https://github.com/grpc/grpc/blob/master/doc/server-reflection.md), allowing you to test the API in Postman.
:::

```protobuf
service Exporter {
  rpc CreateExport (CreateExportRequest) returns (stream CreateExportResponse);
}

enum ExportFormat {
	PlainText = 0;
	HtmlDark = 1;
	HtmlLight = 2;
	CSV = 3;
	JSON = 4;
}

message CreateExportRequest {
  string token = 1;
  string channel_id = 2;
  ExportFormat export_format = 3;
  string date_format = 4;
  string after = 5;
  string before = 6;
}

message CreateExportResponse {
  oneof ResponseType {
    double progress = 1;
    ExportComplete data = 2;
  }
}

message ExportComplete {
  int32 message_count = 1;
  bytes data = 2;
}
```

## `Exporter/CreateExport`

> `rpc CreateExport (CreateExportRequest) returns (stream CreateExportResponse);`

### Export Formats Enum

| **Type**  | **ID** | **Description**                         | **File Extension** |
| --------- | ------ | --------------------------------------- | ------------------ |
| PlainText | 0      | Export to a plaintext file              | txt                |
| HtmlDark  | 1      | Export to an HTML file in dark mode     | html               |
| HtmlLight | 2      | Export to an HTML file in light mode    | html               |
| CSV       | 3      | Export to a comma separated values file | csv                |
| JSON      | 4      | Export to a JSON file                   | json               |

### CreateExportRequest

| **Field**     | **Type**                              | **Description**                                                                                                                                                                  |
| ------------- | ------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| token         | string                                | The bot token for performing requests                                                                                                                                            |
| channel_id    | string                                | The id of the channel to export                                                                                                                                                  |
| export_format | ?[ExportFormat](#export-formats-enum) | The format to export the channel as, defaults to `PlainText`                                                                                                                     |
| date_format   | ?string                               | The [date format](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) for dates in exported files, defaults to `dd-MMM-yy hh:mm tt` |
| after         | ?string                               | Only include messages sent after this date                                                                                                                                       |
| before        | ?string                               | Only include messages sent before this date                                                                                                                                      |

### CreateExportResponse

| **Field** | **Type**                           | **Description**                                   |
| --------- | ---------------------------------- | ------------------------------------------------- |
| progress  | int64                              | A decimal representing the progress of the export |
| data      | ?[ExportComplete](#exportcomplete) | The file data once `progress` equals `1`          |

### ExportComplete

| **Field**     | **Type** | **Description**                  |
| ------------- | -------- | -------------------------------- |
| message_count | int      | The number of messages exported  |
| data          | byte[]   | The exported file in 32kb chunks |

### Example

```ts
import { credentials } from "@grpc/grpc-js";
import { ExporterClient } from "@fyko/export-api/client";
import {
  CreateExportRequest,
  CreateExportResponse,
  ExportFormat,
} from "@fyko/export-api/types";
import { writeFile } from "fs/promises";

// creates a new gRPC client
const client = new ExporterClient(
  `localhost:${process.env.PORT}`,
  credentials.createInsecure()
);

void (async () => {
  // new CreateExport Request
  const request = new CreateExportRequest();
  // set required options
  request.setChannelId(process.env.DISCORD_CHANNEL!);
  request.setToken(process.env.DISCORD_TOKEN!);
  // set optional options
  request.setExportFormat(ExportFormat.HTMLDARK);

  //
  return new Promise(async (res, rej) => {
    // "POST" the request
    const stream = client.createExport(request);

    const chunks: (string | Uint8Array)[] = [];
    let progress = 0;
    stream.on("data", (response: CreateExportResponse) => {
      // if `response.progress` is present
      const p = response.getProgress();
      if (p && p > progress) {
        progress = p;
        console.log((p * 100).toFixed() + "%");
      }

      // if finally sending the file itself, push to chunk array
      const data = response.getData();
      const inner = data?.getData();
      if (inner) {
        console.log(`Inner exists!`);
        chunks.push(inner);
      }
    });

    // once the server closes the stream,
    // we can finally write the file
    stream.on("end", async () => {
      await writeFile("./foo.html", chunks);
      return res(void 0);
    });

    stream.on("error", rej);
  });
})();
```
