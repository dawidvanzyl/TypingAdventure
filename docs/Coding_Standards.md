## TypingAdventure Coding Standards

These standards apply to all C# / .NET code in this repository (including tests), and to how we use Git for this project. They are a mix of **must-follow rules** and strong recommendations.

---

## General Principles

- **Prefer clarity over cleverness**: Readability and maintainability beat micro-optimizations and tricks.
- **Prefer async all the way**: Avoid blocking on async code (no `Result`, `Wait()`, etc.).
- **Handle errors where they make sense, surface them in UI**: Use `try/catch` close to the failing operation, but centralize user-facing error reporting and logging in the UI layer (e.g., console entry point).

---

## C# Style & Naming

- **Baseline**: Follow the official Microsoft C# coding conventions.
- **Naming**:
  - **Classes, structs, interfaces, enums, delegates**: `PascalCase` (e.g., `GameEngine`, `IAiClient`).
  - **Methods and properties**: `PascalCase` (e.g., `GeneratePremiseAsync`, `StorySummary`).
  - **Fields (private)**: `_camelCase` with leading underscore (e.g., `_aiClient`).
  - **Local variables and parameters**: `camelCase`.
  - **Interfaces**: Start with `I` (e.g., `IAiClient`).
- **Braces**:
  - **All `if` statements must have braces**, even for single-line bodies.
  - Apply the same style consistently to `else`, `for`, `foreach`, `while`, etc.
- **Switch usage**:
  - **Prefer `switch`/`switch expressions` where possible** instead of long `if/else if` chains over the same value.
  - Use pattern matching in `switch` where it improves clarity.
- **Async**:
  - Methods doing async work should end with `Async` (e.g., `GeneratePremiseAsync`).
  - Avoid `async void` except for event handlers.
- **One class/record/struct per file**:
  - Each file must contain exactly one top-level class, record, or struct.
  - This keeps code organized, improves discoverability, and simplifies maintenance.
- **Formatting**:
  - Use the project’s default formatter settings where available.
  - Keep methods short and focused; extract helper methods when they grow too large.

---

## Project Structure & Organization

- **Layers & responsibilities (classic DDD / Clean Architecture)**:
  - **Domain**:
    - Contains the core business model: entities, value objects, domain services, domain events.
    - May define domain-level interfaces (e.g., repositories) that express *what* the domain needs, not *how* it is implemented.
    - **Does not depend on** Application, Infrastructure, or UI.
  - **Application**:
    - Contains use cases / application services that orchestrate domain logic and transactions.
    - Coordinates domain objects, but avoids technical concerns (DB, HTTP, etc.).
    - May define application-level interfaces for services it needs from Infrastructure.
    - **Depends on** Domain (and optional shared/core projects), but not on Infrastructure or UI.
  - **Infrastructure**:
    - Contains technical details: persistence, external services, file system, AI clients, etc.
    - Provides concrete implementations of interfaces defined in Domain and Application.
    - **Depends on** Domain and Application (and optional shared/core projects), but they never depend on Infrastructure.
  - **UI**:
    - Delivery mechanisms (e.g., `UI.Console`, future web UIs).
    - Handles input/output and delegates to Application.
    - **Depends on** Application (and optional shared/core projects if explicitly allowed), but not on Infrastructure directly.

- **Project naming conventions**:
  - **UI projects**: `UI.{UiType}` (e.g., `UI.Console`).
  - **Application projects**: `Application` (or `Application.{Name}` if multiple application layers are ever needed).
  - **Domain projects**: `Domain.{Name}` (e.g., `Domain.Game`).
  - **Infrastructure projects**: `Infrastructure.{Name}` (e.g., `Infrastructure.Persistence`, `Infrastructure.AI`).
  - **Shared/Core (optional)**: `Common`, `Shared`, or `Core` for cross-cutting concerns (e.g., primitives, base abstractions) that can be referenced by multiple inner layers.

- **Dependency rules (inward-only)**:
  - **Allowed**:
    - UI → Application (and optionally → Shared/Core if you choose to allow it).
    - Application → Domain (and Shared/Core).
    - Infrastructure → Domain, Application (to implement their interfaces), and Shared/Core.
  - **Not allowed**:
    - Domain → Application, Infrastructure, or UI.
    - Application → UI or Infrastructure concrete types (only abstractions/interfaces defined in Domain/Application/Shared).
    - UI → Infrastructure directly, except in very explicit, documented edge cases.
  - **Principle**: inner layers (Domain, then Application) **know nothing about** outer layers (Infrastructure, UI); outer layers depend on and implement inner-layer abstractions.

