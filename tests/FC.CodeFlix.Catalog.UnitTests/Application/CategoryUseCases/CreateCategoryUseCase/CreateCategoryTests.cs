using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.CreateCategoryUseCase;

[Collection(nameof(CreateCategoryTestsFixture))]
public class CreateCategoryTests
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CreateCategoryTestsFixture _fixture;

    public CreateCategoryTests(CreateCategoryTestsFixture fixture)
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory_WhenInputHasValidValues_ShouldCreate))]
    [Trait("Application", "CreateCategory - Use Case")]
    public async Task CreateCategory_WhenInputHasValidValues_ShouldCreate()
    {
        var useCase = new CreateCategory(_repositoryMock.Object, _uowMock.Object);
        var input = _fixture.GetValidCreateCategoryInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        _repositoryMock.Verify(x => x.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateCategory_WhenInputHasOnlyNameValue_ShouldCreate))]
    [Trait("Application", "CreateCategory - Use Case")]
    public async Task CreateCategory_WhenInputHasOnlyNameValue_ShouldCreate()
    {
        var useCase = new CreateCategory(_repositoryMock.Object, _uowMock.Object);
        var input = _fixture.GetValidCreateCategoryInputWithOnlyName();

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(string.Empty);
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        _repositoryMock.Verify(x => x.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(CreateCategory_WhenInputHasNameAndDescriptionValue_ShouldCreate))]
    [Trait("Application", "CreateCategory - Use Case")]
    public async Task CreateCategory_WhenInputHasNameAndDescriptionValue_ShouldCreate()
    {
        var useCase = new CreateCategory(_repositoryMock.Object, _uowMock.Object);
        var input = _fixture.GetValidCreateCategoryInputWithNameAndDescription();

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        _repositoryMock.Verify(x => x.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(CreateCategory_WhenInputHasInvalidValue_ShouldthrowException))]
    [Trait("Application", "CreateCategory - Use Case")]
    [MemberData(nameof(CreateCategoryTestsGenerator.GetInvalidInputs), parameters: 24, MemberType = typeof(CreateCategoryTestsGenerator))]
    public async Task CreateCategory_WhenInputHasInvalidValue_ShouldthrowException(CreateCategoryInput input,
        string exceptionMessage)
    {
        var useCase = new CreateCategory(_repositoryMock.Object, _uowMock.Object);

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }
}
