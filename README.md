# Huby Server

Back-end code to the open source social app.

## Dependencies

- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
- [PostgreSQL](https://www.postgresql.org/)

## Setup

```bash
# Clone repository
git clone https://github.com/happotato/huby-server.git

# Change to directory
cd huby-server/Huby

# Restore 
dotnet restore 
```
### Environment

You may need to create the `appsettings.json` or `appsettings.Development.json` file at [Huby](Huby).

### Database

```bash
# Install tools
dotnet tool install --global dotnet-ef 

# Update database
dotnet ef database update
```

### Running

```bash
# Build
dotnet build

# Start the application
dotnet run
```
## License

[GPLv3](LICENSE.txt)
