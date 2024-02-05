using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FC.CodeFlix.Catalog.IntegrationTests.Base;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.Repositories.Category;

[Collection(nameof(CategoryRepositoryTestsFixture))]
public class CategoryRepositoryTests
{
    private readonly CategoryRepositoryTestsFixture _fixture;
    private readonly CodeflixCatalogDbContext _codeflixCatalogDbContext;
    private readonly CategoryRepository _sut;

    public CategoryRepositoryTests(CategoryRepositoryTestsFixture fixture)
    {
        _fixture = fixture;
        _codeflixCatalogDbContext = _fixture.CreateDbContext();
        _sut = new CategoryRepository(_codeflixCatalogDbContext);
    }
    
    [Fact(DisplayName = nameof(Insert_WhenCategoryIsOK_ShouldSaveInDatabase))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    public async Task Insert_WhenCategoryIsOK_ShouldSaveInDatabase()
    {
        var exampleCategory = _fixture.GetValidCategory();

        await _sut.Insert(exampleCategory, CancellationToken.None);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);

        var savedCategory = await _fixture.CreateDbContext(true)
            .Categories.FindAsync(exampleCategory.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be(exampleCategory.Name);
        savedCategory.Description.Should().Be(exampleCategory.Description);
        savedCategory.IsActive.Should().Be(exampleCategory.IsActive);
        savedCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Get_WhenCategoryExists_ShouldFindInDatabase))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    public async Task Get_WhenCategoryExists_ShouldFindInDatabase()
    {
        var exampleCategory = _fixture.GetValidCategory();
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await _codeflixCatalogDbContext.AddRangeAsync(exampleCategoriesList);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);

        var savedCategory = await _sut.Get(exampleCategory.Id, CancellationToken.None);

        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be(exampleCategory.Name);
        savedCategory.Id.Should().Be(exampleCategory.Id);
        savedCategory.Description.Should().Be(exampleCategory.Description);
        savedCategory.IsActive.Should().Be(exampleCategory.IsActive);
        savedCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Get_WhenCategoryDoesntExists_ShouldThrowNotFoundException))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    public async Task Get_WhenCategoryDoesntExists_ShouldThrowNotFoundException()
    {
        var exampleId = Guid.NewGuid();
        await _codeflixCatalogDbContext.AddRangeAsync(_fixture.GetExamplesCategoriesList(15));

        var task = async () => await _sut.Get(exampleId, CancellationToken.None);

        await task.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Category '{exampleId}' not found.");
    }

    [Fact(DisplayName = nameof(Update_WhenCategoryUpdated_ShouldUpdateInDatabase))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    public async Task Update_WhenCategoryUpdated_ShouldUpdateInDatabase()
    {
        var exampleCategory = _fixture.GetValidCategory();
        var newCategoryValues = _fixture.GetValidCategory();
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await _codeflixCatalogDbContext.AddRangeAsync(exampleCategoriesList);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);

        exampleCategory.Update(newCategoryValues.Name, newCategoryValues.Description);
        await _sut.Update(exampleCategory, CancellationToken.None);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);

        var savedCategory = await _fixture.CreateDbContext(true).Categories.FindAsync(exampleCategory.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be(exampleCategory.Name);
        savedCategory.Id.Should().Be(exampleCategory.Id);
        savedCategory.Description.Should().Be(exampleCategory.Description);
        savedCategory.IsActive.Should().Be(exampleCategory.IsActive);
        savedCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Delete_WhenCategoryDeleted_ShouldNotExistsInDatabase))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    public async Task Delete_WhenCategoryDeleted_ShouldNotExistsInDatabase()
    {
        var exampleCategory = _fixture.GetValidCategory();
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await _codeflixCatalogDbContext.AddRangeAsync(exampleCategoriesList);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);

        await _sut.Delete(exampleCategory, CancellationToken.None);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);

        var savedCategory = await _fixture.CreateDbContext().Categories.FindAsync(exampleCategory.Id);
        savedCategory.Should().BeNull();
    }

    [Fact(DisplayName = nameof(Search_WhenHasResults_ShouldReturnListAndTotal))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    public async Task Search_WhenHasResults_ShouldReturnListAndTotal()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(15);
        await _codeflixCatalogDbContext.AddRangeAsync(exampleCategoriesList);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);
        var searchInput = new SearchInput(1,20,"","",SearchOrder.Asc);

        var output = await _sut.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
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
    
    [Fact(DisplayName = nameof(Search_WhenHasResults_ShouldReturnListById))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    public async Task Search_WhenHasResults_ShouldReturnListById()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(15);
        var categoriesIdsToGet = Enumerable.Range(1, 3).Select(_ =>
        {
            var indexToGet = new Random().Next(0, exampleCategoriesList.Count - 1);
            return exampleCategoriesList[indexToGet];
        }).Select(category => category.Id).Distinct().ToList();
        await _codeflixCatalogDbContext.AddRangeAsync(exampleCategoriesList);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);
        
        IReadOnlyList<Domain.Entities.Category> categoriesList = await _sut.GetListByIds(categoriesIdsToGet, CancellationToken.None);

        categoriesList.Should().NotBeNull();
        categoriesList.Should().HaveCount(categoriesIdsToGet.Count);
        foreach (var outputItem in categoriesList)
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
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    public async Task Search_WhenHasntResults_ShouldReturnEmptyListAndZeroTotal()
    {
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var output = await _sut.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(default);
        output.Items.Should().HaveCount(default(int));
    }

    [Theory(DisplayName = nameof(Search_WhenHasResults_ShouldReturnListAndTotalPaginated))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task Search_WhenHasResults_ShouldReturnListAndTotalPaginated(
            int quantityCategoriesToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
        )
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(quantityCategoriesToGenerate);
        await _codeflixCatalogDbContext.AddRangeAsync(exampleCategoriesList);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        var output = await _sut.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(quantityCategoriesToGenerate);
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
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    [InlineData("Action",1, 5, 1, 1)]
    [InlineData("Horror",1, 5, 3, 3)]
    [InlineData("Horror",2, 5, 0, 3)]
    [InlineData("Sci-fi",1, 5, 4, 4)]
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
        await _codeflixCatalogDbContext.AddRangeAsync(exampleCategoriesList);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        var output = await _sut.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
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
    [Trait("Integration/Infra.Data", "CategoryRepository - Persistence")]
    [InlineData("name","asc")]
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
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList();
        await _codeflixCatalogDbContext.AddRangeAsync(exampleCategoriesList);
        await _codeflixCatalogDbContext.SaveChangesAsync(CancellationToken.None);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderby, searchOrder);

        var output = await _sut.Search(searchInput, CancellationToken.None);

        var expectedList = _fixture.CloneOrderedCategoriesList(exampleCategoriesList, orderby, searchOrder);
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        for (int i = 0; i < exampleCategoriesList.Count; i++)
        {
            var expectedItem = expectedList[i];
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
