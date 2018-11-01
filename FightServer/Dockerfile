FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY FightServer/FightServer.csproj .
RUN dotnet restore FightServer.csproj
COPY ./FightServer/ .
COPY GameLogic /GameLogic
RUN dotnet build FightServer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FightServer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY ./FightServer/default_map.json /maps/2/objects.json
ENTRYPOINT ["dotnet", "FightServer.dll"]