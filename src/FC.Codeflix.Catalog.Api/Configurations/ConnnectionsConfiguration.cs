using FC.CodeFlix.Catalog.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ConnnectionsConfiguration
{
    public static IServiceCollection AddAppConnections(this IServiceCollection services, IConfiguration configuration)
        => services.AddDbConnections(configuration);

    public static IServiceCollection AddDbConnections(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDb");
        return services.AddDbContext<CodeflixCatalogDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
    }
}
