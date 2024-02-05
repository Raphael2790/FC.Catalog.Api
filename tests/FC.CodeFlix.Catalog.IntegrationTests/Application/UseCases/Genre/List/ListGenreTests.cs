using FC.CodeFlix.Catalog.Application.UseCases.Genre.List;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.List;

[Collection(nameof(ListGenreTestsFixture))]
public class ListGenreTests
{
    private readonly ListGenreTestsFixture _fixture;
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly CodeflixCatalogDbContext _context;
    private readonly IListGenres _sut;

    public ListGenreTests(ListGenreTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext(true);
        _genreRepository = new GenreRepository(_context);
        _categoryRepository = new CategoryRepository(_context);
        _sut = new ListGenres(_genreRepository, _categoryRepository);
    }

    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("Integration/Application ", "ListGenres - Use Cases")]
    public async Task ListGenres()
    {
        var exampleGenres = _fixture.GetExampleGenreList();
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.SaveChangesAsync();

        var input = new ListGenresInput(1, 20);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(item => item.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
        });
    }
    
    [Fact(DisplayName = nameof(ListGenresReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Application ", "ListGenres - Use Cases")]
    public async Task ListGenresReturnsEmptyWhenPersistenceIsEmpty()
    {
        var input = new ListGenresInput(1, 20);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(default);
        output.Items.Should().HaveCount(default(int));
    }
    
    [Fact(DisplayName = nameof(ListGenresVerifyRelations))]
    [Trait("Integration/Application ", "ListGenres - Use Cases")]
    public async Task ListGenresVerifyRelations()
    {
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories();
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
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
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeContext.GenresCategories.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        var input = new ListGenresInput(1, 20);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(item => item.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            var outputItemCategoryIds = outputItem.Categories.Select(item => item.Id);
            outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                var exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }
    
    [Theory(DisplayName = nameof(ListGenresPaginated))]
    [Trait("Integration/Application ", "ListGenres - Use Cases")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task ListGenresPaginated(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
        )
    {
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories(quantityToGenerate);
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
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
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeContext.GenresCategories.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        var input = new ListGenresInput(page, perPage);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(item => item.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            var outputItemCategoryIds = outputItem.Categories.Select(item => item.Id);
            outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                var exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }
    
    [Theory(DisplayName = nameof(ListGenreByText))]
    [Trait("Integration/Application ", "ListGenres - Use Cases")]
    [InlineData("Action",1, 5, 1, 1)]
    [InlineData("Horror",1, 5, 3, 3)]
    [InlineData("Horror",2, 5, 0, 3)]
    [InlineData("Sci-fi",1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task ListGenreByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
        )
    {
        var exampleGenres = _fixture.GetExampleGenreListWithNames(new List<string>
        {
            "Actions","Horror - Robots", "Horror", "Horror - Based On Real Facts",
            "Drama", "Sci-fi IA", "Sci-fi Space", "Sci-fi Robots", "Sci-fi Future"
        });
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
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
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeContext.GenresCategories.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        var input = new ListGenresInput(page, perPage, search);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(item => item.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Name.Should().Contain(search);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            var outputItemCategoryIds = outputItem.Categories.Select(item => item.Id);
            outputItemCategoryIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                var exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        });
    }
    
    [Theory(DisplayName = nameof(ListGenresOrdered))]
    [Trait("Integration/Application ", "ListGenres - Use Cases")]
    [InlineData("name","asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "desc")]
    [InlineData("id", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("", "asc")]
    public async Task ListGenresOrdered(
        string orderby,
        string order
        )
    {
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories();
        var exampleCategories = _fixture.GetExampleCategoriesList();
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            int relationsCount = random.Next(0, 3);
            for (int i = 0; i < relationsCount; i++)
            {
                int selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
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
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.Genres.AddRangeAsync(exampleGenres);
        await arrangeContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeContext.GenresCategories.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();
        var orderEnum = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;    
        var input = new ListGenresInput(1, 20, dir: orderEnum, sort: orderby);

        var output = await _sut.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenres.Count);
        output.Items.Should().HaveCount(exampleGenres.Count);
        var expectedList = _fixture.CloneOrderedGenresList(exampleGenres, orderby, orderEnum);
        
        for (int i = 0; i < exampleGenres.Count; i++)
        {
            var expectedItem = expectedList[i];
            var outputItem = output.Items[i];
            expectedItem.Should().NotBeNull();
            outputItem.Name.Should().Be(expectedItem!.Name);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            var outputItemCategoryIds = outputItem.Categories.Select(item => item.Id);
            outputItemCategoryIds.Should().BeEquivalentTo(expectedItem.Categories);
            outputItem.Categories.ToList().ForEach(outputCategory =>
            {
                var exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputCategory.Name.Should().Be(exampleCategory!.Name);
            });
        }
    }
}