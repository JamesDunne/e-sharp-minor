#!/bin/bash
if ! dotnet publish --configuration Debug --runtime linux-arm; then
	exit 1
fi
rsync -Pr bin/Debug/netcoreapp2.1/linux-arm/publish/ pi:e-sharp-minor/
