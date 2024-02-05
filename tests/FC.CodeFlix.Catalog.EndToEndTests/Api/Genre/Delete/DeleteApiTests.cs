using System.Net;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Delete;

[Collection(nameof(DeleteApiTestsFixture))]
public class DeleteApiTests
{
    private readonly DeleteApiTestsFixture _fixture;

    public DeleteApiTests(DeleteApiTestsFixture fixture) 
        => _fixture = fixture;
    
    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("EndToEnd/Api", "Genre/Delete - Endpoints")]
    public async Task DeleteGenre()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);

        var (response, output) =
            await _fixture.ApiClient.Delete<object>($"genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        
        var genreDb = await _fixture.Persistence.GetGenreById(targetGenre.Id);
        genreDb.Should().BeNull();
    }
    
    [Fact(DisplayName = nameof(WhenNotFound404))]
    [Trait("EndToEnd/Api", "Genre/Delete - Endpoints")]
    public async Task WhenNotFound404()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        var targetGenre = Guid.NewGuid();
        await _fixture.Persistence.InsertList(exampleGenres);

        var (response, output) =
            await _fixture.ApiClient.Delete<ProblemDetails>($"genres/{targetGenre}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre '{targetGenre}' not found.");
    }
    
    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("EndToEnd/Api", "Genre/Delete - Endpoints")]
    public async Task DeleteGenreWithRelations()
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
            await _fixture.ApiClient.Delete<object>($"genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        
        var genreDb = await _fixture.Persistence.GetGenreById(targetGenre.Id);
        genreDb.Should().BeNull();
        var relationsDb = await _fixture.Persistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        relationsDb.Should().BeEmpty();
    }
}