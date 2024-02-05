using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Update;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Update.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Update.Interface;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.UpdateGenreUseCase;

[Collection(nameof(UpdateGenreTestsFixture))]
public class UpdateGenreTests
{
    private readonly UpdateGenreTestsFixture _fixture;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IUpdateGenre _sut;

    public UpdateGenreTests(UpdateGenreTestsFixture fixture)
    {
        _fixture = fixture;
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new UpdateGenre(_genreRepositoryMock.Object, _unitOfWorkMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        var exempleGenre = _fixture.GetExampleGenre();
        var newName = _fixture.GetValidGenreName();
        var newIsActiveValue = !exempleGenre.IsActive;
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        var input = new UpdateGenreInput(exempleGenre.Id,newName, newIsActiveValue);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exempleGenre.Id);
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActiveValue);
        output.CreatedAt.Should().Be(exempleGenre.CreatedAt);
        output.Categories.Should().BeEmpty();
        
        _genreRepositoryMock
            .Verify(x => 
                x.Update(It.Is<Genre>(x => x.Id == exempleGenre.Id), It.IsAny<CancellationToken>()), 
                Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = nameof(Update_WhenGenreNotFound_ThrowNotFoundException))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task Update_WhenGenreNotFound_ThrowNotFoundException()
    {
        var exampleId = Guid.NewGuid();
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre {exampleId} not found."));
        var input = new UpdateGenreInput(exampleId, _fixture.GetValidGenreName(), true);

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre {exampleId} not found.");
    }
    
    [Theory(DisplayName = nameof(Update_WhenGenreNameIsInvalid_ThrowsException))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task Update_WhenGenreNameIsInvalid_ThrowsException(string? name)
    {
        var exempleGenre = _fixture.GetExampleGenre();
        var newIsActiveValue = !exempleGenre.IsActive;
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        var input = new UpdateGenreInput(exempleGenre.Id, name!, newIsActiveValue);
        
        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<EntityValidationException>()
            .WithMessage("Name should not be null or empty");
    }
    
    [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateGenreOnlyName(bool isActive)
    {
        var exempleGenre = _fixture.GetExampleGenre(isActive: isActive);
        var newName = _fixture.GetValidGenreName();
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        var input = new UpdateGenreInput(exempleGenre.Id,newName);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exempleGenre.Id);
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(isActive);
        output.CreatedAt.Should().Be(exempleGenre.CreatedAt);
        output.Categories.Should().BeEmpty();
        
        _genreRepositoryMock
            .Verify(x => 
                    x.Update(It.Is<Genre>(x => x.Id == exempleGenre.Id), It.IsAny<CancellationToken>()), 
                Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = nameof(Update_AddingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task Update_AddingCategoriesIds()
    {
        var exempleGenre = _fixture.GetExampleGenre();
        var newName = _fixture.GetValidGenreName();
        var exampleCategoriesIds = _fixture.GetRandomIdsList();
        var newIsActiveValue = !exempleGenre.IsActive;
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds);
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        var input = new UpdateGenreInput(exempleGenre.Id,newName, newIsActiveValue, exampleCategoriesIds);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exempleGenre.Id);
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActiveValue);
        output.CreatedAt.Should().Be(exempleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(categoryId => output.Categories.Should().Contain(relation => relation.Id == categoryId));
        
        _genreRepositoryMock
            .Verify(x => 
                    x.Update(It.Is<Genre>(x => x.Id == exempleGenre.Id), It.IsAny<CancellationToken>()), 
                Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = nameof(Update_ReplacingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task Update_ReplacingCategoriesIds()
    {
        var exempleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandomIdsList());
        var newName = _fixture.GetValidGenreName();
        var exampleCategoriesIds = _fixture.GetRandomIdsList();
        var newIsActiveValue = !exempleGenre.IsActive;
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds);
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        var input = new UpdateGenreInput(exempleGenre.Id,newName, newIsActiveValue, exampleCategoriesIds);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exempleGenre.Id);
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActiveValue);
        output.CreatedAt.Should().Be(exempleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(categoryId => output.Categories.Should().Contain(relation => relation.Id == categoryId));
        
        _genreRepositoryMock
            .Verify(x => 
                    x.Update(It.Is<Genre>(x => x.Id == exempleGenre.Id), It.IsAny<CancellationToken>()), 
                Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = nameof(Update_WhenCategoriesNotFound_ThrowsException))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task Update_WhenCategoriesNotFound_ThrowsException()
    {
        var exempleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandomIdsList());
        var newName = _fixture.GetValidGenreName();
        var exampleCategoriesIds = _fixture.GetRandomIdsList(10);
        var listReturnedByRepository = exampleCategoriesIds.GetRange(0, exampleCategoriesIds.Count - 2);
        var idsNotReturnedByRepository = exampleCategoriesIds.GetRange(exampleCategoriesIds.Count - 2, 2);
        var newIsActiveValue = !exempleGenre.IsActive;
        _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(listReturnedByRepository);
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        var input = new UpdateGenreInput(exempleGenre.Id,newName, newIsActiveValue, exampleCategoriesIds);

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        var notFoundIdsAsString = string.Join(", ", idsNotReturnedByRepository);
        await task.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {notFoundIdsAsString}");
    }
    
    [Fact(DisplayName = nameof(Update_WithoutCategoriesIds_ShouldNotRemoveCategories))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task Update_WithoutCategoriesIds_ShouldNotRemoveCategories()
    {
        var exampleCategoriesIds = _fixture.GetRandomIdsList();
        var exempleGenre = _fixture.GetExampleGenre(categoriesIds: exampleCategoriesIds);
        var newName = _fixture.GetValidGenreName();
        var newIsActiveValue = !exempleGenre.IsActive;
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        var input = new UpdateGenreInput(exempleGenre.Id,newName, newIsActiveValue);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exempleGenre.Id);
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActiveValue);
        output.CreatedAt.Should().Be(exempleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriesIds.Count);
        exampleCategoriesIds.ForEach(categoryId => output.Categories.Should().Contain(relation => relation.Id == categoryId));
        
        _genreRepositoryMock
            .Verify(x => 
                    x.Update(It.Is<Genre>(x => x.Id == exempleGenre.Id), It.IsAny<CancellationToken>()), 
                Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = nameof(Update_WithEmptyCategoriesIdsList_ShouldRemoveAllCategories))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task Update_WithEmptyCategoriesIdsList_ShouldRemoveAllCategories()
    {
        var exampleCategoriesIds = _fixture.GetRandomIdsList();
        var exempleGenre = _fixture.GetExampleGenre(categoriesIds: exampleCategoriesIds);
        var newName = _fixture.GetValidGenreName();
        var newIsActiveValue = !exempleGenre.IsActive;
        _genreRepositoryMock
            .Setup(x => 
                x.Get(It.Is<Guid>(x => x == exempleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleGenre);
        var input = new UpdateGenreInput(exempleGenre.Id,newName, newIsActiveValue, new List<Guid>());

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exempleGenre.Id);
        output.Name.Should().Be(newName);
        output.IsActive.Should().Be(newIsActiveValue);
        output.CreatedAt.Should().Be(exempleGenre.CreatedAt);
        output.Categories.Should().BeEmpty();

        _genreRepositoryMock
            .Verify(x => 
                    x.Update(It.Is<Genre>(x => x.Id == exempleGenre.Id), It.IsAny<CancellationToken>()), 
                Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}