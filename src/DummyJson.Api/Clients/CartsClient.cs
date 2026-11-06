using DummyJson.Api.Http;
using DummyJson.Api.Models;
using RestSharp;

namespace DummyJson.Api.Clients;

public sealed class CartsClient(ApiTransport transport)
{
    public Task<ApiResponse<Cart>> AddAsync(CreateCartRequest body) =>
        transport.SendAsync<Cart>(new RestRequest("carts/add", Method.Post).AddJsonBody(body));

    public Task<ApiResponse<Cart>> DeleteAsync(int id) =>
        transport.SendAsync<Cart>(new RestRequest("carts/{id}", Method.Delete).AddUrlSegment("id", id));
}
