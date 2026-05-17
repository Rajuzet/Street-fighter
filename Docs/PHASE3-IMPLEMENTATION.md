# Phase 3 Implementation: Character Roster, UI/HUD, and Game Modes

## Overview

Phase 3 introduces the complete character roster system with unlock tracking, character select screen, in-match HUD, floating damage numbers, and the game mode framework including Versus, Arcade, and Training modes.

## New Files Created

### Character Roster & Selection

#### `UnityClient/Assets/Scripts/Data/CharacterRosterData.cs`
- **Purpose**: ScriptableObject defining a fighter entry in the roster.
- **Features**:
  - Unique `characterId` and `displayName`
  - Portrait, icon, and prefab references
  - Base profile and stat overrides (health, stamina, attack, defense)
  - Unique moves list (special/super/ultra)
  - Color palette/costume variants
  - Unlock conditions and default-unlocked flag

#### `UnityClient/Assets/Scripts/Data/CharacterColorData.cs`
- **Purpose**: ScriptableObject for color palette/costume variants.
- **Features**:
  - Tint colors for material tinting
  - Material overrides for full skin swaps
  - Shader tint color support
  - `ApplyToMaterial` and `ApplyToRenderer` methods

#### `UnityClient/Assets/Scripts/Characters/CharacterDatabase.cs`
- **Purpose**: Runtime service that loads all roster entries and provides lookup by ID or index.
- **Features**:
  - Dictionary-based fast lookup by `characterId`
  - Index-based access for grid navigation
  - Runtime addition support (DLC unlock)
  - `IGameService` integration for `ServiceLocator`

#### `UnityClient/Assets/Scripts/Characters/CharacterSetup.cs`
- **Purpose**: Spawns a character instance from roster data and applies overrides.
- **Features**:
  - `InitializeFromRoster` method for runtime setup
  - Profile/stats injection from `CharacterRosterData`
  - Color palette application via `CharacterColorData`
  - Move injection hooks for `CombatSystemManager`

#### `UnityClient/Assets/Scripts/Characters/CharacterUnlockManager.cs`
- **Purpose**: Tracks unlock state for characters and color variants.
- **Features**:
  - Default unlock initialization from roster data
  - Character and color unlock methods
  - Save/restore via `UnlockSaveData` serializable container
  - Reset support for new game

#### `UnityClient/Assets/Scripts/UI/CharacterSelectController.cs`
- **Purpose**: Character select screen controller with navigation and lock-in.
- **Features**:
  - Grid generation from `CharacterDatabase`
  - Two-player selection (P1/P2) with independent cursors
  - Color picker cycling per player
  - Lock-in confirmation with events
  - Portrait, name, and color preview updates

### UI/HUD Systems

#### `UnityClient/Assets/Scripts/UI/HUDManager.cs`
- **Purpose**: In-match HUD for health, stamina, combo, timer, and portraits.
- **Features**:
  - Health bars with smooth drain bar animation
  - Stamina bar fill updates
  - Combo counter display with auto-hide
  - Match timer display (MM:SS)
  - Portrait sprites per player
  - Round win indicator management

#### `UnityClient/Assets/Scripts/UI/DamageNumberSpawner.cs`
- **Purpose**: Floating damage numbers at hit locations.
- **Features**:
  - Object pooling (default pool size: 20)
  - Color-coded text: normal (white), critical (red), combo (yellow)
  - Scale pop for critical hits
  - Upward float + fade animation
  - `DamageResult` event callback support

#### `UnityClient/Assets/Scripts/UI/CharacterPortrait.cs`
- **Purpose**: Portrait display with tint overlay and animations.
- **Features**:
  - Sprite assignment with optional color tint
  - Background color for player identity
  - Fade in/out transitions
  - Scale-pop animation on change
  - Clear/reset support

### Game Modes

#### `UnityClient/Assets/Scripts/Core/GameModeBase.cs`
- **Purpose**: Abstract base class for all game modes.
- **Features**:
  - Player spawn point references
  - Match timer management
  - `InitializeMode`, `StartMatch`, `UpdateMatch`, `EndMatch` hooks
  - `SpawnPlayers` with `CharacterSetup` integration
  - Health-based winner evaluation
  - Abstract methods for mode-specific implementation

#### `UnityClient/Assets/Scripts/Core/VersusMode.cs`
- **Purpose**: 1v1 Versus mode with best-of rounds.
- **Features**:
  - Configurable `roundsToWin`
  - Round start/end delays
  - Player spawns from roster selections
  - K.O. detection with double K.O. handling
  - Health/stamina/combo HUD updates from live stats
  - Victory/defeat animation triggers
  - Match completion state

