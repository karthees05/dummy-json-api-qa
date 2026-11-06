@api @auth
Feature: Authentication
  As an API consumer
  I want to authenticate and retrieve my own profile
  So that I can identify the current user securely

  @smoke
  Scenario: Authenticate a user and store the access token
    Given the configured DummyJSON user credentials
    When I authenticate the user
    Then the response status should be 200 and the content type should be JSON
    And a non-empty access token should be stored for this scenario

  Scenario: Validate the current authenticated user's identity
    Given I have authenticated as the configured DummyJSON user
    When I request the current authenticated user
    Then the response status should be 200 and the content type should be JSON
    And the current user should have id 1, first name "Emily" and last name "Johnson"
