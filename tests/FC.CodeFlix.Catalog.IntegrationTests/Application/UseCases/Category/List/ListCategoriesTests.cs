using FC.CodeFlix.Catalog.Application.UseCases.Category.List;
using FC.CodeFlix.Catalog.Application.UseCases.Category.List.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.List.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.List;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTests
{
    private readonly ListCategoriesTestFixture _fixture;
    private readonly CodeflixCatalogDbContext _context;
    private readonly IListCategories _sut;

    public ListCategoriesTests(ListCategoriesTestFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext();
        ICategoryRepository categoryRepository = new CategoryRepository(_context);
        _sut = new ListCategories(categoryRepository);
    }

    [Fact(DisplayName = nameof(Search_WhenHasResults_ShouldReturnListAndTotal))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    public async Task Search_WhenHasResults_ShouldReturnListAndTotal()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(15);
        await _context.AddRangeAsync(exampleCategoriesList);
        await _context.SaveChangesAsync(CancellationToken.None);
        var searchInput = new ListCategoriesInput(1, 20);

        var output = await _sut.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList
                .Find(x => x.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(Search_WhenHasntResults_ShouldReturnEmptyListAndZeroTotal))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    public async Task Search_WhenHasntResults_ShouldReturnEmptyListAndZeroTotal()
    {
        var searchInput = new ListCategoriesInput(1, 20);

        var output = await _sut.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(default);
        output.Items.Should().HaveCount(default(int));
    }

    [Theory(DisplayName = nameof(Search_WhenHasResults_ShouldReturnListAndTotalPaginated))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task Search_WhenHasResults_ShouldReturnListAndTotalPaginated(
            int quantityCategoriesToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
        )
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(quantityCategoriesToGenerate);
        await _context.AddRangeAsync(exampleCategoriesList);
        await _context.SaveChangesAsync(CancellationToken.None);
        var searchInput = new ListCategoriesInput(page, perPage);

        var output = await _sut.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList
                .Find(x => x.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(Search_WhenHasResults_ShouldReturnListAndTotalPaginatedWithSearch))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task Search_WhenHasResults_ShouldReturnListAndTotalPaginatedWithSearch(
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
        )
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesListWithName(new List<string>
        {
            "Actions","Horror - Robots", "Horror", "Horror - Based On Real Facts",
            "Drama", "Sci-fi IA", "Sci-fi Space", "Sci-fi Robots", "Sci-fi Future"
        });
        await _context.AddRangeAsync(exampleCategoriesList);
        await _context.SaveChangesAsync(CancellationToken.None);
        var searchInput = new ListCategoriesInput(page, perPage, search);

        var output = await _sut.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList
                .Find(x => x.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(exampleItem!.Name);
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(Search_WhenHasResults_ShouldReturnListAndTotalPaginatedOrdered))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "desc")]
    [InlineData("id", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("", "asc")]
    public async Task Search_WhenHasResults_ShouldReturnListAndTotalPaginatedOrdered(
            string orderby,
            string order
        )
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(15);
        await _context.AddRangeAsync(exampleCategoriesList);
        await _context.SaveChangesAsync(CancellationToken.None);
        var inputOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new ListCategoriesInput(1, 20, "", orderby, inputOrder);

        var output = await _sut.Handle(searchInput, CancellationToken.None);

        var orderedList = _fixture.CloneOrderedCategoriesList(exampleCategoriesList,
            searchInput.Sort!, (SearchOrder)searchInput.Dir!);
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        for (int i = 0; i < exampleCategoriesList.Count; i++)
        {
            var expectedItem = orderedList[i];
            var outputItem = output.Items[i];

            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(expectedItem!.Name);
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}
