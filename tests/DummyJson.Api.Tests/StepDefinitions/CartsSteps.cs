using System.Globalization;
using DummyJson.Api.Clients;
using DummyJson.Api.Models;
using DummyJson.Api.Tests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.Api.Tests.StepDefinitions;

[Binding]
public sealed class CartsSteps(CartsClient client, ProductsClient productsClient, ScenarioState state)
{
    private CreateCartRequest? _request;
    private Dictionary<int, decimal> _expectedPercentages = [];
    private readonly Dictionary<int, Product> _catalogue = [];
    private DateTimeOffset _deleteStarted;

    [Given(@"a cart for user (\d+) containing the following products")]
    public async Task GivenCart(int userId, Table table)
    {
        var items = table.Rows.Select(row => new CartItemRequest(
            int.Parse(row["ProductId"], CultureInfo.InvariantCulture),
            int.Parse(row["Quantity"], CultureInfo.InvariantCulture))).ToList();
        _request = new CreateCartRequest(userId, items);
        _expectedPercentages = table.Rows.ToDictionary(
            row => int.Parse(row["ProductId"], CultureInfo.InvariantCulture),
            row => decimal.Parse(row["DiscountPercentage"], CultureInfo.InvariantCulture));

        foreach (var item in items)
        {
            var response = await productsClient.GetAsync(item.Id);
            ResponseAssertions.HasStatusAndJson(response, 200);
            _catalogue.Add(item.Id, response.Body);
        }
    }

    [When("I add the cart")]
    public async Task WhenIAddCart() => state.Response = await client.AddAsync(_request
        ?? throw new InvalidOperationException("Supply cart products first."));

    [Then("the cart should contain exactly the five requested products and quantities")]
    public void ThenValidateCartContents()
    {
        var cart = state.Body<Cart>();
        var request = _request!;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(cart.Id, Is.GreaterThan(0));
            Assert.That(cart.UserId, Is.EqualTo(request.UserId));
            Assert.That(cart.TotalProducts, Is.EqualTo(5));
            Assert.That(cart.Products, Has.Count.EqualTo(5));
            Assert.That(cart.Products.Select(product => (product.Id, product.Quantity)),
                Is.EquivalentTo(request.Products.Select(product => (product.Id, product.Quantity))));
            Assert.That(cart.TotalQuantity, Is.EqualTo(request.Products.Sum(product => product.Quantity)));
        }
    }

    [Then(@"the sum of the product discount percentages should be (\d+\.\d+)")]
    public void ThenValidatePercentageTotal(decimal expected)
    {
        var cart = state.Body<Cart>();
        using (Assert.EnterMultipleScope())
        {
            foreach (var product in cart.Products)
            {
                Assert.That(product.DiscountPercentage, Is.EqualTo(_expectedPercentages[product.Id]),
                    $"Expected discount percentage for product {product.Id}");
                Assert.That(product.DiscountPercentage, Is.EqualTo(_catalogue[product.Id].DiscountPercentage),
                    $"Catalogue discount percentage for product {product.Id}");
            }
            Assert.That(cart.Products.Sum(product => product.DiscountPercentage), Is.EqualTo(expected),
                "Sum of product discount percentages (percentage points)");
        }
    }

    [Then("the cart monetary totals should agree with the product prices and discounts")]
    public void ThenValidateMoney()
    {
        var cart = state.Body<Cart>();
        decimal expectedTotal = 0;
        decimal expectedDiscountedTotal = 0;
        using (Assert.EnterMultipleScope())
        {
            foreach (var product in cart.Products)
            {
                var baseline = _catalogue[product.Id];
                var quantity = _request!.Products.Single(item => item.Id == product.Id).Quantity;
                var lineTotal = baseline.Price * quantity;
                var discountedLine = decimal.Round(lineTotal * (1 - baseline.DiscountPercentage / 100m),
                    0, MidpointRounding.AwayFromZero);
                expectedTotal += lineTotal;
                expectedDiscountedTotal += discountedLine;
                Assert.That(product.Price, Is.EqualTo(baseline.Price), $"Product {product.Id} price");
                Assert.That(product.Total, Is.EqualTo(lineTotal), $"Product {product.Id} line total");
                Assert.That(product.DiscountedPrice, Is.EqualTo(discountedLine),
                    $"Product {product.Id} discounted line total");
            }
            Assert.That(cart.Total, Is.EqualTo(expectedTotal).Within(0.01m));
            Assert.That(cart.DiscountedTotal, Is.EqualTo(expectedDiscountedTotal));
        }
    }

    [When(@"I delete cart (\d+)")]
    public async Task WhenIDeleteCart(int id)
    {
        _deleteStarted = DateTimeOffset.UtcNow;
        state.Response = await client.DeleteAsync(id);
    }

    [Then(@"cart (\d+) should be marked as deleted with a valid deletion timestamp")]
    public void ThenValidateDeletedCart(int id)
    {
        var cart = state.Body<Cart>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(cart.Id, Is.EqualTo(id));
            Assert.That(cart.IsDeleted, Is.True);
            Assert.That(cart.DeletedOn, Is.Not.Null);
            Assert.That(cart.DeletedOn, Is.InRange(_deleteStarted.AddMinutes(-2), DateTimeOffset.UtcNow.AddMinutes(2)),
                "Deletion time should correspond to this request (with clock-skew tolerance)");
        }
    }
}
