using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Update.Interface;

public interface IUpdateCategory : IRequestHandler<UpdateCategoryInput,CategoryOutputModel>
{}
