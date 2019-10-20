# TrickingRoyal [![Build Status](https://antonwieslander.visualstudio.com/TrickingRoyal/_apis/build/status/T0shik.Tricking-Royal?branchName=master)](https://antonwieslander.visualstudio.com/TrickingRoyal/_build/latest?definitionId=5&branchName=master)

Live version of the platform: https://www.trickingroyal.com/

Social Network for online Tricking battles. The aim of this project is to provide a platform for Trickers to connect with each other share content and push each other out of the comfort zone in a playful rewarding way.

##### What is Tricking?

[Hooked 2018](https://www.youtube.com/watch?v=rZBIjYxFKHA), [I Can Be A Hero](https://www.youtube.com/watch?v=bafipTSFBMc), [BiG](https://www.seansevestre.com/big), [Alex D](https://www.youtube.com/watch?v=rVdX6tAGXZ4), [Guthrie](https://www.youtube.com/watch?v=11Z_0VKnHyw)

## Getting Started

The following instructions will help you setup the project on your local 
development machine.

### Prerequisites

- [ASP.NET Core](https://dotnet.microsoft.com/download) - The main `c#` based server side platform. Global version set to `v2.2.300` in `global.json` file in the root of the project.
- [Nodejs](https://nodejs.org/en/) - You will need nodejs to use the `npm` `(node package manager)` and run the vue client.
- [MSSQL](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) - You will ether need to download and install the Microsoft SQL server or use the build in Visual Studio local database. 
- [FFMPEG](https://www.ffmpeg.org/download.html) - Used to trim and downscale videos once they are uploaded.   

### Installing 

All we are trying to achieve is getting the code on to your computer and making sure the project is pointing to your database and that the database is created.

1. After setting up the prerequisites, `git clone` or `fork` this repository.
2. Set the `DefaultConnection` connection string in the `appsettings.json` file, to connect to your database in the following projects: `IdentityServer` `Battles.Cdn` `Battles.Api`. Example (replace `<param>`): `"Data Source=<Server>;Database=<DatabaseName>;Integrated Security=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"`. If you are using the MSSQL server you might need to setup database access permissions.
3. Create the database using the `entity framework core` `Update Database` command located in `./TrickingRoyal/src/TrickingRoyal.Database/ef-migration-scripts.txt` file. Example: `dotnet ef database update -c AppDbContext -s ../IdentityServer`
4. Place the `FFMPEG` executables in the `Battles.Cdn` project in a `ffmpeg` folder (create the folder if not there). If you would like a different file path for the executables change the `VideoEditingExecutables` variable in `appsettings.json`.

You can verify that the connection is successful by checking if the tables are populated in the database.

## Executing Tests

Tests are located in the `/test` folder and can be run by executing `dotnet test` from the command line in the root of the solution (where the `.sln` file is located)

## Deployment

Deployment of this project is handled through `VSTS` and the build pipeline is configured in the `azure-pipelines.yml` file. The release pipeline is private and cannot be accessed.
