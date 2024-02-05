using System.Net;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Create;

[Collection(nameof(CreateApiTestsFixture))]
public class CreateApiTests
{
    private readonly CreateApiTestsFixture _fixture;

    public CreateApiTests(CreateApiTestsFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("EndToEnd/Api", "Genre/Create - Endpoints")]
    public async Task CreateGenre()
    {
        var apiInput = new CreateGenreInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean());

        var (response, output) =
            await _fixture.ApiClient.Post<ApiResponse<GenreOutputModel>>("genres", apiInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(apiInput.Name);
        output.Data.IsActive.Should().Be(apiInput.IsActive);
        output.Data.Categories.Should().BeEmpty();
        
        var genreFromDb = await _fixture.Persistence.GetGenreById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(apiInput.Name);    
        genreFromDb.IsActive.Should().Be(apiInput.IsActive);
    }
    
    [Fact(DisplayName = nameof(CreateGenreWithRelations))]
    [Trait("EndToEnd/Api", "Genre/Create - Endpoints")]
    public async Task CreateGenreWithRelations()
    {
        var categories = _fixture.GetExampleCategoriesList();
        await _fixture.CategoryPersistence.InsertList(categories);
        var relatedCategories = categories.Skip(3).Take(3).Select(x => x.Id).ToList();
        var apiInput = new CreateGenreInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean(), relatedCategories);

        var (response, output) =
            await _fixture.ApiClient.Post<ApiResponse<GenreOutputModel>>("genres", apiInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(apiInput.Name);
        output.Data.IsActive.Should().Be(apiInput.IsActive);
        output.Data.Categories.Should().HaveCount(relatedCategories.Count);
        var categoryIds = output.Data.Categories.Select(x => x.Id).ToList();
        categoryIds.Should().BeEquivalentTo(relatedCategories);
        
        var genreFromDb = await _fixture.Persistence.GetGenreById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(apiInput.Name);    
        genreFromDb.IsActive.Should().Be(apiInput.IsActive);
        var relationsFromDb = await _fixture.Persistence.GetGenresCategoriesRelationsByGenreId(output.Data.Id);
        relationsFromDb.Should().HaveCount(relatedCategories.Count);
        var categoryIdsFromDb = relationsFromDb.Select(x => x.CategoryId).ToList();
        categoryIdsFromDb.Should().BeEquivalentTo(relatedCategories);
    }
    
    [Fact(DisplayName = nameof(ErrorWithInvalidRelations))]
    [Trait("EndToEnd/Api", "Genre/Create - Endpoints")]
    public async Task ErrorWithInvalidRelations()
    {
        var categories = _fixture.GetExampleCategoriesList();
        await _fixture.CategoryPersistence.InsertList(categories);
        var relatedCategories = categories.Skip(3).Take(3).Select(x => x.Id).ToList();
        var invalidCategoryId = Guid.NewGuid();
        relatedCategories.Add(invalidCategoryId);
        var apiInput = new CreateGenreInput(_fixture.GetValidGenreName(), _fixture.GetRandomBoolean(), relatedCategories);

        var (response, output) =
            await _fixture.ApiClient.Post<ProblemDetails>("genres", apiInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {invalidCategoryId}");
    }
}