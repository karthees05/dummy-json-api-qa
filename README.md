# DummyJSON API BDD framework

C# API tests using Reqnroll, RestSharp and NUnit. Nine Gherkin scenarios cover authentication, users, products and carts against https://dummyjson.com.

## Run tests

Requires the .NET 10 SDK and an internet connection. Run from the repository root:

```sh
dotnet restore DummyJson.sln
dotnet build DummyJson.sln
dotnet test DummyJson.sln
```

Run only smoke tests with `dotnet test DummyJson.sln --filter "TestCategory=smoke"`.

## Configuration

Defaults are in `tests/DummyJson.Api.Tests/appsettings.json`. Override them with `DUMMYJSON_BASE_URL`, `DUMMYJSON_USERNAME` or `DUMMYJSON_PASSWORD`.

## HTML report

Requires Node.js and Java 21 in addition to .NET:

```sh
npm ci
npm run allure:clean
npm run test:bdd
npm run allure:generate
npm run allure:open
```

The report opens at http://127.0.0.1:5050. A standalone copy is available at `artifacts/allure-report/index.html`. If tests fail, still run the generate and open commands to inspect the report.

## GitHub Actions

The workflow in `.github/workflows/api-tests.yml` runs on pushes, pull requests and manual dispatch. Download the `allure-html-report` artifact from the workflow run to view the report.
