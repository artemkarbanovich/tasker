using Microsoft.Data.Sqlite;
using System.Data;
using Tasker.Core.Entities;
using Tasker.Core.Interfaces.Repositories;

namespace Tasker.Infrastructure.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SqliteConnection _connection;

    public RefreshTokenRepository(SqliteConnection connection)
    {
        _connection = connection;

        if (_connection.State is not ConnectionState.Open)
            _connection.Open();
    }

    ~RefreshTokenRepository()
    {
        if (_connection is not null && _connection.State is not ConnectionState.Closed)
            _connection.Close();
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "INSERT INTO RefreshTokens (Id, UserId, Token, CreationTime, ExpiryTime) VALUES (@id, @userId, @token, @creationTime, @expiryTime);"
        };

        command.Parameters.AddWithValue("@id", refreshToken.Id);
        command.Parameters.AddWithValue("@userId", refreshToken.UserId);
        command.Parameters.AddWithValue("@token", refreshToken.Token);
        command.Parameters.AddWithValue("@creationTime", refreshToken.CreationTime);
        command.Parameters.AddWithValue("@expiryTime", refreshToken.ExpiryTime);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string userId, string token)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT * FROM RefreshTokens WHERE UserId = @userId AND Token = @token;"
        };

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@token", token);

        using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows || !await reader.ReadAsync())
            return null;

        return new RefreshToken(
            id: (string)reader["Id"],
            userId: (string)reader["UserId"],
            token: (string)reader["Token"],
            creationTime: Convert.ToDateTime(reader["CreationTime"]),
            expiryTime: Convert.ToDateTime(reader["ExpiryTime"]));
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "UPDATE RefreshTokens SET Token = @token, CreationTime = @creationTime, ExpiryTime = @expiryTime WHERE Id = @id;"
        };

        command.Parameters.AddWithValue("@token", refreshToken.Token);
        command.Parameters.AddWithValue("@creationTime", refreshToken.CreationTime);
        command.Parameters.AddWithValue("@expiryTime", refreshToken.ExpiryTime);
        command.Parameters.AddWithValue("@id", refreshToken.Id);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteRefreshTokenAsync(string userId, string token)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "DELETE FROM RefreshTokens WHERE UserId = @userId AND Token = @token;"
        };

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@token", token);

        await command.ExecuteNonQueryAsync();
    }
}
