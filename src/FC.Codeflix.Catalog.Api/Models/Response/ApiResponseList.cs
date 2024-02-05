using FC.CodeFlix.Catalog.Application.Common;

namespace FC.Codeflix.Catalog.Api.Models.Response;

public class ApiResponseList<TItemData> : ApiResponse<IReadOnlyList<TItemData>>
{
    public ApiResponseMeta Meta { get; }
    public ApiResponseList(PaginatedListOutput<TItemData> paginatedOutput) 
        : base(paginatedOutput.Items)
    {
        Meta = new ApiResponseMeta(paginatedOutput.Page, paginatedOutput.PerPage, paginatedOutput.Total);
    }
}