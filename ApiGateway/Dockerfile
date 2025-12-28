FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Common ./Common
COPY ApiGateway ./ApiGateway/

RUN dotnet restore ApiGateway/ApiGateway.csproj

WORKDIR /app/ApiGateway
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/ApiGateway/out .

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
