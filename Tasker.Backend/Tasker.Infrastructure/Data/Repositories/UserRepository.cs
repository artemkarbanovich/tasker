using Microsoft.Data.Sqlite;
using System.Data;
using Tasker.Core.Entities;
using Tasker.Core.Enums;
using Tasker.Core.Exceptions;
using Tasker.Core.Interfaces.Repositories;

namespace Tasker.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SqliteConnection _connection;

    public UserRepository(SqliteConnection connection)
    {
        _connection = connection;

        if (_connection.State is not ConnectionState.Open)
            _connection.Open();
    }

    public async Task<bool> IsUserExistAsync(string identifier, UserIdentifierType userIdentifierType)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT EXISTS(SELECT 1 FROM Users WHERE "
        };

        command.CommandText += userIdentifierType switch
        {
            UserIdentifierType.Id => "Id = @identifier);",
            UserIdentifierType.Email => "NormalizedEmail = @identifier);",
            UserIdentifierType.Username => "NormalizedUsername = @identifier);",
            _ => throw new DefaultException("Unknown database exception")
        };

        if (userIdentifierType is UserIdentifierType.Id)
            command.Parameters.AddWithValue("@identifier", identifier);
        else
            command.Parameters.AddWithValue("@identifier", identifier.ToUpper());

        var isExist = await command.ExecuteScalarAsync();

        return isExist is not null && (long)isExist == 1;
    }

    public async Task AddUserAsync(User user)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "INSERT INTO Users(Id, Email, NormalizedEmail, Username, NormalizedUsername, RegistrationDate, PasswordHash, PasswordSalt, Role) " +
            "VALUES(@id, @email, @normalizedEmail, @username, @normalizedUsername, @registrationDate, @passwordHash, @passwordSalt, @role);"
        };

        command.Parameters.AddWithValue("@id", user.Id);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@normalizedEmail", user.NormalizedEmail);
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@normalizedUsername", user.NormalizedUsername);
        command.Parameters.AddWithValue("@registrationDate", user.RegistrationDate);
        command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@passwordSalt", user.PasswordSalt);
        command.Parameters.AddWithValue("@role", user.Role);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<User?> GetUserAsync(string identifier, UserIdentifierType userIdentifierType)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT Id, Email, Username, RegistrationDate, PasswordHash, PasswordSalt, Role FROM Users WHERE "
        };

        command.CommandText += userIdentifierType switch
        {
            UserIdentifierType.Id => "Id = @identifier;",
            UserIdentifierType.Email => "NormalizedEmail = @identifier;",
            UserIdentifierType.Username => "NormalizedUsername = @identifier;",
            _ => throw new DefaultException("Unknown database exception")
        };

        if (userIdentifierType is UserIdentifierType.Id)
            command.Parameters.AddWithValue("@identifier", identifier);
        else
            command.Parameters.AddWithValue("@identifier", identifier.ToUpper());

        using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows || !await reader.ReadAsync())
            return null;

        return new User(
            id: (string)reader["Id"],
            email: (string)reader["Email"],
            username: (string)reader["Username"],
            registrationDate: Convert.ToDateTime(reader["RegistrationDate"]),
            passwordHash: (byte[])reader["PasswordHash"],
            passwordSalt: (byte[])reader["PasswordSalt"],
            role: (string)reader["Role"]);
    }
}
