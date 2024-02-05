using System.Net;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.DTO;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.Extensions;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.List;

[Collection(nameof(ListApiTestsFixture))]
public class ListApiTests : IDisposable
{
    private readonly ListApiTestsFixture _fixture;

    public ListApiTests(ListApiTestsFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("EndToEnd/Api", "Genre/ListGenres - Endpoints")]
    public async Task List()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new ListGenresInput
        {
            Page = 1,
            PerPage = exampleGenres.Count
        };

        var (response, output) =
            await _fixture.ApiClient.Get<TestApiResponseList<GenreOutputModel>>("genres", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.PerPage.Should().Be(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Data.Should().HaveCount(exampleGenres.Count);
        output.Data!.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.FirstOrDefault(exampleItem => exampleItem.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
        });
    }
    
    [Fact(DisplayName = nameof(ListWithRelations))]
    [Trait("EndToEnd/Api", "Genre/ListGenres - Endpoints")]
    public async Task ListWithRelations()
    {
        var exampleGenres = _fixture.GetExampleGenreList(15);
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = random.Next(2, exampleCategories.Count - 1);
                var selectedCategory = exampleCategories[selectedCategoryIndex];
                if(!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });
        var relations = new List<GenresCategories>();
        exampleGenres.ForEach(genre =>
        {
            genre.Categories.ToList()
                .ForEach(categoryId => relations.Add(new GenresCategories(categoryId, genre.Id)));
        });
        await _fixture.Persistence.InsertList(exampleGenres);
        await _fixture.CategoryPersistence.InsertList(exampleCategories);
        await _fixture.Persistence.InsertCategoriesGenresRelationsList(relations);

        var input = new ListGenresInput
        {
            Page = 1,
            PerPage = exampleGenres.Count
        };

        var (response, output) =
            await _fixture.ApiClient.Get<TestApiResponseList<GenreOutputModel>>("genres", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.PerPage.Should().Be(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Data.Should().HaveCount(exampleGenres.Count);
        output.Data!.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.FirstOrDefault(exampleItem => exampleItem.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            var relatedOutputCategories = outputItem.Categories.Select(x => x.Id).ToList();
            exampleItem.Categories.Should().BeEquivalentTo(relatedOutputCategories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                var exampleCategory = exampleCategories
                    .FirstOrDefault(exampleCategory => exampleCategory.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }
    
    [Fact(DisplayName = nameof(EmptyWhenThereAreNoItems))]
    [Trait("EndToEnd/Api", "Genre/ListGenres - Endpoints")]
    public async Task EmptyWhenThereAreNoItems()
    {
        var input = new ListGenresInput
        {
            Page = 1,
            PerPage = 15
        };

        var (response, output) =
            await _fixture.ApiClient.Get<TestApiResponseList<GenreOutputModel>>("genres", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(default);
        output.Data.Should().BeEmpty();
    }
    
    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/Api", "Genre/ListGenres - Endpoints")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task ListPaginated(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems)
    {
        var exampleGenres = _fixture.GetExampleGenreList(quantityToGenerate);
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new ListGenresInput
        {
            Page = page,
            PerPage = perPage
        };

        var (response, output) =
            await _fixture.ApiClient.Get<TestApiResponseList<GenreOutputModel>>("genres", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.PerPage.Should().Be(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.Total.Should().Be(quantityToGenerate);
        output.Data.Should().HaveCount(expectedQuantityItems);
        output.Data!.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.FirstOrDefault(exampleItem => exampleItem.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
        });
    }
    
    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/Api", "Genre/ListGenres - Endpoints")]
    [InlineData("Action",1, 5, 1, 1)]
    [InlineData("Horror",1, 5, 3, 3)]
    [InlineData("Horror",2, 5, 0, 3)]
    [InlineData("Sci-fi",1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems)
    {
        var exampleGenres = _fixture.GetExampleGenreListWithNames(new List<string>
        {
            "Actions","Horror - Robots", "Horror", "Horror - Based On Real Facts",
            "Drama", "Sci-fi IA", "Sci-fi Space", "Sci-fi Robots", "Sci-fi Future"
        });
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new ListGenresInput
        {
            Page = page,
            PerPage = perPage,
            Search = search
        };

        var (response, output) =
            await _fixture.ApiClient.Get<TestApiResponseList<GenreOutputModel>>("genres", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.PerPage.Should().Be(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);
        output.Data!.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.FirstOrDefault(exampleItem => exampleItem.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
        });
    }
    
    [Theory(DisplayName = nameof(ListOrdered))]
    [Trait("EndToEnd/Api", "Genre/ListGenres - Endpoints")]
    [InlineData("name","asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "desc")]
    [InlineData("id", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("", "asc")]
    public async Task ListOrdered(
        string orderby,
        string order)
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        await _fixture.Persistence.InsertList(exampleGenres);

        var orderEnum = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        
        var input = new ListGenresInput
        {
            Page = 1,
            PerPage = 10,
            Sort = orderby,
            Dir = orderEnum
        };

        var (response, output) =
            await _fixture.ApiClient.Get<TestApiResponseList<GenreOutputModel>>("genres", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.PerPage.Should().Be(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Data.Should().HaveCount(exampleGenres.Count);
        var expectedList = _fixture.CloneOrderedGenresList(exampleGenres, orderby, orderEnum);
        for (int i = 0; i < exampleGenres.Count; i++)
        {
            var outputItem = output.Data!.ElementAt(i);
            var exampleItem = exampleGenres.FirstOrDefault(exampleItem => exampleItem.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
        };
    }

    public void Dispose() 
        => _fixture.CleanDbContext();
}