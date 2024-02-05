using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.Interface;

public interface IDeleteGenre
    : IRequestHandler<DeleteGenreInput>
{ }