using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.Infra.Data.Context;
using FC.CodeFlix.Catalog.Infra.Data.Models;
using FC.CodeFlix.Catalog.Infra.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.Repositories.Genre;

[Collection(nameof(GenreRepositoryTestsFixture))]
public class GenreRepositoryTests
{
    private readonly GenreRepositoryTestsFixture _fixture;
    private readonly CodeflixCatalogDbContext _context;
    private IGenreRepository _sut;

    public GenreRepositoryTests(GenreRepositoryTestsFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.CreateDbContext();
        _sut = new GenreRepository(_context);
    }
    
    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task Insert()
    {
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        foreach(var categorie in exampleCategories)
            exampleGenre.AddCategory(categorie.Id);
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.SaveChangesAsync();

        await _sut.Insert(exampleGenre, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var assertContext = _fixture.CreateDbContext(true);
        var savedGenre = await assertContext
            .Genres.FindAsync(exampleGenre.Id);
        savedGenre.Should().NotBeNull();
        savedGenre!.Name.Should().Be(exampleGenre.Name);
        savedGenre.IsActive.Should().Be(exampleGenre.IsActive);
        savedGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genresCategoriesRelation = await assertContext
            .GenresCategories.Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync();
        genresCategoriesRelation.Should().HaveCount(exampleCategories.Count);
        foreach (var relation in genresCategoriesRelation)
        {
            var expectedCategory = exampleCategories
                .FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();
        }
    }
    
    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task Get()
    {
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        foreach(var categorie in exampleCategories)
            exampleGenre.AddCategory(categorie.Id);
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await _context.GenresCategories.AddAsync(relation);
        }
        await _context.SaveChangesAsync();

        _sut = new GenreRepository(_fixture.CreateDbContext(true));

        var genre = await _sut.Get(exampleGenre.Id, CancellationToken.None);
        
        genre.Should().NotBeNull();
        genre.Name.Should().Be(exampleGenre.Name);
        genre.IsActive.Should().Be(exampleGenre.IsActive);
        genre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genre.Categories.Should().HaveCount(exampleCategories.Count);
        foreach (var categoryId in genre.Categories)
        {
            var expectedCategory = exampleCategories
                .FirstOrDefault(x => x.Id == categoryId);
            expectedCategory.Should().NotBeNull();
        }
    }
    
    [Fact(DisplayName = nameof(GetWhenNotFoundThrowsException))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task GetWhenNotFoundThrowsException()
    {
        var notFoundId = Guid.NewGuid();
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        foreach(var categorie in exampleCategories)
            exampleGenre.AddCategory(categorie.Id);
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await _context.GenresCategories.AddAsync(relation);
        }
        await _context.SaveChangesAsync();

        await _sut.Awaiting(x => x.Get(notFoundId, CancellationToken.None))
            .Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Genre '{notFoundId}' not found.");
    }
    
    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task Delete()
    {
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        foreach(var categorie in exampleCategories)
            exampleGenre.AddCategory(categorie.Id);
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await _context.GenresCategories.AddAsync(relation);
        }
        await _context.SaveChangesAsync();

        var newContext = _fixture.CreateDbContext(true);
        _sut = new GenreRepository(newContext);

        await _sut.Delete(exampleGenre, CancellationToken.None);

        await newContext.SaveChangesAsync();

