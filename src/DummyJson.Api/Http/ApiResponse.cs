using System.Net;
using System.Text.Json;
using RestSharp;

namespace DummyJson.Api.Http;

public interface IApiResponse
{
    HttpStatusCode StatusCode { get; }
    string? ContentType { get; }
}

public sealed class ApiResponse<T>(RestResponse response) : IApiResponse
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public HttpStatusCode StatusCode => response.StatusCode;
    public string? ContentType => response.ContentType;

    public T Body => JsonSerializer.Deserialize<T>(response.Content ?? "", JsonOptions)
        ?? throw new InvalidOperationException($"The API returned a null {typeof(T).Name} payload.");
}
