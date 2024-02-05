using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.Interface;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.UoW;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.Repositories.Category;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Create;

[Collection(nameof(CreateCategoryTestsFixture))]
public class CreateCategoryTests
{
    private readonly CreateCategoryTestsFixture _fixture;
    private readonly CodeflixCatalogDbContext _context;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _uoW;
    private readonly ICreateCategory _sut;

    public CreateCategoryTests(CreateCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext();
        _categoryRepository = new CategoryRepository(_context);
        _uoW = new UnitOfWork(_context);
        _sut = new CreateCategory(_categoryRepository, _uoW);
    }

    [Fact(DisplayName = nameof(CreateCategory_WhenInputHasValidValues_ShouldCreate))]
    [Trait("Integration/Application", "CreateCategory - Use Case")]
    public async Task CreateCategory_WhenInputHasValidValues_ShouldCreate()
    {
        var input = _fixture.GetValidCreateCategoryInput();

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);

        var newDbContext = _fixture.CreateDbContext(true);
        var savedCategory = await newDbContext
            .Categories
            .FindAsync(output.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be(input.Name);
        savedCategory.Description.Should().Be(input.Description);
        savedCategory.IsActive.Should().Be(input.IsActive);
        savedCategory.Id.Should().Be(output.Id);
        savedCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(CreateCategory_WhenInputHasOnlyNameValue_ShouldCreate))]
    [Trait("Integration/Application", "CreateCategory - Use Case")]
    public async Task CreateCategory_WhenInputHasOnlyNameValue_ShouldCreate()
    {
        var input = _fixture.GetValidCreateCategoryInputWithOnlyName();

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be("");
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);

        var newDbContext = _fixture.CreateDbContext(true);
        var savedCategory = await newDbContext
            .Categories
            .FindAsync(output.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be(input.Name);
        savedCategory.Description.Should().Be("");
        savedCategory.IsActive.Should().BeTrue();
        savedCategory.Id.Should().Be(output.Id);
        savedCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(CreateCategory_WhenInputHasNameAndDescriptionValue_ShouldCreate))]
    [Trait("Integration/Application", "CreateCategory - Use Case")]
    public async Task CreateCategory_WhenInputHasNameAndDescriptionValue_ShouldCreate()
    {
        var input = _fixture.GetValidCreateCategoryInputWithNameAndDescription();

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);

        var newDbContext = _fixture.CreateDbContext(true);
        var savedCategory = await newDbContext
            .Categories
            .FindAsync(output.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be(input.Name);
        savedCategory.Description.Should().Be(input.Description);
        savedCategory.IsActive.Should().BeTrue();
        savedCategory.Id.Should().Be(output.Id);
        savedCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(CreateCategory_WhenInputHasInvalidValue_ShouldThrowException))]
    [Trait("Integration/Application", "CreateCategory - Use Case")]
    [MemberData(nameof(CreateCategoryTestsGenerator.GetInvalidInputs), parameters: 4, MemberType = typeof(CreateCategoryTestsGenerator))]
    public async Task CreateCategory_WhenInputHasInvalidValue_ShouldThrowException(CreateCategoryInput input,
        string exceptionMessage)
    {
        Func<Task> task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);

        var context = _fixture.CreateDbContext(true);
        var categoriesSaved = await context.Categories.ToListAsync();
        categoriesSaved.Should().BeEmpty();
    }
}