        var assertContext = _fixture.CreateDbContext(true);
        var genre = await assertContext.Genres
            .AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == exampleGenre.Id);
        genre.Should().BeNull();
        var categoriesIds = await assertContext.GenresCategories
            .Where(x => x.GenreId == exampleGenre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        categoriesIds.Should().HaveCount(default(int));
    }
    
    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task Update()
    {
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        foreach(var categorie in exampleCategories)
            exampleGenre.AddCategory(categorie.Id);
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await _context.GenresCategories.AddAsync(relation);
        }
        await _context.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        _sut = new GenreRepository(actContext);
        
        exampleGenre.Update(_fixture.GetValidGenreName());
        if(exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        await _sut.Update(exampleGenre, CancellationToken.None);

        await actContext.SaveChangesAsync();
        
        var assertContext = _fixture.CreateDbContext(true);
        var savedGenre = await assertContext
            .Genres.FindAsync(exampleGenre.Id);
        savedGenre.Should().NotBeNull();
        savedGenre!.Name.Should().Be(exampleGenre.Name);
        savedGenre.IsActive.Should().Be(exampleGenre.IsActive);
        savedGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genresCategoriesRelation = await assertContext
            .GenresCategories.Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync();
        genresCategoriesRelation.Should().HaveCount(exampleCategories.Count);
        foreach (var expectedCategory in genresCategoriesRelation.Select(relation => exampleCategories
                     .FirstOrDefault(x => x.Id == relation.CategoryId)))
        {
            expectedCategory.Should().NotBeNull();
        }
    }
    
    [Fact(DisplayName = nameof(UpdateRemovingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task UpdateRemovingRelations()
    {
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        foreach(var categorie in exampleCategories)
            exampleGenre.AddCategory(categorie.Id);
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await _context.GenresCategories.AddAsync(relation);
        }
        await _context.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        _sut = new GenreRepository(actContext);
        
        exampleGenre.Update(_fixture.GetValidGenreName());
        if(exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        await _sut.Update(exampleGenre, CancellationToken.None);

        await actContext.SaveChangesAsync();
        
        var assertContext = _fixture.CreateDbContext(true);
        var updatedGenre = await assertContext
            .Genres.FindAsync(exampleGenre.Id);
        updatedGenre.Should().NotBeNull();
        updatedGenre!.Name.Should().Be(exampleGenre.Name);
        updatedGenre.IsActive.Should().Be(exampleGenre.IsActive);
        updatedGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genresCategoriesRelation = await assertContext
            .GenresCategories.Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync();
        genresCategoriesRelation.Should().HaveCount(default(int));
    }
    
    [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task UpdateReplacingRelations()
    {
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        var updateExampleCategories = _fixture.GetExampleCategoriesList(2);
        foreach(var categorie in exampleCategories)
            exampleGenre.AddCategory(categorie.Id);
        await _context.Categories.AddRangeAsync(exampleCategories);
        await _context.Categories.AddRangeAsync(updateExampleCategories);
        await _context.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await _context.GenresCategories.AddAsync(relation);
        }
        await _context.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        _sut = new GenreRepository(actContext);
        
        exampleGenre.Update(_fixture.GetValidGenreName());
        if(exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        foreach (var category in updateExampleCategories)
            exampleGenre.AddCategory(category.Id);
        await _sut.Update(exampleGenre, CancellationToken.None);

        await actContext.SaveChangesAsync();
        
        var assertContext = _fixture.CreateDbContext(true);
        var updatedGenre = await assertContext
            .Genres.FindAsync(exampleGenre.Id);
        updatedGenre.Should().NotBeNull();
        updatedGenre!.Name.Should().Be(exampleGenre.Name);
        updatedGenre.IsActive.Should().Be(exampleGenre.IsActive);
        updatedGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genresCategoriesRelation = await assertContext
            .GenresCategories.Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync();
        genresCategoriesRelation.Should().HaveCount(exampleGenre.Categories.Count);
        foreach (var expectedCategory in genresCategoriesRelation.Select(relation => updateExampleCategories
                     .FirstOrDefault(x => x.Id == relation.CategoryId)))
        {
            expectedCategory.Should().NotBeNull();
        }
    }
    
    [Fact(DisplayName = nameof(SearchReturnsItemsAndTotal))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task SearchReturnsItemsAndTotal()
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        await _context.Genres.AddRangeAsync(exampleGenres);
        await _context.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        _sut = new GenreRepository(actContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);
        
        var searchOutput = await _sut.Search(searchInput, CancellationToken.None);

        searchOutput.Should().NotBeNull();
        searchOutput.CurrentPage.Should().Be(searchInput.Page);
        searchOutput.PerPage.Should().Be(searchInput.PerPage);
        searchOutput.Total.Should().Be(exampleGenres.Count);
        searchOutput.Items.Should().HaveCount(exampleGenres.Count);
        foreach (var item in searchOutput.Items)
        {
            var exampleGenre = exampleGenres.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.IsActive.Should().Be(exampleGenre.IsActive);
            item.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        }
    }
    
    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task SearchReturnsRelations()
    {
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories(10);
        await _context.Genres.AddRangeAsync(exampleGenres);
        var random = new Random();
        foreach (var exampleGenre in exampleGenres)
        {
            var categoriesList = _fixture.GetExampleCategoriesList(random.Next(0, 4));
            if (!categoriesList.Any()) continue;
            categoriesList.ForEach(category => exampleGenre.AddCategory(category.Id));
            await _context.Categories.AddRangeAsync(categoriesList);
            var relations = categoriesList
                .Select(category => new GenresCategories(category.Id, exampleGenre.Id));
            await _context.GenresCategories.AddRangeAsync(relations);
            
        }
        await _context.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        _sut = new GenreRepository(actContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);
        
        var searchOutput = await _sut.Search(searchInput, CancellationToken.None);

        searchOutput.Should().NotBeNull();
        searchOutput.CurrentPage.Should().Be(searchInput.Page);
        searchOutput.PerPage.Should().Be(searchInput.PerPage);
        searchOutput.Total.Should().Be(exampleGenres.Count);
        searchOutput.Items.Should().HaveCount(exampleGenres.Count);
        foreach (var item in searchOutput.Items)
        {
            var exampleGenre = exampleGenres.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.IsActive.Should().Be(exampleGenre.IsActive);
            item.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            item.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            item.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
        }
    }
    
    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceResultIsEmpty))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    public async Task SearchReturnsEmptyWhenPersistenceResultIsEmpty()
    {
        var actContext = _fixture.CreateDbContext();
        _sut = new GenreRepository(actContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);
        
        var searchOutput = await _sut.Search(searchInput, CancellationToken.None);

        searchOutput.Should().NotBeNull();
        searchOutput.CurrentPage.Should().Be(searchInput.Page);
        searchOutput.PerPage.Should().Be(searchInput.PerPage);
        searchOutput.Total.Should().Be(default);
        searchOutput.Items.Should().HaveCount(default(int));
    }
    
    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task SearchReturnsPaginated(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
        )
    {
        var exampleGenres = _fixture.GetExampleGenreListWithoutCategories(quantityToGenerate);
        await _context.Genres.AddRangeAsync(exampleGenres);
        var random = new Random();
        foreach (var exampleGenre in exampleGenres)
        {
            var categoriesList = _fixture.GetExampleCategoriesList(random.Next(0, 4));
            if (!categoriesList.Any()) continue;
            categoriesList.ForEach(category => exampleGenre.AddCategory(category.Id));
            await _context.Categories.AddRangeAsync(categoriesList);
            var relations = categoriesList
                .Select(category => new GenresCategories(category.Id, exampleGenre.Id));
            await _context.GenresCategories.AddRangeAsync(relations);
            
        }
        await _context.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        _sut = new GenreRepository(actContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);
        
        var searchOutput = await _sut.Search(searchInput, CancellationToken.None);

        searchOutput.Should().NotBeNull();
        searchOutput.CurrentPage.Should().Be(searchInput.Page);
        searchOutput.PerPage.Should().Be(searchInput.PerPage);
        searchOutput.Total.Should().Be(exampleGenres.Count);
        searchOutput.Items.Should().HaveCount(expectedQuantityItems);
        foreach (var item in searchOutput.Items)
        {
            var exampleGenre = exampleGenres.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.IsActive.Should().Be(exampleGenre.IsActive);
            item.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            item.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            item.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
        }
    }
    
    [Theory(DisplayName = nameof(SearchReturnsByText))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    [InlineData("Action",1, 5, 1, 1)]
    [InlineData("Horror",1, 5, 3, 3)]
    [InlineData("Horror",2, 5, 0, 3)]
    [InlineData("Sci-fi",1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchReturnsByText(
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
        await _context.Genres.AddRangeAsync(exampleGenres);
        var random = new Random();
        foreach (var exampleGenre in exampleGenres)
        {
            var categoriesList = _fixture.GetExampleCategoriesList(random.Next(0, 4));
            if (!categoriesList.Any()) continue;
            categoriesList.ForEach(category => exampleGenre.AddCategory(category.Id));
            await _context.Categories.AddRangeAsync(categoriesList);
            var relations = categoriesList
                .Select(category => new GenresCategories(category.Id, exampleGenre.Id));
            await _context.GenresCategories.AddRangeAsync(relations);
            
        }
        await _context.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        _sut = new GenreRepository(actContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);
        
        var searchOutput = await _sut.Search(searchInput, CancellationToken.None);

        searchOutput.Should().NotBeNull();
        searchOutput.CurrentPage.Should().Be(searchInput.Page);
        searchOutput.PerPage.Should().Be(searchInput.PerPage);
        searchOutput.Total.Should().Be(expectedQuantityTotalItems);
        searchOutput.Items.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (var item in searchOutput.Items)
        {
            var exampleGenre = exampleGenres.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.IsActive.Should().Be(exampleGenre.IsActive);
            item.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            item.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            item.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
        }
    }
    
    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "GenreRepository - Persistence")]
    [InlineData("name","asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "desc")]
    [InlineData("id", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
            string orderby,
            string order
        )
    {
        var examplegenresList = _fixture.GetExampleGenreList();
        await _context.Genres.AddRangeAsync(examplegenresList);
        await _context.SaveChangesAsync(CancellationToken.None);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderby, searchOrder);

        var output = await _sut.Search(searchInput, CancellationToken.None);

        var expectedList = _fixture.CloneOrderedGenresList(examplegenresList, orderby, searchOrder);
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(examplegenresList.Count);
        output.Items.Should().HaveCount(examplegenresList.Count);
        for (int i = 0; i < examplegenresList.Count; i++)
        {
            var expectedItem = expectedList[i];
            var outputItem = output.Items[i];

            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(expectedItem!.Name);
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}