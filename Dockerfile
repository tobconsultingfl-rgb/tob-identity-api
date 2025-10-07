FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

RUN dotnet restore
RUN dotnet build -c Release -o /build
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80

ENTRYPOINT ["dotnet", "TOB.Identity.API.dll"]
