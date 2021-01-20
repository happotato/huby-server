# Huby Server

Back-end code to the open source social app.

## Dependencies

- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
- [PostgreSQL](https://www.postgresql.org/)

## Environment

You may need to create the `appsettings.json` or `appsettings.Development.json` file at [Huby](Huby).

## Setup

```bash
# Clone repository
git clone https://github.com/happotato/huby-server.git

# Change to directory
cd huby-server/Huby

# Install dependencies
dotnet restore

# Build
dotnet build

# Start the application
dotnet run
```

## License

[GPLv3](LICENSE.txt)
