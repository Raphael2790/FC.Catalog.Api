using FC.CodeFlix.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using FC.CodeFlix.Catalog.Infra.Data.Models;

namespace FC.CodeFlix.Catalog.Infra.Data.Context;

public class CodeflixCatalogDbContext
    : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GenresCategories> GenresCategories => Set<GenresCategories>();

    public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
