FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY FrontendApi.csproj .
RUN dotnet restore FrontendApi.csproj
COPY . .
WORKDIR /src/
RUN dotnet build FrontendApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FrontendApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FrontendApi.dll"]