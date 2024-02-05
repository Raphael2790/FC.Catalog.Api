using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.Interface;

public interface IGetGenre
    : IRequestHandler<GetGenreInput, GenreOutputModel>
{ }