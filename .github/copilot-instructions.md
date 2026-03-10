# Copilot Instructions for TypingAdventure

## Project Overview

TypingAdventure is a C# / .NET console application that uses AI to power an interactive text adventure game. Players type free-form actions; the AI narrates the world, enforces game rules, and tracks key facts to maintain story consistency.

For full project goals and game loop description, see [`docs/PROJECT_BRIEF.md`](../docs/PROJECT_BRIEF.md).

---

## Tech Stack

- **Language**: C# with .NET (see `TypingAdventure.slnx` for target framework)
- **AI Integration**: `Cerebras.SDK` — wraps the external AI API
- **Configuration**: `Microsoft.Extensions.Configuration` with `appsettings.json`
- **Test Framework**: xUnit
- **Assertion Library**: FluentAssertions
- **Retry Policy**: Polly (for transient AI call failures)

---

## Architecture

The solution follows **Clean Architecture / Domain-Driven Design** with strict inward-only dependencies:

| Layer | Project naming | Responsibility |
|---|---|---|
| Domain | `Domain.{Name}` | Business model: entities, value objects, domain interfaces |
| Application | `Application` | Use cases, orchestration of domain logic |
| Infrastructure | `Infrastructure.{Name}` | External services, AI client, persistence |
| UI | `UI.{Type}` | Input/output, game loop, user-facing error reporting |

**Dependency rule**: inner layers (Domain → Application) know nothing about outer layers (Infrastructure, UI). Infrastructure implements interfaces defined by Domain/Application.

---

## Build & Test

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build --no-restore

# Run all tests
dotnet test --no-build --verbosity normal
```

---

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

### Event Handler Patterns

- **Use `delegate Task` for custom event handlers** instead of `EventHandler<T>` or `async void` delegates. This ensures callers can `await` the handler and exceptions propagate correctly.
- **Naming**: Delegate types for event handlers follow the `{EventName}Handler` convention (e.g., `AiCallPendingHandler`).
- **Subscriptions**: When subscribing with an async lambda, return `Task.CompletedTask` if there is nothing to await.

```csharp
// ✅ CORRECT: delegate returns Task
public delegate Task AiCallPendingHandler(int attemptNumber, TimeSpan waitTime);

public event AiCallPendingHandler OnAiCallPending;

// ✅ CORRECT: async subscription
engine.OnAiCallPending += async (attemptNumber, waitTime) =>
{
    // UI/console entry-point example: console output is allowed here. Infrastructure must not write directly to the console.
    Console.WriteLine($"Retrying in {waitTime.TotalSeconds}s (attempt {attemptNumber})...");
    await Task.Delay(waitTime);
};

// ❌ INCORRECT: EventHandler is void-returning, so this becomes an async void handler — exceptions are unobservable
public event EventHandler? OnSomethingHappened;

engine.OnSomethingHappened += async (sender, args) =>
{
    await Task.Delay(1000);
    // ...
}; // async void
```

### One Class/Record/Struct per File

- Each file must contain exactly one top-level class, record, or struct.
- This keeps code organized, improves discoverability, and simplifies maintenance.

### readonly Fields

- Fields that are not reassigned after initialization should be marked `readonly`. This is enforced by `.editorconfig` rules as a warning-level preference.

### Modern C# Features

Adopt modern C# language features where they improve clarity. Do not introduce a modern feature purely for novelty — prefer it when it genuinely reduces noise or makes intent clearer.

#### Primary Constructors (C# 12)
Prefer primary constructors for classes and records that simply receive and store dependencies. The corresponding backing field assignment is implicit.

```csharp
// ✅ PREFERRED: primary constructor
public class GenreDetector(IAiClient aiClient)
{
    private readonly IAiClient _aiClient = aiClient;
    ...
}

// ⚠️ ALSO ACCEPTABLE: traditional constructor
public class GenreDetector
{
    private readonly IAiClient _aiClient;
    public GenreDetector(IAiClient aiClient) => _aiClient = aiClient;
}
```

#### Collection Expressions (C# 12)
Use collection expressions (`[]`) for empty or inline collection initialization instead of `new List<T>()` or `Array.Empty<T>()`.

```csharp
// ✅ PREFERRED
public List<AiClientCall> Calls { get; } = [];

