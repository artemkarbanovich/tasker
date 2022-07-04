using Microsoft.Data.Sqlite;
using System.Data;

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
}
