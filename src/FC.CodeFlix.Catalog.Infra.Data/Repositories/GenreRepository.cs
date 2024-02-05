using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.Infra.Data.Repositories;

public class GenreRepository
    : IGenreRepository
{
    private readonly DbSet<Genre> _genres;
    private readonly DbSet<GenresCategories> _genresCategories;

    public GenreRepository(CodeflixCatalogDbContext context)
    {
        _genres = context.Genres;
        _genresCategories = context.GenresCategories;
    }

    public async Task Insert(Genre genre, CancellationToken cancellationToken)
    {
        await _genres.AddAsync(genre, cancellationToken);
        await UpdateRelation(genre, cancellationToken);
    }

    public async Task<Genre> Get(Guid id, CancellationToken cancellationToken)
    {
        var genre = await _genres.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
        NotFoundException.ThrowIfNull(genre, $"Genre '{id}' not found.");
        var categoriesIds = await _genresCategories.Where(x => x.GenreId == genre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);
        foreach (var categoryId in categoriesIds)
            genre.AddCategory(categoryId);
        return genre;
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken)
    {
        _genresCategories
            .RemoveRange(_genresCategories.Where(x => x.GenreId == aggregate.Id));
        _genres.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task Update(Genre genre, CancellationToken cancellationToken)
    {
        _genres.Update(genre);
        _genresCategories.RemoveRange(_genresCategories.Where(x => x.GenreId == genre.Id));
        await UpdateRelation(genre, cancellationToken);
    }

    public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = _genres.AsNoTrackingWithIdentityResolution();

        query = AddOrderToQuery(query, input.OrderBy, input.SearchOrder);

        if (!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(x => x.Name.Contains(input.Search));
        
        var genres = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        var total = await query.CountAsync(cancellationToken);
        
        if (!genres.Any())
            return new SearchOutput<Genre>(
                input.Page,
                input.PerPage,
                total,
                genres);
        
        var genreIds = genres.Select(x => x.Id).ToList();
        var relations = await _genresCategories
            .Where(relation => genreIds.Contains(relation.GenreId))
            .ToListAsync(cancellationToken);
        var relationGroup = relations.GroupBy(x => x.GenreId).ToList();
        relationGroup.ForEach(grupo =>
        {
            var genre = genres.Find(x => x.Id == grupo.Key);
            if (genre is null) return;
            foreach(var item in grupo)
                genre.AddCategory(item.CategoryId);
        });

        return new SearchOutput<Genre>(
            input.Page,
            input.PerPage,
            total,
            genres);
    }

    private async Task UpdateRelation(Genre genre, CancellationToken cancellationToken)
    {
        if (genre.Categories.Any())
        {
            var relations = genre.Categories
                .Select(categoryId => new GenresCategories(categoryId, genre.Id));
            await _genresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }
    
    private static IQueryable<Genre> AddOrderToQuery(
        IQueryable<Genre> genres,
        string orderBy,
        SearchOrder order
    )
    {
        var query = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => genres.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => genres.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => genres.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => genres.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => genres.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => genres.OrderByDescending(x => x.CreatedAt),
            _ => genres.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        return query;
    }
}