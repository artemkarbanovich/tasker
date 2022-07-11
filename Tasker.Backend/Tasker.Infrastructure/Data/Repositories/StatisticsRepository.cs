using Microsoft.Data.Sqlite;
using System.Data;
using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Logic.Statistics.Responses;

namespace Tasker.Infrastructure.Data.Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly SqliteConnection _connection;

    public StatisticsRepository(SqliteConnection connection)
    {
        _connection = connection;

        if (_connection.State is not ConnectionState.Open)
            _connection.Open();
    }

    public async Task<List<GetStatisticsItemResponse>> GetStatisticsAsync()
    {
        var command = new SqliteCommand()
        {
            Connection = _connection,
            CommandText = "SELECT U.Email, SUM(O.ExecutedCount), MAX(O.ExecutedLastTime), COUNT(O.Id), (SELECT COUNT(*) FROM Objectives WHERE UserId = O.UserId AND Objectives.IsDeleted = 1) " +
                "FROM Objectives O JOIN Users U ON O.UserId = U.Id  " +
                "GROUP BY O.UserId;                                 "
        };

        using var reader = await command.ExecuteReaderAsync();
        var statistics = new List<GetStatisticsItemResponse>();

        if (!reader.HasRows)
            return statistics;

        while (await reader.ReadAsync())
            statistics.Add(new GetStatisticsItemResponse(
                UserEmail: (string)reader.GetValue(0),
                ObjectivesTotalExecutedCount: (int)(long)reader.GetValue(1),
                ObjectiveExecutedLastTime: reader.GetValue(2) is DBNull ? null : Convert.ToDateTime(reader.GetValue(2)),
                TotalObjectivesCount: (int)(long)reader.GetValue(3),
                ObjectivesDeletedCount: (int)(long)reader.GetValue(4)));

        return statistics;
    }
}
