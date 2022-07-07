using Microsoft.Data.Sqlite;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Tasker.Core.Entities;
using Tasker.Core.Enums;
using Tasker.Core.Interfaces.Repositories;

namespace Tasker.Infrastructure.Data;

public class DatabaseConfigurator
{
    private readonly SqliteConnection _connection;

    public DatabaseConfigurator(SqliteConnection connection)
    {
        _connection = connection;

        if (_connection.State is not ConnectionState.Open)
            _connection.Open();
    }
    
    ~DatabaseConfigurator()
    {
        if (_connection is not null && _connection.State is not ConnectionState.Closed)
            _connection.Close();
    }

    public async Task CreateDatabaseAsync()
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText =
                "CREATE TABLE IF NOT EXISTS Users(                  " +
                "   Id TEXT PRIMARY KEY,                            " +
                "   Email TEXT NOT NULL,                            " +
                "   NormalizedEmail TEXT NOT NULL,                  " +
                "   Username TEXT NOT NULL,                         " +
                "   NormalizedUsername TEXT NOT NULL,               " +
                "   RegistrationDate TEXT NOT NULL,                 " +
                "   PasswordHash BLOB NOT NULL,                     " +
                "   PasswordSalt BLOB NOT NULL,                     " +
                "   Role TEXT NOT NULL                              " +
                ");                                                 " +
                "                                                   " +
                "CREATE TABLE IF NOT EXISTS RefreshTokens(          " +
                "   Id TEXT PRIMARY KEY,                            " +
                "   UserId TEXT NOT NULL,                           " +
                "   Token TEXT NOT NULL,                            " +
                "   CreationTime TEXT NOT NULL,                     " +
                "   ExpiryTime TEXT NOT NULL,                       " +
                "                                                   " +
                "   FOREIGN KEY(UserId) REFERENCES Users(Id)        " +
                "       ON DELETE CASCADE ON UPDATE CASCADE         " +
                ");                                                 " +
                "                                                   " +
                "CREATE TABLE IF NOT EXISTS FreeApis(               " +
                "   Id TEXT PRIMARY KEY,                            " +
                "   ApiUrl TEXT NOT NULL,                           " +
                "   Name TEXT NOT NULL,                             " +
                "   ApiDescription TEXT NOT NULL,                   " +
                "   ApiIconUrl TEXT,                                " +
                "   RapidApiHost TEXT NOT NULL,                     " +
                "   IsQueryRequired INTEGER NOT NULL,               " +
                "   QueryKey TEXT,                                  " +
                "   QueryDescription TEXT                           " +
                ");                                                 " +
                "                                                   " +
                "CREATE TABLE IF NOT EXISTS Objectives(             " +
                "   Id TEXT PRIMARY KEY,                            " +
                "   UserId TEXT NOT NULL,                           " +
                "   Name TEXT NOT NULL,                             " +
                "   Description TEXT NOT NULL,                      " +
                "   CreationTime TEXT NOT NULL,                     " +
                "   LatestUpdateTime TEXT NOT NULL,                 " +
                "   StartAt TEXT NOT NULL,                          " +
                "   PeriodInMinutes INTERGER NOT NULL,              " +
                "   FreeApiId TEXT NOT NULL,                        " +
                "   Query TEXT,                                     " +
                "   ExecutedCount INTEGER NOT NULL,                 " +
                "   ExecutedLastTime TEXT,                          " +
                "   IsDeleted INTERGER NOT NULL,                    " +
                "                                                   " +
                "   FOREIGN KEY(UserId) REFERENCES Users(ID)        " +
                "       ON DELETE CASCADE ON UPDATE CASCADE,        " +
                "   FOREIGN KEY(FreeApiId) REFERENCES FreeApis(Id)  " +
                "       ON DELETE CASCADE ON UPDATE CASCADE         " +
                ");                                                 "
        };

        await command.ExecuteNonQueryAsync();
    }

    public async Task AddAdminAsync(IUserRepository userRepository)
    {
        if (await userRepository.IsUserExistAsync("ADMIN", UserIdentifierType.Username))
            return;

        using var hmac = new HMACSHA512();

        var user = new User(
            id: Guid.NewGuid().ToString(),
            email: "admin@tasker.com",
            username: "admin",
            registrationDate: DateTime.UtcNow,
            passwordHash: hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
            passwordSalt: hmac.Key,
            role: "Admin");

        await userRepository.AddUserAsync(user);
    }

    public async Task SeedFreeApisAsync(IFreeApiRepository freeApiRepository)
    {
        if (await freeApiRepository.GetRowsCountAsync() > 0)
            return;

        var freeApis = new List<FreeApi>()
        {
            new FreeApi(
                id: Guid.NewGuid().ToString(),
                apiUrl: "https://weatherapi-com.p.rapidapi.com/current.json",
                name: "WeatherAPI",
                apiDescription: "WeatherAPI provides the functionality to get realtime weather by location",
                apiIconUrl: "https://rapidapi.com/cdn/images?url=https://rapidapi-prod-apis.s3.amazonaws.com/6389b57aea57dbd256397c20c5b9bb56.png",
                rapidApiHost: "weatherapi-com.p.rapidapi.com",
                isQueryRequired: true,
                queryKey: "?q=",
                queryDescription: "Input location to check the weather"
            ),
            new FreeApi(
                id: Guid.NewGuid().ToString(),
                apiUrl: "https://covid-19-statistics.p.rapidapi.com/reports",
                name: "COVID-19 Statistics",
                apiDescription: "COVID-19 statistics based on public data by Johns Hopkins CSSE",
                apiIconUrl: "https://rapidapi.com/cdn/images?url=https://rapidapi-prod-collections.s3.amazonaws.com/bc460248-b172-4774-894f-005ae7eb0bb0.png",
                rapidApiHost: "covid-19-statistics.p.rapidapi.com",
                isQueryRequired: true,
                queryKey: "?q=",
                queryDescription: "Input country to get the report of COVID-19"
            ),
             new FreeApi(
                id: Guid.NewGuid().ToString(),
                apiUrl: "https://numbersapi.p.rapidapi.com/random",
                name: "Number API",
                apiDescription: "An API to get interesting facts about numbers",
                apiIconUrl: "https://rapidapi.com/cdn/images?url=https://s3.amazonaws.com/mashape-production-logos/apis/53aa3b60e4b00287471a0ca2_medium",
                rapidApiHost: "numbersapi.p.rapidapi.com",
                isQueryRequired: true,
                queryKey: "/",
                queryDescription: "Input 'trivia', 'math', 'date' or 'year' to get random fact"
            )
        };

        foreach (var api in freeApis)
            await freeApiRepository.AddFreeApiAsync(api);
    }
}
