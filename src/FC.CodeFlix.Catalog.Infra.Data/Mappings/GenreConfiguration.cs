using FC.CodeFlix.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.CodeFlix.Catalog.Infra.Data.Mappings;

public class GenreConfiguration
    : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder) 
        => builder.HasKey(genre => genre.Id);
}