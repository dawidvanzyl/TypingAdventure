## TypingAdventure Coding Standards

These standards apply to all C# / .NET code in this repository (including tests), and to how we use Git for this project. They are a mix of **must-follow rules** and strong recommendations.

---

## General Principles

- **Prefer clarity over cleverness**: Readability and maintainability beat micro-optimizations and tricks.
- **Prefer async all the way**: Avoid blocking on async code (no `Result`, `Wait()`, etc.).
- **Handle errors where they make sense, surface them in UI**: Use `try/catch` close to the failing operation, but centralize user-facing error reporting and logging in the UI layer (e.g., console entry point).

---

## C# Style & Naming

### Automated Formatting via .editorconfig

This project uses **`.editorconfig`** to enforce consistent code formatting and style rules. Developers should rely on their IDE's EditorConfig support (built-in to Visual Studio, Rider, VS Code, etc.) to automatically apply these rules.

**Key rules enforced by .editorconfig include:**
- **Naming conventions**: Classes, methods, properties use `PascalCase`; private/internal fields use `_camelCase` prefix; interfaces start with `I`.
- **Brace placement**: All control flow statements (`if`, `else`, `for`, `foreach`, `while`, etc.) must have braces.
- **Switch expressions**: Modern `switch` expressions are preferred over traditional `if/else if` chains.
- **Access modifiers**: All non-interface members must have explicit access modifiers (e.g., `public`, `private`, `internal`).
- **Using directives**: Must appear outside the namespace declaration.
- **Namespaces**: Must use file-scoped syntax (e.g., `namespace Foo;` instead of `namespace Foo { ... }`).
- **Line endings**: CRLF (`\r\n`) for all C# source files — enforced by `.editorconfig` (other files should also use CRLF where practical).
- **Indentation**: 4 spaces, keep tabs.

**For the complete list of formatting rules, see the [`.editorconfig`](../.editorconfig) file in the repository root.** Most rules are enforced at the `error` or `warning` level and will be caught during build or by your IDE.

### Naming Conventions (Automated by .editorconfig)

Naming conventions are enforced by `.editorconfig` rules. However, understand the rationale:
- **Classes, structs, interfaces, enums, delegates**: `PascalCase` (e.g., `GameEngine`, `IAiClient`).
- **Methods and properties**: `PascalCase` (e.g., `GeneratePremiseAsync`, `StorySummary`).
- **Fields (private/internal)**: `_camelCase` with leading underscore (e.g., `_aiClient`).
- **Local variables and parameters**: `camelCase`.
- **Interfaces**: Start with `I` (e.g., `IAiClient`).
- **Delegates for event handlers**: `{EventName}Handler` (e.g., `AiCallPendingHandler`).


### Async Methods

- **Naming convention**: Methods performing async work should end with `Async` (e.g., `GeneratePremiseAsync`). This is a **naming convention** that helps callers identify that a method returns a `Task` or `Task<T>`.
- **Patterns**: Use `async/await` syntax and actual async/await patterns are enforced by `.editorconfig` rules (e.g., preference for async methods, handling of `Task` return types).
- **Avoid `async void`**: Do not use `async void` except for event handlers. Prefer `async Task` for fire-and-forget patterns or return an awaitable `Task`.

### One Class/Record/Struct per File

- Each file must contain exactly one top-level class, record, or struct.
- This keeps code organized, improves discoverability, and simplifies maintenance.

### readonly Fields

- Fields that are not reassigned after initialization should be marked `readonly`. This is enforced by `.editorconfig` rules as a warning-level preference.

### Formatting Standards Enforced by .editorconfig

The following rules are automatically enforced by `.editorconfig` and should not require manual attention if your IDE is properly configured:

#### Access Modifiers
- All non-interface members must have explicit access modifiers.
- Examples:
  - `public class GameEngine { }`
  - `private readonly HttpClient _client;`
  - `internal static void Configure() { }`
- Do not rely on implicit access levels; always be explicit about visibility.

#### Using Directives
- Using directives must be placed **outside** the namespace declaration (enforced as an error).
- Example:
  ```csharp
  using System;
  using MyProject.Domain;
  
  namespace MyProject.Application;
  
  public class GameService { ... }
  ```

#### File-Scoped Namespaces
- Prefer **file-scoped namespace syntax** over block-scoped namespaces (enforced as an error).
- Modern syntax (preferred):
  ```csharp
  namespace MyProject.Application;
  
  public class GameService { ... }
  ```
- Legacy syntax (not allowed):
  ```csharp
  namespace MyProject.Application
  {
      public class GameService { ... }
  }
  ```
- Each file contains one namespace declaration at the top, simplifying indentation and improving readability.

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

- **Enums**:
  - Keep enums in an `Enums` folder within the project they belong to (e.g., `Domain.Game/Enums/`).
  - Place enums in the project whose layer owns the concept — domain enums live in a `Domain.*` project, not in Application or Infrastructure.

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

