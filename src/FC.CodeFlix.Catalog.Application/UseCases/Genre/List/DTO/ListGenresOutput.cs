using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.List.DTO;

public class ListGenresOutput
    : PaginatedListOutput<GenreOutputModel>
{
    public ListGenresOutput(
        int page, 
        int perPage,
        int total, 
        IReadOnlyList<GenreOutputModel> items
        ) 
        : base(page, perPage, total, items)
    {
    }
    
    public static ListGenresOutput FromSearchOutput(SearchOutput<Domain.Entities.Genre> searchOutput)
        => new (
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items.Select(GenreOutputModel.FromGenre).ToList());

    public void FillWithCategoriesNames(IReadOnlyList<Domain.Entities.Category> categories)
    {
        foreach (var genreOutputModel in Items)
        foreach (var categoryModel in genreOutputModel.Categories.ToList())
            categoryModel.Name = categories?.FirstOrDefault(category => category.Id == categoryModel.Id)?.Name;

    }
}