using System;
using FC.CodeFlix.Catalog.Domain.Enums;
using FC.CodeFlix.Catalog.UnitTests.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entities.CastMember;

[CollectionDefinition(nameof(CastMemberTestsFixture))]
public class CastMemberTestsFixtureCollection
    : ICollectionFixture<CastMemberTestsFixture>
{ }

public class CastMemberTestsFixture
    : BaseFixture
{
    public Catalog.Domain.Entities.CastMember GetExampleCastMember()
        => new(GetValidName(), GetRandomCastMember());
    
    public string GetValidName()
        => Faker.Name.FullName();
    
    public CastMemberType GetRandomCastMember()
        => (CastMemberType)new Random().Next(1, 2);
}