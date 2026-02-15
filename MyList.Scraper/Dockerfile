FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MyList.Shared/MyList.Shared.csproj MyList.Shared/
COPY MyList.Scraper/MyList.Scraper.csproj MyList.Scraper/
RUN dotnet restore MyList.Scraper/MyList.Scraper.csproj

COPY MyList.Shared/ MyList.Shared/
COPY MyList.Scraper/ MyList.Scraper/
RUN dotnet publish MyList.Scraper/MyList.Scraper.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyList.Scraper.dll"]
