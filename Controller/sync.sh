#!/bin/bash
if ! dotnet publish --configuration pi-release --runtime linux-arm; then
	exit 1
fi
#rsync -Pr bin/pi-release/netcoreapp2.1/linux-arm/publish/ pi:e-sharp-minor/
rsync -a -H -x -v -r --delete --progress -e "ssh -T -o Compression=no -x" bin/pi-release/netcoreapp2.1/linux-arm/publish/ pi:e-sharp-minor/