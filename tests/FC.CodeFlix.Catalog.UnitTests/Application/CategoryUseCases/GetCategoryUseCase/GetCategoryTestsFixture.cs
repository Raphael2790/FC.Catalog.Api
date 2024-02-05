using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.DTO;
using FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.Common;
using System;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.GetCategoryUseCase;

[CollectionDefinition(nameof(GetCategoryTestsFixture))]
public class GetCategoryTestsFixtureCollection : ICollectionFixture<GetCategoryTestsFixture>
{ }

public class GetCategoryTestsFixture : CategoryUseCasesBaseFixture
{
    public GetCategoryInput GetGetCategoryInput(Guid guid)
        => new(guid);
}
