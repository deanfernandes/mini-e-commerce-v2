FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG GITHUB_USERNAME
ARG GITHUB_PAT

WORKDIR /src

RUN dotnet nuget add source "https://nuget.pkg.github.com/${GITHUB_USERNAME}/index.json" \
    --name github \
    --username "${GITHUB_USERNAME}" \
    --password "${GITHUB_PAT}" \
    --store-password-in-clear-text

COPY . .

RUN dotnet restore ./AuthService.Api.csproj
RUN dotnet publish ./AuthService.Api.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AuthService.Api.dll"]