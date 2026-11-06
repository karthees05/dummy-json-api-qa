using DummyJson.Api.Http;

namespace DummyJson.Api.Tests.Support;

public sealed class ScenarioState
{
    public IApiResponse? Response { get; set; }
    public string? AccessToken { get; set; }

    public T Body<T>() => Response is ApiResponse<T> response
        ? response.Body
        : throw new InvalidOperationException($"No {typeof(T).Name} response exists in this scenario.");
}
