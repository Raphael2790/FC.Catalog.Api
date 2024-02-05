using FC.CodeFlix.Catalog.IntegrationTests.Base;

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

public class GenreUseCasesBaseFixture
    : BaseFixture
{
    public Domain.Entities.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null, string? name = null)
    {
        var genre = new Domain.Entities.Genre(name ?? GetValidGenreName(), isActive ?? GetRandomBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<Domain.Entities.Genre> GetExampleGenreList(int count = 10)
        => Enumerable.Range(1, count)
            .Select(_ => GetExampleGenre(categoriesIds: GetRandomIdsList()))
            .ToList();

    public List<Domain.Entities.Genre> GetExampleGenreListWithNames(IEnumerable<string> names)
        => names.Select(name => GetExampleGenre(name: name)).ToList();

    public List<Domain.Entities.Genre> GetExampleGenreListWithoutCategories(int count = 10)
        => Enumerable.Range(1, count)
            .Select(_ => GetExampleGenre(categoriesIds: new List<Guid>()))
            .ToList();

    private List<Guid> GetRandomIdsList(int? count = null)
        => Enumerable.Range(1, count ?? new Random().Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();

    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];
    
    public List<Domain.Entities.Category> GetExampleCategoriesList(int length = 10)
        => Enumerable.Range(default, length)
            .Select(_ => GetValidCategory())
            .ToList();
    
    private string GetValidCategoryName()
    {
        var categoryName = string.Empty;
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    private string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    private Domain.Entities.Category GetValidCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());
}