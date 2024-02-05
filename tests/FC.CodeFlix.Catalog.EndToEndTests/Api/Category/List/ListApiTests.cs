using System.Net;
using System.Text.Json;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.List.DTO;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Xunit.Abstractions;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.List;

[Collection(nameof(ListApiTestsFixture))]
public class ListApiTests
    : IDisposable
{
    private readonly ListApiTestsFixture _fixture;
    private readonly ITestOutputHelper _output;

    public ListApiTests(ListApiTestsFixture fixture, ITestOutputHelper output) 
        => (_fixture, _output) = (fixture, output);

    [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
    [Trait("EndToEnd/Api", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotalByDefault()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<CategoryOutputModel>>($"categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(15);
        output.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.PerPage.Should().Be(15);
        output.Meta.CurrentPage.Should().Be(1);
        foreach (var categoryOutput in output.Data!)
        {
            var exampleItem = exampleCategoriesList
                .FirstOrDefault(x => x.Id == categoryOutput.Id);
            exampleItem.Should().NotBeNull();
            categoryOutput.Name.Should().Be(exampleItem!.Name);
            categoryOutput.Description.Should().Be(exampleItem.Description);
            categoryOutput.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromSeconds(1));
            categoryOutput.IsActive.Should().Be(exampleItem.IsActive);   
        }
    }
    
    [Fact(DisplayName = nameof(ListCategories_WhenDoesntHaveCategories_ReturnEmptyList))]
    [Trait("EndToEnd/Api", "Category/List - Endpoints")]
    public async Task ListCategories_WhenDoesntHaveCategories_ReturnEmptyList()
    {
        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<CategoryOutputModel>>($"categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().HaveCount(default(int));
        output.Meta!.Total.Should().Be(default);
    }
    
    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/Api", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(perPage: 5, page: 1);

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<CategoryOutputModel>>($"categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().HaveCount(5);
        output.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        foreach (var categoryOutput in output.Data!)
        {
            var exampleItem = exampleCategoriesList
                .FirstOrDefault(x => x.Id == categoryOutput.Id);
            exampleItem.Should().NotBeNull();
            categoryOutput.Name.Should().Be(exampleItem!.Name);
            categoryOutput.Description.Should().Be(exampleItem.Description);
            categoryOutput.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromSeconds(1));
            categoryOutput.IsActive.Should().Be(exampleItem.IsActive);   
        }
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
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(perPage: perPage, page: page);

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<CategoryOutputModel>>($"categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().HaveCount(expectedQuantityItems);
        output.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        foreach (var categoryOutput in output.Data!)
        {
            var exampleItem = exampleCategoriesList
                .FirstOrDefault(x => x.Id == categoryOutput.Id);
            exampleItem.Should().NotBeNull();
            categoryOutput.Name.Should().Be(exampleItem!.Name);
            categoryOutput.Description.Should().Be(exampleItem.Description);
            categoryOutput.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromSeconds(1));
            categoryOutput.IsActive.Should().Be(exampleItem.IsActive);   
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
            "Actions", "Horror - Robots", "Horror", "Horror - Based On Real Facts",
            "Drama", "Sci-fi IA", "Sci-fi Space", "Sci-fi Robots", "Sci-fi Future"
        });
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(perPage: perPage, page: page, search: search);

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<CategoryOutputModel>>($"categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().HaveCount(expectedQuantityItemsReturned);
        output.Meta!.Total.Should().Be(expectedQuantityTotalItems);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        foreach (var categoryOutput in output.Data!)
        {
            var exampleItem = exampleCategoriesList
                .FirstOrDefault(x => x.Id == categoryOutput.Id);
            exampleItem.Should().NotBeNull();
            categoryOutput.Name.Should().Be(exampleItem!.Name);
            categoryOutput.Description.Should().Be(exampleItem.Description);
            categoryOutput.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromSeconds(1));
            categoryOutput.IsActive.Should().Be(exampleItem.IsActive);   
        }
    }

    [Theory(DisplayName = nameof(Search_WhenHasResults_ShouldReturnListAndTotalPaginatedOrdered))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "desc")]
    [InlineData("id", "asc")]
    [InlineData("", "asc")]
    public async Task Search_WhenHasResults_ShouldReturnListAndTotalPaginatedOrdered(
        string orderby,
        string order
    )
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var apiOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc; 
        var input = new ListCategoriesInput(dir:apiOrder, sort:orderby);

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<CategoryOutputModel>>($"categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().HaveCount(exampleCategoriesList.Count);
        output.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        var orderedList = _fixture.CloneOrderedCategoriesList(exampleCategoriesList,
            input.Sort, input.Dir);

        var count = 0;
        var expectedLog = orderedList.Select(x => $"{++count} {x.Name} {x.CreatedAt} {JsonSerializer.Serialize(x)}");
        count = 0;
        var outputLog = output.Data!.Select(x => $"{++count} {x.Name} {x.CreatedAt} {JsonSerializer.Serialize(x)}");
        _output.WriteLine("Expecteds...");
        _output.WriteLine(string.Join('\n', expectedLog));
        _output.WriteLine("Outputs...");
        _output.WriteLine(string.Join('\n', outputLog));
        
        for (var i = 0; i < exampleCategoriesList.Count; i++)
        {
            var expectedItem = orderedList[i];
            var outputItem = output.Data![i];

            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(expectedItem!.Name);
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().BeCloseTo(expectedItem.CreatedAt, TimeSpan.FromSeconds(1));
        }
    }
    
    [Theory(DisplayName = nameof(Search_WhenHasResults_ShouldReturnListAndTotalPaginatedOrdered))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData("createdAt", "desc")]
    [InlineData("createdAt", "asc")]
    public async Task Search_WhenHasResults_ShouldReturnListAndTotalPaginatedOrderedByCreatedAt(
        string orderby,
        string order
    )
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var apiOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc; 
        var input = new ListCategoriesInput(dir:apiOrder, sort:orderby);

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<CategoryOutputModel>>($"categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().HaveCount(exampleCategoriesList.Count);
        output.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        var orderedList = _fixture.CloneOrderedCategoriesList(exampleCategoriesList,
            input.Sort, input.Dir);
        DateTime? lastDateTime = null;
        foreach (var categoryOutput in output.Data!)
        {
            var exampleItem = orderedList
                .FirstOrDefault(x => x.Id == categoryOutput.Id);
            exampleItem.Should().NotBeNull();
            categoryOutput!.Name.Should().Be(exampleItem!.Name);
            categoryOutput!.Description.Should().Be(exampleItem.Description);
            categoryOutput!.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromSeconds(1));
            categoryOutput!.IsActive.Should().Be(exampleItem.IsActive);

            if (!lastDateTime.HasValue) continue;
            
            if (order is "asc")
                (categoryOutput.CreatedAt >= lastDateTime.Value).Should().BeTrue();
            else
                (categoryOutput.CreatedAt <= lastDateTime.Value).Should().BeTrue();
        }
    }

    public void Dispose()
        => _fixture.CleanDbContext();
}