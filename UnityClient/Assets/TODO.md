# Street Fighter Phase 2B Combat Polish TODO

## PRIORITY ORDER (Combat Feel → Enemies → Anims → Technical)

[x] 1. **Enhance AttackDefinition.cs** (Added CriticalChance/Multiplier, ScreenFlash, SlowMoDuration, HitVFXKey, ImpactSFXLayer)
    - Add CriticalChance/Multiplier, ScreenFlashIntensity, SlowMoDuration, HitVFX prefab key, ImpactSFX layers.  
    - Expose for data-driven tuning.

[x] 2. **Update CombatSystemManager.cs**  
    - Added comboCount tracking/multiplier (10% per hit).  
    - Critical RNG check + enhanced hitstop/shake.  
    - Event publish, layered SFX, VFX TODO hooks.  
    - Reset on cancel.

[x] 3. **Overhaul CombatAI.cs → Aggro Groups**  
    - Added threatLevel, circling (angle offset), combatSpacing, turnCooldown.  
    - States: Circling/Attack/Dodge/React; heavy attacks high threat.  
    - OnHitReaction subscribe, 30% dodge chance.  
    - Created EnemyAggroGroup.cs stub (CanAttack/Register, max simultaneous).

[ ] 4. **DamageReaction.cs + New HealthSystem.cs**  
    - Health regen post-combat.  
    - Hit stun FSM, guard break (stagger).  
    - Directional reactions.

[ ] 5. **StaminaSystem.cs Balancing**  
    - Tune costs for sprint/block/combo scaling drain.

[ ] 6. **Polish Feel: HitStopManager.cs, CameraShake.cs**  
    - Dynamic profiles (light/heavy).  
    - Slowmo finishers (timeScale lerp).

[ ] 7. **KnockbackHandler.cs**  
    - Air juggle vectors, recovery states.

[ ] 8. **Animations: CharacterAnimationController.cs**  
    - Root motion (ApplyRootMotion).  
    - Interrupt rules, blend smoothing (crossfade).

[ ] 9. **Events/Dependencies**  
    - CombatEvents.cs: CriticalHitEvent, GuardBreakEvent, AggroEvent.  
    - HitDetectionSystem.cs: multi-hit/parry.  
    - ComboChain.cs: transitions/cancels.

[ ] 10. **Technical: New Files**  
    - VFXPoolManager.cs (pooling).  
    - CombatTelemetry.cs (debug UI).  
    - HitboxGizmos.cs (editor gizmos).  
    - RagdollManager.cs recovery.

[ ] 11. **Testing/Balancing**  
    - Update TestingWorkflow.md.  
    - Create CombatBalancing.md guide.  
    - Prefab updates (Player/Dummy).  
    - Unity Profiler validation.

**Next:** After each major step, update TODO.md with progress [x]. Run tests in CombatArena scene.

