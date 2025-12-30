FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /source

COPY *.sln .

COPY EvacuationPlanning/*.csproj ./EvacuationPlanning/

RUN dotnet restore

COPY EvacuationPlanning/. ./EvacuationPlanning/

WORKDIR /source/EvacuationPlanning

RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app ./

ENTRYPOINT ["dotnet", "EvacuationPlanning.dll"]