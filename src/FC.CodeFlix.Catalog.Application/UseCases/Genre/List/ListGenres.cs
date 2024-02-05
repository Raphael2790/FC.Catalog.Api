using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.List;

public class ListGenres
    : IListGenres
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ListGenres(IGenreRepository genreRepository, ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ListGenresOutput> Handle(ListGenresInput request, CancellationToken cancellationToken)
    {
        var searchInput = request.ToSearchInput();

        var searchOutput = await _genreRepository.Search(searchInput, cancellationToken);

        var output = ListGenresOutput.FromSearchOutput(searchOutput);

        var categoriesIdsSearchOutput = searchOutput.Items
            .SelectMany(item => item.Categories).Distinct().ToList();

        if (!categoriesIdsSearchOutput.Any()) return output;
        
        var categories = await _categoryRepository.GetListByIds(categoriesIdsSearchOutput, cancellationToken);
        output.FillWithCategoriesNames(categories);

        return output;
    }
}