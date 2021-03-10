#!/bin/sh -l

cd /app/
dotnet run src --project src/MarkdownLinksVerifier.csproj --configuration Release
