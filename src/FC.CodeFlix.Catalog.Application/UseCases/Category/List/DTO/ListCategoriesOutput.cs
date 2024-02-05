using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.List.DTO;

public class ListCategoriesOutput
    : PaginatedListOutput<CategoryOutputModel>
{
    public ListCategoriesOutput(int page, int perPage, int total, IReadOnlyList<CategoryOutputModel> items)
        : base(page, perPage, total, items)
    {
    }
}
