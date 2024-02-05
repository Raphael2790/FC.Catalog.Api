using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.UoW;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Delete;

[Collection(nameof(DeleteGenreTestsFixture))]
public class DeleteGenreTests
{
    private readonly DeleteGenreTestsFixture _fixture;
    private readonly CodeflixCatalogDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenreRepository _genreRepository;
    private readonly IDeleteGenre _sut;

    public DeleteGenreTests(DeleteGenreTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext(true);
        _unitOfWork = new UnitOfWork(_context);
        _genreRepository = new GenreRepository(_context);
        _sut = new DeleteGenre(_genreRepository, _unitOfWork);
    }

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Integration/Application ", "DeleteCategory - Use Cases")]
    public async Task DeleteGenre()
    {
        var exampleGenreList = _fixture.GetExampleGenreList();
        var targetGenre = exampleGenreList[5];
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.SaveChangesAsync();
        var input = new DeleteGenreInput(targetGenre.Id);

        await _sut.Handle(input, CancellationToken.None);

        var assertionDbContext = _fixture.CreateDbContext(true);
        var genre = await assertionDbContext.Genres.FindAsync(targetGenre.Id);
        genre.Should().BeNull();
    }
    
    [Fact(DisplayName = nameof(DeleteGenreThrowsWhenNotFound))]
    [Trait("Integration/Application ", "DeleteCategory - Use Cases")]
    public async Task DeleteGenreThrowsWhenNotFound()
    {
        var exampleGenreList = _fixture.GetExampleGenreList();
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.SaveChangesAsync();
        var randomGuid = Guid.NewGuid();
        var input = new DeleteGenreInput(randomGuid);

        await _sut.Awaiting(x => x.Handle(input, CancellationToken.None))
            .Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }
    
    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("Integration/Application ", "DeleteCategory - Use Cases")]
    public async Task DeleteGenreWithRelations()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        var exampleGenreList = _fixture.GetExampleGenreList();
        var targetGenre = exampleGenreList[5];
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.GenresCategories.AddRangeAsync(
            exampleCategories.Select(category => new GenresCategories(category.Id, targetGenre.Id))
        );
        await arrangeDbContext.SaveChangesAsync();
        var input = new DeleteGenreInput(targetGenre.Id);

        await _sut.Handle(input, CancellationToken.None);

        var assertionDbContext = _fixture.CreateDbContext(true);
        var genre = await assertionDbContext.Genres.FindAsync(targetGenre.Id);
        genre.Should().BeNull();
        var relations = await assertionDbContext.GenresCategories.AsTracking()
            .Where(relation => relation.GenreId == targetGenre.Id)
            .ToListAsync();
        relations.Should().HaveCount(default(int));
    }
}