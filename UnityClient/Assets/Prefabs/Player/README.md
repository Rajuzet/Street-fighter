# Player Prefab Structure

This folder contains the runtime player prefab architecture and required component structure.

## Required components
- Root GameObject: `Player_Root`
- `CharacterController`
- `PlayerController`
- `PlayerCombatController`
- `CharacterAnimationController`
- `ThirdPersonCameraRig` child object
- `Animator` component with player animation controller asset

## Multiplayer-ready design
- The player prefab is built to support authoritative movement and animation state synchronization.
- All runtime logic is decoupled through `ServiceLocator` and `IInputService` abstractions.
- Combat actions and movement are separated for future network reconciliation and prediction.

## Editor helper
- Use `StreetFighter -> Build Player Prefab` to generate `Assets/Prefabs/Player/PlayerCharacter.prefab`.
