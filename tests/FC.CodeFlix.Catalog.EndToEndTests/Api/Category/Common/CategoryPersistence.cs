using FC.CodeFlix.Catalog.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entities;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;

public class CategoryPersistence
{
    private readonly CodeflixCatalogDbContext _context;
    private readonly DbSet<DomainEntities.Category> _categories;

    public CategoryPersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
        _categories = _context.Categories;
    }

    public async Task<DomainEntities.Category?> GetById(Guid id)
        => await _categories.AsNoTrackingWithIdentityResolution()
                    .FirstOrDefaultAsync(x => x.Id == id);

    public async Task InsertList(List<DomainEntities.Category> exampleCategoriesList)
    {
        await _categories.AddRangeAsync(exampleCategoriesList);
        await _context.SaveChangesAsync();
    }
}
