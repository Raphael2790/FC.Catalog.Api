using FC.CodeFlix.Catalog.EndToEndTests.Base;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entities;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;

public class CategoryBaseFixture
    : BaseFixture
{
    public CategoryPersistence Persistence;

    public CategoryBaseFixture()
        : base()
    {
        Persistence = new CategoryPersistence(CreateDbContext());
    }

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

    public List<DomainEntities.Category> GetExamplesCategoriesList(int length = 15)
        => Enumerable.Range(1, length)
            .Select(_ => GetValidCategory())
            .ToList();
}
