using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Get;

[Collection(nameof(GetGenreTestsFixture))]
public class GetGenreTests
{
    private readonly GetGenreTestsFixture _fixture;
    private readonly CodeflixCatalogDbContext _context;
    private readonly IGetGenre _sut;

    public GetGenreTests(GetGenreTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext();
        IGenreRepository genreRepository = new GenreRepository(_context);
        _sut = new GetGenre(genreRepository);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var exampleGenreList = _fixture.GetExampleGenreList();
        var expectedGenre = exampleGenreList[5];
        await _context.Genres.AddRangeAsync(exampleGenreList);
        await _context.SaveChangesAsync();
        var input = new GetGenreInput(expectedGenre.Id);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
    }
    
    [Fact(DisplayName = nameof(GetGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreThrowsWhenNotFound()
    {
        var exampleGenreList = _fixture.GetExampleGenreList();
        var expectedGenre = Guid.NewGuid();
        await _context.Genres.AddRangeAsync(exampleGenreList);
        await _context.SaveChangesAsync();
        var input = new GetGenreInput(expectedGenre);

        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
            .Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{expectedGenre}' not found.");
    }
    
    [Fact(DisplayName = nameof(GetGenreWithCategoriesRelations))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreWithCategoriesRelations()
    {
        var exampleGenreList = _fixture.GetExampleGenreList();
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        var expectedGenre = exampleGenreList[5];
        exampleCategories.ForEach(category => expectedGenre.AddCategory(category.Id));
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.Genres.AddRangeAsync(exampleGenreList);
        await _context.GenresCategories.AddRangeAsync(
            expectedGenre.Categories
                .Select(categoryId => new GenresCategories(categoryId, expectedGenre.Id))
        );
        await _context.SaveChangesAsync();
        var input = new GetGenreInput(expectedGenre.Id);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        output.Categories.Should().HaveCount(expectedGenre.Categories.Count);
        foreach (var category in output.Categories)
        {
            expectedGenre.Categories.Should().Contain(category.Id);
            category.Name.Should().BeNull();
        }
    }
}