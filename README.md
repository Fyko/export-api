# 📥 Discord Channel Exporter API
An API to create HTML exports of Discord text-channels

## Preface
This API utilized [`Tyrrrz/DiscordChatExporter`](https://github.com/Tyrrrz/DiscordChatExporter) for exporting channels.

## Usage
### Docker Image
The Export API can be found on the [Docker Hub](https://hub.docker.com/) at [`fyko/export-api`](https://hub.docker.com/r/fyko/export-api).  

#### Using the Docker Image
```sh
docker run -p yourport:80 --rm -it fyko/export-api
```
or within Compose
```yaml
services:
  exportapi:
    image: fyko/export-api
    ports:
      - "yourport:80"
    expose:
      - "yourport"
```

### Performing Requests
`POST` to `/v1/export` with the JSON body:
```json
{
	"token": "discord bot token",
	"channelId": "guild channel id"
}
```
#### Response Codes
**Status**|**Description**
-----:|-----
200|Success - exported channel sent as text/html
401|Unauthorized - bad Discord bot token
409|Conflict - unknown channel

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
```rs
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
