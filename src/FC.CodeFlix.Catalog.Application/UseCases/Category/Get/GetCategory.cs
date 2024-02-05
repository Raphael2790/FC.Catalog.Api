using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.Interface;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Mapper;
using FC.CodeFlix.Catalog.Domain.Repositories;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Get;

public class GetCategory : IGetCategory
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategory(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryOutputModel> Handle(GetCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(request.Id, cancellationToken);

        return CategoryMapper.CategoryToGetCategoryOutput(category);
    }
}
