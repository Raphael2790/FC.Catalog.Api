using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.Interface;

public interface IDeleteCategory : IRequestHandler<DeleteCategoryInput> {}
