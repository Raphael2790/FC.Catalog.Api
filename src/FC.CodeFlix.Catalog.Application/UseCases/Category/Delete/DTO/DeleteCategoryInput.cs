using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.DTO;

public class DeleteCategoryInput : IRequest
{
    public Guid Id { get; set; }

    public DeleteCategoryInput(Guid id)
        => Id = id;
}
