FROM mcr.microsoft.com/dotnet/core/runtime:3.1-nanoserver-1903 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
COPY ["DockerWindowsSignalRepro.csproj", "."]
RUN dotnet restore "DockerWindowsSignalRepro.csproj"
COPY . .
RUN dotnet build "DockerWindowsSignalRepro.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerWindowsSignalRepro.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerWindowsSignalRepro.dll"]