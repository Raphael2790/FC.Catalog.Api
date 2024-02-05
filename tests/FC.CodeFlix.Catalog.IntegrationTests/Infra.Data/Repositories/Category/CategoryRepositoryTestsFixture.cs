using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entities;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.Repositories.Category;

[CollectionDefinition(nameof(CategoryRepositoryTestsFixture))]
public class CategoryRepositoryTestsFixtureCollection
    : ICollectionFixture<CategoryRepositoryTestsFixture>
{ }

public class CategoryRepositoryTestsFixture 
    : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = string.Empty;
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    public bool GetRandomBoolean()
        => new Random().NextDouble() < 0.5;

    public DomainEntities.Category GetValidCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<DomainEntities.Category> GetExamplesCategoriesList(int length = 10)
        => Enumerable.Range(1, length)
            .Select(_ => GetValidCategory())
            .ToList();

    public List<DomainEntities.Category> GetExamplesCategoriesListWithName(List<string> names)
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
