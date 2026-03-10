# Typing Adventure

AI-driven text adventure engine built in C#.

The player types actions.
The AI narrates the world and enforces rules.

## What It Does

* Second-person narrative
* No player action auto-completion
* Enforces world logic
* Tracks key facts to maintain consistency
* Allows consequences (including death)

## Core Structure

* `GameState` – stores story log and summarised key facts
* `AiClient` – wraps the AI provider API
* Narrator system prompt – defines game rules
* Periodic fact summarisation to reduce token usage

See also: `docs/Coding_Standards.md` for coding style, testing expectations, and Git workflow conventions.

## Run

1. Create `src/UI.Console/appsettings.Development.json` with your AI provider API key:

```json
{
  "AiClient": {
    "ApiKey": "<your-api-key>"
  }
}
```

> This file is git-ignored and must never be committed.

2. Run:

```bash
dotnet run --project src/UI.Console
```

## Purpose

Experimenting with:

* AI as a rule-bound game engine
* State compression via summarisation
* Emergent narrative without hardcoded branching
