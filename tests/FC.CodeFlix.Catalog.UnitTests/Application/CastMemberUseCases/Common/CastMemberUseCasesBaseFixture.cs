using System;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Enums;
using FC.CodeFlix.Catalog.UnitTests.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.Common;

public class CastMemberUseCasesBaseFixture
    : BaseFixture
{
    public CastMember GetExampleCastMember()
        => new(GetValidName(), GetRandomCastMember());
    
    public string GetValidName()
        => Faker.Name.FullName();
    
    public CastMemberType GetRandomCastMember()
        => (CastMemberType)new Random().Next(1, 2);
}