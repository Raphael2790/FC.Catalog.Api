using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Create.Interface;

public interface ICreateCategory : IRequestHandler<CreateCategoryInput,CategoryOutputModel> {}
