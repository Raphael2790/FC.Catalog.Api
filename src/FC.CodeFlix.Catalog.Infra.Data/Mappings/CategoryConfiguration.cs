using FC.CodeFlix.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.CodeFlix.Catalog.Infra.Data.Mappings;

public class CategoryConfiguration
    : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .HasKey(c => c.Id);
        builder
            .Property(c => c.Name)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder
            .Property(c => c.Description)
            .HasMaxLength(10_000)
            .IsUnicode(false);
    }
}
