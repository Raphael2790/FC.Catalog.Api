using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Delete;

[CollectionDefinition(nameof(DeleteGenreTestsFixture))]
public class DeletegenreTestsFixtureCollection
    : ICollectionFixture<DeleteGenreTestsFixture>{}

public class DeleteGenreTestsFixture
    : GenreUseCasesBaseFixture
{
    
}