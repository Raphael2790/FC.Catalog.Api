using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.Interface;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.UoW;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entities;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Update;

[Collection(nameof(UpdateCategoryTestsFixture))]
public class UpdateCategoryTests
{
    private readonly UpdateCategoryTestsFixture _fixture;
    private readonly CodeflixCatalogDbContext _context;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUpdateCategory _sut;

    public UpdateCategoryTests(UpdateCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext();
        _categoryRepository = new CategoryRepository(_context);
        _unitOfWork = new UnitOfWork(_context);
        _sut = new UpdateCategory(_categoryRepository, _unitOfWork);
    }

    [Theory(DisplayName = nameof(Handle_WhenCategoryExists_ShouldUpdate))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestsDataGenerator.GetCategoriesToUpdate), parameters: 5,
        MemberType = typeof(UpdateCategoryTestsDataGenerator))]
    public async Task Handle_WhenCategoryExists_ShouldUpdate(DomainEntities.Category exampleCategory, UpdateCategoryInput input)
    {
        await _context.AddRangeAsync(_fixture.GetExamplesCategoriesList());
        var tracking = await _context.AddAsync(exampleCategory);
        await _context.SaveChangesAsync();
        tracking.State = EntityState.Detached;
        CategoryOutputModel output = await _sut.Handle(input, CancellationToken.None);

        var newDbContext = _fixture.CreateDbContext(true);
        var savedCategory = await newDbContext
            .Categories
            .FindAsync(output.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be(input.Name);
        savedCategory.Description.Should().Be(input.Description);
        savedCategory.IsActive.Should().Be((bool)input.IsActive!);
        savedCategory.Id.Should().Be(output.Id);
        savedCategory.CreatedAt.Should().NotBeSameDateAs(default);
        savedCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Theory(DisplayName = nameof(Handle_WhenIsActiveIsntPassed_ShouldUpdate))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestsDataGenerator.GetCategoriesToUpdate), parameters: 5,
        MemberType = typeof(UpdateCategoryTestsDataGenerator))]
    public async Task Handle_WhenIsActiveIsntPassed_ShouldUpdate(DomainEntities.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
        await _context.AddRangeAsync(_fixture.GetExamplesCategoriesList());
        var tracking = await _context.AddAsync(exampleCategory);
        await _context.SaveChangesAsync();
        tracking.State = EntityState.Detached;
        CategoryOutputModel output = await _sut.Handle(input, CancellationToken.None);

        var newDbContext = _fixture.CreateDbContext(true);
        var savedCategory = await newDbContext
            .Categories
            .FindAsync(output.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be(input.Name);
        savedCategory.Description.Should().Be(input.Description);
        savedCategory.IsActive.Should().Be(exampleCategory.IsActive);
        savedCategory.Id.Should().Be(output.Id);
        savedCategory.CreatedAt.Should().NotBeSameDateAs(default);
        savedCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive!);
    }

    [Theory(DisplayName = nameof(Handle_WhenOnlyNameWasUpdated_ShouldUpdate))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestsDataGenerator.GetCategoriesToUpdate), parameters: 5,
        MemberType = typeof(UpdateCategoryTestsDataGenerator))]
    public async Task Handle_WhenOnlyNameWasUpdated_ShouldUpdate(DomainEntities.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
        await _context.AddRangeAsync(_fixture.GetExamplesCategoriesList());
        var tracking = await _context.AddAsync(exampleCategory);
        await _context.SaveChangesAsync();
        tracking.State = EntityState.Detached;
        CategoryOutputModel output = await _sut.Handle(input, CancellationToken.None);

        var newDbContext = _fixture.CreateDbContext(true);
        var savedCategory = await newDbContext
            .Categories
            .FindAsync(output.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be(input.Name);
        savedCategory.Description.Should().Be(exampleCategory.Description);
        savedCategory.IsActive.Should().Be(exampleCategory.IsActive);
        savedCategory.Id.Should().Be(output.Id);
        savedCategory.CreatedAt.Should().NotBeSameDateAs(default);
        savedCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive!);
    }

    [Fact(DisplayName = nameof(Handle_WhenCategoryDoesntExists_ShouldThrowNotFoundException))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    public async Task Handle_WhenCategoryDoesntExists_ShouldThrowNotFoundException()
    {
        var input = _fixture.GetValidUpdateCategoryInput();
        await _context.AddRangeAsync(_fixture.GetExamplesCategoriesList());
        await _context.SaveChangesAsync();

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found.");
    }

    [Theory(DisplayName = nameof(Handle_WhenOnlyNameWasUpdated_ShouldUpdate))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestsDataGenerator.GetInvalidInputs), parameters: 5,
        MemberType = typeof(UpdateCategoryTestsDataGenerator))]
    public async Task Handle_WhenCategoryCantBeUpdated_ShouldThrowEntityValidationException(UpdateCategoryInput input,
        string expectedMessage)
    {
        var categories = _fixture.GetExamplesCategoriesList();
        await _context.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
        input.Id = categories[0].Id;

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedMessage);
    }
}
