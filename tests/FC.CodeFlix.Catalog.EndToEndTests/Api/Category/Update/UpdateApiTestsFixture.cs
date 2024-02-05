using FC.Codeflix.Catalog.Api.Models.Category;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;
using FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Update;

[CollectionDefinition(nameof(UpdateApiTestsFixture))]
public class UpdateApiTestsFixtureCollection
    : ICollectionFixture<UpdateApiTestsFixture>
{}

public class UpdateApiTestsFixture
    : CategoryBaseFixture
{
    public UpdateCategoryApiInput GetValidInput()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());
    
    public UpdateCategoryApiInput GetValidInputOnlyNameUpdated()
        => new(GetValidCategoryName());
    
    public UpdateCategoryApiInput GetValidInputNameAndDescriptionUpdated()
        => new(GetValidCategoryName(), GetValidCategoryDescription());
    
    public string GetInvalidShortName() 
        => Faker.Commerce.ProductName()[..2];

    public string GetInvalidTooLongName()
    {
        var tooLongName = string.Empty;
        while (tooLongName.Length <= 255)
            tooLongName = $"{tooLongName} {GetValidCategoryName()}";
        return tooLongName;
    }

    public string GetInvalidTooLongDescription()
    {
        var tooLongDescription = string.Empty;
        while (tooLongDescription.Length <= 10_000)
            tooLongDescription = $"{tooLongDescription} {GetValidCategoryDescription()}";
        return tooLongDescription;
    }
}