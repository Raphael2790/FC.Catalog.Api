using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.CodeFlix.Catalog.Domain.Repositories;

public interface IGenreRepository 
    : IGenericRepository<Genre>, ISearchableRepository<Genre> {}