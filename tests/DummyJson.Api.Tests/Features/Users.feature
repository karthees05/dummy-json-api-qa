@api @users
Feature: Users
  As an API consumer
  I want to filter users and submit complete user details
  So that I can validate the user API contract

  Scenario: Count all users with red hair
    When I filter all users by hair colour "Red"
    Then the response status should be 200 and the content type should be JSON
    And exactly 25 users should have hair colour "Red"
    And the filtered users should match an independent count of all users with hair colour "Red"

  Scenario: Add a user with hair and a complete address
    Given a new user with the following details
      | Field       | Value              |
      | FirstName   | Alex               |
      | LastName    | Taylor             |
      | Age         | 31                 |
      | Gender      | female             |
      | HairColor   | Red                |
      | HairType    | Curly              |
      | Street      | 10 Test Street     |
      | City        | New York           |
      | State       | New York           |
      | StateCode   | NY                 |
      | PostalCode  | 10001              |
      | Latitude    | 40.7128            |
      | Longitude   | -74.0060           |
      | Country     | United States      |
    When I add the new user
    Then the response status should be 201 and the content type should be JSON
    And the created user should have a positive id and all the submitted details
