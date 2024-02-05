using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.DTO;
using FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.Common;
using System;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.DeleteCategoryUseCase;

[CollectionDefinition(nameof(DeleteCategoryTestsFixture))]
public class DeleteCategoryTestsFixtureCollection
    : ICollectionFixture<DeleteCategoryTestsFixture>
{ }

public class DeleteCategoryTestsFixture : CategoryUseCasesBaseFixture
{
    public DeleteCategoryInput GetValidDeleteCategoryInput()
        => new(Guid.NewGuid());

    public DeleteCategoryInput GetInvalidDeleteCategoryInput()
        => new(Guid.Empty);
}
