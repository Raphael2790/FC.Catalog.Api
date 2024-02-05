using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.EndToEndTests.Models;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Create;

[Collection(nameof(CreateApiTestsFixture))]
public class CreateApiTests
    : IDisposable
{
    private readonly CreateApiTestsFixture _fixture;

    public CreateApiTests(CreateApiTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("EndToEnd/Api", "Category/Create - Endpoints")]
    public async Task CreateCategory()
    {
        var input = _fixture.GetExampleInput();

        var (response, output) = await _fixture.ApiClient
                                     .Post<TestApiResponse<CategoryOutputModel>>(
                                     "/categories",
                                     input
                                     );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be(input.IsActive);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);

        var dbCategory = await _fixture.Persistence.GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.Id.Should().NotBeEmpty();
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(ThrowExceptionWhenCantInstantiate))]
    [Trait("EndToEnd/Api", "Category/Create - Endpoints")]
    [MemberData(nameof(CreateCategoryApiTestsGenerator.GetInvalidInputs), MemberType = typeof(CreateCategoryApiTestsGenerator))]
    public async Task ThrowExceptionWhenCantInstantiate(CreateCategoryInput input, string expectedProblemDetail)
    {
        var (response, output) = await _fixture.ApiClient
                                     .Post<ProblemDetails>(
                                     "/categories",
                                     input
                                     );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors ocurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be(expectedProblemDetail);
    }

    public void Dispose()
        => _fixture.CleanDbContext();
}
