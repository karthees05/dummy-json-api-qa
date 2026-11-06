using DummyJson.Api.Clients;
using DummyJson.Api.Models;
using DummyJson.Api.Tests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.Api.Tests.StepDefinitions;

[Binding]
public sealed class ProductsSteps(ProductsClient client, ScenarioState state)
{
    [When(@"I search for the product ""([^""]*)""")]
    public async Task WhenISearchForProduct(string title) => state.Response = await client.SearchAsync(title);

    [Then(@"the product ""([^""]*)"" should cost (\d+\.\d+)")]
    public void ThenValidatePrice(string title, decimal price)
    {
        var matches = state.Body<ProductsResponse>().Products.Where(product => product.Title == title).ToList();
        Assert.That(matches, Has.Count.EqualTo(1), "Exactly one product must match the full title");
        Assert.That(matches[0].Price, Is.EqualTo(price));
    }

    [When(@"I request products with limit (\d+) and skip (\d+)")]
    public async Task WhenIRequestPage(int limit, int skip) => state.Response = await client.GetPageAsync(limit, skip);

    [Then(@"the page should contain only product (\d+) titled ""([^""]*)"" priced (\d+\.\d+)")]
    public void ThenValidateSingleProduct(int id, string title, decimal price)
    {
        var products = state.Body<ProductsResponse>().Products;
        Assert.That(products, Has.Count.EqualTo(1), "Pagination must return only the fifteenth product");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(products[0].Id, Is.EqualTo(id));
            Assert.That(products[0].Title, Is.EqualTo(title));
            Assert.That(products[0].Price, Is.EqualTo(price));
        }
    }

    [Then(@"the pagination metadata should have limit (\d+) and skip (\d+)")]
    public void ThenValidatePagination(int limit, int skip)
    {
        var page = state.Body<ProductsResponse>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(page.Limit, Is.EqualTo(limit));
            Assert.That(page.Skip, Is.EqualTo(skip));
            Assert.That(page.Total, Is.GreaterThanOrEqualTo(skip + limit));
        }
    }

    [When(@"I update product (\d+) with the title ""([^""]*)""")]
    public async Task WhenIUpdateProduct(int id, string title) => state.Response = await client.UpdateTitleAsync(id, title);

    [Then(@"the updated product should have id (\d+) and title ""([^""]*)""")]
    public void ThenValidateUpdatedProduct(int id, string title)
    {
        var product = state.Body<Product>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(product.Id, Is.EqualTo(id));
            Assert.That(product.Title, Is.EqualTo(title));
        }
    }
}
