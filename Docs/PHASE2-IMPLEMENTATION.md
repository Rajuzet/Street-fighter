# Phase 2 Implementation Guide — Expanded Combat & Parkour

## Overview

Phase 2 extends the Street Fighter framework with deep combat mechanics and parkour movement systems. This document describes the new systems, their integration points, and usage patterns.

## New Systems

### 1. Hit Detection System (`HitDetectionSystem.cs`)

**Location:** `UnityClient/Assets/Scripts/Characters/HitDetectionSystem.cs`  
**Namespace:** `StreetFighter.Characters`

Frame-perfect overlap-box hit detection that only detects hits during active attack frames.

- `StartAttack(AttackDefinition)` — Begins scanning for hits during active frames
- Uses `CombatHitEvent` to broadcast hit results to other systems
- Supports blocking/guard damage routing
- Gizmo visualization for hitbox debugging

**Integration:** Called by `CombatSystemManager.ExecuteMove()` at attack start.

---

### 2. Character Stats (`CharacterStats.cs`)

**Location:** `UnityClient/Assets/Scripts/Characters/CharacterStats.cs`  
**Namespace:** `StreetFighter.Characters`

Character health, armor, and guard management.

- `TakeDamage(float)` — Applies health damage with events
- `TakeGuardDamage(float)` — Applies guard damage (triggers guard break)
- `Heal(float)` — Restores health
- Events: `OnHealthChanged`, `OnGuardChanged`, `OnDeath`

**Integration:** Used by `DamageSystem` for final damage application.

---

### 3. Stamina System (`StaminaSystem.cs`)

**Location:** `UnityClient/Assets/Scripts/Characters/StaminaSystem.cs`  
**Namespace:** `StreetFighter.Characters`

Stamina management for blocking, sprinting, attacking, and dodging.

- `TryConsumeStamina(float)` — Conditional consumption (returns bool)
- `ConsumeStamina(float)` — Unconditional consumption (for continuous drains)
- `SetBlocking(bool)` — Enables block drain
- Regenerates after `regenDelay` seconds of non-use
- Events: `OnStaminaChanged`, `OnExhausted`, `OnRecovered`

**Integration:** `CombatSystemManager` validates stamina before executing moves. `PlayerCombatController` sets blocking state.

---

### 4. Damage System (`DamageSystem.cs`)

**Location:** `UnityClient/Assets/Scripts/Characters/DamageSystem.cs`  
**Namespace:** `StreetFighter.Characters`

Final damage calculation with combo scaling and armor penetration.

- `ApplyDamage(move, attacker, defender, isBlocked)` — Full damage pipeline
- Combo scaling: `1 + (comboCount * 0.1)` up to max multiplier of 3x
- Armor reduction with penetration support
- Guard break logic when guard health reaches zero
- Knockback calculation with combo scaling
- Returns `DamageResult` struct with full hit outcome

**Integration:** Called by `HitDetectionSystem` on hit confirmation.

---

### 5. Hit Reaction System (`HitReactionSystem.cs`)

**Location:** `UnityClient/Assets/Scripts/Characters/HitReactionSystem.cs`  
**Namespace:** `StreetFighter.Characters`

Handles all hit reactions: knockback, stagger, stun, block, and parry.

- `TriggerReaction(HitReaction, direction, force)` — Main entry point
- Priority-based reaction system (stronger reactions override weaker ones)
- Coroutine-driven with animation controller integration
- Reactions: `Light`, `Stagger`, `Knockback`, `Stun`, `Block`, `Parry`

**Integration:** Called by `DamageSystem` after damage calculation. `PlayerController` checks `IsReacting` to disable movement.

---

### 6. Parkour System (`ParkourSystem.cs`)

**Location:** `UnityClient/Assets/Scripts/Characters/ParkourSystem.cs`  
**Namespace:** `StreetFighter.Characters`

Environmental traversal: vault, ledge grab, and slide.

- **Vault:** Auto-detects obstacles 0.5–1.2m high, performs arc animation
- **Ledge Grab:** Detects ledges up to 2m, supports hang-and-pull-up (Space to pull up)
- **Slide:** Manual (C/Left Ctrl), reduces controller height, speed boost
- `IsParkouring` — Blocks normal movement while active
- `CancelParkour()` — Emergency abort

