using FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;


namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Get;

[CollectionDefinition(nameof(GetApiTestsFixture))]
public class GetApiTestsFixtureCollection
    : ICollectionFixture<GetApiTestsFixture>
{ }

public class GetApiTestsFixture
    : CategoryBaseFixture
{}
