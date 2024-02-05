using System.Text;
using System.Text.Json;
using FC.Codeflix.Catalog.Api.Configurations.Policies;
using Microsoft.AspNetCore.WebUtilities;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    
    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        };
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(
        string route,
        object payload)
        where TOutput : class
    {
        var response = await _httpClient.PostAsync(
            route,
            GetContent(payload)
            );

        TOutput? output = null;
        var outputString = await response.Content.ReadAsStringAsync();

        if(!string.IsNullOrWhiteSpace(outputString))
            output = DeserializeResponse<TOutput>(outputString);

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(string route)
        where TOutput : class
    {
        var response = await _httpClient.DeleteAsync(
            route
            );

        TOutput? output = null;
        var outputString = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrWhiteSpace(outputString))
            output = DeserializeResponse<TOutput>(outputString);

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(
        string route,
        object? queryStringParametersObject = null)
        where TOutput : class
    {
        var url = PrepareRoute(route, queryStringParametersObject);
        var response = await _httpClient.GetAsync(
            url
            );

        TOutput? output = null;
        var outputString = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrWhiteSpace(outputString))
            output = DeserializeResponse<TOutput>(outputString);

        return (response, output);
    }

    private string PrepareRoute(string route, object? queryStringParametersObject)
    {
        if (queryStringParametersObject is null)
            return route;

        var jsonObject = JsonSerializer.Serialize(queryStringParametersObject, _jsonSerializerOptions);
        var queryDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject);
        return QueryHelpers.AddQueryString(route, queryDictionary!);
    }

    private StringContent GetContent(object payload)
        => new(JsonSerializer.Serialize(payload, _jsonSerializerOptions), Encoding.UTF8, "application/json");

    private TOutput? DeserializeResponse<TOutput>(string stringResponse)
        => JsonSerializer.Deserialize<TOutput>(stringResponse, _jsonSerializerOptions);

    public async Task<(HttpResponseMessage response, TOutput? output)> Put<TOutput>(string route, object payload)
        where TOutput : class
    {
        var response = await _httpClient.PutAsync(
            route,
            GetContent(payload)
        );

        TOutput? output = null;
        var outputString = await response.Content.ReadAsStringAsync();

        if(!string.IsNullOrWhiteSpace(outputString))
            output = DeserializeResponse<TOutput>(outputString);

        return (response, output);
    }
}