**Integration:** `PlayerController` checks `IsParkouring` in `CanMove`. Animation states set via `CharacterAnimationController`.

---

### 7. Hit Stop Manager (`HitStopManager.cs`)

**Location:** `UnityClient/Assets/Scripts/Core/HitStopManager.cs`  
**Namespace:** `StreetFighter.Core`

Global time freeze for impact feel (fighting-game style).

- `TriggerHitStop()` — Default duration
- `TriggerHitStop(float duration)` — Custom duration
- `TriggerHitStop(float base, float strength)` — Scaled by hit strength
- Eases time scale back to normal using animation curve
- Singleton pattern with safe cleanup

**Integration:** Called by `CombatSystemManager` on every attack hit, with increased duration for critical hits.

---

### 8. Camera Shake (`CameraShake.cs`)

**Location:** `UnityClient/Assets/Scripts/Core/CameraShake.cs`  
**Namespace:** `StreetFighter.Core`

Cinemachine-integrated camera shake for impact feedback.

- `Shake()` — Default shake profile
- `Shake(float magnitude)` — Custom magnitude
- `Shake(float amplitude, float frequency, float duration)` — Full control
- Decay curve for natural falloff
- Auto-finds `CinemachineVirtualCamera` if not assigned

**Integration:** Called by `CombatSystemManager` on hits, with 2x magnitude for critical hits.

---

### 9. Combo Critical Event (`ComboCriticalEvent.cs`)

**Location:** `UnityClient/Assets/Scripts/Core/ComboCriticalEvent.cs`  
**Namespace:** `StreetFighter.Core`

Event raised when a critical hit occurs during a combo.

- Properties: `ComboCount`, `Move`, `CriticalMultiplier`, `Attacker`, `Target`
- Published via `IEventBus` for UI/screenshake/slowmo hooks

**Integration:** Published by `CombatSystemManager` on critical hit detection.

---

### 10. Move Type & Rules (`MoveType.cs`)

**Location:** `UnityClient/Assets/Scripts/Data/MoveType.cs`  
**Namespace:** `StreetFighter.Combat`

Move categorization and combo chaining validation.

- `MoveType`: `None`, `Punch`, `Kick`, `Power`, `Special`, `Throw`, `Counter`
- `MoveTypeRules.CanChainInto(from, to)` — Validates combo transitions
  - Power moves can't chain into other power moves
  - Special moves can only chain into power or counter
  - Throws can only chain into punches
  - Counters can chain into any attack

**Integration:** Used by `ComboChain` to validate and clear invalid combos.

---

## Updated Systems

### AnimationIds
- Added: `KickTrigger`, `PowerTrigger`, `SpecialTrigger`, `ThrowTrigger`
- Added: `HitReactTrigger`, `ParryTrigger`, `DodgeTrigger`, `IsStaggered`, `IsStunned`
- Added: `IsVaulting`, `IsLedgeGrabbing`, `IsSliding`, `ComboCount`

### CharacterAnimationController
- Added trigger methods: `TriggerKick()`, `TriggerPower()`, `TriggerSpecial()`, `TriggerThrow()`, `TriggerDodge()`
- Added state setters: `SetStaggered()`, `SetStunned()`, `SetVaulting()`, `SetLedgeGrabbing()`, `SetSliding()`, `SetComboCount()`

