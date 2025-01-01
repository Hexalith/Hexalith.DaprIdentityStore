# Hexalith.DaprIdentityStore.UnitTests

This project contains unit tests for the Hexalith.DaprIdentityStore library, which provides Dapr-based identity storage functionality.

## Overview

The unit tests in this project verify the functionality of the Dapr-based identity store implementation. The tests ensure that the identity store correctly handles user identity information through Dapr's state management.

## Project Structure

- `Stores/` - Contains tests for store implementations
  - Tests for Dapr state store integration
  - Identity information storage and retrieval tests

## Test Dependencies

The project uses:
- XUnit as the testing framework
- FluentAssertions for test assertions
- Hexalith.TestMocks for mocking dependencies

## Running the Tests

The tests can be run using:
- Visual Studio's Test Explorer
- `dotnet test` command from the command line
- Your preferred CI/CD pipeline

## Test Categories

1. Store Tests
   - Identity storage operations
   - State persistence verification
   - Error handling scenarios

## Configuration

The project includes an `appsettings.json` file that contains test-specific configuration settings. This file is automatically copied to the output directory during build.

## Contributing

When adding new tests:
1. Follow the existing test structure and naming conventions
2. Use FluentAssertions for assertions
3. Include both positive and negative test cases
4. Add appropriate XML documentation comments
5. Ensure tests are isolated and don't depend on external state

## Related Projects

- Hexalith.DaprIdentityStore - The main library being tested
- Hexalith.TestMocks - Provides mocking utilities for testing
