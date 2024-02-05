using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.DTO;

public class CreateGenreInput 
    : IRequest<GenreOutputModel>
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public List<Guid>? CategoriesIds { get; set; }

    public CreateGenreInput(string name, bool isActive, List<Guid>? categoriesIds = null)
    {
        Name = name;
        IsActive = isActive;
        CategoriesIds = categoriesIds;
    }
}