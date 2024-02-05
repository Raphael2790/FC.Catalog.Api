using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.IntegrationTests.Base;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.UoW;

[CollectionDefinition(nameof(UnitOfWorkTestsFixture))]
public class UnitOfWorkTestsFixtureCollection 
    : ICollectionFixture<UnitOfWorkTestsFixture>
{ }

public class UnitOfWorkTestsFixture
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

    public Category GetValidCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<Category> GetExamplesCategoriesList(int length = 10)
        => Enumerable.Range(1, length)
            .Select(_ => GetValidCategory())
            .ToList();
}
