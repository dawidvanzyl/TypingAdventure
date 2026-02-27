# TypingAdventure - Project Brief

## Overview
TypingAdventure is a .NET 10 interactive text-based adventure game that uses AI to generate dynamic, branching narratives. Players provide inputs that shape the direction of the story, with the AI serving as a creative narrator.

**Repository**: https://github.com/dawidvanzyl/TypingAdventure  
**Platform**: .NET 10  
**Type**: Console Application (Interactive Text Adventure)

---

## Project Goals
1. Create an engaging interactive storytelling experience where player choices matter
2. Leverage AI to generate creative, contextually-aware narratives
3. Build a reusable framework for AI-driven interactive fiction
4. Maintain story coherence and consistency across multiple turns

---

## Architecture Overview

### Core Components
- **TypingAdventure** (Main Project)
  - Console-based entry point and game loop
  - Handles user input and output
  - Orchestrates game state and AI interactions
  
- **Cerebras** (Referenced Project)
  - Likely contains AI client implementation
  - Manages API communication and completions

### Conceptual Architecture & Responsibilities
- **AI client layer**: Handles communication with external AI services and abstracts model-specific details.
- **Prompt construction layer**: Shapes system and user prompts for different game phases (premise, per-turn narration, summarization).
- **Game state layer**: Maintains story progression, important facts, and overall game status (including when the game has ended).

---

## Technology Stack
- **.NET Platform**: Modern .NET (currently targeting .NET 10)
- **Configuration**: `Microsoft.Extensions.Configuration` with JSON-based settings
- **Language Features**: 
  - Implicit usings enabled
  - Nullable reference types disabled
  - Top-level statements

### Dependencies
- `Microsoft.Extensions.Configuration` and related JSON/binder packages
- `Cerebras` (local project reference for AI integration)

---

## Game Loop Flow
This is the intended high-level flow of the game, regardless of internal implementation details:
1. **Initialization**: Player provides a theme.
2. **Premise Generation**: The AI creates an opening premise based on the theme.
3. **Main Loop** (while the game is active):
   - Accept player input (narrative action/choice).
   - Generate an AI response using the current game context.
   - Update the evolving story log.
   - Maintain a rolling story summary to keep context manageable.
4. **Game End**: The loop terminates when the game state indicates the story has concluded.

---

## Current Implementation Details
- Story configuration is loaded from external settings (e.g., `appsettings.json`) rather than hard-coded values.
- Story context is maintained through:
  - A full conversation history log capturing the evolving narrative.
  - A compressed story summary used to keep prompts within the model's context window.
  - A game state object that tracks game progress, important facts, and overall status.
- The summary is refreshed regularly (typically every turn) to balance narrative continuity with token efficiency.

### Key Facts System
- **Purpose**: Extract and track durable, game-relevant facts from the evolving story to maintain world state consistency.
- **Implementation**: After each turn, the `GameEngine.ApplyTurnAsync` method calls a private `AddKeyFactsAsync` method that:
  - Calls `GetKeyFactsAsync` to extract facts from the AI
  - Deduplicates facts and adds only new ones to `GameState.KeyFacts`
  - Uses the `Application.Prompts.KeyFacts` class for the system prompt and prompt builder
- **Prompt System**: The extraction logic uses:
  - `Application.Prompts.KeyFacts.SystemPrompt`: Instructs the model to extract factual information (not narrative), format as `<Category>: <Value>`, avoid speculation, and not repeat existing facts
  - `Application.Prompts.KeyFacts.BuildKeyFactsPrompt(story)`: Builds the user prompt containing the story text
- **Storage**: Extracted facts are stored in `GameState.KeyFacts` as a `List<string>`.
- **Usage**: Facts are included in the turn prompt (via `Application.Prompts.Narrator.BuildTurnPrompt`) so the AI respects established state when generating responses.
- **Error Handling**: If key fact extraction fails, a `KeyFactExtractionException` is thrown; this is caught at the call site and can be logged or handled as needed.

---

## Project Structure
At a high level, the solution consists of a console-based game project and a separate AI client library project, keeping gameplay logic and AI integration concerns decoupled.

---

## Configuration
- Application uses `appsettings.json` for configuration
- Likely includes AI API credentials and endpoint settings

---

## Future Enhancement Areas
- Story branching visualization
- Persistent game saves
- Multiple narrative styles/tones
- Difficulty levels and constraints
- Multiplayer story generation
- Performance optimization for token management

---

## Development Guidelines for Agents
When working on tasks for this project:
1. Maintain the async/await pattern used throughout
2. Follow the existing prompt builder pattern for consistency
3. Ensure story coherence by respecting `GameState` context
4. Test narrative quality with various themes and inputs
5. Monitor AI response quality and token usage
6. Consider game-over conditions and story conclusion logic
