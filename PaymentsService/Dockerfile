FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Common ./Common
COPY PaymentsService ./PaymentsService

RUN dotnet restore PaymentsService/PaymentsService.csproj

WORKDIR /app/PaymentsService
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/PaymentsService/out .

ENV DB_CONNECTION="Host=db;Database=mydb;Username=postgres;Password=secret"

ENTRYPOINT ["dotnet", "PaymentsService.dll"]
