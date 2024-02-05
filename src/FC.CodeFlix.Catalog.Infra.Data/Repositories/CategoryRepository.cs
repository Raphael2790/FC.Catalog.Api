using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.Infra.Data.Repositories;

public class CategoryRepository
    : ICategoryRepository
{
    private readonly  DbSet<Category> _categories;

    public CategoryRepository(CodeflixCatalogDbContext context) 
        => _categories = context.Categories;

    public Task Delete(Category aggregate, CancellationToken _)
        => Task.FromResult(_categories.Remove(aggregate));

    public async Task<Category> Get(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categories.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(category, $"Category '{id}' not found.");
        return category!;
    }

    public async Task Insert(Category aggregate, CancellationToken cancellationToken)
        => await _categories.AddAsync(aggregate, cancellationToken);

    public async Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = _categories.AsNoTrackingWithIdentityResolution();
        query = AddOrderToQuery(query, input.OrderBy, input.SearchOrder);
        if (!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(x => x.Name.Contains(input.Search));

        var items = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken: cancellationToken);
        var total = await query.CountAsync(cancellationToken: cancellationToken);
        return new SearchOutput<Category>(input.Page, input.PerPage,total, items);
    }

    public async Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken) 
        => (await _categories.Where(category => ids.Contains(category.Id))
            .ToListAsync(cancellationToken: cancellationToken))
            .Select(category => category.Id).ToList();

    public async Task<IReadOnlyList<Category>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken)
        => (await _categories.Where(category => ids.Contains(category.Id))
                .ToListAsync(cancellationToken: cancellationToken))
            .ToList();

    public Task Update(Category genre, CancellationToken _)
        => Task.FromResult(_categories.Update(genre));

    private static IQueryable<Category> AddOrderToQuery(
            IQueryable<Category> categories,
            string orderBy,
            SearchOrder order
        )
    {
        var query = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => categories.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => categories.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => categories.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => categories.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => categories.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => categories.OrderByDescending(x => x.CreatedAt),
            _ => categories.OrderBy(x => x.Name).ThenBy(x => x.Id),
        };

        return query;
    }
}
