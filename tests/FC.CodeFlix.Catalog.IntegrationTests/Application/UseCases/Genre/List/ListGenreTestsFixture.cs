using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.List;

[CollectionDefinition(nameof(ListGenreTestsFixture))]
public class ListGenreTestsFixtureCollection
    : ICollectionFixture<ListGenreTestsFixture>{}

public class ListGenreTestsFixture
    : GenreUseCasesBaseFixture
{
    public List<Domain.Entities.Genre> CloneOrderedGenresList(
        IEnumerable<Domain.Entities.Genre> genresList,
        string orderBy,
        SearchOrder searchOrder
    )
    {
        var listClone = new List<Domain.Entities.Genre>(genresList);
        var orderedEnumerable = (orderBy.ToLower(), searchOrder) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }
}