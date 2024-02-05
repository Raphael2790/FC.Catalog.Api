using FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Delete;

[CollectionDefinition(nameof(DeleteApiTestsFixture))]
public class DeleteApiTestsFixtureCollection
    : ICollectionFixture<DeleteApiTestsFixture>
{ }

public class DeleteApiTestsFixture
    : CategoryBaseFixture
{}