### PlayerCombatController
- Kick input (E key / gamepad alternate attack)
- Power attack (hold attack button > 0.3s)
- Special attack (attack + forward movement)
- Dodge (Shift + Block, no movement)
- Stamina validation before attack execution
- Hit reaction lockout (can't attack while staggered/stunned)

### PlayerController
- Parkour integration: `CanMove` checks `ParkourSystem.IsParkouring`
- Hit reaction lock: blocks movement while reacting
- `ApplyExternalForce()` for knockback support

### CombatSystemManager
- Integrated `HitDetectionSystem`, `StaminaSystem`, `DamageSystem`
- Combo reset timer (2s delay)
- Combo scaling and critical hit logic
- Animation combo count synchronization
- `GetComboCount()` public accessor

### ComboChain
- Move type inference from `AttackDefinition` properties
- `MoveTypeRules.CanChainInto()` validation
- Auto-clears invalid combo chains
- `IReadOnlyList<AttackDefinition> ChainMoves` accessor

---

## Combat Flow

```
Player Input
    ↓
PlayerCombatController (input mapping + stamina check)
    ↓
CombatSystemManager.ExecuteMove(moveId)
    ↓
StaminaSystem.TryConsumeStamina() — validate cost
    ↓
HitDetectionSystem.StartAttack() — begin active frames
    ↓
[On Hit Detected]
    ↓
DamageSystem.ApplyDamage() — calculate with combo/armor/guard
    ↓
CharacterStats.TakeDamage() / TakeGuardDamage()
    ↓
HitReactionSystem.TriggerReaction() — stagger/stun/knockback
    ↓
HitStopManager.TriggerHitStop() — freeze time
    ↓
CameraShake.Shake() — screen impact
    ↓
[Critical Hit?]
    ↓
ComboCriticalEvent → IEventBus (UI/screenshake/slowmo)
```

---

## Parkour Flow

```
Player Movement
    ↓
ParkourSystem.Update() — auto-detect obstacles
    ↓
[Vault Detected] → VaultAction coroutine (arc movement)
[Ledge Detected] → LedgeGrabAction (hang + Space to pull up)
[Slide Input] → SlideAction (reduced height + speed boost)
    ↓
PlayerController.CanMove — blocked during parkour
    ↓
CharacterAnimationController — vault/ledge/slide states
```

---

## Namespace Map

| File | Namespace |
|------|-----------|
| `HitDetectionSystem.cs` | `StreetFighter.Characters` |
| `CharacterStats.cs` | `StreetFighter.Characters` |
| `StaminaSystem.cs` | `StreetFighter.Characters` |
| `DamageSystem.cs` | `StreetFighter.Characters` |
| `HitReactionSystem.cs` | `StreetFighter.Characters` |
| `ParkourSystem.cs` | `StreetFighter.Characters` |
| `HitStopManager.cs` | `StreetFighter.Core` |
| `CameraShake.cs` | `StreetFighter.Core` |
| `ComboCriticalEvent.cs` | `StreetFighter.Core` |
| `MoveType.cs` | `StreetFighter.Combat` |

---

## Next Steps / TODOs

1. **Backend Integration:** Server-side validation for stamina costs, combo counts, and damage calculation
2. **Animation Assets:** Create animator states for `KickTrigger`, `PowerTrigger`, `SpecialTrigger`, `IsVaulting`, etc.
3. **UI Systems:** Stamina bar, combo counter, hit reaction indicators
4. **VFX Integration:** Hit sparks, guard break effects, critical hit flash
5. **Audio Layering:** Separate SFX for punch vs kick vs power impacts
6. **Network Sync:** Sync `HitReactionSystem` states, `ParkourSystem` actions across clients
7. **Balance Tuning:** Adjust stamina costs, damage multipliers, knockback forces via ScriptableObjects

---

## Files Changed in Phase 2

### New Files (10)
1. `UnityClient/Assets/Scripts/Characters/HitDetectionSystem.cs`
2. `UnityClient/Assets/Scripts/Characters/CharacterStats.cs`
3. `UnityClient/Assets/Scripts/Characters/StaminaSystem.cs`
4. `UnityClient/Assets/Scripts/Characters/DamageSystem.cs`
5. `UnityClient/Assets/Scripts/Characters/HitReactionSystem.cs`
6. `UnityClient/Assets/Scripts/Characters/ParkourSystem.cs`
7. `UnityClient/Assets/Scripts/Core/HitStopManager.cs`
8. `UnityClient/Assets/Scripts/Core/CameraShake.cs`
9. `UnityClient/Assets/Scripts/Core/ComboCriticalEvent.cs`
10. `UnityClient/Assets/Scripts/Data/MoveType.cs`

### Updated Files (6)
1. `UnityClient/Assets/Scripts/Characters/AnimationIds.cs`
2. `UnityClient/Assets/Scripts/Characters/CharacterAnimationController.cs`
3. `UnityClient/Assets/Scripts/Characters/PlayerCombatController.cs`
4. `UnityClient/Assets/Scripts/Characters/PlayerController.cs`
5. `UnityClient/Assets/Scripts/Characters/CombatSystemManager.cs`
6. `UnityClient/Assets/Scripts/Characters/ComboChain.cs`

### Documentation (1)
1. `Docs/PHASE2-IMPLEMENTATION.md`
