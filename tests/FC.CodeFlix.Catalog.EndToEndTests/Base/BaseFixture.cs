using Bogus;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base;

public class BaseFixture
{
    protected Faker Faker { get; private set; }

    public HttpClient HttpClient { get; private set; }

    public CustomWebApplicationFactory<Program> WebAppFactory { get; private set; }

    public ApiClient ApiClient { get; set; }
    private readonly string _connectionString;


    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient =  WebAppFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);
        var configuration = WebAppFactory.Services.GetService<IConfiguration>();
        ArgumentNullException.ThrowIfNull(configuration);
        _connectionString = configuration.GetConnectionString("CatalogDb");
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var dbContext = new CodeflixCatalogDbContext(
                            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                            .UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString))
                            .Options
                        );

        return dbContext;
    }

    public void CleanDbContext()
    {
        var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
