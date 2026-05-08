# Prefab Hierarchy

Player Prefab:
├── MeshRenderer/SkinnedMesh
├── Animator (PlayerCombat.controller)
├── CharacterController
├── Rigidbody (for knockback)
├── PlayerController
├── PlayerCombatController
├── CombatSystemManager
├── StaminaSystem
├── HitDetectionSystem
├── DamageReaction
├── RagdollManager
├── CameraRig (child: CinemachineVirtualCamera + ImpulseSource + BasicMultiChannelPerformer)
└── VFX children (punch spark, hit effect)

Enemy/Dummy:
├── Same as Player + CombatAI + NavMeshAgent (remove CC)

Assign ScriptableObjects:
- CombatSystemManager.moves = [Jab.asset, Cross.asset]

