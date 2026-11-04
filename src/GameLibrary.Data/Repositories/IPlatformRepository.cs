using GameLibrary.Data.Models;

namespace GameLibrary.Data.Repositories;

public interface IPlatformRepository
{
    Task<IReadOnlyList<Platform>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Platform?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> InsertAsync(Platform item, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Platform item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
