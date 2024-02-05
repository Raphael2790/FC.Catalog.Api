using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Get;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Get;

[Collection(nameof(GetCategoryTestsFixture))]
public class GetCategoryTests
{
    private readonly GetCategoryTestsFixture _fixture;
    private readonly CodeflixCatalogDbContext _context;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGetCategory _sut;

    public GetCategoryTests(GetCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext();
        _categoryRepository = new CategoryRepository(_context);
        _sut = new GetCategory(_categoryRepository);
    }

    [Fact(DisplayName = nameof(GetCategory_WhenInputHasExistingCategoryGUID_ShouldReturnCategoryFound))]
    [Trait("Integration/Application", "GetCategory - Use Cases")]
    public async Task GetCategory_WhenInputHasExistingCategoryGUID_ShouldReturnCategoryFound()
    {
        var category = _fixture.GetValidCategory();
        var input = new GetCategoryInput(category.Id);
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(category.Name);
        output.Description.Should().Be(category.Description);
        output.IsActive.Should().Be(category.IsActive);
        output.Id.Should().Be(category.Id);
        output.CreatedAt.Should().Be(category.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetCategory_WhenInputHasDoesntExistsCategoryGUID_ShouldThrowNotFoundException))]
    [Trait("Integration/Application", "GetCategory - Use Cases")]
    public async Task GetCategory_WhenInputHasDoesntExistsCategoryGUID_ShouldThrowNotFoundException()
    {
        var categoryId = Guid.NewGuid();
        var input = new GetCategoryInput(categoryId);
        var category = _fixture.GetValidCategory();
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{categoryId}' not found.");
    }
}
