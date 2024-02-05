using FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.DeleteGenreUseCase;

[CollectionDefinition(nameof(DeleteGenreTestsFixture))]
public class DeleteGenreTestsFixtureCollection
    : ICollectionFixture<DeleteGenreTestsFixture>{}

public class DeleteGenreTestsFixture
    : GenreUseCasesBaseFixture
{ }