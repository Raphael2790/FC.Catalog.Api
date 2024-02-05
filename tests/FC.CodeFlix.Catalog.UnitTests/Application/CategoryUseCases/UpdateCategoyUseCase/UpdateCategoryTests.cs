using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.Interface;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.UpdateCategoyUseCase;

[Collection(nameof(UpdateCategoryTestsFixture))]
public class UpdateCategoryTests
{
    private readonly UpdateCategoryTestsFixture _fixture;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IUpdateCategory _sut;

    public UpdateCategoryTests(UpdateCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _sut = new UpdateCategory(_categoryRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Theory(DisplayName = nameof(Handle_WhenCategoryExists_ShouldUpdate))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestsDataGenerator.GetCategoriesToUpdate), parameters: 10,
        MemberType = typeof(UpdateCategoryTestsDataGenerator))]
    public async Task Handle_WhenCategoryExists_ShouldUpdate(Category category, UpdateCategoryInput input)
    {
        _categoryRepositoryMock.Setup(x => x.Get(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        CategoryOutputModel output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);

        _categoryRepositoryMock.Verify(x => x.Get(category.Id, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.Update(category, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Theory(DisplayName = nameof(Handle_WhenActiveIsntProvide_ShouldUpdate))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestsDataGenerator.GetCategoriesToUpdate), parameters: 10,
        MemberType = typeof(UpdateCategoryTestsDataGenerator))]
    public async Task Handle_WhenActiveIsntProvide_ShouldUpdate(Category category, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
        _categoryRepositoryMock.Setup(x => x.Get(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        CategoryOutputModel output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(category.IsActive!);

        _categoryRepositoryMock.Verify(x => x.Get(category.Id, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.Update(category, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Theory(DisplayName = nameof(Handle_WhenProvidedOnlyName_ShouldUpdate))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestsDataGenerator.GetCategoriesToUpdate), parameters: 10,
        MemberType = typeof(UpdateCategoryTestsDataGenerator))]
    public async Task Handle_WhenProvidedOnlyName_ShouldUpdate(Category category, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
        _categoryRepositoryMock.Setup(x => x.Get(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        CategoryOutputModel output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(category.Description);
        output.IsActive.Should().Be(category.IsActive!);

        _categoryRepositoryMock.Verify(x => x.Get(category.Id, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(x => x.Update(category, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(Handle_WhenCategoryDoesntExists_ShouldThrowNotFoundException))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    public async Task Handle_WhenCategoryDoesntExists_ShouldThrowNotFoundException()
    {
        var input = _fixture.GetValidUpdateCategoryInput();
        _categoryRepositoryMock.Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category '{input.Id}' not found."));

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<NotFoundException>();
        _categoryRepositoryMock.Verify(x => x.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(Handle_WhenCantUpdateCategory_ShouldThrowEntityValidationException))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestsDataGenerator.GetInvalidInputs),
                parameters: 12, MemberType = typeof(UpdateCategoryTestsDataGenerator))]
    public async Task Handle_WhenCantUpdateCategory_ShouldThrowEntityValidationException(UpdateCategoryInput input,
                                                                                         string expectedExceptionMessage)
    {
        var exampleCategory = _fixture.GetValidCategory();
        input.Id = exampleCategory.Id;
        _categoryRepositoryMock.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);

        _categoryRepositoryMock.Verify(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
