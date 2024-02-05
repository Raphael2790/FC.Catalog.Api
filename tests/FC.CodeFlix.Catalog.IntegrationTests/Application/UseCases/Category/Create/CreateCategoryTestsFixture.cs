using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;
using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Common;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Create;

[CollectionDefinition(nameof(CreateCategoryTestsFixture))]
public class CreateCategoryTestsFixtureCollection
    : ICollectionFixture<CreateCategoryTestsFixture>
{ }

public class CreateCategoryTestsFixture
    : CategoryUseCasesBaseFixture
{
    public CreateCategoryInput GetValidCreateCategoryInput()
    {
        var exampleCategory = GetValidCategory();
        return new CreateCategoryInput(exampleCategory.Name, exampleCategory.Description, exampleCategory.IsActive);
    }

    public CreateCategoryInput GetValidCreateCategoryInputWithOnlyName()
      => new(GetValidCategoryName());

    public CreateCategoryInput GetValidCreateCategoryInputWithNameAndDescription()
       => new(GetValidCategoryName(), GetValidCategoryDescription());

    public CreateCategoryInput GetInvalidCategoryInputShortName()
    {
        var inputWithShortName = GetValidCreateCategoryInput();
        inputWithShortName.Name = inputWithShortName.Name[..2];
        return inputWithShortName;
    }

    public CreateCategoryInput GetInvalidCategoryInputTooLongName()
    {
        var inputWithTooLongName = GetValidCreateCategoryInput();
        while (inputWithTooLongName.Name.Length <= 255)
            inputWithTooLongName.Name = $"{inputWithTooLongName.Name} {GetValidCategoryName()}";
        return inputWithTooLongName;
    }

    public CreateCategoryInput GetInvalidCreateCategoryInputNullDescription()
    {
        var inputWithNullDescription = GetValidCreateCategoryInput();
        inputWithNullDescription.Description = null;
        return inputWithNullDescription;
    }

    public CreateCategoryInput GetInvalidCategoryInputTooLongDescription()
    {
        var inputWithTooLongDescription = GetValidCreateCategoryInput();
        while (inputWithTooLongDescription.Description!.Length <= 10_000)
            inputWithTooLongDescription.Description = $"{inputWithTooLongDescription.Description} {GetValidCategoryDescription()}";
        return inputWithTooLongDescription;
    }
}
