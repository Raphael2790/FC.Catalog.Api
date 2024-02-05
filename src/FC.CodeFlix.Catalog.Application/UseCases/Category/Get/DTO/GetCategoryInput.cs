using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Get.DTO;

public class GetCategoryInput : IRequest<CategoryOutputModel>
{
    public Guid Id { get; set; }

    public GetCategoryInput(Guid id)
      =>  Id = id;
}
