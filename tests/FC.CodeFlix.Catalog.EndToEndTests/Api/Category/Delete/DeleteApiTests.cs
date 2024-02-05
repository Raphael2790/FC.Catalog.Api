using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Delete;

[Collection(nameof(DeleteApiTestsFixture))]
public class DeleteApiTests
    : IDisposable
{
    private readonly DeleteApiTestsFixture _fixture;

    public DeleteApiTests(DeleteApiTestsFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("EndToEnd/Api", "Category/Delete - Endpoints")]
    public async Task DeleteCategory()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture.ApiClient
                                            .Delete<object>($"categories/{exampleCategory.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var dbCategory = await _fixture.Persistence.GetById(exampleCategory.Id);
        dbCategory.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("EndToEnd/Api", "Category/Delete - Endpoints")]
    public async Task DeleteCategory_WhenNotFound_ReturnsError()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient
                                            .Delete<ProblemDetails>($"categories/{exampleCategory}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Detail.Should().Be($"Category '{exampleCategory}' not found.");
        output.Type.Should().Be("NotFound");
        output.Title.Should().Be("Not Found");
        output.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    public void Dispose()
        => _fixture.CleanDbContext();
}
