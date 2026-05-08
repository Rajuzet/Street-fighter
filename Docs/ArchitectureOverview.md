# Street-Fighter Architecture Overview

## Core Layers

### Unity Client
- Modular gameplay systems built on `Assets/Scripts/`
- `Core` initialization, service registry, scene and event managers
- `Combat`, `Movement`, `Characters`, `Multiplayer`, `UI`, `AI`, `World` subsystems
- Addressables-driven resources, input system, Cinemachine camera stacks
- ECS-compatible structure for performance-critical systems
- State machines and event bus for gameplay flow

### Backend Server
- Node.js + Express + TypeScript
- JWT-based authentication and session validation
- PostgreSQL for persistent player and game data
- Redis for ephemeral state, matchmaking cache, and anti-cheat session state
- Socket.IO for multiplayer lobby and match synchronization
- API versioning and error-handling middleware

### Shared Contracts
- `Shared/` stores DTOs and network enums for client/server interoperability
- Strongly-typed messages for auth, matchmaking, player state, cosmetics

## Multiplayer Architecture
- Dedicated server pattern with authoritative match state managed by backend
- Lobby service for grouping players and balancing teams
- Ranked match pipelines with ELO-style ranking metadata
- Real-time synchronization events and room-based state updates
- Client-side prediction + server reconciliation support
- Anti-cheat hooks in backend validation layers

## Expansion Path
- MMO service core via horizontal matchmaking and shard-ready session architecture
- Creator economy via modular cosmetic and shop services
- Live events and tournament services in backend service layer
- Social world systems through friend, clan, and chat graph services
