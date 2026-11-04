using System.Data;
using Microsoft.Data.SqlClient;
using GameLibrary.Data.Models;

namespace GameLibrary.Data.Repositories;

public sealed class GameRepository : IGameRepository
{
    private readonly SqlConnectionFactory connectionFactory;

    public GameRepository(SqlConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return ReadManyAsync("SELECT Id, Title, Notes, ReleaseYear, FranchiseId, PlatformId FROM dbo.Games ORDER BY Title, Id;", null, cancellationToken);
    }

    public async Task<Game?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var items = await ReadManyAsync("SELECT Id, Title, Notes, ReleaseYear, FranchiseId, PlatformId FROM dbo.Games WHERE Id = @Id;",
            command => command.Parameters.Add("@Id", SqlDbType.Int).Value = id, cancellationToken);
        return items.Count == 0 ? null : items[0];
    }

    public async Task<int> InsertAsync(Game item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO dbo.Games (Title, Notes, ReleaseYear, FranchiseId, PlatformId) OUTPUT INSERTED.Id VALUES (@Title, @Notes, @ReleaseYear, @FranchiseId, @PlatformId);";
        AddParameters(command, item);
        int id = (int)(await command.ExecuteScalarAsync(cancellationToken)
            ?? throw new InvalidOperationException("Insert did not return an ID."));
        item.Id = id;
        return id;
    }

    public async Task<bool> UpdateAsync(Game item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE dbo.Games SET Title = @Title, Notes = @Notes, ReleaseYear = @ReleaseYear, FranchiseId = @FranchiseId, PlatformId = @PlatformId WHERE Id = @Id;";
        AddParameters(command, item);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = item.Id;
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM dbo.Games WHERE Id = @Id;";
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    private async Task<IReadOnlyList<Game>> ReadManyAsync(string sql, Action<SqlCommand>? addParameters,
        CancellationToken cancellationToken)
    {
        var items = new List<Game>();
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        addParameters?.Invoke(command);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(Map(reader));
        }
        return items;
    }

    private static void AddParameters(SqlCommand command, Game item)
    {
        command.Parameters.Add("@Title", SqlDbType.NVarChar, 120).Value = item.Title;
        command.Parameters.Add("@Notes", SqlDbType.NVarChar, 1000).Value = item.Notes;
        command.Parameters.Add("@ReleaseYear", SqlDbType.Int).Value = item.ReleaseYear;
        command.Parameters.Add("@FranchiseId", SqlDbType.Int).Value = item.FranchiseId;
        command.Parameters.Add("@PlatformId", SqlDbType.Int).Value = item.PlatformId;
    }

    private static Game Map(SqlDataReader reader)
    {
        return new Game
        {
            Id = reader.GetInt32(0),
            Title = reader.GetString(1),
            Notes = reader.GetString(2),
            ReleaseYear = reader.GetInt32(3),
            FranchiseId = reader.GetInt32(4),
            PlatformId = reader.GetInt32(5),
        };
    }

    public Task<IReadOnlyList<Game>> SearchAsync(string? title, string? notes,
        CancellationToken cancellationToken = default)
    {
        return ReadManyAsync(
            "SELECT Id, Title, Notes, ReleaseYear, FranchiseId, PlatformId FROM dbo.Games " +
            "WHERE (@Title = N'' OR Title LIKE N'%' + @Title + N'%' ESCAPE N'~') " +
            "AND (@Notes = N'' OR Notes LIKE N'%' + @Notes + N'%' ESCAPE N'~') ORDER BY Title, Id;",
            command =>
            {
                command.Parameters.Add("@Title", SqlDbType.NVarChar, -1).Value = EscapeLike(title);
                command.Parameters.Add("@Notes", SqlDbType.NVarChar, -1).Value = EscapeLike(notes);
            }, cancellationToken);
    }

    private static string EscapeLike(string? value)
    {
        return (value ?? string.Empty).Trim().Replace("~", "~~")
            .Replace("%", "~%").Replace("_", "~_").Replace("[", "~[");
    }
}
