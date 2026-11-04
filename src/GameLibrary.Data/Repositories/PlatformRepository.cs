using System.Data;
using Microsoft.Data.SqlClient;
using GameLibrary.Data.Models;

namespace GameLibrary.Data.Repositories;

public sealed class PlatformRepository : IPlatformRepository
{
    private readonly SqlConnectionFactory connectionFactory;

    public PlatformRepository(SqlConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<IReadOnlyList<Platform>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return ReadManyAsync("SELECT Id, Name, Manufacturer FROM dbo.Platforms ORDER BY Name, Id;", null, cancellationToken);
    }

    public async Task<Platform?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var items = await ReadManyAsync("SELECT Id, Name, Manufacturer FROM dbo.Platforms WHERE Id = @Id;",
            command => command.Parameters.Add("@Id", SqlDbType.Int).Value = id, cancellationToken);
        return items.Count == 0 ? null : items[0];
    }

    public async Task<int> InsertAsync(Platform item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO dbo.Platforms (Name, Manufacturer) OUTPUT INSERTED.Id VALUES (@Name, @Manufacturer);";
        AddParameters(command, item);
        int id = (int)(await command.ExecuteScalarAsync(cancellationToken)
            ?? throw new InvalidOperationException("Insert did not return an ID."));
        item.Id = id;
        return id;
    }

    public async Task<bool> UpdateAsync(Platform item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE dbo.Platforms SET Name = @Name, Manufacturer = @Manufacturer WHERE Id = @Id;";
        AddParameters(command, item);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = item.Id;
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM dbo.Platforms WHERE Id = @Id;";
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    private async Task<IReadOnlyList<Platform>> ReadManyAsync(string sql, Action<SqlCommand>? addParameters,
        CancellationToken cancellationToken)
    {
        var items = new List<Platform>();
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

    private static void AddParameters(SqlCommand command, Platform item)
    {
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 80).Value = item.Name;
        command.Parameters.Add("@Manufacturer", SqlDbType.NVarChar, 80).Value = item.Manufacturer;
    }

    private static Platform Map(SqlDataReader reader)
    {
        return new Platform
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Manufacturer = reader.GetString(2),
        };
    }
}
