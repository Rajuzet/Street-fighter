# STREET-FIGHTER

**Status: Phase 1 Complete** ✅

A scalable AAA live-service multiplayer urban combat game engine built on Unity 6, Node.js, PostgreSQL, Redis, and Docker.

## Core Capabilities (Phase 1 Implemented)

- **Real-time Combat:** Punch/kick/block mechanics with combo chaining
- **Player Controller:** Movement, sprint, jump with camera-relative input
- **Third-Person Camera:** Cinemachine integration with smooth look controls
- **Audio System:** Sound pooling, music management, bank-based organization
- **Save/Load:** JSON-based player progression persistence
- **Event Bus:** Decoupled gameplay communication
- **Service Locator:** Dependency injection for multiplayer extensibility
- **Backend API:** JWT auth, user management, lobby service foundation
- **Real-Time Server:** Socket.IO for multiplayer synchronization
- **Database:** PostgreSQL schema with user profiles, clans, cosmetics support

## Architecture Highlights

- **Multiplayer-Ready:** Input abstraction allows network player injection
- **SOLID Principles:** Clean, testable, enterprise-grade architecture
- **Production Code:** Zero placeholders—all systems fully implemented
- **Modular Design:** Assembly definitions enable independent compilation
- **Docker Support:** Local dev stack and production containerization
- **CI/CD:** GitHub Actions pipeline with automated builds and tests
- **Documentation:** 8 comprehensive guides for development and deployment

## Quick Start

### Backend
```bash
cd Backend
npm install
npm run dev
# Server runs on http://localhost:4000
```

### Unity
1. Open `UnityClient/` in Unity 6
2. Import packages: Input System, Addressables, Cinemachine
3. Create Bootstrap scene with `GameBootstrap` prefab
4. Press Play to test player movement and combat

See [UnityQuickStart.md](Docs/UnityQuickStart.md) for detailed setup.

## Project Structure

```
Street-fighter/
├── UnityClient/          # Unity 6 game client
│   └── Assets/Scripts/   # Core, Input, Camera, Characters, Audio, Save, Data systems
├── Backend/              # Node.js + Express API and WebSocket server
├── Database/             # PostgreSQL schema and migrations
├── Docs/                 # Architecture, guides, roadmaps
├── DevOps/               # Docker, CI/CD templates
├── Shared/               # Cross-platform DTOs and contracts
├── Tools/                # Developer utilities
└── AssetsPipeline/       # Asset processing and import rules
```

## Phase Roadmap

| Phase | Focus | Status |
|-------|-------|--------|
| **1** | Core architecture, character controller, combat foundation | ✅ **COMPLETE** |
| 2 | Character polish, expanded combat moves, animations | Planned |
| 3 | Multiplayer integration, matchmaking, synchronization | Planned |
| 4 | Open-world systems, NPCs, mission design | Planned |
| 5 | Economy, progression, cosmetics shop | Planned |
| 6 | Optimization, live service, analytics | Planned |

## Key Files

- [PHASE1-COMPLETE.md](PHASE1-COMPLETE.md) — What's implemented in Phase 1
- [Docs/CodingStandards.md](Docs/CodingStandards.md) — Development guidelines
- [Docs/MultiplayerArchitecture.md](Docs/MultiplayerArchitecture.md) — Network design patterns
- [Docs/UnityQuickStart.md](Docs/UnityQuickStart.md) — Getting started

## Technology Stack

- **Game Engine:** Unity 6
- **Language:** C# (Client), TypeScript (Backend)
- **Backend:** Node.js, Express.js
- **Database:** PostgreSQL, Redis
- **Rendering:** URP (scalable to HDRP)
- **Networking:** Socket.IO, WebSockets
- **DevOps:** Docker, GitHub Actions

## Getting Started

1. Read [PHASE1-COMPLETE.md](PHASE1-COMPLETE.md) for implementation overview
2. Follow [Docs/UnityQuickStart.md](Docs/UnityQuickStart.md) for local setup
3. Review [Docs/CodingStandards.md](Docs/CodingStandards.md) before contributing
4. Check GitHub Actions status for backend CI/CD

## Architecture Philosophy

Street-Fighter is built on:
- **Separation of Concerns:** Each system handles one responsibility
- **Multiplayer-First:** Input, movement, and combat designed for network injection
- **Event-Driven:** Gameplay communication via event bus, not tight coupling
- **Data-Driven:** All configuration via ScriptableObjects, not hardcoded values
- **Scalability:** Can grow from 1v1 matches to MMO world servers

## Next Phase

Phase 2 focuses on:
- Enhanced character animations and state machines
- Expanded combat move library (kicks, power moves, special abilities)
- Animation blending and transitions
- Ground detection refinement
- Sound effects integration

See [Docs/MVP-Roadmap.md](Docs/MVP-Roadmap.md) for full 10-phase plan.

---

**Ready to build the next generation of multiplayer fighting games.**
