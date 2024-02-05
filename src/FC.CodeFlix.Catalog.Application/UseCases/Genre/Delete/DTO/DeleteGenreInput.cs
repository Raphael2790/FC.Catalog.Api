using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.DTO;

public class DeleteGenreInput
    : IRequest
{
    public Guid Id { get; set; }
    
    public DeleteGenreInput(Guid id) 
        => Id = id;
}