using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.DTO;
using FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Create;

[CollectionDefinition(nameof(CreateGenreTestsFixture))]
public class CreateGenreTestsFixtureCollection
    : ICollectionFixture<CreateGenreTestsFixture>{}

public class CreateGenreTestsFixture
    : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput()
        => new CreateGenreInput(GetValidGenreName(), GetRandomBoolean());
}