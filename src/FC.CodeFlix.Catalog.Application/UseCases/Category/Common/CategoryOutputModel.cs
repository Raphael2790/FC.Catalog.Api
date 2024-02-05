namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Common;

public class CategoryOutputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public CategoryOutputModel(string name, string description, DateTime createdAt, Guid id, bool isActive)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = createdAt;
    }
}
