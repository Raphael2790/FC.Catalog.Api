using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Update;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Update.DTO;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.UoW;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Update;

[Collection(nameof(UpdateGenreTestsFixture))]
public class UpdateGenreTests
{
    private readonly UpdateGenreTestsFixture _fixture;
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CodeflixCatalogDbContext _context;
    private readonly UpdateGenre _sut;

    public UpdateGenreTests(UpdateGenreTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext(true);
        _categoryRepository = new CategoryRepository(_context);
        _genreRepository = new GenreRepository(_context);
        _unitOfWork = new UnitOfWork(_context);
        _sut = new UpdateGenre(_genreRepository, _unitOfWork, _categoryRepository);
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Integration/Application", "Update Genre - Use Cases")]
    public async Task UpdateGenre()
    {
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories();
        var targetGenre = exampleGenres[5];
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.SaveChangesAsync();

        var input = new UpdateGenreInput(targetGenre.Id, _fixture.GetValidGenreName(), !targetGenre.IsActive);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        var assertionContext = _fixture.CreateDbContext(true);
        assertionContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        var genreFromDb = await assertionContext.Genres.AsTracking().FirstOrDefaultAsync(genre => genre.Id == targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
    }
    
    [Fact(DisplayName = nameof(UpdateGenreWithRelatedCategories))]
    [Trait("Integration/Application", "Update Genre - Use Cases")]
    public async Task UpdateGenreWithRelatedCategories()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories();
        var targetGenre = exampleGenres[5];
        var relatedCategories = exampleCategories.GetRange(0, 5);
        var newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id));
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeContext.GenresCategories.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        var input = new UpdateGenreInput(
            targetGenre.Id, 
            _fixture.GetValidGenreName(), 
            !targetGenre.IsActive, 
            newRelatedCategories.Select(category => category.Id).ToList());

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(newRelatedCategories.Count);
        var relatedCategoriesFromOutput = output.Categories.Select(relatedCategory => relatedCategory.Id);
        relatedCategoriesFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
        var assertionContext = _fixture.CreateDbContext(true);
        assertionContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        var genreFromDb = await assertionContext.Genres.AsTracking().FirstOrDefaultAsync(genre => genre.Id == targetGenre.Id);
        var relationsFromDb = await assertionContext.GenresCategories
            .AsTracking()
            .Where(relation => relation.GenreId == targetGenre.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        relationsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
    }
    
    [Fact(DisplayName = nameof(UpdateGenreWithoutNewRelatedCategories))]
    [Trait("Integration/Application", "Update Genre - Use Cases")]
    public async Task UpdateGenreWithoutNewRelatedCategories()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories();
        var targetGenre = exampleGenres[5];
        var relatedCategories = exampleCategories.GetRange(0, 5);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id));
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeContext.GenresCategories.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        var input = new UpdateGenreInput(
            targetGenre.Id, 
            _fixture.GetValidGenreName(), 
            !targetGenre.IsActive);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(relatedCategories.Count);
        var expectedRelatedCategoriesIds = relatedCategories.Select(category => category.Id).ToList();
        var relatedCategoriesFromOutput = output.Categories.Select(relatedCategory => relatedCategory.Id);
        relatedCategoriesFromOutput.Should().BeEquivalentTo(expectedRelatedCategoriesIds);
        var assertionContext = _fixture.CreateDbContext(true);
        assertionContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        var genreFromDb = await assertionContext.Genres.AsTracking().FirstOrDefaultAsync(genre => genre.Id == targetGenre.Id);
        var relationsFromDb = await assertionContext.GenresCategories
            .AsTracking()
            .Where(relation => relation.GenreId == targetGenre.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        relationsFromDb.Should().BeEquivalentTo(expectedRelatedCategoriesIds);
    }
    
    [Fact(DisplayName = nameof(UpdateGenreWithEmptyRelatedCategories))]
    [Trait("Integration/Application", "Update Genre - Use Cases")]
    public async Task UpdateGenreWithEmptyRelatedCategories()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories();
        var targetGenre = exampleGenres[5];
        var relatedCategories = exampleCategories.GetRange(0, 5);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id));
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeContext.GenresCategories.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        var input = new UpdateGenreInput(
            targetGenre.Id, 
            _fixture.GetValidGenreName(), 
            !targetGenre.IsActive,
            new List<Guid>());

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(default(int));
        var relatedCategoriesFromOutput = output.Categories.Select(relatedCategory => relatedCategory.Id);
        relatedCategoriesFromOutput.Should().BeEquivalentTo(new List<Guid>());
        var assertionContext = _fixture.CreateDbContext(true);
        assertionContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        var genreFromDb = await assertionContext.Genres.AsTracking().FirstOrDefaultAsync(genre => genre.Id == targetGenre.Id);
        var relationsFromDb = await assertionContext.GenresCategories
            .AsTracking()
            .Where(relation => relation.GenreId == targetGenre.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        relationsFromDb.Should().BeEquivalentTo(new List<Guid>());
    }
    
    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenCategorieDoesntExists))]
    [Trait("Integration/Application", "Update Genre - Use Cases")]
    public async Task UpdateGenreThrowsWhenCategorieDoesntExists()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories();
        var targetGenre = exampleGenres[5];
        var relatedCategories = exampleCategories.GetRange(0, 5);
        var newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        var relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id));
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeContext.GenresCategories.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();
        var categoriesToBeRelated = newRelatedCategories.Select(category => category.Id).ToList();
        var invalidCategory = Guid.NewGuid();
        categoriesToBeRelated.Add(invalidCategory);
        
        var input = new UpdateGenreInput(
            targetGenre.Id, 
            _fixture.GetValidGenreName(), 
            !targetGenre.IsActive,
            categoriesToBeRelated
            );

        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
            .Should().ThrowExactlyAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {string.Join(',', invalidCategory)}");
    }
    
    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "Update Genre - Use Cases")]
    public async Task UpdateGenreThrowsWhenNotFound()
    {
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories();
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.SaveChangesAsync();
        var randomGuid = Guid.NewGuid();
        
        var input = new UpdateGenreInput(randomGuid, _fixture.GetValidGenreName(), true);

        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
            .Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }
}