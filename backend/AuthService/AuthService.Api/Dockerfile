FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend/Kafka.Contracts/Kafka.Contracts.csproj backend/Kafka.Contracts/
COPY backend/AuthService/AuthService.Api/AuthService.Api.csproj backend/AuthService/AuthService.Api/
RUN dotnet restore backend/AuthService/AuthService.Api/AuthService.Api.csproj

COPY . ./

RUN dotnet publish backend/AuthService/AuthService.Api/AuthService.Api.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AuthService.Api.dll"]