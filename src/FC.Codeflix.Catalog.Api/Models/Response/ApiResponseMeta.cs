namespace FC.Codeflix.Catalog.Api.Models.Response;

public class ApiResponseMeta
{
    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
    
    
    public ApiResponseMeta(int currentPage, int perPage, int total)
    {
        CurrentPage = currentPage;
        PerPage = perPage;
        Total = total;
    }
}