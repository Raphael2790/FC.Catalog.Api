using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.DTO;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.DeleteCategoryUseCase;

[Collection(nameof(DeleteCategoryTestsFixture))]
public class DeleteCategoryTests
{
    private readonly DeleteCategoryTestsFixture _fixture;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _uOWMock;
    private readonly DeleteCategory _sut;

    public DeleteCategoryTests(DeleteCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _uOWMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _sut = new DeleteCategory(_categoryRepositoryMock.Object, _uOWMock.Object);
    }

    [Fact(DisplayName = nameof(Delete_WhenCategoryExist_ShouldDeleteCategory))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task Delete_WhenCategoryExist_ShouldDeleteCategory()
    {
        var categoryExample = _fixture.GetValidCategory();
        var input = new DeleteCategoryInput(categoryExample.Id);
        _categoryRepositoryMock.Setup(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryExample);

        await _sut.Handle(input, CancellationToken.None);

        _categoryRepositoryMock.Verify(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.Delete(categoryExample, It.IsAny<CancellationToken>()), Times.Once);
        _uOWMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(Delete_WhenCategoryDoesntExists_ShouldThrowException))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task Delete_WhenCategoryDoesntExists_ShouldThrowException()
    {
        var categoryId = Guid.NewGuid();
        var input = new DeleteCategoryInput(categoryId);
        _categoryRepositoryMock.Setup(x => x.Get(categoryId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category '{categoryId}' not found"));

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();
        _categoryRepositoryMock.Verify(x => x.Get(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
