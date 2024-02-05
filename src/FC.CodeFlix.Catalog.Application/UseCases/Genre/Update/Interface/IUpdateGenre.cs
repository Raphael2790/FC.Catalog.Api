using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Update.DTO;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Update.Interface;

public interface IUpdateGenre 
    : IRequestHandler<UpdateGenreInput, GenreOutputModel>
{ }