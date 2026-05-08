# Multiplayer Architecture Readiness - Phase 1

## Design Principles

Street-Fighter's Phase 1 architecture is designed with multiplayer-first principles:

1. **Separated Authority**
   - Local player input is handled locally via `IInputService`
   - Character animation state is local-first with server reconciliation
   - Combat moves are authoritative on the server for validation
   - Movement is client-predicted with server correction

2. **Input Abstraction**
   - `IInputService` abstracts input acquisition
   - Network input can be switched in by a `RemoteInputService` for other players
   - All movement uses `Vector2` inputs that can come from network or local

3. **Stateless Combat System**
   - `CombatSystemManager` broadcasts move execution via `EventBus`
   - Moves are validated server-side before applying damage
   - Combo chains are regenerated from network events, not stored locally

4. **Animation Bridging**
   - `CharacterAnimationController` accepts numeric parameters
   - Animator states are deterministic based on parameter inputs
   - Network can sync animation parameters without SendMessage

## Adding Network Support in Phase 3

### Step 1: Create Remote Player Controller
- Inherit from a base class derived from `PlayerController`
- Replace `IInputService` dependency with network input stream
- Sync position, rotation, and animation parameters via `OnPhotonSerializeView`

### Step 2: Network Combat Validation
- Client executes move locally
- Backend validates move legality
- Backend calculates damage
- Backend broadcasts damage event to all players
- All clients apply damage locally

### Step 3: State Synchronization
- Broadcast `CombatMoveExecutedEvent` through network message
- Subscribe remote players to combat events
- Sync health, stamina, and status effects

## Example: Network Integration Point

```csharp
// Client sends attack input
if (inputService.AttackPressed)
{
    var moveInfo = new CombatMoveInfo { moveId = "punch_light", position = transform.position };
    photonNetwork.RPC(nameof(OnRemoteCombatMove), RpcTarget.All, moveInfo);
}

// Server validates then broadcasts
[PunRPC]
void OnRemoteCombatMove(CombatMoveInfo moveInfo)
{
    if (!localPlayer) return; // Only validate on server
    
    var valid = ValidateMoveEligibility(moveInfo);
    if (!valid) return;
    
    combatSystem.ExecuteMove(moveInfo.moveId);
}
```

## Future Scaling Considerations

- All player state is non-destructively stored (no local-only data mutations)
- Player prefab can be instantiated remotely with a different input provider
- BeautifulServices can inject different dependencies per player context
- Audio, animation, and visual effects are all event-driven and can be played on any client
