using System;
using System.Threading;
using System.Threading.Tasks;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Get;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Get.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Get.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.GetCastMemberUseCase;

[Collection(nameof(GetCastMemberTestsFixture))]
public class GetCastMemberTests
{
    private readonly IGetCastMember _sut;
    private readonly GetCastMemberTestsFixture _fixture;
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;

    public GetCastMemberTests(GetCastMemberTestsFixture fixture)
    {
        _fixture = fixture;
        _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        _sut = new GetCastMember(_castMemberRepositoryMock.Object);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Application", "GetCastMember - Use Cases")]
    public async Task Get()
    {
        var castMember = _fixture.GetExampleCastMember();
        var input = new GetCastMemberInput(castMember.Id);
        
        _castMemberRepositoryMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMember);
        
        var output = await _sut.Handle(input, CancellationToken.None);
        
        output.Should().NotBeNull();
        output.Id.Should().Be(castMember.Id);
        output.Name.Should().Be(castMember.Name);
        output.Type.Should().Be(castMember.Type);
        output.CreatedAt.Should().BeCloseTo(castMember.CreatedAt, TimeSpan.FromSeconds(1));
        
        _castMemberRepositoryMock
            .Verify(x => x.Get(It.Is<Guid>(id => id == input.Id), It.IsAny<CancellationToken>()), Times.Once);
    } 
    
    [Fact(DisplayName = nameof(ThrowIfNotFound))]
    [Trait("Application", "GetCastMember - Use Cases")]
    public async Task ThrowIfNotFound()
    {
        var input = new GetCastMemberInput(Guid.NewGuid());
        
        _castMemberRepositoryMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Cast Member not found"));
        
        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
            .Should().ThrowExactlyAsync<NotFoundException>();
    } 
}