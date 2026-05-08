# Testing Workflow

1. Install Cinemachine (Window > Package Manager).
2. Build Player Prefab using PlayerPrefabBuilderEditor.
3. Create CombatArena scene: Player + TrainingDummy (CombatAI with aggro=0 for dummy).
4. Assign StaminaConfig, AttackDefinitions (create SO assets).
5. Add DamageReaction to Player/Dummy to subscribe HitEvent.
6. Play:
   - Light/Heavy combos (attack + sprint-attack).
   - Block (hold B), Parry (tap B during attack).
   - Dodge roll (Dodge input).
   - Hit dummy: Verify shake/stop/knockback.
   - Dummy AI chases/attacks.
7. Network: Add PhotonView to managers, test sync.

Multiplayer-safe: Events RPC'd via Photon.
