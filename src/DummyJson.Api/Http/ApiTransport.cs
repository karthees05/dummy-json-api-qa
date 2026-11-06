using System.Diagnostics;
using DummyJson.Api.Configuration;
using RestSharp;

namespace DummyJson.Api.Http;

public sealed class ApiTransport : IDisposable
{
    private readonly RestClient _client;
    private readonly Action<string> _log;

    public ApiTransport(ApiSettings settings, Action<string> log)
    {
        _client = new RestClient(new RestClientOptions(settings.BaseUrl)
        {
            Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds)
        });
        _log = log;
    }

    public async Task<ApiResponse<T>> SendAsync<T>(RestRequest request)
    {
        request.AddHeader("Accept", "application/json");
        var timer = Stopwatch.StartNew();
        var response = await _client.ExecuteAsync(request);
        _log($"{request.Method} {request.Resource} -> {(int)response.StatusCode} ({timer.ElapsedMilliseconds} ms)");

        if (response.ResponseStatus != ResponseStatus.Completed)
            throw new HttpRequestException(
                $"{request.Method} {request.Resource} failed: {response.ResponseStatus}. Check network access and timeout settings.");

        return new ApiResponse<T>(response);
    }

    public void Dispose() => _client.Dispose();
}
