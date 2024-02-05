using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.Interface;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Mapper;
using FC.CodeFlix.Catalog.Domain.Repositories;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Create;

public class CreateCategory : ICreateCategory
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryOutputModel> Handle(CreateCategoryInput input, CancellationToken cancellationToken)
    {
        var category = CategoryMapper.CreateCategoryInputToCategory(input);

        await _categoryRepository.Insert(category, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return CategoryMapper.CategoryToCreateCategoryOutput(category);
    }
}
