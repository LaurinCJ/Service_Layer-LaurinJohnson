using System.Data;
using Microsoft.Data.SqlClient;
using GameLibrary.Data.Models;

namespace GameLibrary.Data.Repositories;

public sealed class FranchiseRepository : IFranchiseRepository
{
    private readonly SqlConnectionFactory connectionFactory;

    public FranchiseRepository(SqlConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<IReadOnlyList<Franchise>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return ReadManyAsync("SELECT Id, Name, Developer FROM dbo.Franchises ORDER BY Name, Id;", null, cancellationToken);
    }

    public async Task<Franchise?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var items = await ReadManyAsync("SELECT Id, Name, Developer FROM dbo.Franchises WHERE Id = @Id;",
            command => command.Parameters.Add("@Id", SqlDbType.Int).Value = id, cancellationToken);
        return items.Count == 0 ? null : items[0];
    }

    public async Task<int> InsertAsync(Franchise item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO dbo.Franchises (Name, Developer) OUTPUT INSERTED.Id VALUES (@Name, @Developer);";
        AddParameters(command, item);
        int id = (int)(await command.ExecuteScalarAsync(cancellationToken)
            ?? throw new InvalidOperationException("Insert did not return an ID."));
        item.Id = id;
        return id;
    }

    public async Task<bool> UpdateAsync(Franchise item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE dbo.Franchises SET Name = @Name, Developer = @Developer WHERE Id = @Id;";
        AddParameters(command, item);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = item.Id;
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM dbo.Franchises WHERE Id = @Id;";
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    private async Task<IReadOnlyList<Franchise>> ReadManyAsync(string sql, Action<SqlCommand>? addParameters,
        CancellationToken cancellationToken)
    {
        var items = new List<Franchise>();
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

    private static void AddParameters(SqlCommand command, Franchise item)
    {
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = item.Name;
        command.Parameters.Add("@Developer", SqlDbType.NVarChar, 120).Value = item.Developer;
    }

    private static Franchise Map(SqlDataReader reader)
    {
        return new Franchise
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Developer = reader.GetString(2),
        };
    }
}
