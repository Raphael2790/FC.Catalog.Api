using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Get.Interface;

public interface IGetCategory : IRequestHandler<GetCategoryInput, CategoryOutputModel> {}
