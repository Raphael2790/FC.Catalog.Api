using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.EndToEndTests.Models;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Get;

[Collection(nameof(GetApiTestsFixture))]
public class GetApiTests
    : IDisposable
{
    private readonly GetApiTestsFixture _fixture;

    public GetApiTests(GetApiTestsFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("EndToEnd/Api", "Category/Create - Endpoints")]
    public async Task GetCategory()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture.ApiClient
                                            .Get<TestApiResponse<CategoryOutputModel>>($"categories/{exampleCategory.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(exampleCategory.Name);
        output.Data.Description.Should().Be(exampleCategory.Description);
        output.Data.CreatedAt.Should().BeCloseTo(exampleCategory.CreatedAt, TimeSpan.FromSeconds(1));
        output.Data.IsActive.Should().Be(exampleCategory.IsActive);
    }

    [Fact(DisplayName = nameof(GetCategory_WhenNotFound_ReturnsError))]
    [Trait("EndToEnd/Api", "Category/Create - Endpoints")]
    public async Task GetCategory_WhenNotFound_ReturnsError()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var id = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient
                                            .Get<ProblemDetails>($"categories/{id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category '{id}' not found.");
        output.Type.Should().Be("NotFound");
    }
    
    public void Dispose()
        => _fixture.CleanDbContext();
}
