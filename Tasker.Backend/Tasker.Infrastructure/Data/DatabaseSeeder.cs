using Microsoft.Data.Sqlite;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Tasker.Core.Entities;
using Tasker.Core.Enums;
using Tasker.Core.Interfaces.Repositories;

namespace Tasker.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly SqliteConnection _connection;

    public DatabaseSeeder(SqliteConnection connection)
    {
        _connection = connection;

        if (_connection.State is not ConnectionState.Open)
            _connection.Open();
    }
    
    ~DatabaseSeeder()
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
                "CREATE TABLE IF NOT EXISTS Users(              " +
                "   Id TEXT PRIMARY KEY,                        " +
                "   Email TEXT NOT NULL,                        " +
                "   NormalizedEmail TEXT NOT NULL,              " +
                "   Username TEXT NOT NULL,                     " +
                "   NormalizedUsername TEXT NOT NULL,           " +
                "   RegistrationDate TEXT NOT NULL,             " +
                "   PasswordHash BLOB NOT NULL,                 " +
                "   PasswordSalt BLOB NOT NULL,                 " +
                "   Role TEXT NOT NULL                          " +
                ");                                             " +
                "                                               " +
                "CREATE TABLE IF NOT EXISTS RefreshTokens(      " +
                "   Id TEXT PRIMARY KEY,                        " +
                "   UserId TEXT NOT NULL,                       " +
                "   Token TEXT NOT NULL,                        " +
                "   CreationTime TEXT NOT NULL,                 " +
                "   ExpiryTime TEXT NOT NULL,                   " +
                "                                               " +
                "   FOREIGN KEY(UserId) REFERENCES Users(Id)    " +
                "       ON DELETE CASCADE ON UPDATE CASCADE     " +
                ");                                             "
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
}
