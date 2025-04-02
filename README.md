# Assumptions
- A payment request amount of 0 fails validation to keep noise at a minimum, I would double check this with the product owner to make sure though.
- Payments that fail validation are not stored in the database, cause an HTTP 400 response and have their status marked as Rejected.

# Design Decisions
- A focus on readability and maintainability, extracting logic into services and using interfaces to allow for easy testing.
- A focus on testability, using dependency injection to allow for easy mocking of services.

# Improvements
- Add integration tests to test the API end to end.
- Add more validation to the payment request, such as checking the card number is a valid card number.
- Add more logging to the application to help with debugging.
- Add logging and monitoring resources to expose metrics and logs for each environment it is deployed to.
- Add real database storage/event sourcing to store each step of the payment process, to ensure that the payment process is auditable.
- Add Authentication and Authorization to the API to ensure that only authorized users can make payments.

## How to run the application
- Run the bank simulator by running `docker-compose up` in the imposters directory.
- Run the PaymentGateway.Api project in Visual Studio or by running `dotnet run` in the PaymentGateway.Api directory.
- The API will be available at `https://localhost:7092`.
- The API has a swagger UI available at `https://localhost:7092/swagger/index.html`.

- To run the tests, run `dotnet test` in the test directory or run the tests in Visual Studio.

## Instructions for candidates

This is the .NET version of the Payment Gateway challenge. If you haven't already read this [README.md](https://github.com/cko-recruitment/) on the details of this exercise, please do so now. 

## Template structure
```
src/
    PaymentGateway.Api - a skeleton ASP.NET Core Web API
test/
    PaymentGateway.Api.Tests - an empty xUnit test project
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```

Feel free to change the structure of the solution, use a different test library etc.