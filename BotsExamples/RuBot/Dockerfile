FROM microsoft/dotnet:2.1-sdk as build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app

FROM microsoft/dotnet:2.1-runtime as final
WORKDIR /app
COPY --from=build /app /app
ENTRYPOINT ["dotnet", "RuBot.dll"]