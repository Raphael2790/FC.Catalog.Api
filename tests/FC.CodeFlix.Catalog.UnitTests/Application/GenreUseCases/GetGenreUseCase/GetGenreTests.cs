using System;
using System.Threading;
using System.Threading.Tasks;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.GetGenreUseCase;

[Collection(nameof(GetGenreTestsFixture))]
public class GetGenreTests
{
    private readonly GetGenreTestsFixture _fixture;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly IGetGenre _sut;

    public GetGenreTests(GetGenreTestsFixture fixture)
    {
        _fixture = fixture;
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _sut = new GetGenre(_genreRepositoryMock.Object);
    }
    
    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var exampleCategoriesIds = _fixture.GetRandomIdsList();
        var exempleGenre = _fixture.GetExampleGenre(categoriesIds: exampleCategoriesIds);
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        var input = new GetGenreInput(exempleGenre.Id);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exempleGenre.Id);
        output.Name.Should().Be(exempleGenre.Name);
        output.IsActive.Should().Be(exempleGenre.IsActive);
        output.CreatedAt.Should().Be(exempleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(categoryId => output.Categories.Should().Contain(relation => relation.Id == categoryId));
        
        _genreRepositoryMock
            .Verify(x => 
                    x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()), 
                Times.Once);
    }
    
    [Fact(DisplayName = nameof(Get_WhenGenreNotFound_ThrowsException))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task Get_WhenGenreNotFound_ThrowsException()
    {
        var exampleId = Guid.NewGuid();
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exampleId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found"));
        var input = new GetGenreInput(exampleId);
        
        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
                .Should().ThrowExactlyAsync<NotFoundException>()
                .WithMessage($"Genre '{exampleId}' not found");

        _genreRepositoryMock
            .Verify(x => 
                    x.Get(It.Is<Guid>(x => x == exampleId), It.IsAny<CancellationToken>()), 
                Times.Once);
    }
}