# Use the official Microsoft .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ./Arc3 .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build the runtime image using the official Microsoft .NET runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "arc3.dll" ]
