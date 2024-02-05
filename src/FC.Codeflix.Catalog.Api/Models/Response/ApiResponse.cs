namespace FC.Codeflix.Catalog.Api.Models.Response;

public class ApiResponse<TData>
{
    public TData Data { get; set; }
    
    public ApiResponse(TData data)
    {
        Data = data;
    }
}