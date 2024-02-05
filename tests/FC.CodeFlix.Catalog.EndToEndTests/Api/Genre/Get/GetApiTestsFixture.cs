using FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Get;

[CollectionDefinition(nameof(GetApiTestsFixture))]
public class GetApiTestsFixtureCollection
    : ICollectionFixture<GetApiTestsFixture>{}

public class GetApiTestsFixture
    : GenreBaseFixture
{
    
}