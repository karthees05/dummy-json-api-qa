using DummyJson.Api.Http;
using DummyJson.Api.Models;
using RestSharp;

namespace DummyJson.Api.Clients;

public sealed class AuthClient(ApiTransport transport)
{
    public Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest body) =>
        transport.SendAsync<LoginResponse>(new RestRequest("auth/login", Method.Post).AddJsonBody(body));

    public Task<ApiResponse<User>> GetCurrentUserAsync(string token) =>
        transport.SendAsync<User>(new RestRequest("auth/me").AddHeader("Authorization", $"Bearer {token}"));
}
