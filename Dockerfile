FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /app
COPY *.sln .
COPY ["covidtracking/covidtracking.csproj", "covidtracking/"]
COPY ["covidtracking.UnitTests/covidtracking.UnitTests.csproj", "covidtracking.UnitTests/"]
RUN dotnet restore
COPY . .
RUN dotnet build

FROM build AS publish
WORKDIR /app/covidtracking
RUN dotnet publish -c Release -o out

FROM build AS test
WORKDIR /app/covidtracking.UnitTests/
RUN dotnet test --logger:trx

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS runtime
WORKDIR /app
COPY --from=publish /app/covidtracking/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "covidtracking.dll"]