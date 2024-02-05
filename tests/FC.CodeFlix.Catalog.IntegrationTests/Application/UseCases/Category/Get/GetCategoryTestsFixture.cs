using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Common;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Get;

[CollectionDefinition(nameof(GetCategoryTestsFixture))]
public class GetCategoryTestsFixtureCollection
    : ICollectionFixture<GetCategoryTestsFixture>
{ }

public class GetCategoryTestsFixture
    : CategoryUseCasesBaseFixture
{
}