// ⚠️ ALSO ACCEPTABLE
public List<AiClientCall> Calls { get; } = new List<AiClientCall>();
```

#### Pattern Matching
Use pattern matching in `switch` expressions and `is` checks to express intent concisely. Enforced and preferred by `.editorconfig`.

```csharp
// ✅ PREFERRED
var label = genre switch
{
    Genre.Fantasy => "Fantasy",
    Genre.Horror  => "Horror",
    _             => "Other"
};
```

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

### Local Handling

- Use `try/catch` where an operation can reasonably fail (I/O, network calls, deserialization, etc.).
- **Catch the narrowest exception type that makes sense** — never use a bare `catch` or `catch (Exception)` to silently swallow all failures.

### Exception Propagation vs. Graceful Degradation

Choose the right strategy based on whether a failure is recoverable at the call site:

- **Propagate** when the caller must know about the failure (e.g., a network call that the UI should report to the user):
  ```csharp
  // ✅ CORRECT: let the AI client failure travel up to the UI layer
  public async Task<string> GetCompletionAsync(string prompt)
  {
      try { ... }
      catch (JsonException ex) { throw new KeyFactExtractionException("Failed to parse key facts.", ex); }
      // HttpRequestException is not caught here — it propagates to the retry policy and then the UI
  }
  ```

- **Degrade gracefully** when a non-critical service can safely return a sensible default and callers should not be interrupted:
  ```csharp
  // ✅ CORRECT: genre detection is best-effort; return a safe default on parse failure only
  public async Task<Genre> DetectAsync(string theme)
  {
      var response = await _aiClient.GetCompletionAsync(...);
      return Enum.TryParse<Genre>(response?.Trim(), ignoreCase: true, out var genre)
          ? genre
          : Genre.Agnostic;
      // AI client exceptions are NOT caught here — they propagate
  }
  ```

### Logging in Infrastructure Layer

- **Infrastructure layers must not write directly to the console.** `Console.WriteLine` in a non-UI layer is a standards violation.
- If diagnostic logging is needed in Infrastructure, inject and use `ILogger<T>` (Microsoft.Extensions.Logging) rather than writing to standard output.
- **User-facing error messages are always the responsibility of the UI layer.** Infrastructure and Application layers communicate failures via exceptions or error result types.

```csharp
// ❌ INCORRECT: Infrastructure writing directly to console
catch (Exception ex) { Console.WriteLine($"An error occurred: {ex.Message}"); throw; }

// ✅ CORRECT: rethrow and let the UI layer handle presentation
catch (Exception) { throw; }

// ✅ ALSO CORRECT: use ILogger for structured diagnostics (when injected)
catch (Exception ex) { _logger.LogError(ex, "AI call failed"); throw; }
```

### AI Calls

- Handle transient failures around AI calls (timeouts, network issues) with a retry policy (e.g., Polly).
- Surface a meaningful message at the UI layer once all retries are exhausted.

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

### Test Doubles (Fakes)

Prefer **hand-written fakes** over mocking frameworks for external dependencies. Fakes are easier to read, easier to extend, and avoid coupling tests to internal call signatures.

A well-designed fake should:
- Implement the same interface as the real dependency.
- Record every call it receives so tests can assert on interactions.
- Use a `Queue<string>` (or similar) to return pre-configured responses, keeping each test self-contained.

```csharp
// ✅ CORRECT: Fake with call tracking and response queuing
public class FakeAiClient : IAiClient
{
    public List<AiClientCall> Calls { get; } = [];
    public Queue<string> Responses { get; } = new();

    public Task<string> GetCompletionAsync(string systemPrompt, string userPrompt)
    {
        Calls.Add(new AiClientCall(systemPrompt, userPrompt));
        return Task.FromResult(Responses.TryDequeue(out var response) ? response : string.Empty);
    }
}

// ✅ CORRECT: usage in a test
[Fact]
public async Task GeneratePremiseAsync_WithValidTheme_CallsAiClientOnce()
{
    // Arrange
    var fake = new FakeAiClient();
    fake.Responses.Enqueue("A brave knight sets out on a perilous quest.");
    var engine = new GameEngine(fake);

    // Act
    await engine.GeneratePremiseAsync("medieval fantasy");

    // Assert
    fake.Calls.Should().HaveCount(1);
}
```

---

## Git & Workflow Conventions

- **Commits**:
  - Prefer **small, focused commits** that do one kind of thing (e.g., "add GameEngine tests", "refactor prompt builder").
  - Write **descriptive commit messages** that explain the "why" more than the "what".
- **Branching**:
  - Work on **feature branches** off the main branch (e.g., `feature/add-game-engine-tests`).
  - Keep branches short-lived; merge once the feature is complete and tested.
- **Reviews (when applicable)**:
  - PRs should be small, focused, and include tests when changing or adding behavior.
  - Address review feedback promptly; if a change is substantial, consider a follow-up PR.

---

## When in Doubt

- Prefer existing patterns in this codebase over introducing new patterns.
- If you're unsure about a style/detail, follow:
  1. Microsoft C# conventions.
  2. The nearest similar code in this project.
