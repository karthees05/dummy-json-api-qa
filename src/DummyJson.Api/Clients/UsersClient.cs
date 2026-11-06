using DummyJson.Api.Http;
using DummyJson.Api.Models;
using RestSharp;

namespace DummyJson.Api.Clients;

public sealed class UsersClient(ApiTransport transport)
{
    public Task<ApiResponse<UsersResponse>> FilterByHairColorAsync(string color) =>
        transport.SendAsync<UsersResponse>(new RestRequest("users/filter")
            .AddQueryParameter("key", "hair.color").AddQueryParameter("value", color)
            .AddQueryParameter("limit", "0"));

    public Task<ApiResponse<UsersResponse>> GetAllAsync() =>
        transport.SendAsync<UsersResponse>(new RestRequest("users").AddQueryParameter("limit", "0"));

    public Task<ApiResponse<User>> AddAsync(CreateUserRequest body) =>
        transport.SendAsync<User>(new RestRequest("users/add", Method.Post).AddJsonBody(body));
}
