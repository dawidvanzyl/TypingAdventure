# Backlog

## Bug Fixes

- [x] **Fix CI/CD Pipeline**
  GitHub Actions installs .NET 8 but the project targets .NET 10. Update the workflow so builds and tests actually run.

- [x] **Fix AiClient Retry Counter Bug**
  The `attempt` counter in the Polly retry policy is a shared closure variable that never resets between calls. After the first retried call, subsequent retries use wrong wait times and will eventually throw `IndexOutOfRangeException`.

- [x] **Secure API Key — appsettings.Development.json**
  Move the live Cerebras API key out of the committed `appsettings.json` into `appsettings.Development.json`, add that file to `.gitignore`, and document the setup in the README.

- [ ] **Unify JSON Libraries**
  The `Cerebras.SDK` uses `Newtonsoft.Json` while the rest of the codebase uses `System.Text.Json`. Migrate the SDK to `System.Text.Json` to eliminate the dual-library dependency.

## Core Gameplay

- [ ] **Game Over Detection**
  Add a `gameOver: true/false` field to all key facts schemas. The engine reads this flag after each turn and sets `GameState.GameOver`, ending the loop. The AI is instructed to set it when the player dies or the story concludes fatally.

- [ ] **Danger Level System**
  Add a `dangerLevel` field (low/medium/high/critical) to all key facts schemas. The AI escalates danger when the player is passive, makes reckless choices, or ignores threats. At critical, combined with the game over flag, the player dies. Prompt engineering reinforces that passivity is fatal.

- [ ] **Win / Story Conclusion**
  The game ends when the AI decides the narrative arc has reached a natural conclusion. The AI sets `gameOver: true` for both death and story completion, with the narrative text itself making the outcome clear.

- [ ] **Inventory System**
  The AI manages item pickup and drops through the key facts JSON based on narrative context. The player can type `inventory` at any time to see a formatted list of currently held items from `GameState.Inventory`.

## Quality & Robustness

- [ ] **Error Handling in the Console UI**
  Wrap the game loop in structured error handling. Catch known exception types (`HttpRequestException`, `KeyFactExtractionException`, `JsonException`, etc.) and display friendly, readable messages instead of raw stack traces.

## New Features

- [ ] **Save / Load Game (SQLite)**
  Persist `GameState` to a SQLite database. Players can save a session and resume it later. Supports multiple saves.

- [ ] **Narrative Tone Selection**
  After choosing a theme, players choose a tone (e.g. comedic, gritty, epic, mysterious). The tone is injected into the narrator system prompt to colour the AI's writing style throughout the session.

- [ ] **Difficulty Levels**
  Easy/normal/hard control how punishing the AI is. Difficulty is injected into prompts — on easy the AI is forgiving, on hard consequences are swift and the danger level escalates faster.

- [ ] **Story Recap Command**
  Players can type `recap` at any time to get a formatted summary of the story so far, drawn from `GameState.StorySummary`.

- [ ] **Better Console UI**
  Improve formatting with visual separators between turns, colour-coded output (narration vs prompts vs system messages), cleaner input prompts, and a styled header/intro screen.
