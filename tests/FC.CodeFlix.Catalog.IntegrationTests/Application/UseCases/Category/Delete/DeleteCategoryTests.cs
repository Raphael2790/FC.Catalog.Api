using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.UoW;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.Delete;

[Collection(nameof(DeleteCategoryTestsFixture))]
public class DeleteCategoryTests
{
    private readonly DeleteCategoryTestsFixture _fixture;
    private readonly CodeflixCatalogDbContext _context; 
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDeleteCategory _sut;

    public DeleteCategoryTests(DeleteCategoryTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext();
        _categoryRepository = new CategoryRepository(_context);
        _unitOfWork = new UnitOfWork(_context);
        _sut = new DeleteCategory(_categoryRepository, _unitOfWork);
    }

    [Fact(DisplayName = nameof(Delete_WhenCategoryExist_ShouldDeleteCategory))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async Task Delete_WhenCategoryExist_ShouldDeleteCategory()
    {
        var categoryExample = _fixture.GetValidCategory();
        var categories = _fixture.GetExamplesCategoriesList();
        await _context.AddRangeAsync(categories);
        var tracking = await _context.AddAsync(categoryExample);
        await _context.SaveChangesAsync();
        //sempre apos a persistencia para desligar o tracking da entidade
        tracking.State = EntityState.Detached;

        var input = new DeleteCategoryInput(categoryExample.Id);

        await _sut.Handle(input, CancellationToken.None);

        var newContext = _fixture.CreateDbContext(true);
        var deletedCategory = await newContext.Categories.FindAsync(categoryExample.Id);
        deletedCategory.Should().BeNull();
        var savedCategories = await newContext.Categories.ToListAsync();
        savedCategories.Should().HaveCount(categories.Count);
    }

    [Fact(DisplayName = nameof(Delete_WhenCategoryDoesntExists_ShouldThrowException))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async Task Delete_WhenCategoryDoesntExists_ShouldThrowException()
    {
        var categories = _fixture.GetExamplesCategoriesList();
        await _context.AddRangeAsync(categories);
        await _context.SaveChangesAsync();

        var input = new DeleteCategoryInput(Guid.NewGuid());

        var task = async () => await _sut.Handle(input, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found.");
    }
}
