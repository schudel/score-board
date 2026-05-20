# GitHub Copilot Instructions – Score-Board

## Repository Overview

This is a full-stack score-tracking application:

- **`score-board-client`** – Angular 9 SPA. Main application with login, match management, live view, real-time chat via SignalR, i18n (en-us / de-de / de-ch).
- **`score-board-redirect`** – Angular 9 minimal app. Redirects to the correct locale-prefixed URL of the main app.
- **`score-board-server`** – .NET Core 3.1 Web API. JWT authentication, NHibernate ORM with SQL Server, SignalR hubs, Serilog logging, Swagger.

> This is a **legacy project** (EOL runtimes). Focus on correctness, security, and clarity over adding new features.

---

## Architecture Rules

- The Angular frontend communicates with the backend **only via the REST API** and **SignalR hubs** (`/chat`, `/live`).
- The backend follows a layered architecture: `API → Services → Infrastructure (Repositories) → Domain`.
- Do not bypass layers. Controllers call services; services call repositories; repositories use NHibernate sessions.
- The redirect app is independent of the main client and has no backend dependency.

---

## Coding Standards

### C# / .NET

- Target: .NET Core 3.1 (do not upgrade framework version without explicit request)
- Use `async/await` with `.ConfigureAwait(false)` in library/service code
- Prefer `IOptions<T>` for configuration injection over direct `IConfiguration` access
- Use constructor injection everywhere – no service locator pattern
- Follow existing naming: `I{Name}Service` / `{Name}Service`, `I{Name}Repository` / `{Name}Repository`
- DTOs in `ScoreBoard.API/Dtos/`, domain models in `ScoreBoard.Domain/Models/`
- Tests use xUnit and live in the corresponding `.Facts` project

### TypeScript / Angular

- Use Angular services for all HTTP calls (`score-board-client/src/app/services/rest/`)
- Environment-specific values go in `environment.ts` / `environment.prod.ts` – never inline in components
- Use reactive forms (`FormGroup`, `FormControl`) – no template-driven forms
- All HTTP interceptors are in `src/app/interceptors/`
- Auth guards are in `src/app/guards/`
- Follow existing component structure: `components/common/` and `components/routes/`

---

## Security Rules

### NEVER do any of the following

- Hardcode secrets, API keys, passwords, connection strings, or tokens in source files
- Add real credentials to `appsettings.json`, `appsettings.Development.json`, `environment.ts`, or `environment.prod.ts`
- Commit `.env` files, `secrets.json`, or `*.pfx` / `*.pem` / `*.key` files
- Log sensitive data (passwords, tokens, PII)
- Disable TLS/certificate validation

### Always do

- Use **`{placeholder}`** syntax for secrets in config files (as already established in this repo)
- Reference secrets via `IOptions<AppSettings>` (backend) or `environment.ts` variables (frontend)
- Use .NET User Secrets for local backend development: `dotnet user-secrets set "AppSettings:Secret" "..."`
- Use GitHub Secrets for CI/CD pipelines
- Validate JWT tokens with `ValidateIssuer = true` and `ValidateAudience = true` in production

### Known issues to fix (do not make worse)

- `RequireHttpsMetadata = false` in `Startup.cs` – flag as TODO if touched
- reCAPTCHA verification must be server-side – do not extend the broken client-side implementation
- SMTP credentials in `EmailService.cs` must be moved to `IOptions<SmtpSettings>` – if touching this file, fix it

---

## Configuration Pattern

Backend secrets go through this hierarchy (highest priority wins):
1. Environment variables (`AppSettings__Secret=...`)
2. `dotnet user-secrets` (local dev)
3. `appsettings.Development.json` (placeholders only)
4. `appsettings.json` (placeholders only)

Frontend environment config:
- `environment.ts` → dev builds
- `environment.prod.ts` → prod builds (`ng build --prod`)
- No real secrets belong in either file (site keys for public APIs are acceptable)

---

## Tests and Build Commands

### Backend

```bash
# Build
dotnet build score-board-server/ScoreBoard.sln

# Test (all projects)
dotnet test score-board-server/ScoreBoard.sln

# Run API
dotnet run --project score-board-server/ScoreBoard.API
```

### Frontend (client)

```bash
npm install            # in score-board-client/
npm start              # dev server (en-us)
npm run build:en-us    # prod build, en-us locale
npm run build-locale   # all locales
npm test               # unit tests (watch)
npm run tc-test        # unit tests (single run, CI)
npm run lint           # tslint
```

### Frontend (redirect)

```bash
npm install            # in score-board-redirect/
npm start
npm run deploy
```

---

## Adding New Features

1. **Backend**: Add domain model → repository interface + implementation → service interface + implementation → controller endpoint → DTO → register in `DependencyInjectionManager.cs`
2. **Frontend**: Add service in `services/rest/` → add model in `models/` → add component → add route in `app-routing.module.ts`
3. Write unit tests for new service logic
4. Do not add new NuGet or npm packages without explicit request
5. Mark any uncertain assumptions with `// TODO:` comments

---

## Dependency Updates

- Do not update dependencies without explicit request
- When updating: prefer minor/patch updates over major version bumps
- After updating, run the full test suite
- For security patches: update only the affected package and document why

## Legacy Code Guidelines

- This codebase uses patterns that may be considered outdated (e.g. `@aspnet/signalr` instead of `@microsoft/signalr`)
- Do not refactor working legacy code unless explicitly asked
- If you must touch legacy code, preserve its existing behavior
- Add `// TODO: upgrade to current equivalent` comments on clearly outdated APIs if helpful
- NHibernate is the ORM – do not suggest switching to EF Core unless explicitly asked
