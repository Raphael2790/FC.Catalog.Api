using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Create;

public class CreateGenre : ICreateGenre
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreOutputModel> Handle(CreateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = new Domain.Entities.Genre(request.Name, request.IsActive);

        if ((request.CategoriesIds?.Count ?? default) > default(int))
        {
            await ValidateCategoriesIds(request, cancellationToken);
            request.CategoriesIds?.ForEach(genre.AddCategory);
        }
        
        await _genreRepository.Insert(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return GenreOutputModel.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(CreateGenreInput request, CancellationToken cancellationToken)
    {
        var persistedCategoriesIds = 
            await _categoryRepository.GetIdsListByIds(request.CategoriesIds!, cancellationToken);

        if (persistedCategoriesIds.Count < request.CategoriesIds!.Count)
        {
            var notFoundIds = request.CategoriesIds.Except(persistedCategoriesIds);
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(',', notFoundIds)}");
        }
    }
}