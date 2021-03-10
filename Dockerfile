FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

COPY . ./

RUN dotnet publish ./src/MarkdownLinksVerifier.csproj -c Release -o out --no-self-contained

# Build the runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY --from=build /out .

ENTRYPOINT [ "dotnet", "/MarkdownLinksVerifier.dll" ]
