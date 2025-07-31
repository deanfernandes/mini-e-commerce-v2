FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore ./ProductService.Api/ProductService.Api.csproj
RUN dotnet publish ./ProductService.Api/ProductService.Api.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ProductService.Api.dll"]