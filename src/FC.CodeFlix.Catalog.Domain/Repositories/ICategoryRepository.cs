using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.CodeFlix.Catalog.Domain.Repositories;

public interface ICategoryRepository
    : IGenericRepository<Category>, ISearchableRepository<Category>
{
    Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken);
}
