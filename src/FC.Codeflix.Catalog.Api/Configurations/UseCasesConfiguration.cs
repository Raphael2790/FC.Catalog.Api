using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.Infra.Data.UoW;
using MediatR;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
        => services.AddMediatR(typeof(CreateCategory))
                   .AddRepositories();

    private static IServiceCollection AddRepositories(this IServiceCollection services)
        => services.AddTransient<ICategoryRepository, CategoryRepository>()
                   .AddTransient<IUnitOfWork, UnitOfWork>()
                   .AddTransient<IGenreRepository, GenreRepository>(); 
}
