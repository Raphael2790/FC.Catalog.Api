using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.Interface;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.CreateGenreUseCase;

[Collection(nameof(CreateGenreTestsFixture))]
public class CreateGenreTests
{
    private readonly CreateGenreTestsFixture _fixture;
    private readonly Mock<IGenreRepository> _repositoryMock;
    private readonly Mock<ICategoryRepository> _categoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ICreateGenre _sut;

    public CreateGenreTests(CreateGenreTestsFixture fixture)
    {
        _fixture = fixture;
        _repositoryMock = new Mock<IGenreRepository>();
        _categoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new CreateGenre(_repositoryMock.Object, _unitOfWorkMock.Object, _categoryMock.Object);
    }

    [Fact(DisplayName = nameof(CreateGenre_WhenInputHasValidValues_ShouldCreate))]
    [Trait("Application", "CreateGenre - Use Case")]
    public async Task CreateGenre_WhenInputHasValidValues_ShouldCreate()
    {
        var input = _fixture.GetExampleInput();

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().HaveCount(default(int));
        output.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        _repositoryMock
            .Verify(x => x.Insert(It.IsAny<Genre>(),It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock
            .Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = nameof(CreateGenreWithRelatedCategories_WhenInputHasValidValues_ShouldCreate))]
    [Trait("Application", "CreateGenre - Use Case")]
    public async Task CreateGenreWithRelatedCategories_WhenInputHasValidValues_ShouldCreate()
    {
        var input = _fixture.GetExampleInputWithCategories();
        _categoryMock.Setup(x =>
                x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(input.CategoriesIds!);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().HaveCount(input.CategoriesIds?.Count ?? default);
        input.CategoriesIds?.ForEach(categoryId => output.Categories.Should()
            .Contain(relation => relation.Id == categoryId));
        output.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        _repositoryMock
            .Verify(x => x.Insert(It.IsAny<Genre>(),It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock
            .Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = nameof(CreateGenre_WhenRelatedCategoriesNotFound_ShouldThrowException))]
    [Trait("Application", "CreateGenre - Use Case")]
    public async Task CreateGenre_WhenRelatedCategoriesNotFound_ShouldThrowException()
    {
        var input = _fixture.GetExampleInputWithCategories();
        var exampleGuid = input.CategoriesIds![^1];
        _categoryMock.Setup(x =>
                x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(input.CategoriesIds.FindAll(x => x != exampleGuid));

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<RelatedAggregateException>()
                .WithMessage($"Related category id (or ids) not found: {exampleGuid}");
        
        _categoryMock.Verify(x => 
            x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Theory(DisplayName = nameof(CreateGenre_WhenNameIsInvalid_ShouldThrowException))]
    [Trait("Application", "CreateGenre - Use Case")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task CreateGenre_WhenNameIsInvalid_ShouldThrowException(string? name)
    {
        var input = _fixture.GetExampleInputWithoutName(name);

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<EntityValidationException>()
            .WithMessage("Name should not be null or empty");
    }
}