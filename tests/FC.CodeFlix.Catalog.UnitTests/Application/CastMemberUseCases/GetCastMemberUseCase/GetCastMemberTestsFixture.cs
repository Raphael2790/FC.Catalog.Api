using FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.GetCastMemberUseCase;

[CollectionDefinition(nameof(GetCastMemberTestsFixture))]
public class GetCastMemberTestsFixtureCollection
    : ICollectionFixture<GetCastMemberTestsFixture>{}

public class GetCastMemberTestsFixture
    : CastMemberUseCasesBaseFixture
{
}