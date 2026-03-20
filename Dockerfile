FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CollegeLms.sln .
COPY src/CollegeLms.Domain/CollegeLms.Domain.csproj src/CollegeLms.Domain/
COPY src/CollegeLms.Infrastructure/CollegeLms.Infrastructure.csproj src/CollegeLms.Infrastructure/
COPY src/CollegeLms.Api/CollegeLms.Api.csproj src/CollegeLms.Api/
COPY tests/CollegeLms.UnitTests/CollegeLms.UnitTests.csproj tests/CollegeLms.UnitTests/
COPY tests/CollegeLms.IntegrationTests/CollegeLms.IntegrationTests.csproj tests/CollegeLms.IntegrationTests/
RUN dotnet restore

COPY src/ src/
WORKDIR /src/src/CollegeLms.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/uploads
VOLUME /app/uploads

EXPOSE 8080
ENTRYPOINT ["dotnet", "CollegeLms.Api.dll"]
