# Phase 1: Core Architecture Implementation Guide

## Overview
Phase 1 establishes a production-ready Unity framework and backend API infrastructure for Street-Fighter.

## Completed Components

### Backend Foundation
- ✅ Express.js API server with TypeScript
- ✅ PostgreSQL schema and user authentication
- ✅ JWT-based session management
- ✅ Socket.IO real-time server
- ✅ Environment configuration and Docker containerization
- ✅ GitHub Actions CI pipeline

### Unity Framework
- ✅ Service locator and dependency injection
- ✅ GameBootstrap initialization system
- ✅ Event bus for decoupled communication
- ✅ Scene loader with Addressables integration
- ✅ Input manager using Unity Input System
- ✅ Audio manager with bank-based lookup
- ✅ Save/load system with JSON persistence
- ✅ Third-person camera with Cinemachine
- ✅ Player controller with movement, sprint, jump
- ✅ Combat system foundation with combo chains
- ✅ Animation controller and parameter bridging

### Data Architecture
- ✅ ScriptableObject contracts (AudioBank, CombatMoveData, CharacterProfileData, GameSettings, SaveSettings)
- ✅ Assembly definitions for module isolation
- ✅ Package dependencies configured (Input System, Addressables, Cinemachine)

### Documentation
- ✅ Coding standards and architecture patterns
- ✅ Project structure guide
- ✅ GitHub branching and workflow strategy

## Next Steps (Phase 1 Continuation)

1. **Create Player Prefab**
   - Use `StreetFighter -> Build Player Prefab` in the Editor menu
   - Verify all components are wired and functional

2. **Setup Bootstrap Scene**
   - Create a Bootstrap scene as the entry point
   - Add `GameBootstrap` prefab to the scene
   - Configure ScriptableObject references (SaveSettings, AudioBank, GameSettings)

3. **Configure Addressables**
   - Create Addressable groups for scenes, prefabs, and audio
   - Label Bootstrap scene as "bootstrap"
   - Label Player prefab group as "characters"

4. **Test Local Gameplay**
   - Build and run a simple test scene
   - Verify player movement, camera, and input
   - Confirm audio and save systems initialize without errors

5. **Backend API Integration (Optional Phase 1 Extension)**
   - Create `BackendClient` MonoBehaviour wrapper for REST/WebSocket calls
   - Implement user login flow
   - Add lobby service stubs

## File Structure Summary
```
UnityClient/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameBootstrap.cs
│   │   │   ├── ServiceLocator.cs
│   │   │   ├── GameStateManager.cs
│   │   │   ├── EventBus.cs
│   │   │   └── SceneLoader.cs
│   │   ├── Input/
│   │   │   ├── IInputService.cs
│   │   │   └── PlayerInputActions.cs
│   │   ├── Audio/
│   │   │   ├── IAudioService.cs
│   │   │   ├── AudioManager.cs
│   │   │   └── AudioBank.cs
│   │   ├── Save/
│   │   │   ├── ISaveService.cs
│   │   │   ├── SaveSystem.cs
│   │   │   └── SaveSettings.cs
│   │   ├── Camera/
│   │   │   └── ThirdPersonCameraRig.cs
│   │   ├── Characters/
│   │   │   ├── PlayerController.cs
│   │   │   ├── PlayerCombatController.cs
│   │   │   ├── CombatSystemManager.cs
│   │   │   ├── ComboChain.cs
│   │   │   ├── CharacterAnimationController.cs
│   │   │   ├── AnimationIds.cs
│   │   │   └── CombatMoveExecutedEvent.cs
│   │   ├── Data/
│   │   │   ├── CombatMoveData.cs
│   │   │   ├── CharacterProfileData.cs
│   │   │   └── GameSettings.cs
│   │   └── Editor/
│   │       └── PlayerPrefabBuilderEditor.cs
│   ├── Prefabs/Player/
│   ├── ScriptableObjects/
│   ├── Scenes/
│   └── Addressables/
├── Packages/manifest.json
└── ProjectSettings/
```

## Verification Checklist

- [ ] Backend: `npm install` and `npm run dev` run without errors
- [ ] Unity: All assembly definitions compile without errors
- [ ] Editor: `StreetFighter -> Build Player Prefab` works correctly
- [ ] Runtime: Bootstrap scene loads and initializes services
- [ ] Input: WASD movement and spacebar jump respond correctly
- [ ] Camera: Mouse look controls camera rotation
- [ ] Animation: Player animator responds to movement speed
- [ ] Combat: Attack input triggers combo system
- [ ] Audio: AudioBank initialized and ready for sound playback
- [ ] Save: SaveSystem.Save() and Load() work with the configured prefix
