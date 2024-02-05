using FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.UpdateGenreUseCase;

[CollectionDefinition(nameof(UpdateGenreTestsFixture))]
public class UpdateGenreTestsFixtureCollection
    : ICollectionFixture<UpdateGenreTestsFixture>{}

public class UpdateGenreTestsFixture
    : GenreUseCasesBaseFixture
{
    
}