using System;
using System.Threading;
using System.Threading.Tasks;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.Interface;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.DeleteGenreUseCase;

[Collection(nameof(DeleteGenreTestsFixture))]
public class DeleteGenreTests
{
    private readonly DeleteGenreTestsFixture _fixture;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IDeleteGenre _sut;

    public DeleteGenreTests(DeleteGenreTestsFixture fixture)
    {
        _fixture = fixture;
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new DeleteGenre(_genreRepositoryMock.Object, _unitOfWorkMock.Object);
    }
    
    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var exempleGenre = _fixture.GetExampleGenre();
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        
        var input = new DeleteGenreInput(exempleGenre.Id);

        await _sut.Handle(input, CancellationToken.None);
        
        _genreRepositoryMock
            .Verify(x => x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        _genreRepositoryMock
            .Verify(x => 
                    x.Delete(It.Is<Genre>(x => x.Id == exempleGenre.Id), It.IsAny<CancellationToken>()), 
                Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = nameof(Delete_ThowException_WhenNotFoundGenre))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task Delete_ThowException_WhenNotFoundGenre()
    {
        var exempleGenre = Guid.NewGuid();
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre '{exempleGenre}' not found."));
        
        var input = new DeleteGenreInput(exempleGenre);

        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
            .Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{exempleGenre}' not found.");
        
        _genreRepositoryMock
            .Verify(x => x.Get(It.Is<Guid>(id => id == exempleGenre), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}