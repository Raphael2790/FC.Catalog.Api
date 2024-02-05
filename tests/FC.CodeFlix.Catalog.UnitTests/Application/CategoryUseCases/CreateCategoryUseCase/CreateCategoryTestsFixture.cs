using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;
using FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.CreateCategoryUseCase;

[CollectionDefinition(nameof(CreateCategoryTestsFixture))]
public class CreateCategoryTestsCollection
    : ICollectionFixture<CreateCategoryTestsFixture>
{ }

public class CreateCategoryTestsFixture : CategoryUseCasesBaseFixture
{
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

    public CreateCategoryInput GetValidCreateCategoryInput()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public CreateCategoryInput GetValidCreateCategoryInputWithOnlyName()
       => new(GetValidCategoryName());

    public CreateCategoryInput GetValidCreateCategoryInputWithNameAndDescription()
       => new(GetValidCategoryName(), GetValidCategoryDescription());
}
