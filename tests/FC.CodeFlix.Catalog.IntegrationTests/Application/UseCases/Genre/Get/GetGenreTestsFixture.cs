using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Get;

[CollectionDefinition(nameof(GetGenreTestsFixture))]
public class GetGenreTestsFixtureCollection
    : ICollectionFixture<GetGenreTestsFixture>{}

public class GetGenreTestsFixture
    : GenreUseCasesBaseFixture
{
    
}