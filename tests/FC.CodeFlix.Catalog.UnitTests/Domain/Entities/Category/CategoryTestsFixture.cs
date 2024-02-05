using FC.CodeFlix.Catalog.UnitTests.Common;
using Xunit;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entities;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entities.Category;

public class CategoryTestsFixture : BaseFixture
{
    public CategoryTestsFixture() : base(){}

    public DomainEntities.Category GetValidCategory(bool active = true)
        => new(GetValidCategoryName(), GetValidCategoryDescription(), active);

    public string GetValidCategoryName()
    {
        var categoryName = string.Empty;
        while(categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if(categoryName.Length > 255)
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
}

[CollectionDefinition(nameof(CategoryTestsFixture))]
public class CategoryTestsFixtureCollection : ICollectionFixture<CategoryTestsFixture> { }
