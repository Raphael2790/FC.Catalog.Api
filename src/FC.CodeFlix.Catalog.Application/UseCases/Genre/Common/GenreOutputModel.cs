using System.Collections.ObjectModel;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;

public class GenreOutputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<CategoryOutputModel> Categories { get; set; }

    public GenreOutputModel(
        Guid id, 
        string name, 
        bool isActive, 
        DateTime createdAt, 
        IReadOnlyList<CategoryOutputModel> categories)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        CreatedAt = createdAt;
        Categories = categories;
    }
    
    public static GenreOutputModel FromGenre(Domain.Entities.Genre genre)
        => new(genre.Id, genre.Name, genre.IsActive, genre.CreatedAt, genre.Categories.Select(categoryId => new CategoryOutputModel(categoryId)).ToList().AsReadOnly());
}

public class CategoryOutputModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public CategoryOutputModel(Guid id, string? name = null)
    {
        Id = id;
        Name = name;
    }
}