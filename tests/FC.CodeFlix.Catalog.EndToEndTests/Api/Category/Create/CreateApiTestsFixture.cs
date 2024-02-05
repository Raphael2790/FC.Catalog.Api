using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;
using FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Create;

[CollectionDefinition(nameof(CreateApiTestsFixture))]
public class CreateApiTestsFixtureCollection
    : ICollectionFixture<CreateApiTestsFixture>
{ }

public class CreateApiTestsFixture
    : CategoryBaseFixture
{
    public CreateCategoryInput GetExampleInput()
        => new(GetValidCategoryName(),
               GetValidCategoryDescription(),
               GetRandomBoolean());

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
