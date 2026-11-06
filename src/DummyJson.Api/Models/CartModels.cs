namespace DummyJson.Api.Models;

public sealed record CartItemRequest(int Id, int Quantity);
public sealed record CreateCartRequest(int UserId, List<CartItemRequest> Products);
public sealed record CartProduct(
    int Id, string Title, decimal Price, int Quantity, decimal Total,
    decimal DiscountPercentage, decimal DiscountedPrice);

public sealed record Cart(
    int Id, int UserId, List<CartProduct> Products, decimal Total, decimal DiscountedTotal,
    int TotalProducts, int TotalQuantity, bool? IsDeleted, DateTimeOffset? DeletedOn);
