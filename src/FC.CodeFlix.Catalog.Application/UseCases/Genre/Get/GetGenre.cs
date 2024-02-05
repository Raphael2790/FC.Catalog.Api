using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Get;

public class GetGenre 
    : IGetGenre
{
    private readonly IGenreRepository _genreRepository;

    public GetGenre(IGenreRepository genreRepository) 
        => _genreRepository = genreRepository;

    public async Task<GenreOutputModel> Handle(GetGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.Get(request.Id, cancellationToken);
        return GenreOutputModel.FromGenre(genre);
    }
}