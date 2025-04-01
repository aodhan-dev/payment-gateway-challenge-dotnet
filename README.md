# Assumptions
- A payment request amount of 0 fails validation to keep noise at a minimum, I would double check this with the product owner to make sure though.
- Payments that fail validation are not stored in the database, cause an HTTP 400 response and have their status marked as Rejected.

# Design Decisions
- A focus on readability and maintainability, extracting logic into services and using interfaces to allow for easy testing.
- A focus on testability, using dependency injection to allow for easy mocking of services.

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