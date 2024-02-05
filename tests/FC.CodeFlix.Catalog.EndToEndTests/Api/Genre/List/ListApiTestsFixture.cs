using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.List;

[CollectionDefinition(nameof(ListApiTestsFixture))]
public class ListApiTestsFixtureCollection 
    : ICollectionFixture<ListApiTestsFixture>
{ }

public class ListApiTestsFixture
    : GenreBaseFixture
{
    public List<Domain.Entities.Genre> GetExampleGenreListWithNames(IEnumerable<string> names)
        => names.Select(name => GetExampleGenre(name: name)).ToList();
    
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