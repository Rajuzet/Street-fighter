# PHASE4_TODO.md — Street-fighter Vertical Slice (Multiplayer Production Mode)

## 0. Re-audit & stabilization gates (before implementation)
- [ ] Re-audit multiplayer authority paths (intent → server outcome → client apply)
- [ ] Re-audit combat systems integration points (hit detection → damage reaction → audio/VFX)
- [ ] Verify prediction/reconciliation stability status (if prediction exists; otherwise implement latency-tolerant server authority for round/KO)
- [ ] Preserve networking architecture (SocketClientService + socket events)
- [ ] Preserve EventBus architecture for gameplay/UI signals

## 1. Backend: Match Flow State Machine (authoritative)
- [ ] Extend `Backend/src/types/multiplayerTypes.ts` with match/lobby/round/character selection DTOs
- [ ] Extend `Backend/src/sockets/socketServer.ts` with room + match events:
  - [ ] lobby create/join/leave
  - [ ] player ready
  - [ ] character select + lock
  - [ ] synchronized countdown start
  - [ ] round start (timer)
  - [ ] KO detection and round end
  - [ ] winner screen (match end)
  - [ ] rematch flow (synchronized)
  - [ ] return-to-lobby
- [ ] Reconnect-safe: re-emit authoritative state to rejoining session(s)
- [ ] Anti-double-hit / seq safety remains enforced

## 2. Unity: Match Flow System (single manager)
- [ ] Implement `MatchFlowManager` (or integrate into existing session facade) to manage:
  - [ ] lobby UI transitions
  - [ ] room join
  - [ ] ready/locking
  - [ ] character select
  - [ ] countdown
  - [ ] round loop
  - [ ] KO/winner
  - [ ] rematch + return-to-lobby
- [ ] Ensure only one match manager instance (no duplicates)
- [ ] Add match flow event logging + diagnostics hooks

## 3. Unity: Multiplayer Character Selection
- [ ] Implement `CharacterSelectionManager` with preview
- [ ] Network-safe selection lock + server-authoritative spawn selection at round start
- [ ] Support scalable roster via data-driven character IDs

## 4. Unity: UI/UX vertical slice
- [ ] Main menu + multiplayer menu (controller + KBM)
- [ ] Lobby UI (player list + ready)
- [ ] Character select UI (portraits, confirm, lock)
- [ ] HUD (health, stamina, combo count, timer, ping)
- [ ] Victory/defeat screens
- [ ] Loading screens
- [ ] Responsive layout; avoid multiplayer conflicting UI updates

## 5. Combat Polish (no combat logic rewrites)
- [ ] Hit feedback (sparks/flash hooks if present), screen shake tuning envelopes
- [ ] KO effect triggers and timing
- [ ] Combo readability: UI event hooks
- [ ] Audio polish: pooling + no duplicate playback prevention

## 6. Multiplayer Stability Validation
- [ ] Synchronized round timer
- [ ] Synchronized KO/winner state
- [ ] Disconnect recovery flow (rejoin → authoritative replay)
- [ ] Rematch synchronization
- [ ] Lobby state consistency

## 7. Performance/Optimization
- [ ] UI allocation minimization and event-driven updates
- [ ] RPC/event batching where possible
- [ ] VFX pooling (or minimal allocations)
- [ ] Audio pooling
- [ ] GC spike audit during combat

## 8. Debugging + QA Checklists
- [ ] Match diagnostics overlay + sync debugging
- [ ] UI state debugging
- [ ] Round flow logging + network tracing
- [ ] Multiplayer QA checklist
- [ ] Latency testing checklist
- [ ] Production validation checklist

## 9. Build + Deploy Prep
- [ ] Windows build pipeline notes/scripts
- [ ] Server configuration docs
- [ ] Multiplayer config + environment variables
- [ ] Production logging configs

## Deliverables
- [ ] Full modified files list
- [ ] Match systems added
- [ ] UI systems added
- [ ] Multiplayer improvements added
- [ ] Remaining technical debt
- [ ] Remaining desync risks
- [ ] Remaining optimization risks
- [ ] Required Unity setup steps
- [ ] Required backend setup steps
- [ ] Recommended Phase 5 roadmap

