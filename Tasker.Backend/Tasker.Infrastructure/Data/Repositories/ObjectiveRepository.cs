using Microsoft.Data.Sqlite;
using System.Data;
using Tasker.Core.Entities;
using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Logic.Objective.Responses;

namespace Tasker.Infrastructure.Data.Repositories;

public class ObjectiveRepository : IObjectiveRepository
{
    private readonly SqliteConnection _connection;

    public ObjectiveRepository(SqliteConnection connection)
    {
        _connection = connection;

        if (_connection.State is not ConnectionState.Open)
            _connection.Open();
    }

    public async Task AddObjectiveAsync(Objective objective)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "INSERT INTO Objectives(Id, UserId, Name, Description, CreationTime, LatestUpdateTime, StartAt, PeriodInMinutes, FreeApiId, Query, ExecutedCount, ExecutedLastTime, IsDeleted) " +
                "VALUES(@id, @userId, @name, @description, @creationTime, @latestUpdateTime, @startAt, @periodInMinutes, @freeApiId, @query, @executedCount, @executedLastTime, @isDeleted);"
        };

        command.Parameters.AddWithValue("@id", objective.Id);
        command.Parameters.AddWithValue("@userId", objective.UserId);
        command.Parameters.AddWithValue("@name", objective.Name);
        command.Parameters.AddWithValue("@description", objective.Description);
        command.Parameters.AddWithValue("@creationTime", objective.CreationTime);
        command.Parameters.AddWithValue("@latestUpdateTime", objective.LatestUpdateTime);
        command.Parameters.AddWithValue("@startAt", objective.StartAt);
        command.Parameters.AddWithValue("@periodInMinutes", objective.PeriodInMinutes);
        command.Parameters.AddWithValue("@freeApiId", objective.FreeApiId);
        command.Parameters.AddWithValue("@query", objective.Query ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@executedCount", objective.ExecutedCount);
        command.Parameters.AddWithValue("@executedLastTime", objective.ExecutedLastTime ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@isDeleted", objective.IsDeleted);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<GetObjectivesItemResponse>> GetObjectivesAsync(string userId)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT Id, Name, Description, ExecutedLastTime   " +
                "FROM Objectives                                            " +
                "WHERE IsDeleted = 0 AND UserId = @userId                   " +
                "ORDER BY DATE(LatestUpdateTime) DESC;                      "
        };

        command.Parameters.AddWithValue("@userId", userId);

        using var reader = await command.ExecuteReaderAsync();
        var objectivesResponse = new List<GetObjectivesItemResponse>();

        if (!reader.HasRows)
            return objectivesResponse;

        while(await reader.ReadAsync())
        {
            objectivesResponse.Add(new GetObjectivesItemResponse(
                Id: (string)reader["Id"],
                Name: (string)reader["Name"],
                Description: (string)reader["Description"],
                ExecutedLastTime: reader["ExecutedLastTime"] is DBNull ? null : Convert.ToDateTime(reader["ExecutedLastTime"])));
        }

        return objectivesResponse;
    }

    public async Task<Objective?> GetObjectiveByIdAsync(string objectiveId)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT * FROM Objectives WHERE Id = @objectiveId;"
        };

        command.Parameters.AddWithValue("@objectiveId", objectiveId);

        using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows || !await reader.ReadAsync())
            return null;

        return new Objective(
            id: (string)reader["Id"],
            userId: (string)reader["UserId"],
            name: (string)reader["Name"],
            description: (string)reader["Description"],
            creationTime: Convert.ToDateTime(reader["CreationTime"]),
            latestUpdateTime: Convert.ToDateTime(reader["LatestUpdateTime"]),
            startAt: Convert.ToDateTime(reader["StartAt"]),
            periodInMinutes: (int)(long)reader["PeriodInMinutes"],
            freeApiId: (string)reader["FreeApiId"],
            query: reader["Query"] is DBNull ? null : (string)reader["Query"],
            executedCount: (int)(long)reader["ExecutedCount"],
            executedLastTime: reader["ExecutedLastTime"] is DBNull ? null : Convert.ToDateTime(reader["ExecutedLastTime"]),
            isDeleted: (long)reader["IsDeleted"] == 1);
    }

    public async Task DeleteObjectiveByIdAsync(string userId, string objectiveId)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "UPDATE Objectives SET IsDeleted = 1 WHERE UserId = @userId AND Id = @objectiveId;"
        };

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@objectiveId", objectiveId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateObjectiveAsync(Objective objective)
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "UPDATE Objectives SET Name = @name, Description = @description, LatestUpdateTime = @latestUpdateTime, StartAt = @startAt, PeriodInMinutes = @periodInMinutes, FreeApiId = @freeApiId, Query = @query " +
                "WHERE UserId = @userId AND Id = @objectiveId;"
        };

        command.Parameters.AddWithValue("@name", objective.Name);
        command.Parameters.AddWithValue("@description", objective.Description);
        command.Parameters.AddWithValue("@latestUpdateTime", objective.LatestUpdateTime);
        command.Parameters.AddWithValue("@startAt", objective.StartAt);
        command.Parameters.AddWithValue("@periodInMinutes", objective.PeriodInMinutes);
        command.Parameters.AddWithValue("@freeApiId", objective.FreeApiId);
        command.Parameters.AddWithValue("@query", objective.Query ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@userId", objective.UserId);
        command.Parameters.AddWithValue("@objectiveId", objective.Id);

        await command.ExecuteNonQueryAsync();
    }
}
