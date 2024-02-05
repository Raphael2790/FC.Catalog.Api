using FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Delete;

[CollectionDefinition(nameof(DeleteApiTestsFixture))]
public class DeleteApiTestsFixtureCollection
    : ICollectionFixture<DeleteApiTestsFixture>{}

public class DeleteApiTestsFixture
    : GenreBaseFixture
{
    
}