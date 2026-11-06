namespace DummyJson.Api.Models;

public sealed record Product(int Id, string Title, decimal Price, decimal DiscountPercentage);
public sealed record ProductsResponse(List<Product> Products, int Total, int Skip, int Limit);
public sealed record UpdateProductRequest(string Title);
