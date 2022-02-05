# 📥 Discord Channel Exporter API
[![License](https://img.shields.io/github/license/fyko/export-api)](https://github.com/fyko/export-api/blob/master/LICENSE.md)
[![Test](https://github.com/Fyko/export-api/workflows/Test/badge.svg)](https://github.com/Fyko/export-api/actions?query=workflow%3ATest)
[![Docker Pulls](https://img.shields.io/badge/-Docker%20Image-grey?logo=docker)](https://github.com/Fyko/export-api/pkgs/container/export-api)
[![Ko-fi Donate](https://img.shields.io/badge/kofi-donate-brightgreen.svg?label=Donate%20with%20Ko-fi&logo=ko-fi&colorB=F16061&link=https://ko-fi.com/carterh&logoColor=FFFFFF)](https://ko-fi.com/carterh)  
An API to create HTML exports of Discord text-channels

## Preface
This API utilized [`Tyrrrz/DiscordChatExporter`](https://github.com/Tyrrrz/DiscordChatExporter) for exporting channels.

## Usage
### Docker Image
The Export API can be found on the [Docker Hub](https://hub.docker.com/) at [`fyko/export-api`](https://hub.docker.com/r/fyko/export-api).

#### Using the Docker Image
```sh
docker run -p yourport:80 --rm -it ghcr.io/fyko/export-api
```
or within Compose
```yaml
services:
  exportapi:
    image: ghcr.io/fyko/export-api
    ports:
      - "yourport:80"
    expose:
      - "yourport"
```

## Performing Requests: Version 2

__Export Formats__
| **Type**  | **ID** | **Description**                         | **File Extension** |
|-----------|--------|-----------------------------------------|--------------------|
| PlainText | 0      | Export to a plaintext file              | txt                |
| HtmlDark  | 1      | Export to an HTML file in dark mode     | html               |
| HtmlLight | 2      | Export to an HTML file in light mode    | html               |
| CSV       | 3      | Export to a comma separated values file | csv                |
| JSON      | 4      | Export to a JSON file                   | json               |

### `POST` `/v2/export`
Exports a channel. On success, it returns a file stream.

__JSON Body__  
| **Field**     | **Type**      | **Description**                                                                                                                                                                  |
|---------------|---------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| token         | string        | The bot token for performing requests                                                                                                                                            |
| channel_id    | string        | The id of the channel to export                                                                                                                                                  |
| export_format | ?ExportFormat | The format to export the channel as, defaults to `HtmlDark`                                                                                                                      |
| date_format   | ?string        | The [date format](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) for dates in exported files, defaults to `dd-MMM-yy hh:mm tt` |
| after         | ?string       | Only include messages sent after this date                                                                                                                                       |
| before        | ?string       | Only include messages sent before this date                                                                                                                                      |

### Examples
#### Typescript:
```ts
import fetch from 'node-fetch';

async function exportChannel(channel_id: string, token: string): Promise<Buffer> {
	const response = await fetch('http://localhost:8008/v2/export', {
		method: 'POST',
		body: JSON.stringify({ channel_id, token }),
		headers: {
			'Content-Type': 'application/json'
		}
	});
	if (response.ok) {
		return response.buffer();
	}
	throw Error('Channel export failed!');
}
```
#### Rust
```rust
// reqwest = { version = "0.10", features = ["json"] }
use reqwest::Client;
use std::collections::HashMap;
use std::io::copy;
use std::fs::File;

async fn export_channel(channelId: &str, token: &str) -> Result<File, reqwest::Error> {
	let client = Client::new();
	let mut map = HashMap::new();
	map.insert("channel_id", "channel id");
	map.insert("token", "discord token");

	let file = client.post("http://localhost:8008/v2/export").json(&map).await?.text().await?;

	let dest = File::create("myexport.html")?;
	copy(&mut file.as_bytes(), &mut dest)?;

	Ok(dest)
}
```


## Performing Requets: Version 1

### `POST` `/v1/export`
__JSON Body__
| **Field** | **Type** | **Description**                       |
|-----------|----------|---------------------------------------|
| token     | string   | The bot token for performing requests |
| channelId | string   | The id of the channel to export       |

__Response Codes__
| **Status** | **Description**                              |
|------------|----------------------------------------------|
| 200        | Success - exported channel sent as text/html |
| 401        | Unauthorized - bad Discord bot token         |
| 409        | Conflict - unknown channel                   |

### Examples
#### Typescript:
```ts
import fetch from 'node-fetch';

async function exportChannel(channelId: string, token: string): Promise<Buffer> {
	const response = await fetch('http://localhost:8008/v1/export', {
		method: 'POST',
		body: JSON.stringify({ channelId, token }),
		headers: {
			'Content-Type': 'application/json'
		}
	});
	if (response.ok) {
		return response.buffer();
	}
	throw Error('Channel export failed!');
}
```
#### Rust
```rust
// reqwest = { version = "0.10", features = ["json"] }
use reqwest::Client;
use std::collections::HashMap;
use std::io::copy;
use std::fs::File;

async fn export_channel(channelId: &str, token: &str) -> Result<File, reqwest::Error> {
	let client = Client::new();
	let mut map = HashMap::new();
	map.insert("channelId", "channel id");
	map.insert("token", "discord token");

	let file = client.post("http://localhost:8008/v1/export").json(&map).await?.text().await?;

	let dest = File::create("myexport.html")?;
	copy(&mut file.as_bytes(), &mut dest)?;

	Ok(dest)
}
```

