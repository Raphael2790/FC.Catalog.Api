using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.Interface;

public interface ICreateGenre : IRequestHandler<CreateGenreInput, GenreOutputModel>{}