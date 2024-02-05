using System;
using FC.CodeFlix.Catalog.Domain.Enums;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entities.CastMember;

[Collection(nameof(CastMemberTestsFixture))]
public class CastMemberTests
{
    private readonly CastMemberTestsFixture _fixture;

    public CastMemberTests(CastMemberTestsFixture fixture) 
        => _fixture = fixture;
    
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Instantiate()
    {
        var name = _fixture.GetValidName();
        var type = _fixture.GetRandomCastMember();
        var castMember = new Catalog.Domain.Entities.CastMember(name, type);

        castMember.Should().NotBeNull();
        castMember.Id.Should().NotBeEmpty();
        castMember.Name.Should().NotBeEmpty();
        castMember.Name.Should().Be(name);
        castMember.Type.Should().Be(type);
        castMember.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }
    
    [Theory(DisplayName = nameof(ThrowErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ThrowErrorWhenNameIsInvalid(string? name)
    {
        var type = _fixture.GetRandomCastMember();
        var action = () => new Catalog.Domain.Entities.CastMember(name!, type);
        
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty");
    }
    
    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Update()
    {
        var castMember = _fixture.GetExampleCastMember();
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMember();
        
        castMember.Update(newName, newType);
        
        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(newType);
    }
    
    [Theory(DisplayName = nameof(UpdateThrowsErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateThrowsErrorWhenNameIsInvalid(string? newName)
    {
        var castMember = _fixture.GetExampleCastMember();
        var newType = _fixture.GetRandomCastMember();
        
        var action = () => castMember.Update(newName!, newType);
        
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty");
    }
}