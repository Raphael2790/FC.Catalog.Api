using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.List.Interface;

public interface IListGenres
    : IRequestHandler<ListGenresInput, ListGenresOutput>
{ }