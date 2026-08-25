FROM mcr.microsoft.com/dotnet/sdk:8.0.408-bookworm-slim AS build
WORKDIR /src
COPY CSharp-Alerting-System.csproj ./
RUN dotnet restore CSharp-Alerting-System.csproj
COPY Program.cs ./
COPY Alerting ./Alerting
RUN dotnet publish CSharp-Alerting-System.csproj -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0.15-bookworm-slim
WORKDIR /app
COPY --from=build --chown=1654:1654 /app/publish ./
USER 1654
ENV ASPNETCORE_URLS=http://0.0.0.0:8080 \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sky.Alerting.dll"]
