# Security Policy

## Maintenance Status

This is a **legacy / personal project**. It is provided as-is for educational and reference purposes.

- Active security patches: **not guaranteed**
- The backend targets .NET Core 3.1 (end-of-life since December 2022)
- The frontend targets Angular 9 (end-of-life)
- It is strongly recommended **not to deploy this software in a production environment** without first upgrading all dependencies

| Version | Supported |
|---------|-----------|
| current `develop` branch | Limited (personal best-effort) |
| older branches | No |

---

## Reporting a Vulnerability

If you discover a security vulnerability in this repository, please **do not open a public GitHub issue**.

Instead, contact the maintainer directly:

> **TODO:** Add a security contact e-mail address or GitHub private vulnerability reporting URL here.
>
> Example: `security@TODO-your-domain.com`  
> Or use [GitHub's private vulnerability reporting](https://docs.github.com/en/code-security/security-advisories/guidance-on-reporting-and-writing/privately-reporting-a-security-vulnerability) if enabled on this repository.

Please include:
- A description of the vulnerability
- Steps to reproduce
- Potential impact
- Any suggested fix (optional)

You will receive an acknowledgement within **7 days** (best-effort for a personal project).

---

## Guidelines

### No Secrets in Issues or Pull Requests

**Never post** any of the following in public GitHub issues, pull requests, or discussions:
- API keys, tokens, secrets
- Passwords or connection strings
- Private keys or certificates
- Personal data

### Hardcoded Secrets Policy

This repository follows the principle that **no secrets are ever committed to source control**:

- Secrets are managed via **.NET User Secrets** (local development) or **environment variables** (CI/CD/production)
- `appsettings.json` / `environment.ts` files contain only **placeholder tokens** (e.g. `{mySecretKey}`)
- If you suspect a secret was accidentally committed, treat it as compromised and rotate it immediately, then use `git filter-repo` or open a fresh repository

### Dependency Security

- Run `dotnet list package --vulnerable` regularly
- Run `npm audit` regularly
- Dependabot alerts are recommended (see `.github/dependabot.yml`)

---

## Known Security Limitations

The following known issues exist in the current codebase and should be addressed before any production deployment:

1. **`RequireHttpsMetadata = false`** in JWT Bearer configuration (`Startup.cs`) – must be set to `true` in production
2. **JWT issuer and audience not validated** – `ValidateIssuer` and `ValidateAudience` are `false`
3. **reCAPTCHA verification** is partially implemented client-side (Angular) instead of server-side
4. **SMTP credentials** are hardcoded as `const` fields in `EmailService.cs` – must be moved to configuration
5. **.NET Core 3.1 and Angular 9 are end-of-life** – known unpatched CVEs may exist in dependencies
