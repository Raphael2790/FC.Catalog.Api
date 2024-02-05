using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;

public class UpdateCategoryInput : IRequest<CategoryOutputModel>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }

    public UpdateCategoryInput(Guid id, 
        string name, 
        string? description = null, 
        bool? isActive = null)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
    }
}
