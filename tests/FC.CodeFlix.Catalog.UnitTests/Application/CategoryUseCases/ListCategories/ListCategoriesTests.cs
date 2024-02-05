using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.List.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.List.Interfaces;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.List;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CategoryUseCases.ListCategories;

[Collection(nameof(ListCategoriesTestsFixture))]
public class ListCategoriesTests
{
    private readonly ListCategoriesTestsFixture _fixture;
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly IListCategories _sut;

    public ListCategoriesTests(ListCategoriesTestsFixture fixture)
    {
        _fixture = fixture;
        _repositoryMock = new();
        _sut = new UseCase.ListCategories(_repositoryMock.Object);
    }

    [Fact(DisplayName = nameof(List_WhenSearchInputIsValid_ShouldReturnOutput))]
    [Trait("Application", "ListCategories - Use Cases")]
    public async Task List_WhenSearchInputIsValid_ShouldReturnOutput()
    {
        var categoriesList = _fixture.GetValidCategoriesList();
        var input = _fixture.GetExampleInput();
        var outputSearch = new SearchOutput<Category>(
            currentPage: (int)input.Page!,
            perPage: (int)input.PerPage!,
            items: categoriesList,
            total: new Random().Next(50, 200)
        );
        _repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputSearch);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputSearch.CurrentPage);
        output.PerPage.Should().Be(outputSearch.PerPage);
        output.Total.Should().Be(outputSearch.Total);
        output.Items.Should().HaveCount(outputSearch.Items.Count);
        ((List<CategoryOutputModel>)output.Items).ForEach(item =>
        {
            var category = outputSearch.Items.FirstOrDefault(x => x.Id == item.Id);
            item.Should().NotBeNull();
            item.Name.Should().Be(category!.Name);
            item.Description.Should().Be(category.Description);
            item.IsActive.Should().Be(category.IsActive);
            item.CreatedAt.Should().Be(category.CreatedAt);
        });

        _repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(List_WhenSearchInputIsValidAndEmpty_ShouldReturnOutput))]
    [Trait("Application", "ListCategories - Use Cases")]
    public async Task List_WhenSearchInputIsValidAndEmpty_ShouldReturnOutput()
    {
        var input = _fixture.GetExampleInput();
        var outputSearch = new SearchOutput<Category>(
            currentPage: (int)input.Page!,
            perPage: (int)input.PerPage!,
            items: new List<Category>().AsReadOnly(),
            total: 0
        );
        _repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputSearch);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputSearch.CurrentPage);
        output.PerPage.Should().Be(outputSearch.PerPage);
        output.Total.Should().Be(outputSearch.Total);
        output.Items.Should().HaveCount(0);

        _repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory(DisplayName = nameof(List_WhenSearchInputDoesntHaveAllParameters_ShouldReturnOutput))]
    [MemberData(nameof(ListCategoriesTestsGenerator.GetInputsWithoutAllParameters),
        parameters: 12, MemberType = typeof(ListCategoriesTestsGenerator))
    ]
    [Trait("Application", "ListCategories - Use Cases")]
    public async Task List_WhenSearchInputDoesntHaveAllParameters_ShouldReturnOutput(ListCategoriesInput input)
    {
        var categoriesList = _fixture.GetValidCategoriesList();
        var outputSearch = new SearchOutput<Category>(
            currentPage: (int)input.Page!,
            perPage: (int)input.PerPage!,
            items: categoriesList,
            total: new Random().Next(50, 200)
        );
        _repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputSearch);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputSearch.CurrentPage);
        output.PerPage.Should().Be(outputSearch.PerPage);
        output.Total.Should().Be(outputSearch.Total);
        output.Items.Should().HaveCount(outputSearch.Items.Count);
        ((List<CategoryOutputModel>)output.Items).ForEach(item =>
        {
            var category = outputSearch.Items.FirstOrDefault(x => x.Id == item.Id);
            item.Should().NotBeNull();
            item.Name.Should().Be(category!.Name);
            item.Description.Should().Be(category.Description);
            item.IsActive.Should().Be(category.IsActive);
            item.CreatedAt.Should().Be(category.CreatedAt);
        });

        _repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.SearchOrder == input.Dir
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
