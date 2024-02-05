using FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.GetGenreUseCase;

[CollectionDefinition(nameof(GetGenreTestsFixture))]
public class GetGenreTestsFixtureCollection
    : ICollectionFixture<GetGenreTestsFixture>{}

public class GetGenreTestsFixture
    : GenreUseCasesBaseFixture
{
    
}