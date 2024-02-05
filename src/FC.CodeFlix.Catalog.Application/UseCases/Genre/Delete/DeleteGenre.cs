using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete;

public class DeleteGenre
    : IDeleteGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.Get(request.Id, cancellationToken);
        await _genreRepository.Delete(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return Unit.Value;
    }
}