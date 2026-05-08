# Street Fighter Phase 2A Animator Setup Guide

## Animator Controller

1. Create 'PlayerCombat.controller'.
2. Base Layer: Locomotion blend tree (speed).
3. Combat Layer (Override): 
   - States: Idle, Block (held), Parry (interrupt), DodgeRoll (iframe), LightAttack1/2/3, HeavyAttack, HitReactLight/Heavy, Knockback, Ragdoll.
4. Transitions: Attack -> HitReact on event, Block -> Parry on input, Any -> Dodge.
5. Blend Tree for combos (1D time-based).
6. Animation Events: 'ActiveFramesStart', 'HitLand', 'RecoveryEnd'.

## Events
- AttackTrigger, Dodge, HitReact, etc.

## Prefab Hierarchy
Player/
 - CapsuleCollider (main)
 - AttackColliders[] (child empty, dynamic BoxCollider)
 - Ragdoll limbs (Rigidbody/Collider, kinematic)
 - Cinemachine ImpulseSource (for shake)
 - ParticleSystems (hit VFX)

Dummy: Add CombatAI, NavMeshAgent.

## Testing Workflow
1. Open CombatArena scene (create with Player + Dummy).
2. Assign AttackDefinitions to CombatMoveData.
3. Play: Test combos, block/parry, dodge, AI aggro.
4. Check: Hitstop/shake/knockback, stamina drain.
