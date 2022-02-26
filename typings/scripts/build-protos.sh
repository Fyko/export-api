#!/bin/bash

BASEDIR=$(dirname "$0")
cd ${BASEDIR}/../

PROTO_DEST=./generated

mkdir -p ${PROTO_DEST}

# JavaScript code generation
yarn run grpc_tools_node_protoc \
    --plugin=protoc-gen-ts=./node_modules/.bin/protoc-gen-ts \
    --ts_out=grpc_js:${PROTO_DEST} \
    --js_out=import_style=commonjs,binary:${PROTO_DEST} \
    --grpc_out=grpc_js:${PROTO_DEST} \
    -I ../ExportAPI/Protos \
    ../ExportAPI/Protos/*.proto

# cd dist

# #client
# mv export_grpc_pb.js client.js
# mv export_grpc_pb.d.ts client.d.ts

# # types
# mv export_pb.js types.js
# mv export_pb.d.ts types.d.ts