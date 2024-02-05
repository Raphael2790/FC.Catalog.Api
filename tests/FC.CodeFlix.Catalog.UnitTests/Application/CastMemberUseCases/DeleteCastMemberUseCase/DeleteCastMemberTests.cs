using System;
using System.Threading;
using System.Threading.Tasks;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Repositories;
using Moq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMemberUseCases.DeleteCastMemberUseCase;

[Collection(nameof(DeleteCastMemberTestsFixture))]
public class DeleteCastMemberTests
{
    private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;
    private readonly DeleteCastMemberTestsFixture _fixture;
    private readonly IDeleteCastMember _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteCastMemberTests(DeleteCastMemberTestsFixture fixture)
    {
        _fixture = fixture;
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        _sut = new DeleteCastMember(_castMemberRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact(DisplayName = nameof(DeleteCastMember))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async Task DeleteCastMember()
    {
        var castMember = _fixture.GetExampleCastMember();
        _castMemberRepositoryMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMember);
        var input = new DeleteCastMemberInput(castMember.Id);

        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
            .Should().NotThrowAsync();

        _castMemberRepositoryMock
            .Verify(x => x.Get(It.Is<Guid>(g => g == input.Id), It.IsAny<CancellationToken>()), Times.Once);
        _castMemberRepositoryMock
            .Verify(x => x.Delete(It.Is<CastMember>(c => c.Id == input.Id), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock
            .Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}