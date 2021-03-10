#!/bin/sh -l

cd /app/src

dotnet restore
dotnet build
dotnet run
