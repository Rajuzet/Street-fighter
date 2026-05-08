# Phase 1 API Reference

## Backend REST API

### Authentication Service
```
POST /api/v1/auth/register
{
  "email": "player@example.com",
  "username": "PlayerName",
  "password": "securePassword123"
}
Response: { user: { id, email, username } }

POST /api/v1/auth/login
{
  "email": "player@example.com",
  "password": "securePassword123"
}
Response: { token: "eyJhbGc..." }
```

### User Service
```
GET /api/v1/users/me
Header: Authorization: Bearer <token>
Response: { profile: { id, email, username, level, experience, currency } }
```

### Health Check
```
GET /health
Response: { status: "ok", service: "street-fighter-backend", version: "0.1.0" }
```

### WebSocket Events
```
socket.on('lobby:join', { lobbyId })
socket.emit('lobby:joined', { lobbyId })

socket.on('match:sync', { matchId, playerPosition, playerAngle })
socket.emit('match:update', { playerState })
```

## Unity Service Interfaces

### IInputService
```csharp
interface IInputService
{
    Vector2 Move { get; }
    Vector2 Look { get; }
    bool JumpPressed { get; }
    bool SprintHeld { get; }
    bool AttackPressed { get; }
    bool BlockHeld { get; }
    void Initialize();
    void Enable();
    void Disable();
}
```

### ISceneLoader
```csharp
interface ISceneLoader
{
    Task LoadSceneAsync(string addressableLabel, LoadSceneMode mode);
    Task UnloadSceneAsync(string addressableLabel);
}
```

### IAudioService
```csharp
interface IAudioService
{
    void Initialize(AudioBank bank);
    void PlaySound(string clipKey, Vector3 position, float volume = 1f);
    void PlayMusic(string clipKey, float volume = 1f);
    void StopMusic();
}
```

### ISaveService
```csharp
interface ISaveService
{
    void Initialize(SaveSettings settings);
    bool Save<T>(string key, T record) where T : class;
    T Load<T>(string key) where T : class, new();
}
```

### IEventBus
```csharp
interface IEventBus
{
    void Subscribe<TEvent>(Action<TEvent> listener);
    void Unsubscribe<TEvent>(Action<TEvent> listener);
    void Publish<TEvent>(TEvent eventData);
}
```

## ServiceLocator API

```csharp
// Register a service
ServiceLocator.Register<IInputService>(new InputManager());

// Resolve a service
var inputService = ServiceLocator.Resolve<IInputService>();

// Reset all services (scene transitions)
ServiceLocator.Reset();
```

## Combat System API

### CombatSystemManager
```csharp
public sealed class CombatSystemManager : MonoBehaviour
{
    /// Executes a move and queues it into the combo chain
    public void ExecuteMove(string moveId);
    
    /// Cancels the current combo
    public void CancelCombo();
}
```

### Animation Events

```csharp
// Raised when a combat move is executed
public sealed class CombatMoveExecutedEvent
{
    public CombatMoveData Move { get; }
}
```

### CharacterAnimationController
```csharp
public sealed class CharacterAnimationController : MonoBehaviour
{
    public void SetMovement(float speed, bool sprinting);
    public void SetJumpState(bool isAirborne);
    public void TriggerAttack();
    public void SetBlocking(bool isBlocking);
}
```

## Game State Machine

```csharp
public enum GameState
{
    Uninitialized,
    Bootstrap,
    MainMenu,
    Loading,
    InGame,
    Paused,
    Shutdown
}

// Usage
GameStateManager.Instance.SetState(GameState.InGame);
GameStateManager.Instance.StateChanged += (newState) => Debug.Log($"State: {newState}");
```

## ScriptableObject Data Contracts

### CombatMoveData
```csharp
[CreateAssetMenu(fileName = "CombatMoveData", menuName = "StreetFighter/Data/CombatMoveData")]
public sealed class CombatMoveData : ScriptableObject
{
    public string MoveId;
    public string DisplayName;
    public float Damage;
    public float Cooldown;
    public float Recovery;
    public bool IsBlockable;
    public string AnimationTrigger;
}
```

