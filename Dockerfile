FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY Huby/Huby.csproj /app/Huby/Huby.csproj
COPY Huby.Data/Huby.Data.csproj /app/Huby.Data/Huby.Data.csproj

RUN dotnet restore Huby
COPY . ./

WORKDIR /app/Huby
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=build-env /app/Huby/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "Huby.dll", "--urls", "http://*:80"]
