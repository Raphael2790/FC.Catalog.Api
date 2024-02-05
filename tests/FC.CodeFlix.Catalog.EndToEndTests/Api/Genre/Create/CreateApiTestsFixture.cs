using FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Create;

[CollectionDefinition(nameof(CreateApiTestsFixture))]
public class CreateApiTestsFixtureCollection
    : ICollectionFixture<CreateApiTestsFixture>{}

public class CreateApiTestsFixture
    : GenreBaseFixture
{
    
}