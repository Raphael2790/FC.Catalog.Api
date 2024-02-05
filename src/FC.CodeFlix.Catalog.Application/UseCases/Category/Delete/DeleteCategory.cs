using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.Interface;
using FC.CodeFlix.Catalog.Domain.Repositories;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Delete;

public class DeleteCategory : IDeleteCategory
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        => (_categoryRepository, _unitOfWork) = (categoryRepository, unitOfWork);
    

    public async Task<Unit> Handle(DeleteCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(request.Id, cancellationToken);
        await _categoryRepository.Delete(category, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return Unit.Value;
    }
}
