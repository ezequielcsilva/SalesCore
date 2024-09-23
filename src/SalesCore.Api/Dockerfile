FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY . .

WORKDIR "/src/"
RUN dotnet restore "src/SalesCore.Api/SalesCore.Api.csproj"
RUN dotnet publish "src/SalesCore.Api/SalesCore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "SalesCore.Api.dll"]