using FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.CreateCastMemberUseCase;

[CollectionDefinition(nameof(CreateCastMemberTestsFixture))]
public class CreateCastMemberTestsFixtureCollection
    : ICollectionFixture<CreateCastMemberTestsFixture>{}

public class CreateCastMemberTestsFixture
    : CastMemberUseCasesBaseFixture
{
    
}