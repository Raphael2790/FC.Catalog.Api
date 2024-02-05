using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Get;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.DTO;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.GetCategoryUseCase;


[Collection(nameof(GetCategoryTestsFixture))]
public class GetCategoryTests
{
    private readonly GetCategoryTestsFixture _fixture;
    private readonly Mock<ICategoryRepository> _categoryRepository;
    public GetCategoryTests(GetCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _categoryRepository = new Mock<ICategoryRepository>();
    }

    [Fact(DisplayName = nameof(GetCategory_WhenInputHasExistingCategoryGUID_ShouldReturnCategoryFound))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async Task GetCategory_WhenInputHasExistingCategoryGUID_ShouldReturnCategoryFound()
    {
        var category = _fixture.GetValidCategory();
        _categoryRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        var input = new GetCategoryInput(category.Id);
        var useCase = new GetCategory(_categoryRepository.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        _categoryRepository.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Name.Should().Be(category.Name);
        output.Description.Should().Be(category.Description);
        output.IsActive.Should().Be(category.IsActive);
        output.Id.Should().Be(category.Id);
        output.CreatedAt.Should().Be(category.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetCategory_WhenInputHasDoesntExistsCategoryGUID_ShouldThrowNotFoundException))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async Task GetCategory_WhenInputHasDoesntExistsCategoryGUID_ShouldThrowNotFoundException()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category '{categoryId}' not found"));
        var input = new GetCategoryInput(categoryId);
        var useCase = new GetCategory(_categoryRepository.Object);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();
        _categoryRepository.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
