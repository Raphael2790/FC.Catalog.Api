using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.List;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.Interface;
using FC.CodeFlix.Catalog.Domain.Entities;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using Xunit;

namespace FC.CodeFlix.Catalog.UnitTests.Application.GenreUseCases.ListGenreUseCase;

[Collection(nameof(ListGenresTestsFixture))]
public class ListGenreTests
{
    private readonly ListGenresTestsFixture _fixture;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly IListGenres _sut;
    
    public ListGenreTests(ListGenresTestsFixture fixture)
    {
        _fixture = fixture;
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _sut = new ListGenres(_genreRepositoryMock.Object, _categoryRepositoryMock.Object);
    }
    
    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListGenre - Use Cases")]
    public async Task List()
    {
        var exampleGenreList = _fixture.GetExampleGenreList();
        var input = _fixture.GetExampleInput();
        var outputSearch = new SearchOutput<Genre>(
            currentPage: input.Page!,
            perPage: input.PerPage!,
            items: exampleGenreList,
            total: new Random().Next(50, 200)
        );
        _genreRepositoryMock
            .Setup(x => 
                x.Search(It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page
                        && searchInput.PerPage == input.PerPage
                        && searchInput.Search == input.Search
                        && searchInput.OrderBy == input.Sort
                        && searchInput.SearchOrder == input.Dir), 
                         It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputSearch);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputSearch.CurrentPage);
        output.PerPage.Should().Be(outputSearch.PerPage);
        output.Total.Should().Be(outputSearch.Total);
        output.Items.Should().HaveCount(outputSearch.Items.Count);
        ((List<GenreOutputModel>)output.Items).ForEach(item =>
        {
            var genre = outputSearch.Items.FirstOrDefault(x => x.Id == item.Id);
            item.Should().NotBeNull();
            item.Name.Should().Be(genre!.Name);
            item.IsActive.Should().Be(genre.IsActive);
            item.CreatedAt.Should().Be(genre.CreatedAt);
            item.Categories.Should().HaveCount(genre.Categories.Count);
            foreach (var categoryId in genre.Categories)
                item.Categories.Should().Contain(relation => relation.Id == categoryId);
        });
        
        _genreRepositoryMock
            .Verify(x => 
                    x.Search(It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page
                        && searchInput.PerPage == input.PerPage
                        && searchInput.Search == input.Search
                        && searchInput.OrderBy == input.Sort
                        && searchInput.SearchOrder == input.Dir), It.IsAny<CancellationToken>()), 
                Times.Once);

        var expectedIds = exampleGenreList.SelectMany(x => x.Categories).ToList();
        _categoryRepositoryMock
            .Verify(x => 
                x.GetListByIds(It.Is<List<Guid>>(parameter => 
                                        parameter.All(id => expectedIds.Contains(id) 
                                        && parameter.Count == expectedIds.Count)), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }
    
    [Fact(DisplayName = nameof(EmptyList))]
    [Trait("Application", "ListGenre - Use Cases")]
    public async Task EmptyList()
    {
        var input = _fixture.GetExampleInput();
        var outputSearch = new SearchOutput<Genre>(
            currentPage: input.Page!,
            perPage: input.PerPage!,
            items: new List<Genre>(),
            total: new Random().Next(50, 200)
        );
        _genreRepositoryMock
            .Setup(x => 
                x.Search(It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page
                        && searchInput.PerPage == input.PerPage
                        && searchInput.Search == input.Search
                        && searchInput.OrderBy == input.Sort
                        && searchInput.SearchOrder == input.Dir), 
                         It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputSearch);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputSearch.CurrentPage);
        output.PerPage.Should().Be(outputSearch.PerPage);
        output.Total.Should().Be(outputSearch.Total);
        output.Items.Should().HaveCount(outputSearch.Items.Count);

        _genreRepositoryMock
            .Verify(x => 
                    x.Search(It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page
                        && searchInput.PerPage == input.PerPage
                        && searchInput.Search == input.Search
                        && searchInput.OrderBy == input.Sort
                        && searchInput.SearchOrder == input.Dir), It.IsAny<CancellationToken>()), 
                Times.Once);
    }
    
    [Fact(DisplayName = nameof(ListUsingInputDefaultValues))]
    [Trait("Application", "ListGenre - Use Cases")]
    public async Task ListUsingInputDefaultValues()
    {
        var input = new ListGenresInput();
        var outputSearch = new SearchOutput<Genre>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<Genre>(),
            total: 0
        );
        _genreRepositoryMock
            .Setup(x => 
                x.Search(It.IsAny<SearchInput>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputSearch);

        _ = await _sut.Handle(input, CancellationToken.None);

        _genreRepositoryMock
            .Verify(x => 
                    x.Search(It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page
                        && searchInput.PerPage == input.PerPage
                        && searchInput.Search == input.Search
                        && searchInput.OrderBy == input.Sort
                        && searchInput.SearchOrder == input.Dir), It.IsAny<CancellationToken>()), 
                Times.Once);
    }
}