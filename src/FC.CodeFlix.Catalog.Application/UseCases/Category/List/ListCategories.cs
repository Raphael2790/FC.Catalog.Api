using FC.CodeFlix.Catalog.Application.UseCases.Category.List.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.List.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Mapper;
using FC.CodeFlix.Catalog.Domain.Repositories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.List;

public class ListCategories : IListCategories
{
    private readonly ICategoryRepository _categoryRepository;
    public ListCategories(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public async Task<ListCategoriesOutput> Handle(ListCategoriesInput request, CancellationToken cancellationToken)
    {
        var searchOutput = await _categoryRepository.Search(
            new((int)request.Page!, (int)request.PerPage!,request.Search!,request.Sort!,(SearchOrder)request.Dir!)
            ,cancellationToken);

        return new(searchOutput.CurrentPage,
                   searchOutput.PerPage,
                   searchOutput.Total,
                   searchOutput.Items.Select(CategoryMapper.CategoryToCreateCategoryOutput)
                   .ToList());
    }
}
