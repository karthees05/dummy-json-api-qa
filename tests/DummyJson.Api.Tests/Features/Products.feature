@api @products
Feature: Products
  As an API consumer
  I want to search, paginate and update products
  So that I can validate product details and simulated updates

  @smoke
  Scenario: Validate the price of iPhone 13 Pro
    When I search for the product "iPhone 13 Pro"
    Then the response status should be 200 and the content type should be JSON
    And the product "iPhone 13 Pro" should cost 1099.99

  Scenario: Retrieve only the fifteenth product using pagination
    When I request products with limit 1 and skip 14
    Then the response status should be 200 and the content type should be JSON
    And the page should contain only product 15 titled "Wooden Bathroom Sink With Mirror" priced 799.99
    And the pagination metadata should have limit 1 and skip 14

  Scenario: Update the title of product 121
    When I update product 121 with the title "STA API Challenge Updated Product"
    Then the response status should be 200 and the content type should be JSON
    And the updated product should have id 121 and title "STA API Challenge Updated Product"
