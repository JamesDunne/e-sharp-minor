#!/bin/bash
if ! dotnet publish --configuration pi-release --runtime linux-arm; then
	exit 1
fi
rsync -Pr bin/pi-release/netcoreapp2.1/linux-arm/publish/ pi:e-sharp-minor/
