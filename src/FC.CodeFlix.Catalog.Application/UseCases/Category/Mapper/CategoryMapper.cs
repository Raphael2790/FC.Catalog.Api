using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Mapper;

public static class CategoryMapper
{
    public static Domain.Entities.Category CreateCategoryInputToCategory(CreateCategoryInput input)
        => new(input.Name, input.Description!, input.IsActive);

    public static CategoryOutputModel CategoryToCreateCategoryOutput(Domain.Entities.Category category)
        => new(category.Name, category.Description, category.CreatedAt, category.Id, category.IsActive);

    public static CategoryOutputModel CategoryToGetCategoryOutput(Domain.Entities.Category category)
        => new(category.Name, category.Description, category.CreatedAt, category.Id, category.IsActive);
}
