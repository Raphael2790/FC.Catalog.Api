using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.UoW;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Create;

[Collection(nameof(CreateGenreTestsFixture))]
public class CreateGenreTests
{
    private readonly CreateGenreTestsFixture _fixture;
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly CodeflixCatalogDbContext _context;
    private readonly ICreateGenre _sut;
    
    public CreateGenreTests(CreateGenreTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext();
        _categoryRepository = new CategoryRepository(_context);
        _genreRepository = new GenreRepository(_context);
        _unitOfWork = new UnitOfWork(_context);
        _sut = new CreateGenre(_genreRepository, _unitOfWork, _categoryRepository);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        var input = _fixture.GetExampleInput();

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default);

        var assertionDbContext = _fixture.CreateDbContext(true);
        var genre = await assertionDbContext.Genres.FindAsync(output.Id);
        genre.Should().NotBeNull();
        genre!.Name.Should().Be(input.Name);
        genre.IsActive.Should().Be(input.IsActive);
    }
    
    [Fact(DisplayName = nameof(CreateGenreWithCategoriesRelations))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenreWithCategoriesRelations()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.SaveChangesAsync();
        var input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories.Select(category => category.Id).ToList();

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default);
        output.Categories.Should().HaveCount(input.CategoriesIds.Count);
        var outputRelations = output.Categories.Select(relation => relation.Id).ToList();
        outputRelations.Should().BeEquivalentTo(input.CategoriesIds);

        var assertionDbContext = _fixture.CreateDbContext(true);
        var genre = await assertionDbContext.Genres.FindAsync(output.Id);
        genre.Should().NotBeNull();
        genre!.Name.Should().Be(input.Name);
        genre.IsActive.Should().Be(input.IsActive);
        var relations = await assertionDbContext.GenresCategories.AsTracking()
            .Where(relation => relation.GenreId == output.Id)
            .ToListAsync();
        relations.Should().HaveCount(input.CategoriesIds.Count);
        var assertRelations = relations.Select(relation => relation.CategoryId).ToList();
        assertRelations.Should().BeEquivalentTo(input.CategoriesIds);
    }
    
    [Fact(DisplayName = nameof(CreateGenreThrowsWhenCategoryDoesntExists))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenreThrowsWhenCategoryDoesntExists()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.SaveChangesAsync();
        var input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories.Select(category => category.Id).ToList();
        var randomGuid = Guid.NewGuid();
        input.CategoriesIds?.Add(randomGuid);

        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
            .Should().ThrowExactlyAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {string.Join(',', randomGuid)}");
    }
}