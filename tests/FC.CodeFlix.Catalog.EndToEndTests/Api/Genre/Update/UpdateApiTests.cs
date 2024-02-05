using System.Net;
using FC.Codeflix.Catalog.Api.Models.Genre;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Update;

[Collection(nameof(UpdateApiTestsFixture))]
public class UpdateApiTests
{
    private readonly UpdateApiTestsFixture _fixture;

    public UpdateApiTests(UpdateApiTestsFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("EndToEnd/Api", "Genre/Update - Endpoints")]
    public async Task UpdateGenre()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());
        
        var (response, output) =
            await _fixture.ApiClient.Put<ApiResponse<GenreOutputModel>>($"genres/{targetGenre.Id}", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        
        var genreFromDb = await _fixture.Persistence.GetGenreById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);    
        genreFromDb.IsActive.Should().Be((bool)input.IsActive);
    }
    
    [Fact(DisplayName = nameof(ProblemDetailsWhenNotFound))]
    [Trait("EndToEnd/Api", "Genre/Update - Endpoints")]
    public async Task ProblemDetailsWhenNotFound()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        var targetGenre = Guid.NewGuid();
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());
        
        var (response, output) =
            await _fixture.ApiClient.Put<ProblemDetails>($"genres/{targetGenre}", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre '{targetGenre}' not found.");
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
    }
    
    [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
    [Trait("EndToEnd/Api", "Genre/Update - Endpoints")]
    public async Task UpdateGenreWithRelations()
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
        
        var relationsCount = random.Next(0, 3);
        var newRelations = new List<Guid>();
        for (var i = 0; i < relationsCount; i++)
        {
            var selectedCategoryIndex = random.Next(2, exampleCategories.Count - 1);
            var selectedCategory = exampleCategories[selectedCategoryIndex];
            if(!newRelations.Contains(selectedCategory.Id))
                newRelations.Add(selectedCategory.Id);
        }

        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean(), newRelations);
        
        var (response, output) =
            await _fixture.ApiClient.Put<ApiResponse<GenreOutputModel>>($"genres/{targetGenre.Id}", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        var relationsFromOutput = output.Data.Categories.Select(category => category.Id).ToList();
        relationsFromOutput.Should().BeEquivalentTo(newRelations);
        
        var genreFromDb = await _fixture.Persistence.GetGenreById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);    
        genreFromDb.IsActive.Should().Be((bool)input.IsActive);
        var relationsFromDb = await _fixture.Persistence.GetGenresCategoriesRelationsByGenreId(genreFromDb.Id);
        relationsFromDb.Should().NotBeEmpty();
        var relationsFromDbIds = relationsFromDb.Select(relation => relation.CategoryId).ToList();
        relationsFromDbIds.Should().BeEquivalentTo(newRelations);
    }
    
    [Fact(DisplayName = nameof(ErrorWhenInvalidRelation))]
    [Trait("EndToEnd/Api", "Genre/Update - Endpoints")]
    public async Task ErrorWhenInvalidRelation()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        var targetGenre = Guid.NewGuid();
        await _fixture.Persistence.InsertList(exampleGenres);

        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());
        
        var (response, output) =
            await _fixture.ApiClient.Put<ProblemDetails>($"genres/{targetGenre}", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {targetGenre}");
    }
    
    [Fact(DisplayName = nameof(PersistRelationsWhenNotPresentInInput))]
    [Trait("EndToEnd/Api", "Genre/Update - Endpoints")]
    public async Task PersistRelationsWhenNotPresentInInput()
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
        
        var input = new UpdateGenreApiInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());
        
        var (response, output) =
            await _fixture.ApiClient.Put<ApiResponse<GenreOutputModel>>($"genres/{targetGenre.Id}", input);
        
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        var relationsFromOutput = output.Data.Categories.Select(category => category.Id).ToList();
        relationsFromOutput.Should().BeEquivalentTo(targetGenre.Categories);
        
        var genreFromDb = await _fixture.Persistence.GetGenreById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);    
        genreFromDb.IsActive.Should().Be((bool)input.IsActive);
        var relationsFromDb = await _fixture.Persistence.GetGenresCategoriesRelationsByGenreId(genreFromDb.Id);
        relationsFromDb.Should().NotBeEmpty();
        var relationsFromDbIds = relationsFromDb.Select(relation => relation.CategoryId).ToList();
        relationsFromDbIds.Should().BeEquivalentTo(targetGenre.Categories);
    }
}