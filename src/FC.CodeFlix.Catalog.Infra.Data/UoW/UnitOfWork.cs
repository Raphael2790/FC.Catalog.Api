using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Infra.Data.Context;

namespace FC.CodeFlix.Catalog.Infra.Data.UoW;

public class UnitOfWork : IUnitOfWork
{
    private readonly CodeflixCatalogDbContext _context;

    public UnitOfWork(CodeflixCatalogDbContext context) 
        => _context = context;

    public async Task Commit(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);

    public Task Rollback(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
