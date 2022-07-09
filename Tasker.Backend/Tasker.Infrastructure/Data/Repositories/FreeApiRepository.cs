using Microsoft.Data.Sqlite;
using System.Data;
using Tasker.Core.Entities;
using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Logic.FreeApi.Responses;

namespace Tasker.Infrastructure.Data.Repositories;

public class FreeApiRepository : IFreeApiRepository
{
    private readonly SqliteConnection _connection;

    public FreeApiRepository(SqliteConnection connection)
    {
        _connection = connection;

        if (_connection.State is not ConnectionState.Open)
            _connection.Open();
    }

    public async Task AddFreeApiAsync(FreeApi freeApi)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "INSERT INTO FreeApis(Id, ApiUrl, Name, ApiDescription, ApiIconUrl, RapidApiHost, IsQueryRequired, QueryKey, QueryDescription) " +
                "VALUES(@id, @apiUrl, @name, @apiDescription, @apiIconUrl, @rapidApiHost, @isQueryRequired, @queryKey, @queryDescription);"
        };

        command.Parameters.AddWithValue("@id", freeApi.Id);
        command.Parameters.AddWithValue("@apiUrl", freeApi.ApiUrl);
        command.Parameters.AddWithValue("@name", freeApi.Name);
        command.Parameters.AddWithValue("@apiDescription", freeApi.ApiDescription);
        command.Parameters.AddWithValue("@apiIconUrl", freeApi.ApiIconUrl ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@rapidApiHost", freeApi.RapidApiHost);
        command.Parameters.AddWithValue("@isQueryRequired", freeApi.IsQueryRequired);
        command.Parameters.AddWithValue("@queryKey", freeApi.QueryKey ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@queryDescription", freeApi.QueryDescription ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<FreeApi?> GetFreeApiByIdAsync(string id)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT * FROM FreeApis WHERE Id = @id;"
        };

        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows || !await reader.ReadAsync())
            return null;

        return new FreeApi(
            id: (string)reader["Id"],
            apiUrl: (string)reader["ApiUrl"],
            name: (string)reader["Name"],
            apiDescription: (string)reader["ApiDescription"],
            apiIconUrl: reader["ApiIconUrl"] is DBNull ? null : (string)reader["ApiIconUrl"],
            rapidApiHost: (string)reader["RapidApiHost"],
            isQueryRequired: (long)reader["IsQueryRequired"] == 1,
            queryKey: reader["QueryKey"] is DBNull ? null : (string)reader["QueryKey"],
            queryDescription: reader["QueryDescription"] is DBNull ? null : (string)reader["QueryDescription"]);
    }

    public async Task<List<GetFreeApisItemResponse>> GetFreeApisAsync()
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT Id, Name, ApiDescription, ApiIconUrl, IsQueryRequired, QueryDescription FROM FreeApis;"
        };

        using var reader = await command.ExecuteReaderAsync();
        var freeApisResponse = new List<GetFreeApisItemResponse>();

        if (!reader.HasRows)
            return freeApisResponse;

        while(await reader.ReadAsync())
        {
            freeApisResponse.Add(new GetFreeApisItemResponse(
                Id: (string)reader["Id"],
                Name: (string)reader["Name"],
                ApiDescription: (string)reader["ApiDescription"],
                ApiIconUrl: reader["ApiIconUrl"] is DBNull ? null : (string)reader["ApiIconUrl"],
                IsQueryRequired: (long)reader["IsQueryRequired"] == 1,
                QueryDescription: reader["QueryDescription"] is DBNull ? null : (string)reader["QueryDescription"]));
        }

        return freeApisResponse;
    }

    public async Task<bool> IsFreeApiExistByIdAsync(string id)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT EXISTS(SELECT 1 FROM Users WHERE Id = @id;"
        };

        command.Parameters.AddWithValue("@id", id);

        var isExist = await command.ExecuteScalarAsync();

        return isExist is not null && (long)isExist == 1;
    }

    public async Task<long> GetRowsCountAsync()
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT COUNT(*) FROM FreeApis;"
        };

        var rowsCount = await command.ExecuteScalarAsync();

        return rowsCount is not null ? (long)rowsCount : 0;
    }
}
