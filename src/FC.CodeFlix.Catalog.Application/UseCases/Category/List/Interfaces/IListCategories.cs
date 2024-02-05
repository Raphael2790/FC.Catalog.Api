using FC.CodeFlix.Catalog.Application.UseCases.Category.List.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.List.Interfaces;

public interface IListCategories 
    : IRequestHandler<ListCategoriesInput, ListCategoriesOutput>
{
}
