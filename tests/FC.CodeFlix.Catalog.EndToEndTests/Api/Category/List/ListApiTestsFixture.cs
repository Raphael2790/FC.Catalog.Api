using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.List;

[CollectionDefinition(nameof(ListApiTestsFixture))]
public class ListApiTestsFixtureCollection
    : ICollectionFixture<ListApiTestsFixture>
{}

public class ListApiTestsFixture
    : CategoryBaseFixture
{
    public List<Domain.Entities.Category> GetExamplesCategoriesListWithName(List<string> names)
        => names.Select(name =>
        {
            var category = GetValidCategory();
            category.Update(name);
            return category;
        }).ToList();

    public List<Domain.Entities.Category> CloneOrderedCategoriesList(
        IEnumerable<Domain.Entities.Category> categoriesList,
        string orderBy,
        SearchOrder searchOrder
    )
    {
        var listClone = new List<Domain.Entities.Category>(categoriesList);
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