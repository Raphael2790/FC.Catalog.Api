using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Update.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Update.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Update;

public class UpdateGenre
    : IUpdateGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    
    public UpdateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreOutputModel> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.Get(request.Id, cancellationToken);
        genre.Update(request.Name);
        if (request.IsActive is not null
            && request.IsActive != genre.IsActive)
        {
            if(request.IsActive.Value) genre.Activate();
            else genre.Deactivate();
        }

        if (request.CategoriesIds is not null)
        {
            genre.RemoveAllCategories();
            if (request.CategoriesIds.Any())
            {
                await ValidateCategoriesIds(request, cancellationToken);
                request.CategoriesIds?.ForEach(genre.AddCategory);   
            }
        }
        
        await _genreRepository.Update(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return GenreOutputModel.FromGenre(genre);
    }
    
    private async Task ValidateCategoriesIds(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var persistedCategoriesIds = 
            await _categoryRepository.GetIdsListByIds(request.CategoriesIds!, cancellationToken);

        if (persistedCategoriesIds.Count < request.CategoriesIds!.Count)
        {
            var notFoundIds = request.CategoriesIds.Except(persistedCategoriesIds);
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(", ", notFoundIds)}");
        }
    }
}