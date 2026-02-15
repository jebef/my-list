FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MyList.Shared/MyList.Shared.csproj MyList.Shared/
COPY MyList.Api/MyList.Api.csproj MyList.Api/
RUN dotnet restore MyList.Api/MyList.Api.csproj

COPY MyList.Shared/ MyList.Shared/
COPY MyList.Api/ MyList.Api/
RUN dotnet publish MyList.Api/MyList.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyList.Api.dll"]
