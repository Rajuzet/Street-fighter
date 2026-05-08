# Unity Development Standards - Street Fighter

## Coding Conventions

### Namespaces
- Follow folder structure: `StreetFighter.Module.Submodule`
- Always place code in appropriate namespace matching folder layout

### Classes
- Public classes: `PascalCase`
- Private/internal classes: `PascalCase` with sealed/internal modifiers where appropriate
- Use `sealed` on final classes to allow inlining

### Methods and Properties
- Public methods: `PascalCase` without underscores
- Private methods: `camelCase` with leading underscore `_methodName()` optional
- Properties: Always use `{ get; private set; }` or `{ get; }` for readonly

### Fields and Variables
- Public fields: Rare, use properties instead
- Serialized fields: `[SerializeField] private Type name = default;`
- Local variables: `camelCase`
- Constants: `SCREAMING_SNAKE_CASE`

### Comments and Documentation
- Use XML documentation (`///`) on all public methods, properties, and classes
- Include param and return type descriptions
- Add inline comments for complex logic only
- Example:
  ```csharp
  /// <summary>
  /// Performs world-space movement and applies gravity.
  /// </summary>
  /// <param name="direction">Desired movement direction.</param>
  public void Move(Vector3 direction)
  {
      // Apply gravity and movement
  }
  ```

## Architecture Patterns

### Dependency Injection
- Use `ServiceLocator.Register<T>(instance)` and `ServiceLocator.Resolve<T>()`
- Prefer constructor injection or property-based injection for testability
- Initialize services in `GameBootstrap`

### Event Bus
- Subscribe to events via `ServiceLocator.Resolve<IEventBus>().Subscribe<TEvent>(listener)`
- Use strongly-typed event classes
- Unsubscribe in `OnDestroy` to prevent memory leaks

### MonoBehaviours
- Do not inherit from MonoBehaviour for stateless utilities or domain logic
- Use separate controllers for input handling, animation, and physics
- Initialize MonoBehaviour dependencies in `Awake()`, not constructor

### Data and Configuration
- Store static or design-time data in ScriptableObjects
- Load assets via Addressables
- Use immutable properties in data objects

## Performance Considerations

- Cache `GetComponent<T>()` results in `Awake()`
- Use object pooling for frequently allocated objects
- Limit coroutines; use events or callbacks instead
- Batch physics queries when possible
- Use `Physics.CheckSphere()` sparingly in Update loops

## Multiplayer Architecture

- Separate gameplay logic from networking concerns
- Use `CombatSystemManager` for authoritative combat
- Player input is local only (`IInputService`)
- Sync movement and animation state through backend
- Validate all damage and status effects server-side

## Module Assembly References

- `StreetFighter.asmdef` (core module, no external deps)
- `StreetFighter.Input.asmdef` → `StreetFighter`
- `StreetFighter.Audio.asmdef` → `StreetFighter`
- `StreetFighter.Save.asmdef` → `StreetFighter`
- `StreetFighter.Events.asmdef` → `StreetFighter`
- `StreetFighter.Camera.asmdef` → `StreetFighter`
- `StreetFighter.Characters.asmdef` → `StreetFighter`
- `StreetFighter.Data.asmdef` → `StreetFighter`

## Scene Management

- Use `ISceneLoader` for async Addressable scene loading
- Persist `GameBootstrap` across scenes
- Load UI and gameplay scenes additively
- Unload unused scenes explicitly

## Testing Considerations

- Keep logic separate from MonoBehaviour lifecycle
- Use interfaces for dependencies to enable mocking
- Avoid static state except for singletons
- Write domain logic as POCO classes when possible
