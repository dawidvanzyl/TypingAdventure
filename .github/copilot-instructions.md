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

## Coding Standards

Full standards are in [`docs/Coding_Standards.md`](../docs/Coding_Standards.md). Key rules:

### C# Style (enforced by `.editorconfig`)
- **Naming**: `PascalCase` for types/methods/properties; `_camelCase` for private fields; `I` prefix for interfaces.
- **Namespaces**: file-scoped syntax (`namespace Foo;`), never block-scoped.
- **Using directives**: outside the namespace declaration.
- **Access modifiers**: always explicit on all members.
- **Braces**: required on all control flow statements.
- **Switch expressions** preferred over `if/else if` chains.
- **`readonly`** on fields not reassigned after construction.
- **One class/record/struct per file**.

### Modern C# Features
- Prefer **primary constructors** (C# 12) for classes that simply receive and store dependencies.
- Use **collection expressions** (`[]`) for empty/inline collections instead of `new List<T>()`.
- Use **pattern matching** in `switch` expressions and `is` checks.

### Async
- All async methods must end with the `Async` suffix.
- Always use `async Task` — never `async void` (except true fire-and-forget event handlers).
- Never block on async code with `.Result` or `.Wait()`.

### Event Handlers
- Use `delegate Task` for custom event handlers (not `EventHandler<T>`), so callers can `await` them and exceptions propagate.

```csharp
// ✅ CORRECT
public delegate Task AiCallPendingHandler(int attemptNumber, TimeSpan waitTime);
public event AiCallPendingHandler OnAiCallPending;
```

### Error Handling
- `try/catch` close to the failing operation; surface errors to the user via the UI layer.
- Catch the narrowest exception type that makes sense — never swallow all exceptions silently.
- **Infrastructure must not write to the console.** Use `ILogger<T>` for diagnostics in non-UI layers.
- Propagate AI client exceptions up; let the UI layer (or retry policy) handle them.

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

## Testing Standards

- **One test project per layer**: `Domain.Game.Tests`, `Application.Tests`, `Infrastructure.AI.Tests`.
- **Test file naming**: `{ClassName}Tests.cs` — one class under test per file.
- **Test method naming**: `{MethodUnderTest}_{Scenario}_{ExpectedResult}`.
- **Structure**: Arrange / Act / Assert (AAA) pattern.
- **Assertions**: always use FluentAssertions (`result.Should().Be(expected)`).
- **Framework**: xUnit (`[Fact]` for single cases; `[Theory]` + `[InlineData]` for multiple inputs).
- **No real network calls** in tests — use fakes/mocks for all external dependencies.
- Prefer **hand-written fakes** over mocking frameworks:

```csharp
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
```

---

## Git & Workflow

- **Small, focused commits** — one concern per commit; describe the "why" in the message.
- **Feature branches** off master (e.g., `feature/add-game-over-detection`).
- Every new feature or behaviour change must include at least one meaningful test.
- PRs should be small, focused, and reviewed before merging.
