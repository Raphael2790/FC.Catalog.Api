using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.DTO;

public class GetGenreInput
    : IRequest<GenreOutputModel>
{
    public Guid Id { get; set; }

    public GetGenreInput(Guid id) 
        => Id = id;
}