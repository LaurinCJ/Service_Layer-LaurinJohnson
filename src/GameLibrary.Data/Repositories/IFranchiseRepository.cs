using GameLibrary.Data.Models;

namespace GameLibrary.Data.Repositories;

public interface IFranchiseRepository
{
    Task<IReadOnlyList<Franchise>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Franchise?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> InsertAsync(Franchise item, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Franchise item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
