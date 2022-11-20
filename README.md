# 📥 Discord Channel Exporter API
> A gRPC service to create HTML exports of Discord text-channels.  

[![Docs](https://img.shields.io/badge/%F0%9F%93%96-Documentation-informational)](https://fyko.github.io/export-api/docs/intro)
[![License](https://img.shields.io/github/license/fyko/export-api)](https://github.com/fyko/export-api/blob/master/LICENSE.md)
[![Test](https://github.com/Fyko/export-api/workflows/Test/badge.svg)](https://github.com/Fyko/export-api/actions?query=workflow%3ATest)
[![Docker Pulls](https://img.shields.io/badge/-Docker%20Image-grey?logo=docker)](https://github.com/Fyko/export-api/pkgs/container/export-api)
[![Ko-fi Donate](https://img.shields.io/badge/kofi-donate-brightgreen.svg?label=Donate%20with%20Ko-fi&logo=ko-fi&colorB=F16061&link=https://ko-fi.com/carterh&logoColor=FFFFFF)](https://ko-fi.com/carterh)  

## Usage
### Docker Image
The Export API can be found on the [GitHub Container Registry](https://pkg.github.com) at [`fyko/export-api`](https://github.com/Fyko/export-api/pkgs/container/export-api).  

```sh
docker run -p yourport:80 --rm -it ghcr.io/fyko/export-api
```
or with Compose
```yaml
services:
  exportapi:
    image: ghcr.io/fyko/export-api
    ports:
      - "yourport:80"
    expose:
      - "yourport"
```  

### Calling the gRPC API
- [Documentation](https://fyko.github.io/export-api/docs/api-versions/gRPC)

## Thanks
This services utilizes [`Tyrrrz/DiscordChatExporter`](https://github.com/Tyrrrz/DiscordChatExporter) for exporting channels.
