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
* `AiClient` – wraps the OpenAI Chat API
* Narrator system prompt – defines game rules
* Periodic fact summarisation to reduce token usage

See also: `.github/copilot-instructions.md` for coding style, testing expectations, and Git workflow conventions.

## Run

1. Add your AI client API key
2. Configure model (default: `gpt-5-mini`)
3. Run:

```bash
dotnet run
```

## Purpose

Experimenting with:

* AI as a rule-bound game engine
* State compression via summarisation
* Emergent narrative without hardcoded branching
