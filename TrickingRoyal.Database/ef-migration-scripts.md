# IS4 Persistant Grants Migrations
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -s ../IdentityServer -o Migrations/IdentityServer/PersistedGrantDb

# IS4 Configuration Migrations
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -s ../IdentityServer -o Migrations/IdentityServer/ConfigurationDb

# App Migrations
dotnet ef migrations add <name> -c AppDbContext -s ../IdentityServer -o Migrations/App

# Update Database
dotnet ef database update -c AppDbContext -s ../IdentityServer

# Generate SQL scripts for re-creating or updating the database in live system
dotnet ef migrations script -i -o "script.sql" -c AppDbContext -s ../IdentityServer

# For Live only
dotnet ef database update -c ConfigurationDbContext -s ../IdentityServer
dotnet ef database update -c PersistedGrantDbContext -s ../IdentityServer