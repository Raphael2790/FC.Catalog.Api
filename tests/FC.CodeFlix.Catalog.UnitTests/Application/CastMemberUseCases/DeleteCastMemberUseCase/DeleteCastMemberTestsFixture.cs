using FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.DeleteCastMemberUseCase;

[CollectionDefinition(nameof(DeleteCastMemberTestsFixture))]
public class DeleteCastMemberTestsFixtureCollection
    : ICollectionFixture<DeleteCastMemberTestsFixture>{}

public class DeleteCastMemberTestsFixture
    : CastMemberUseCasesBaseFixture
{ }