- **Interfaces**:
  - Keep interfaces in an `Interfaces` folder (or clearly named equivalent) within each project.
  - Domain defines interfaces that represent domain-level abstractions (e.g., `IGameRepository`).
  - Application defines interfaces for application-level services it needs from Infrastructure.
  - Infrastructure implements these interfaces; it should not define domain-specific abstractions that Domain must depend on.

- **Separation of concerns**:
  - UI handles interaction and delegates to Application.
  - Application coordinates domain operations and orchestrates workflows.
  - Domain holds business rules and invariants, free from technical details.
  - Infrastructure provides technical capabilities behind interfaces.
  - New behavior should generally be added by:
    1. Extending Domain/Application models and interfaces.
    2. Adding or updating Infrastructure implementations.
    3. Wiring via UI at the edge.

---

## Error Handling & Logging

- **Local handling**:
  - Use `try/catch` where an operation can reasonably fail (I/O, network calls, deserialization, etc.).
  - Catch the narrowest exception type that makes sense.
- **UI-layer responsibility**:
  - The UI/entry layer is responsible for how errors are communicated to the user (logging, messages).
  - Non-UI layers should throw exceptions or return error results rather than writing directly to the console, unless there is a clear reason.
- **AI calls**:
  - Handle transient failures around AI calls (timeouts, network issues) and surface a meaningful message at the UI layer.

---

## Unit Testing Standards

### Test Project Organization

- **One test project per layer** (not per project):
  - `TypingAdventure.Domain.Tests`
  - `TypingAdventure.Application.Tests`
  - `TypingAdventure.Infrastructure.Tests`
- **UI exception**: `TypingAdventure.UI.Web.Tests` for web-based UIs; no tests for Console UI.

### Test File Naming & Scope

- Name test files after the class under test: `{ClassName}Tests.cs` (e.g., `GameEngineTests.cs`).
- Only tests for that single class may live in each test file.

### Test Structure (AAA Style)

- Always use the **Arrange / Act / Assert** pattern.
- Keep tests **atomic** and independent.

### Test Data Strategies

- Use `[Theory]` + `[InlineData]` for testing multiple inputs to the same test.
- Use `IClassFixture<T>` for tests that require shared state between tests.
- Use plain `[Fact]` for single, atomic test cases.

### Test Method Naming

- Follow the pattern: `{MethodUnderTest}_{Scenario}_{ExpectedResult}`.
- Examples:
  - `GeneratePremise_WithValidInput_ReturnsValidStory`
  - `GeneratePremise_WithNullInput_ThrowsArgumentNullException`
  - `ApplyTurn_WithPlayerAction_AppendsResponseToStoryLog`

### Test Isolation

- Tests must pass when run individually or in any order.
- Do not rely on execution order or shared mutable state (except via `IClassFixture<T>`).

### Assertion Library

- Use **FluentAssertions** for all assertions.
- Prefer fluent syntax: `result.Should().Be(expected)` over `Assert.Equal(expected, result)`.
- Example:
  ```csharp
  // Instead of:
  Assert.Equal("Expected", result);
  
  // Use:
  result.Should().Be("Expected");
  ```

### Framework

- Use **xUnit**.

### General Guidelines

- Tests must be **deterministic**: no real network calls, no dependence on secrets.
- Use **fakes/mocks** for external dependencies (e.g., AI client).
- **Every new feature should come with at least one meaningful test**.
- When changing existing behavior, update or add tests to reflect the new expected behavior.

---

## Git & Workflow Conventions

- **Commits**:
  - Prefer **small, focused commits** that do one kind of thing (e.g., “add GameEngine tests”, “refactor prompt builder”).
  - Write **descriptive commit messages** that explain the “why” more than the “what”.
- **Branching**:
  - Work on **feature branches** off the main branch (e.g., `feature/add-game-engine-tests`).
  - Keep branches short-lived; merge once the feature is complete and tested.
- **Reviews (when applicable)**:
  - PRs should be small, focused, and include tests when changing or adding behavior.
  - Address review feedback promptly; if a change is substantial, consider a follow-up PR.

---

## When in Doubt

- Prefer existing patterns in this codebase over introducing new patterns.
- If you’re unsure about a style/detail, follow:
  1. Microsoft C# conventions.
  2. The nearest similar code in this project.

