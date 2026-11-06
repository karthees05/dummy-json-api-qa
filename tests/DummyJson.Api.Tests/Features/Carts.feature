@api @carts
Feature: Carts
  As an API consumer
  I want to create and delete simulated carts
  So that I can validate cart contents and discount percentages

  Scenario: Add five products and validate their total discount percentage
    Given a cart for user 1 containing the following products
      | ProductId | Quantity | DiscountPercentage |
      | 1         | 1        | 10.48              |
      | 2         | 2        | 18.19              |
      | 3         | 1        | 9.84               |
      | 4         | 3        | 12.16              |
      | 5         | 1        | 11.44              |
    When I add the cart
    Then the response status should be 201 and the content type should be JSON
    And the cart should contain exactly the five requested products and quantities
    And the sum of the product discount percentages should be 62.11
    And the cart monetary totals should agree with the product prices and discounts

  Scenario: Delete an existing cart
    When I delete cart 1
    Then the response status should be 200 and the content type should be JSON
    And cart 1 should be marked as deleted with a valid deletion timestamp
