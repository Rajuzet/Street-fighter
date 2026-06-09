# TODO.md — Street-fighter Phase 3C

- [ ] Re-audit Phase 3B synchronization code paths (intents → authoritative outcomes → client apply)
- [ ] Add client NetTickClock + intent tick/seq plumbing in Unity networking
- [ ] Extend MatchStatePayload to include authoritative tick + last processed seq metadata
- [ ] Add Unity CombatPredictionController (ring buffers: predicted states + local inputs)
- [ ] Add Unity CombatReconciliationController (state compare + correction smoothing + partial resim)
- [ ] Modify PlayerCombatController to enqueue predicted inputs while keeping combat execution responsive
- [ ] Modify MultiplayerCombatReplicator to route authoritative snapshots into reconciliation (not direct health set only)
- [ ] Upgrade backend socketServer.ts to enforce seq/anti-double-hit and emit authoritative tick snapshots
- [ ] Extend backend multiplayerTypes.ts to include tick metadata used by client reconciliation
- [ ] Add editor-only PredictionDebugOverlay + correction/desync metrics
- [ ] Run local integration tests: mismatch scenarios, repeated hits, packet jitter
- [ ] Produce final Phase 3C output summary with risks + required Unity/backend setup steps

