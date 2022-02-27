---
id: setup
title: Setup
sidebar_position: 1
---
# Setup

# Docker Compose
:::tip
This is the reccomended way of running Export API.
:::

To start using `fyko/export-api`, add the service to your `docker-compose.yml` file!
```yml
services:
  exportapi:
    image: ghcr.io/fyko/export-api
    expose:
      - "80"
```

You can then interact with the API with `http://exportapi/...`.

# `docker run`
`docker run -p yourport:80 --rm -it ghcr.io/fyko/export-api`

You can then interact with the API with `http://127.0.0.1:yourport/...`.