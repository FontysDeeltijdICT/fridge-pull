FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.sln ./
COPY Api/*.csproj ./Api/
COPY InfluxDb/*.csproj ./InfluxDb/
RUN dotnet restore ./Api/Api.csproj

COPY Api/. ./Api/
COPY InfluxDb/. ./InfluxDb/

RUN dotnet publish ./Api -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Api.dll"]
