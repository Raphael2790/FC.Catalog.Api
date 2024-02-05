using System;
using System.Threading;
using System.Threading.Tasks;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Create;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Create.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Create.Interface;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Enums;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.CreateCastMemberUseCase;

[Collection(nameof(CreateCastMemberTestsFixture))]
public class CreateCastMemberTests
{
    private readonly ICreateCastMember _sut;
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateCastMemberTestsFixture _fixture;
    
    public CreateCastMemberTests(CreateCastMemberTestsFixture fixture)
    {
        _fixture = fixture;
        _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new CreateCastMember(_castMemberRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact(DisplayName = nameof(Create))]
    [Trait("Application", "CastMember - Use Cases")]
    public async Task Create()
    {
        var input = new CreateCastMemberInput(_fixture.GetValidName(), _fixture.GetRandomCastMember());
        
        var output = await _sut.Handle(input, CancellationToken.None);
        
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        
        _castMemberRepositoryMock
            .Verify(x => x.Insert(It.Is<CastMember>(cm => 
                cm.Name == input.Name 
                && cm.Type == input.Type), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock
            .Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Theory(DisplayName = nameof(ThrowsErrorWhenInvalidName))]
    [Trait("Application", "CastMember - Use Cases")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task ThrowsErrorWhenInvalidName(string? name)
    {
        var input = new CreateCastMemberInput(name!, _fixture.GetRandomCastMember());
        
        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
            .Should().ThrowExactlyAsync<EntityValidationException>()
            .WithMessage("Name should not be null or empty");
    }
}