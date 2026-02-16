FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MyList.Shared/MyList.Shared.csproj MyList.Shared/
COPY MyList.Notifier/MyList.Notifier.csproj MyList.Notifier/
RUN dotnet restore MyList.Notifier/MyList.Notifier.csproj

COPY MyList.Shared/ MyList.Shared/
COPY MyList.Notifier/ MyList.Notifier/
RUN dotnet publish MyList.Notifier/MyList.Notifier.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyList.Notifier.dll"]
