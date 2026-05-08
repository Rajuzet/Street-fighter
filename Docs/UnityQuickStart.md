# Getting Started with Phase 1

## Prerequisites
- Unity 6
- Installed packages:
  - Input System
  - Addressables
  - Cinemachine
  - Collections
  - Entities (optional, for future ECS systems)

## Quick Start

### 1. Bootstrap the Backend (Optional)
```bash
cd Backend
npm install
npm run dev
```
Backend will be available at `http://localhost:4000`.

### 2. Open Unity Project
- Open `UnityClient` folder as a Unity project
- Wait for packages to import
- Verify no compilation errors in the Console

### 3. Create Bootstrap Scene
1. Create new scene: `Assets/Scenes/Bootstrap.unity`
2. Create empty GameObject: `GameBootstrap_Root`
3. Add `GameBootstrap` component to it
4. Create another GameObject: `Player_Test`
5. Add `PlayerController`, `PlayerCombatController`, `CharacterAnimationController`, and `CharacterController` components
6. Create child object `CameraRig` with `ThirdPersonCameraRig`
7. Assign references:
   - GameBootstrap.SaveSettings: Create new SaveSettings in `Assets/ScriptableObjects/`
   - GameBootstrap.RuntimeAudioBank: Create new AudioBank in `Assets/ScriptableObjects/`

### 4. Configure Addressables (Basic Setup)
1. Window > Asset Management > Addressables > Groups
2. Create "bootstrap" group
3. Drag Bootstrap scene into the group
4. Right-click on Bootstrap scene > Set Address > "bootstrap"

### 5. Test the Scene
- Press Play in the Bootstrap scene
- Verify Console output: `Street-Fighter backend started on port 4000` (or similar startup messages)
- Test controls:
  - WASD to move
  - Mouse to look
  - Space to jump
  - Shift to sprint
  - Left-click to attack

### 6. Troubleshooting

**Compilation Errors**
- Check assembly definition references in each module folder
- Ensure Packages/manifest.json includes required packages

**Input Not Working**
- Verify Input System package is installed
- Check `PlayerInputActions` is instantiated in `InputManager.Initialize()`

**Audio Manager Errors**
- Create an AudioBank asset and assign it in GameBootstrap
- Verify AudioBank has at least one entry

**Save System Not Working**
- Create SaveSettings asset in `Assets/ScriptableObjects/`
- Verify file permissions in persistent data path

## Architecture Notes

- `ServiceLocator` handles all runtime dependency resolution
- `GameBootstrap` initializes on scene load and registers all services
- `PlayerController` and `PlayerCombatController` are kept separate for multiplayer-ready authority split
- `EventBus` enables decoupled events across modules
- All configuration is data-driven through ScriptableObjects
