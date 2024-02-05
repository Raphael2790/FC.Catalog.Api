using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Common;

public class GenrePersistence
{
    private readonly CodeflixCatalogDbContext _context;
    private readonly DbSet<Domain.Entities.Genre> _genres;

    public GenrePersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
        _genres = _context.Genres;
    }
    
    public async Task InsertList(List<Domain.Entities.Genre> exampleGenresList)
    {
        await _genres.AddRangeAsync(exampleGenresList);
        await _context.SaveChangesAsync();
    }


    public async Task InsertCategoriesGenresRelationsList(List<GenresCategories> relations)
    {
        await _context.GenresCategories.AddRangeAsync(relations);
        await _context.SaveChangesAsync();
    }

    public async Task<Domain.Entities.Genre?> GetGenreById(Guid targetGenreId) =>
        await _genres.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(x => x.Id == targetGenreId);

    public async Task<List<GenresCategories>> GetGenresCategoriesRelationsByGenreId(Guid targetGenreId) =>
        await _context.GenresCategories.AsNoTrackingWithIdentityResolution()
            .Where(x => x.GenreId == targetGenreId)
            .ToListAsync();
}