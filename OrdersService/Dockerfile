FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Common ./Common
COPY OrdersService ./OrdersService/

RUN dotnet restore OrdersService/OrdersService.csproj

WORKDIR /app/OrdersService
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/OrdersService/out .

ENV DB_CONNECTION="Host=db;Database=mydb;Username=postgres;Password=secret"

ENTRYPOINT ["dotnet", "OrdersService.dll"]
