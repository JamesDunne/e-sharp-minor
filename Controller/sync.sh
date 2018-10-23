#!/bin/bash
#cfg=pi-release
cfg=pi-debug
if ! dotnet publish --configuration $cfg --runtime linux-arm; then
	exit 1
fi
rsync -a -H -x -v -r --delete --progress -e "ssh -T -o Compression=no -x" bin/$cfg/netcoreapp2.1/linux-arm/publish/ pi:e-sharp-minor/