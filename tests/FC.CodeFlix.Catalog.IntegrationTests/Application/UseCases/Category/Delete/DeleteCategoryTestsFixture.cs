using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Common;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Delete;

[CollectionDefinition(nameof(DeleteCategoryTestsFixture))]
public class DeleteCategoryTestsFixtureCollection
    : ICollectionFixture<DeleteCategoryTestsFixture>
{ }

public class DeleteCategoryTestsFixture
    : CategoryUseCasesBaseFixture
{

}
