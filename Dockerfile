FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY Huby/Huby.csproj /app/Huby/Huby.csproj
COPY Huby.Data/Huby.Data.csproj /app/Huby.Data/Huby.Data.csproj

RUN dotnet restore Huby
COPY . ./

WORKDIR /app/Huby
RUN dotnet publish -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY --from=build-env /app/Huby/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "Huby.dll", "--urls", "http://*:80"]
