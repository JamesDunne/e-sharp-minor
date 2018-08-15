#!/bin/bash
dotnet publish --configuration Debug --runtime linux-arm
rsync -Pr bin/Debug/netcoreapp2.1/linux-arm/publish/ pi:e-sharp-minor/
