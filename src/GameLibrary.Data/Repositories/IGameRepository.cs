using GameLibrary.Data.Models;

namespace GameLibrary.Data.Repositories;

public interface IGameRepository
{
    Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Game?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> InsertAsync(Game item, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Game item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Game>> SearchAsync(string? title, string? notes, CancellationToken cancellationToken = default);
}
