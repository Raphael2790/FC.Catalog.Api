using FC.CodeFlix.Catalog.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.CodeFlix.Catalog.Infra.Data.Mappings;

public class GenresCategoriesConfiguration
    : IEntityTypeConfiguration<GenresCategories>
{
    public void Configure(EntityTypeBuilder<GenresCategories> builder) 
        => builder.HasKey(genreCategory => new { genreCategory.CategoryId, genreCategory.GenreId });
}