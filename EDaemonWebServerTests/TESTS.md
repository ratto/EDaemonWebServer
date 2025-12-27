# EDaemonWebServerTests Technical Documentation

## Overview

This document describes the technical details and recommended testing conventions for the `EDaemonWebServerTests` project (see `EDaemonWebServerTests.csproj`). It is intended to serve as a reference for human developers and automation agents working against this test project.

## Project metadata

- Target framework: `net8.0` (modern .NET 8 SDK-style project)
- `ImplicitUsings` enabled (reduces using declarations in test files)
- `Nullable` enabled (nullable reference types are on)
- `IsTestProject` is set to `true`
- `IsPackable` is set to `false`

## Important project references and packages

- `Microsoft.NET.Test.Sdk` — test host and runner integration for .NET
- `xunit` and `xunit.runner.visualstudio` — primary test framework and Visual Studio runner
- `Moq` — mocking library used for interaction-style (mockist) tests and stubs
- `coverlet.collector` — code coverage collector for CI and local runs
- Explicit `ProjectReference` to the production project: `..\\EDaemonWebServer\\EDaemonWebServer.csproj`
- A `Using` entry includes `Xunit` so test files may rely on xUnit attributes without additional using lines (depends on implicit usings and project settings)

## Project file customizations

- The csproj removes (excludes) files under `Repositories\\NovaPasta\\**` from compile and content; this indicates some folders are intentionally omitted from the tests build.

## Testing conventions and structure

- Folder layout: keep tests grouped by feature or by the class under test. For example:
  - `Controllers/` — tests for controller-like components
  - `Services/` — tests for service logic
  - `Repositories/` — tests for data access adapters (if present)

- Test class naming: use `ClassNameTests` or `<UnitOfWork>Tests` (e.g., `LoginServiceTests`).
- Test method naming: prefer descriptive names showing scenario and expected outcome, for example: `Authenticate_WhenCredentialsValid_ReturnsToken`.
- Keep tests small and focused: one assertion idea per test where practical.
- Follow Arrange/Act/Assert (AAA) structure inside each test to keep readability consistent.

## xUnit specifics

- Use `[Fact]` for single-case tests and `[Theory]` with `[InlineData]` or other data sources for parameterized cases.
- Use `IClassFixture<T>` and `Collection` fixtures to share expensive setup between tests when necessary, but avoid accidental coupling between tests.
- If tests use async APIs, mark them `async Task` and await results; xUnit handles async tests directly.

## London-style (mockist) testing guidance

### The "London school" (mockist) style emphasizes testing interactions between objects by using mocks to verify behavior and communication across boundaries rather than only asserting final state. When applying this style in this project:

- Use `Moq` to create mocks for collaborators (dependencies) that are external to the unit under test — for example, HTTP clients, message buses, repositories behind an interface.
- Prefer mocking at boundaries (interfaces) rather than internal implementation details.
- Use `MockBehavior.Strict` when you want tests to fail if unexpected interactions occur. Use `MockBehavior.Loose` for more forgiving mocks when verifying only a subset of interactions.
- Set expectations with `Setup(...)` and verify behavior with `Verify(...)`. Example pattern:

  - Arrange: create `var dependency = new Mock<IMyDependency>(MockBehavior.Strict);` and call `dependency.Setup(d => d.Send(It.IsAny<Message>())).Returns(Task.CompletedTask);`
  - Act: invoke the method under test that should call the dependency
  - Assert: call `dependency.Verify(d => d.Send(It.Is<Message>(m => m.Type == "expected")), Times.Once);`

- Use `It.Is<T>(...)` to match complex arguments rather than relying on exact reference equality.
- Avoid over-mocking. If the test becomes a maintenance burden because it mirrors too much implementation detail, consider switching to a state-based (classic/Detroit) test or extract a seam to make behavior easier to verify.

## When to choose London-style tests

- Use for coordination-heavy code (message orchestration, command handlers, event producers) where the main responsibility is to call collaborators in a correct order or with correct messages.
- Avoid for pure algorithmic logic where state-based assertions are simpler and clearer.

## Test reliability and isolation

- Keep tests hermetic: avoid touching the network, file system, or other shared resources. Replace those with fakes or mocks.
- Use deterministic data and RNG seeding if necessary.
- Be mindful of xUnit parallelization: prefer test isolation and name unique shared resources or place tests that must not run concurrently into a single collection fixture with `[Collection("NonParallel")]`.

## Code coverage and CI

- Use `dotnet test` in CI with coverage collection enabled via `coverlet.collector` or a separate coverage job. Typical invocation: `dotnet test --collect:"XPlat Code Coverage"`.
- Require and gate merges on code coverage thresholds if desired, but favor meaningful tests over inflating coverage numbers.

## Examples and quick tips

- Example verify with Moq:
  - `var repo = new Mock<IRepository>(MockBehavior.Strict);`
  - `repo.Setup(r => r.Save(It.IsAny<Entity>())).Returns(Task.CompletedTask);`
  - after act: `repo.Verify(r => r.Save(It.Is<Entity>(e => e.Id == expectedId)), Times.Once);`

- Example xUnit fact:
  - `[Fact]
    public void Calculate_WhenInputValid_ReturnsExpected()`

## Maintenance and readability

- Keep tests as documentation: name them to tell the story of requirements and edge cases.
- Remove fragile tests that break on harmless refactors; instead, refactor code to make seams testable or convert tests to higher-level integration tests where appropriate.

## Where to look in the repository

- `EDaemonWebServerTests.csproj` — authoritative list of packages, references, and project settings.
- Test folders under `EDaemonWebServerTests/` — actual test implementations and examples of patterns used.

## This document should be updated if the project's test runner, mocking library, coverage tooling, or test conventions change.
