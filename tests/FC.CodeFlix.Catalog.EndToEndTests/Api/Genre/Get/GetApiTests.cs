using System.Net;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Get;

[Collection(nameof(GetApiTestsFixture))]
public class GetApiTests
{
    private readonly GetApiTestsFixture _fixture;

    public GetApiTests(GetApiTestsFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("EndToEnd/Api", "Genre/Get - Endpoints")]
    public async Task GetGenre()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);

        var (response, output) =
            await _fixture.ApiClient.Get<ApiResponse<GenreOutputModel>>($"genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
    }
    
    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/Api", "Genre/Get - Endpoints")]
    public async Task NotFound()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        var targetGenre = Guid.NewGuid();
        await _fixture.Persistence.InsertList(exampleGenres);

        var (response, output) =
            await _fixture.ApiClient.Get<ProblemDetails>($"genres/{targetGenre}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre '{targetGenre}' not found.");
    }
    
    [Fact(DisplayName = nameof(GetGenreWithRelations))]
    [Trait("EndToEnd/Api", "Genre/Get - Endpoints")]
    public async Task GetGenreWithRelations()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        var targetGenre = exampleGenres[5];
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

        var (response, output) =
            await _fixture.ApiClient.Get<ApiResponse<GenreOutputModel>>($"genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
        output.Data.Categories.Should().NotBeNullOrEmpty();
        output.Data.Categories.Count.Should().Be(targetGenre.Categories.Count);
        output.Data.Categories.Select(relation => relation.Id).Should().BeEquivalentTo(targetGenre.Categories);
    }
}