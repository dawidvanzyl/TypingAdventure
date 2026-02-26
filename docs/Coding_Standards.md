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
- **Formatting**:
  - Use the project’s default formatter settings where available.
  - Keep methods short and focused; extract helper methods when they grow too large.

---

## Project Structure & Organization

- **Interfaces**:
  - **Place interfaces in an `Interfaces` folder** (or a clearly named equivalent) within each project.
  - Keep interface files small and focused; avoid large, catch-all interfaces.
- **Separation of concerns**:
  - Keep **AI integration**, **game engine logic**, and **UI (console)** separate.
  - New behavior should generally plug in via the engine layer, not directly from the UI to the AI client.

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

## Testing Expectations

- **Unit tests**:
  - **Every new feature should come with at least one meaningful test**.
  - When changing existing behavior, update or add tests to reflect the new expected behavior.
- **Test style**:
  - Keep tests deterministic: no real network calls, no dependence on secrets.
  - Prefer small, focused tests that assert on behavior and invariants rather than exact long strings, unless necessary.
  - Use fakes/mocks for external dependencies (e.g., AI client) rather than hitting real services.

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

