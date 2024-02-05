using System.Net;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Update;

[Collection(nameof(UpdateApiTestsFixture))]
public class UpdateApiTests
    : IDisposable
{
    private readonly UpdateApiTestsFixture _fixture;

    public UpdateApiTests(UpdateApiTestsFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("EndToEnd/Api", "Category/Update - Endpoints")]
    public async Task UpdateCategory()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = _fixture.GetValidInput();

        var (response, output) = await _fixture.ApiClient
            .Put<TestApiResponse<CategoryOutputModel>>($"categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        output.Data.Id.Should().Be(exampleCategory.Id);

        var dbCategory = await _fixture.Persistence.GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive);
    }
    
    [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("EndToEnd/Api", "Category/Update - Endpoints")]
    public async Task UpdateCategoryOnlyName()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = _fixture.GetValidInputOnlyNameUpdated();

        var (response, output) = await _fixture.ApiClient
            .Put<TestApiResponse<CategoryOutputModel>>($"categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(exampleCategory.Description);
        output.Data.IsActive.Should().Be(exampleCategory.IsActive!);
        output.Data.Id.Should().Be(exampleCategory.Id);

        var dbCategory = await _fixture.Persistence.GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
    }
    
    [Fact(DisplayName = nameof(UpdateCategoryNameAndDescription))]
    [Trait("EndToEnd/Api", "Category/Update - Endpoints")]
    public async Task UpdateCategoryNameAndDescription()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = _fixture.GetValidInputNameAndDescriptionUpdated();

        var (response, output) = await _fixture.ApiClient
            .Put<TestApiResponse<CategoryOutputModel>>($"categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be(exampleCategory.IsActive!);
        output.Data.Id.Should().Be(exampleCategory.Id);

        var dbCategory = await _fixture.Persistence.GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
    }
    
    [Fact(DisplayName = nameof(ErrorWhenCategoryNotFound))]
    [Trait("EndToEnd/Api", "Category/Update - Endpoints")]
    public async Task ErrorWhenCategoryNotFound()
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = Guid.NewGuid();
        var input = _fixture.GetValidInputNameAndDescriptionUpdated();

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"categories/{exampleCategory}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Category '{exampleCategory}' not found.");
        output.Title.Should().Be("Not Found");
        output.Status.Should().Be(StatusCodes.Status404NotFound);
    }
    
    [Theory(DisplayName = nameof(ErrorWhenCategoryCantBeInstantiated))]
    [Trait("EndToEnd/Api", "Category/Update - Endpoints")]
    [MemberData(nameof(UpdateApiTestsDataGenerator.GetInvalidInputs), 
        MemberType = typeof(UpdateApiTestsDataGenerator))]
    public async Task ErrorWhenCategoryCantBeInstantiated(
        UpdateCategoryInput input,
        string expectedDetail
    )
    {
        var exampleCategoriesList = _fixture.GetExamplesCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        input.Id = exampleCategory.Id;

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors ocurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);
    }
    
    public void Dispose()
        => _fixture.CleanDbContext();
}