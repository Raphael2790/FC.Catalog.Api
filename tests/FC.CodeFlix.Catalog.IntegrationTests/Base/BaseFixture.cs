using Bogus;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.IntegrationTests.Base;

public class BaseFixture
{
    public BaseFixture()
        => Faker = new Faker("pt_BR");

    protected Faker Faker { get; }

    public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var dbContext = new CodeflixCatalogDbContext(
                            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                            .UseInMemoryDatabase("integration-tests-db")
                            .Options
                        );
        if (preserveData is false)
            dbContext.Database.EnsureDeleted();

        return dbContext;
    }
    
    public bool GetRandomBoolean()
        => new Random().NextDouble() < 0.5;
}
