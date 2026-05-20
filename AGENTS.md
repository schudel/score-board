# AGENTS.md – AI Coding Agent Guidelines

This file describes the repository structure, conventions, and rules for AI coding agents (GitHub Copilot, Codex, Claude, etc.) working in this codebase.

---

## Repository Overview

Score-Board is a full-stack score-tracking application with real-time features:

| Project | Technology | Purpose |
|---------|-----------|---------|
| `score-board-client/` | Angular 9, TypeScript | Main SPA with login, match management, live view, chat |
| `score-board-redirect/` | Angular 9, TypeScript | Minimal redirect app (root domain → locale URL) |
| `score-board-server/` | .NET Core 3.1, C# | REST API, JWT auth, NHibernate, SignalR hubs |

> **Legacy project.** .NET Core 3.1 and Angular 9 are end-of-life. Do not upgrade framework versions unless explicitly requested.

---

## Project Structure

```
score-board/
├── .github/
│   ├── copilot-instructions.md
│   └── dependabot.yml
├── score-board-client/
│   └── src/app/
│       ├── components/common/     # Shared UI components
│       ├── components/routes/     # Route-level components
│       ├── guards/                # Auth guards
│       ├── interceptors/          # HTTP interceptors
│       ├── models/                # TypeScript interfaces/models
│       ├── pipes/                 # Angular pipes
│       └── services/rest/         # All HTTP API calls
├── score-board-redirect/
│   └── src/app/
└── score-board-server/
    ├── ScoreBoard.API/            # Controllers, Startup, DI, Hubs, DTOs
    ├── ScoreBoard.Domain/         # Domain models, enums (no dependencies)
    ├── ScoreBoard.Infrastructure/ # NHibernate mappings, repositories
    ├── ScoreBoard.Services/       # Business logic (UseCases, Helpers, Adapters)
    ├── ScoreBoard.Fakes/          # Test fakes
    ├── ScoreBoard.Init/           # One-time DB seeding (not for production)
    └── *.Facts/                   # xUnit test projects
```

---

## Setup

```bash
# Backend
cd score-board-server
dotnet restore
dotnet user-secrets set "AppSettings:Secret" "your-jwt-secret"
dotnet user-secrets set "AppSettings:DbConnectionString" "Server=...;..."
dotnet run --project ScoreBoard.API

# Frontend (client)
cd score-board-client
npm install
npm start

# Frontend (redirect)
cd score-board-redirect
npm install
npm start
```

---

## Build Commands

### Backend

```bash
dotnet build score-board-server/ScoreBoard.sln
dotnet publish score-board-server/ScoreBoard.API -c Release
```

### Frontend

```bash
# score-board-client
npm run build:en-us          # single locale
npm run build-locale         # all locales (en-us, de-de, de-ch)

# score-board-redirect
npm run deploy
```

---

## Test Commands

### Backend

```bash
dotnet test score-board-server/ScoreBoard.sln
dotnet test score-board-server/ScoreBoard.API.Facts
dotnet test score-board-server/ScoreBoard.Services.Facts
dotnet test score-board-server/ScoreBoard.Domain.Facts
```

### Frontend

```bash
# score-board-client
npm test                # watch mode
npm run tc-test         # single run (CI)
npm run lint

# score-board-redirect
npm test
```

---

## Backend Architecture Rules

- Layers: `API Controllers → Services → Repositories → Domain`
- Never bypass layers (controllers must not call repositories directly)
- `DependencyInjectionManager.cs` is the single place to register all bindings
- Use `async/await` with `.ConfigureAwait(false)` in all service and repository methods
- Inject `IOptions<T>` for configuration; never inject `IConfiguration` directly
- DTOs live in `ScoreBoard.API/Dtos/`; domain models in `ScoreBoard.Domain/Models/`
- Naming: `I{Name}Service` + `{Name}Service`, `I{Name}Repository` + `{Name}Repository`

## Frontend Architecture Rules

