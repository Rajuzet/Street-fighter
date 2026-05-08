# GitHub Strategy

## Branching Model
- `main`: production-ready and deployable.
- `develop`: integration branch for features and stabilizing before merge to main.
- `feature/*`: feature branches for gameplay, backend, UI, and systems.
- `hotfix/*`: emergency fixes for production issues.

## Pull Requests
- Require at least one review and automated CI pass.
- Use descriptive titles and link to relevant docs or tasks.
- Include architecture impact notes for system-level changes.

## CI/CD
- Validate code quality and tests on PR.
- Build backend artifacts and run static analysis.
- Gate merges on successful workflow completion.

## Documentation
- Keep `Docs/` synchronized with architecture and roadmap changes.
- Use code comments and README updates for major decisions.