#### `UnityClient/Assets/Scripts/Core/ArcadeMode.cs`
- **Purpose**: Arcade ladder with progressive opponents.
- **Features**:
  - `ArcadeOpponent` list for stage definition
  - Opponent data, color, timer, and boss flags per stage
  - AI behavior preset mapping
  - Continue support with counter
  - Stage progression tracking

#### `UnityClient/Assets/Scripts/Core/TrainingMode.cs`
- **Purpose**: Training mode with dummy and frame data.
- **Features**:
  - Infinite health/stamina toggles
  - Frame data display (startup/active/recovery/advantage)
  - Hitbox visualization toggle
  - Dummy behavior presets (standing, crouching, blocking, counter)
  - Position reset
  - Recording/playback hooks for input sequences

#### `UnityClient/Assets/Scripts/Core/MatchStateMachine.cs`
- **Purpose**: Central match state machine.
- **Features**:
  - States: None, Intro, RoundStart, Fighting, RoundEnd, MatchEnd
  - UnityEvents for each state transition
  - Configurable durations for intro, round start, round end
  - Pause/resume with `Time.timeScale` control
  - Timer-based auto-transitions

#### `UnityClient/Assets/Scripts/Core/RoundManager.cs`
- **Purpose**: Round UI announcements and win tracking.
- **Features**:
  - Round start text ("ROUND N")
  - FIGHT text display
  - K.O. text
  - Round end winner announcement
  - Match end winner display
  - Animator trigger integration
  - Round win counters per player

## Updated Files

#### `UnityClient/Assets/Scripts/Data/CharacterProfileData.cs`
- **Added**: Fighter-specific stat fields
  - `baseHealth` (default: 1000)
  - `baseStamina` (default: 100)
  - `baseAttack` (default: 1.0, multiplier)
  - `baseDefense` (default: 1.0, multiplier)
  - `staminaRegenRate` (default: 10/sec)

#### `UnityClient/Assets/Scripts/Data/CombatMoveData.cs`
- **Added**: `characterId` field for roster-specific move linkage
- **Added**: `IsCharacterSpecific` convenience property

#### `UnityClient/Assets/Scripts/Characters/AnimationIds.cs`
- **Added**: Phase 3 hashes
  - `VictoryTrigger`
  - `DefeatTrigger`
  - `IsDead`

#### `UnityClient/Assets/Scripts/Characters/CharacterAnimationController.cs`
- **Added**: Match end state methods
  - `SetVictory(bool)` - triggers victory animation
  - `SetDefeat(bool)` - triggers defeat animation
  - `SetDead(bool)` - sets death bool parameter

#### `UnityClient/Assets/Scripts/Characters/PlayerCombatController.cs`
- **Added**: `characterId` field for roster binding
- **Added**: `BindCharacterMoves(CharacterRosterData)` method for per-character move ID setup

#### `UnityClient/Assets/Scripts/Characters/CombatSystemManager.cs`
- **Added**: `LoadCharacterMoves(CharacterRosterData)` method for roster-based move loading
- **Added**: `characterId` field for move filtering
- Merges universal moves with character-specific unique moves from roster

## Architecture

### Namespace Conventions
- `StreetFighter.Characters` - Character-related classes
- `StreetFighter.Core` - Game modes, state machine, round manager
- `StreetFighter.UI` - HUD, select screen, portraits, damage numbers
- `StreetFighter.Data` - ScriptableObjects and data containers

### Data Flow
1. `CharacterDatabase` loads all `CharacterRosterData` assets at startup
2. `CharacterSelectController` reads from `CharacterDatabase` and `CharacterUnlockManager`
3. On confirm, selections flow to game mode (`VersusMode`, `ArcadeMode`, etc.)
4. Game mode spawns players via `CharacterSetup.InitializeFromRoster()`
5. `CharacterSetup` applies stats, colors, and moves to the prefab instance
6. `CombatSystemManager.LoadCharacterMoves()` loads character-specific moves
7. `HUDManager` displays live stats from `CharacterStats` and `StaminaSystem`
8. `DamageNumberSpawner` listens to hit events and spawns floating text
9. `MatchStateMachine` and `RoundManager` drive match flow and announcements

## Integration Points

### Phase 2 Integration
- `CombatSystemManager` now loads moves from roster data
- `PlayerCombatController` binds character-specific moves
- `CharacterAnimationController` supports victory/defeat states
- `HitDetectionSystem` → `DamageNumberSpawner` via `DamageResult` events

### Phase 4 Preparation
- `CharacterDatabase` and `CharacterUnlockManager` ready for server sync
- `GameModeBase` abstract structure supports networked mode extensions
- `MatchStateMachine` state transitions can be server-authoritative
- `CombatSystemManager` move loading ready for server-side validation

## Next Steps (Phase 4)
1. Network sync for hit reactions, parkour, and combat states
2. Server-side validation of stamina/damage/combo calculations
3. AI behavior trees and CPU difficulty scaling
4. Network rollback for authoritative combat resolution
