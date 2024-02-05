using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.UoW;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.UoW;

[Collection(nameof(UnitOfWorkTestsFixture))]
public class UnitOfWorkTests
{
    private readonly UnitOfWorkTestsFixture _fixture;
    private readonly IUnitOfWork _sut;
    private readonly CodeflixCatalogDbContext _context;

    public UnitOfWorkTests(UnitOfWorkTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext();
        _sut = new UnitOfWork(_context);
    } 

    [Fact(DisplayName = "")]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Commit_WhenInvoked_ShouldPersistData()
    {
        var exampleCategories = _fixture.GetExamplesCategoriesList();
        await _context.Categories.AddRangeAsync(exampleCategories);

        await _sut.Commit(CancellationToken.None);

        var newDbContext = _fixture.CreateDbContext(true);
        var savedCategories = await newDbContext.Categories
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync();
        savedCategories.Should().HaveCount(exampleCategories.Count);
    }

    [Fact(DisplayName = "")]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Rollback_WhenInvoked_ShouldPersistData()
    {
        var task = async () => await _sut.Rollback(CancellationToken.None);

        await task.Should().NotThrowAsync();
    }
}