- All HTTP calls go through `services/rest/` – never call `HttpClient` directly in components
- Environment-specific values go in `environment.ts` / `environment.prod.ts`
- Use reactive forms (`FormGroup`, `FormControl`) only – no template-driven forms
- Auth guards are in `src/app/guards/`; HTTP interceptors in `src/app/interceptors/`

---

## Coding Guidelines

- Mark uncertain assumptions with `// TODO:` comments
- Do not add docstrings or comments to code you did not change
- Do not add error handling for impossible scenarios
- Do not create abstractions or helpers for one-time operations
- Do not over-engineer – only make changes that are directly requested
- Do not add new NuGet or npm packages without explicit user request

---

## Security Guidelines

### NEVER

- Hardcode secrets, API keys, passwords, tokens, connection strings in any source file
- Add real credentials to `appsettings*.json`, `environment*.ts`, `Constants.cs`, or test files
- Commit `.env`, `secrets.json`, `*.pfx`, `*.pem`, `*.key` files
- Log sensitive data (passwords, tokens, email addresses, PII)
- Disable TLS or certificate validation

### ALWAYS

- Use `{placeholder}` syntax in committed config files
- Use .NET User Secrets for local backend secrets
- Use environment variables or GitHub Secrets for CI/CD
- Reference secrets via `IOptions<AppSettings>` (backend) or `environment.ts` (frontend)

### Known Issues (do not make worse)

- `RequireHttpsMetadata = false` in `Startup.cs` – add `// TODO: set to true in production` if you touch it
- `ValidateIssuer = false` and `ValidateAudience = false` – document why, or fix
- reCAPTCHA verification is broken (currently done client-side) – do not extend this pattern
- SMTP credentials are hardcoded as constants in `EmailService.cs` – must be moved to `IOptions<SmtpSettings>` if that file is touched

---

## Do / Don't

| Do | Don't |
|----|-------|
| Use `IOptions<T>` for config | Inject `IConfiguration` directly |
| Use constructor injection | Use service locator pattern |
| Write tests in `.Facts` projects | Add test code to production projects |
| Use reactive forms in Angular | Use template-driven forms |
| Placeholder tokens in config files | Real secrets in config files |
| `.ConfigureAwait(false)` in libraries | Omit it in async service code |
| Mark assumptions with `// TODO:` | Silently guess behavior |
| Preserve legacy code behavior | Refactor working code without a reason |

---

## PR Checklist

Before submitting a pull request, verify:

- [ ] No secrets, credentials, or PII in changed files
- [ ] No real values in `appsettings*.json` or `environment*.ts`
- [ ] All new services registered in `DependencyInjectionManager.cs`
- [ ] New service logic has unit tests in the corresponding `.Facts` project
- [ ] `npm run lint` passes (frontend)
- [ ] `dotnet build` succeeds without errors or warnings
- [ ] `dotnet test` passes for all affected test projects
- [ ] No new NuGet or npm packages added without explicit request
- [ ] Legacy code behavior preserved if touched
- [ ] Uncertain assumptions marked with `// TODO:`

---

## Secrets & Configuration Reference

| Secret | How to set locally | CI/CD |
|--------|-------------------|-------|
| JWT Signing Secret | `dotnet user-secrets set "AppSettings:Secret" "..."` | GitHub Secret `APPSETTINGS__SECRET` |
| DB Connection String | `dotnet user-secrets set "AppSettings:DbConnectionString" "..."` | GitHub Secret `APPSETTINGS__DBCONNECTIONSTRING` |
| SMTP Password | Via `IOptions<SmtpSettings>` + User Secrets | GitHub Secret |
| reCAPTCHA Site Key | `environment.ts` (public, safe) | `environment.prod.ts` |
| Giphy API Key | `environment.ts` → move to env var | GitHub Secret |

---

## No Large Refactoring Without Explicit Task

AI agents must **not** perform large-scale refactoring, rename symbols, reorganize file structure, or upgrade framework versions unless the user explicitly requests it. Targeted, minimal changes only.