### CharacterProfileData
```csharp
[CreateAssetMenu(fileName = "CharacterProfileData", menuName = "StreetFighter/Data/CharacterProfileData")]
public sealed class CharacterProfileData : ScriptableObject
{
    public string ProfileName;
    public float MovementSpeed;
    public float SprintSpeed;
    public float JumpHeight;
    public float Gravity;
}
```

### GameSettings
```csharp
[CreateAssetMenu(fileName = "GameSettings", menuName = "StreetFighter/Data/GameSettings")]
public sealed class GameSettings : ScriptableObject
{
    public float InputSmoothing;
    public float CameraSensitivity;
    public float MouseSensitivity;
}
```

### AudioBank
```csharp
[CreateAssetMenu(fileName = "AudioBank", menuName = "StreetFighter/Audio/AudioBank")]
public sealed class AudioBank : ScriptableObject
{
    [System.Serializable]
    public sealed class AudioEntry
    {
        public string Key;
        public AudioClip AudioClip;
    }
    
    public IReadOnlyList<AudioEntry> Entries { get; }
}
```

## Animation Parameter IDs

```csharp
public static class AnimationIds
{
    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    public static readonly int IsAirborne = Animator.StringToHash("IsAirborne");
    public static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
    public static readonly int IsBlocking = Animator.StringToHash("IsBlocking");
}
```

## PlayerController API

### Movement
```csharp
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerController : MonoBehaviour
{
    // Driven by InputService.Move
    // Gravity applied automatically
    // Jump triggered on InputService.JumpPressed
    // Sprint enabled via InputService.SprintHeld
    
    private bool IsGrounded(); // Internal ground detection
}
```

### Input to Movement Flow
1. `PlayerController.Update()` reads `IInputService.Move`
2. Direction is rotated to camera-relative via `ThirdPersonCameraRig.transform`
3. Velocity is applied via `CharacterController.Move()`
4. `CharacterAnimationController.SetMovement()` updates animator

## Network Readiness (Phase 2+)

### Expected Network Extension Points

1. **Remote Player Input**
   - Replace `IInputService` with `NetworkInputService`
   - Queue network input packets in same format

2. **Combat Validation**
   - `CombatSystemManager.ExecuteMove()` sends event request to server
   - Server validates move eligibility
   - Server broadcasts `CombatMoveExecutedEvent`
   - All clients apply damage from event

3. **Animation Sync**
   - Send animator parameter updates every fixed tick
   - `AnimationIds.Speed`, `AnimationIds.IsAirborne`, etc.
   - Deterministic animation playback on all clients

4. **Transform Sync**
   - Server sends authoritative position corrections
   - Client interpolates between sent positions
   - Movement prediction mitigates latency

## Example: Wiring Services

```csharp
// In GameBootstrap.RegisterCoreServices()
ServiceLocator.Register<IInputService>(new InputManager());
ServiceLocator.Register<IAudioService>(new AudioManager());
ServiceLocator.Register<ISaveService>(new SaveSystem());
ServiceLocator.Register<IEventBus>(new EventBus());
ServiceLocator.Register<ISceneLoader>(new SceneLoader());
ServiceLocator.Register<GameStateManager>(GameStateManager.Instance);

// In PlayerController.Awake()
var inputService = ServiceLocator.Resolve<IInputService>();
var eventBus = ServiceLocator.Resolve<IEventBus>();
eventBus.Subscribe<CombatMoveExecutedEvent>(OnCombatMoveExecuted);
```

## Deployment Configuration

### Backend Environment Variables
```
NODE_ENV=production
PORT=4000
DATABASE_URL=postgres://user:password@host:5432/streetfighter
REDIS_URL=redis://redis:6379
JWT_SECRET=your-secret-key-here
JWT_EXPIRES_IN=1h
API_VERSION=v1
```

### Docker Commands
```bash
# Build backend image
docker build -t streetfighter-backend ./Backend

# Run local dev stack
docker-compose up

# View logs
docker-compose logs -f backend
```

All APIs are production-ready and require no futher work for MVP. Extensible by design for Phase 2+.
