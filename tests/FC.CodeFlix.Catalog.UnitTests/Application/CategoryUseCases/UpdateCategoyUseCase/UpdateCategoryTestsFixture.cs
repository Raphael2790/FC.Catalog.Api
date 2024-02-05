using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;
using FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.Common;
using System;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.UpdateCategoyUseCase;

[CollectionDefinition(nameof(UpdateCategoryTestsFixture))]
public class UpdateCategoryTestsFixtureCollection
    : ICollectionFixture<UpdateCategoryTestsFixture>
{ }

public class UpdateCategoryTestsFixture : CategoryUseCasesBaseFixture
{
    public UpdateCategoryInput GetValidUpdateCategoryInput(Guid? id = null)
        => new(id ?? Guid.NewGuid(), GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public UpdateCategoryInput GetInvalidUpdateCategoryInput()
        => new(Guid.Empty, null!, null!, GetRandomBoolean());

    public UpdateCategoryInput GetInvalidCategoryInputShortName()
    {
        var inputWithShortName = GetValidUpdateCategoryInput();
        inputWithShortName.Name = inputWithShortName.Name[..2];
        return inputWithShortName;
    }

    public UpdateCategoryInput GetInvalidCategoryInputTooLongName()
    {
        var inputWithTooLongName = GetValidUpdateCategoryInput();
        while (inputWithTooLongName.Name.Length <= 255)
            inputWithTooLongName.Name = $"{inputWithTooLongName.Name} {GetValidCategoryName()}";
        return inputWithTooLongName;
    }

    public UpdateCategoryInput GetInvalidCategoryInputTooLongDescription()
    {
        var inputWithTooLongDescription = GetValidUpdateCategoryInput();
        while (inputWithTooLongDescription.Description!.Length <= 10_000)
            inputWithTooLongDescription.Description = $"{inputWithTooLongDescription.Description} {GetValidCategoryDescription()}";
        return inputWithTooLongDescription;
    }
}
