using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Update;

[CollectionDefinition(nameof(UpdateGenreTestsFixture))]
public class  UpdateGenreTestsFixtureCollection
    : ICollectionFixture<UpdateGenreTestsFixture>{}

public class UpdateGenreTestsFixture
    : GenreUseCasesBaseFixture
{
    
}