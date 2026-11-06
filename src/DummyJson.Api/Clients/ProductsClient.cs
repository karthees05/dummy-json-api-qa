using DummyJson.Api.Http;
using DummyJson.Api.Models;
using RestSharp;

namespace DummyJson.Api.Clients;

public sealed class ProductsClient(ApiTransport transport)
{
    public Task<ApiResponse<ProductsResponse>> SearchAsync(string title) =>
        transport.SendAsync<ProductsResponse>(new RestRequest("products/search").AddQueryParameter("q", title));

    public Task<ApiResponse<ProductsResponse>> GetPageAsync(int limit, int skip) =>
        transport.SendAsync<ProductsResponse>(new RestRequest("products")
            .AddQueryParameter("limit", limit.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .AddQueryParameter("skip", skip.ToString(System.Globalization.CultureInfo.InvariantCulture)));

    public Task<ApiResponse<Product>> GetAsync(int id) =>
        transport.SendAsync<Product>(new RestRequest("products/{id}").AddUrlSegment("id", id));

    public Task<ApiResponse<Product>> UpdateTitleAsync(int id, string title) =>
        transport.SendAsync<Product>(new RestRequest("products/{id}", Method.Put)
            .AddUrlSegment("id", id).AddJsonBody(new UpdateProductRequest(title)));
}
