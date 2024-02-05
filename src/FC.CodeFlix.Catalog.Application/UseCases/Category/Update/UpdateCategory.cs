using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Mapper;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Update;

public class UpdateCategory : IUpdateCategory
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        => (_categoryRepository, _unitOfWork) = (categoryRepository, unitOfWork); 

    public async Task<CategoryOutputModel> Handle(UpdateCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(request.Id, cancellationToken);
        category.Update(request.Name, request.Description);
        if(request.IsActive is not null && request.IsActive != category.IsActive)
            if((bool)request.IsActive) category.Activate();
            else category.Deactivate();
        await _categoryRepository.Update(category,cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return CategoryMapper.CategoryToCreateCategoryOutput(category);
    }
}
