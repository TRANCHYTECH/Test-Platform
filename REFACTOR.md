# Refactor charter

Working doc for the AI-agent-driven review/refactor pass. Fill in the `TODO` lines before pointing agents at any slice — vague intent produces drifty output.

## 1. Why we are refactoring

Selected goals for this pass (committed 2026-05-14). Anything not on this list is out of scope unless explicitly added later.

- [x] **One-click local run.** Today a fresh contributor must: `docker compose up` infra, set up an SSL cert at `$HOME/.aspnet/https/tranchyone.pem`, `az login`, populate `.development-env`, then run two `dotnet run`s + two `nx serve`s + (optionally) `dapr run` via VS Code tasks. Goal: `dotnet run --project backend/VietGeeks.TestPlatform.AppHost` brings up everything — no Azure creds, no RBAC, no pre-populated Key Vault.
  - **Decided approach: .NET Aspire AppHost** with first-party Dapr integration (Aspire Community Toolkit). Replaces `.vscode/tasks.json` Dapr wiring and the `backend/docker-compose.yml` infra block. Aspire issues the dev cert automatically; no manual `$HOME/.aspnet/https/...` setup.
  - **Secrets model: local-only, no Key Vault.** Production behavior unchanged — `AddTestPlatformKeyVault` continues to load from Azure Key Vault when `KeyVaultName` is set. In dev, the AppHost leaves `KeyVaultName` empty (already the default in `appsettings.json`) and injects per-service values via `WithEnvironment(...)`, sourced from `dotnet user-secrets` on the AppHost project. One-time onboarding paste: 1 required entry (Auth0 client secret), plus optional Mailjet sandbox keys.
  - **Infra: everything Aspire-managed containers.** Mongo (`AddMongoDB`), SQL Server (`AddSqlServer`), Redis (`AddRedis`), and **Azure Service Bus Emulator** (`microsoft/azure-messaging-servicebus-emulator` — Docker container released Dec 2024, supports queues/topics/subscriptions, no Azure account needed). MassTransit connects with `Endpoint=...UseDevelopmentEmulator=true` in dev, real Azure SB + managed identity in prod. **Same transport semantics dev and prod** — message-size limits, DLQ behavior, filter rules all match.
  - **DataProtection: filesystem in dev.** Switch `AddTestPlatformDataProtection` to `PersistKeysToFileSystem(...)` when `IsDevelopment()` and `BlobStorageName` is empty. No Azure Blob, no Key Vault key wrap in dev. Prod unchanged.
  - Service Bus emulator constraints (not blockers here): Linux Docker only, AMQP TCP only (no AMQP-WS), partitioned entities unsupported. This codebase uses none of those features.
  - Touches: `.vscode/launch.json`, `.vscode/tasks.json`, `backend/docker-compose.yml` (delete or shrink), `backend/VietGeeks.TestPlatform.BackOffice/HostBuilderExtensions.cs` (MassTransit dev branch), `backend/VietGeeks.TestPlatform.AspNetCore/HostBuilderExtensions.cs` (DataProtection dev branch), new `backend/VietGeeks.TestPlatform.AppHost` project. **No domain code changes**; only composition-root and dev-orchestration files.

- [x] **Remove dead / PoC code.** `PocProgram.cs` real-estate endpoints in `BackOffice` are unrelated to the test platform and were a scratch space (last touched June 2025). `UsePoc()` is wired into `BackOffice/Program.cs:102`. Delete both, verify Swagger still renders.

- [x] **Upgrade Dapr 1.14 → 1.17.** Three minors behind. v1.17 (Feb 2026) ships lower actor activation/timer latency, `FailurePolicy` on Actor Reminders, and `DrainOngoingCallTimeout` fixes — bug-fixes you don't want to re-debug during slice 5. Dapr is CNCF Graduated (Oct 2024) with quarterly cadence. Prerequisite for slice 5 (TestRunner refactor) per §3.

- [x] **Security hygiene — secrets currently in source.** Plaintext Mongo Atlas password at `backend/.dapr/components/statestore.yaml:14` (tracked in §8 P4). Empty Auth0 `ClientSecret` literal in `appsettings.json:22` (should be a `<KeyVault>` placeholder). Both fixed as part of this refactor pass.

- [x] **Flatten the question domain model.** Today `QuestionDefinition` (Mongo + contract) carries polymorphic `ScoreSettings` with `[BsonKnownTypes]` + `[JsonDerivedType]` keyed off an int enum, denormalized `TotalPoints`, duplicate `IsMandatory`, and two unused enum values (`AnswerType.TrueFalse`, `ShortAnswer` — neither has a calculator or validator path; both throw if instantiated). Goal: collapse to a flat `Scoring` value-object + `AnswerKind` enum (`SingleChoice | MultipleChoice`), one `Scoring.Score(answers, picked)` method replacing the calculator factory + 2 calculators, remove the duplicate `IsMandatory`, drop denormalized `TotalPoints`.
  - **Internal-only**: `QuestionViewModel` + `ScoreSettingsViewModel` wire shape stays identical; a mapper boundary translates between flat-internal and polymorphic-wire. The SPA is untouched. This is the only way to land the flattening without violating "no public-API renames" in §2.
  - Includes a one-time Mongo migration (custom Bson convention or a one-shot script): old docs have `ScoreSettings._t` discriminator + `ScoreSettings.TotalPoints` + outer `IsMandatory`; new docs have a single `Scoring` sub-document.
  - **Prerequisite**: P5 — confirm with product that `TrueFalse` / `ShortAnswer` aren't on the near-term roadmap before deleting the enum values.

Candidates **not** selected for this pass (move into a future pass if priorities change):

- ~~Tighten layering / DDD boundaries~~ — too vague without a specific symptom; revisit when a concrete pain point surfaces.
- ~~Replace deprecated Akita state~~ — large frontend rewrite; do as a focused initiative after the backend stabilizes.
- ~~Upgrade .NET 8 → .NET 9 / Angular 16 → current LTS~~ — .NET 8 is LTS through Nov 2026; no urgency.
- ~~Productionize the OpenAI question-import flow~~ — still PoC-shaped (slice 7 retained in §4 as a placeholder, but not a §1 goal yet).
- ~~Distributed cache for TestRunner~~ — gated on P2 (TestRunner roadmap).
- ~~Test coverage uplift~~ — handled per-slice via the "tests to add first" column in §4; not a standalone goal.
- ~~Observability / OpenTelemetry~~ — defer until App Insights or another sink is actually wired up; no concrete pain reported.

## 2. Explicit non-goals

Committed 2026-05-14. Agents must not "improve" their way past these in any slice.

- **Stay on Azure.** No portability work in this pass. Dapr's neighbors (Key Vault, Service Bus, Blob, ACR, Bicep) keep their Azure-specific implementations. The Service Bus emulator in slice 1.5 is a local-dev convenience, not a step toward AWS/GCP/on-prem.
- **No Auth0 tenant or auth-flow changes.** Multi-auth scheme behavior (cookie for SPA, JWT for user-sync, policy-based scheme switching by audience) is preserved exactly. Local dev hits the real Auth0 dev tenant — same code path as prod.
- **No MongoDB migration.** Mongo + `MongoDB.Entities` stays for domain data; SQL Server + EF Core stays for sessions. No CosmosDB, no Postgres-everywhere proposals.
- **No public-API renames.** Controllers, routes, MassTransit message contracts, Dapr actor interfaces, and Mongo entity shapes stay backwards-compatible. The SPAs are in flight and will break on any of these.
- **No deployment-infra changes.** `deployment/*.bicep`, `.github/workflows/*`, `runner.azcli` are untouched. (Already in `.claude/settings.json` as a hard deny.)
- **No domain code changes in slice 1.5.** That slice is composition-root and dev-orchestration only.
- **No frontend framework upgrade.** Angular 16 + Akita stays in place during this pass. Slice 8 (Akita migration) is sized but not started here; Angular upgrade (slice 9) is a separate later pass.
- **No .NET version bump.** .NET 8 LTS for this entire pass.

## 3. Constraints

- .NET 8 LTS until <TODO date>.
- Mongo + SQL Server side-by-side persistence model stays.
- Multi-auth scheme behavior (cookie for SPA, JWT for user-sync) must be preserved — Auth0 actions depend on it.
- Key Vault `<KeyVault>` placeholder convention must be preserved (the secret-manager prefix scopes by app name).
- Outbox semantics (30s duplicate-detection window) on MassTransit must be preserved.
- **New orchestration code uses Dapr Workflows, not new Actor types.** `ProctorActor` is correctly-shaped for raw actors (one entity per exam, single-threaded, no cross-actor calls) and stays. Any new multi-step coordination (e.g. post-exam grading pipeline, retry-with-compensation flows) should use Dapr Workflows — the 2026 upstream guidance is "start with Workflows unless you specifically need actor primitives," because Workflows are built on actors and avoid the classic footguns (dependency graphs, reentrancy).
- **Vendor lock-in lives in Dapr's *neighbors*, not in Dapr.** Dapr itself runs unchanged on any K8s, self-hosted, or Diagrid-managed. What's Azure-pinned: Key Vault, Service Bus (MassTransit transport), Blob DataProtection keys, ACR, Bicep + `arm-deploy`. If "stay on Azure" is **not** an explicit non-goal (see §2), agents should treat the Dapr layer as the *portable* part and resist the temptation to "simplify" by inlining Azure SDK calls past it.

## 4. Slicing

One PR per slice. Order top-to-bottom; ship the foundation slices before the agent slices.

| # | Slice | Owner | Tests to add first | Status |
|---|-------|-------|--------------------|--------|
| 0 | Repo prep: this charter + `CLAUDE.md` + agent guardrails (`.claude/settings.local.json`) | | n/a | in progress |
| 1 | Delete `PocProgram.cs` + `UsePoc()` wiring; verify Swagger still builds | | smoke run of BackOffice | |
| 1.5 | **One-click local dev** — Aspire AppHost launches Mongo + SQL + Redis + **Service Bus emulator** + both APIs + both SPAs + Dapr sidecars. **No Key Vault locally**: AppHost injects dev secrets via `WithEnvironment(...)` from `dotnet user-secrets`; prod KV path unchanged. MassTransit branches to `UseDevelopmentEmulator=true` in dev. DataProtection switches to `PersistKeysToFileSystem` in dev. Updates `/smoke-backoffice` to use AppHost. New project: `backend/VietGeeks.TestPlatform.AppHost`. Composition-root only — no domain code changes. | | n/a — verify by running it | |
| 2 | TestManager.Data scoring (`Mixers/Calculators/*`) — characterization tests, then any refactor | | xUnit cases per calculator | |
| 2.5 | **Flatten question domain model** (§1) — drop polymorphic `ScoreSettings`, drop `AnswerType.TrueFalse`/`ShortAnswer` enum values, collapse calculators into a single `Scoring.Score(...)`, remove denormalized `TotalPoints`, deduplicate `IsMandatory`, rename `Answer.AnswerDescription` → `Text` internally. **Internal-only** — wire contract preserved via mapper boundary. Includes Mongo migration script. **Prerequisite: slice 2 characterization tests must be green first** (they're the safety net). **Prerequisite: P5 cleared.** Out-of-scope: API v2 / wire-shape change — deferred to a future pass when the SPA team is ready. | | slice 2 tests stay green; add round-trip tests for the mapper boundary | |
| 3 | TestManager service layer (`Infrastructure/Services/*`) — extract interfaces vs concrete | | service-level tests with Mongo testcontainer or mocks | |
| 4 | AccountManager — `AccountController` / `AccountProcessor` cleanup | | xUnit project (new) | |
| 5 | TestRunner — `ProctorActor` + `ProctorService` boundary + distributed cache. **Prerequisite: Dapr 1.14 → 1.17 upgrade (§1, §3).** | | actor unit tests | |
| 6 | BackOffice composition root — split `Program.cs` into feature wireups | | smoke run | |
| 7 | OpenAI question-import productionization | | golden-file tests with recorded fixtures | |
| 8 | Frontend state migration (Akita → <TODO: NgRx Signals? plain signals?>) per feature folder | | jest specs around each store | |
| 9 | Angular upgrade — separate from state migration | | nx affected:test | |

## 5. Guardrails for agents

- **Build must stay green**: `dotnet build backend\VietGeeksTestPlatform.sln` and `nx affected:build` after every slice.
- **Run tests**: `dotnet test ./backend/VietGeeks.TestPlatform.TestManager.UnitTest` (and any new test project in the slice).
- **Do not touch** without explicit human go-ahead:
  - `backend/.env`, anything with `<KeyVault>` placeholders, `Properties/launchSettings.json`
  - `.github/workflows/*`, `deployment/*.bicep`
  - `Directory.Packages.props` version bumps that cross a major (open a separate PR)
- **One slice per PR**; PR description must list which `CLAUDE.md` conventions the change touches.
- **Conventions to follow**: namespaces `VietGeeks.TestPlatform.<Module>`; module registration via `RegisterXxxModule`; FluentValidation per aggregate; AutoMapper profiles under `MapperProfiles/`; throw `TestPlatformException` for domain errors.

## 6. Review loop

- `/review` per PR for diff-level feedback.
- `/ultrareview` once per phase (end of slice 3, 6, 9) for an architecture pass.
- Human signs off before merge on any slice that changes a public contract (controllers, MassTransit messages, actor interfaces) or Mongo schema.

## 7. Known risks before we start

- No integration tests covering BFF + multi-auth + MassTransit — easy to silently break.
- Mongo schemas are implicit (`MongoDB.Entities`) — renames need a migration plan.
- The deployed Container Apps rely on the managed identity having Key Vault `get`/`list` — refactors that rename appName prefixes will silently miss secrets.
- Akita is unmaintained but the SPA depends on it heavily; partial migration risks dual state systems.

## 8. Open prerequisites (resolve before the relevant slice starts)

| # | Question / action | Blocks slice | Owner | Status |
|---|---|---|---|---|
| ~~P1~~ | ~~Confirm developer Azure-RBAC on `tranchy-rg-testplatform-dev-southeastasia`~~ — **resolved by design**: slice 1.5 no longer uses `RunAsExisting`; dev runs without Key Vault. Prod KV access (managed identity on the Container App) is unchanged. | — | — | closed |
| P2 | Decide TestRunner roadmap: mothballed or being revived? (Git shows zero functional changes since Aug 2024.) If mothballed, slice 5 becomes minimum-maintenance; if revived, it leapfrogs slices 2–4. | 5 | <TODO — product> | open |
| P3 | Confirm production status of the `webhook-pubsub` Dapr component. Referenced in `AccountManager/Controllers/AccountProcessor.cs` (`[Topic("webhook-pubsub", ...)]`) but undeclared in any `.dapr/components/*` or `deployment/*.bicep`. Either configured out-of-band in Azure, or dead code superseded by the MassTransit path in `WebhookController`. | 4 | <TODO — ops> | open |
| P4 | Rotate the MongoDB Atlas credentials currently committed in plaintext at `backend/.dapr/components/statestore.yaml` (lines 13-14). Treat as leaked. Then either gitignore the file with a local override, or move secrets to Bicep params like the production Dapr component already does. | (independent — do ASAP) | <TODO — ops> | open |
| P5 | Confirm with product whether `AnswerType.TrueFalse` and `AnswerType.ShortAnswer` are on the roadmap. Today both are declared but have no calculator, no validator path, and `ShortAnswerScoreSettings` is unused while `TrueFalseScoreSettings` doesn't exist at all. If on roadmap, slice 2.5 keeps them as `Unsupported` placeholders. If not, slice 2.5 deletes them. | 2.5 | <TODO — product> | open |

---

## Appendix A — Local dev secrets (for slice 1.5)

Slice 1.5 does **not** use Key Vault in local dev. Production loading via `AddTestPlatformKeyVault` is unchanged (managed identity on the Container App). This appendix lists what the AppHost wires up locally and what each contributor pastes once into `dotnet user-secrets`.

Source of truth: the AppHost project once slice 1.5 lands. Update this appendix in the same PR that adds or removes an env-injection.

### A.1 — What the AppHost wires automatically

The AppHost spins these up and passes connection strings to the consuming services via `WithEnvironment(...)`. No developer action needed.

| Container | Aspire resource | Injected into |
|---|---|---|
| MongoDB | `builder.AddMongoDB("mongo").AddDatabase("testmanager-dev")` | `ConnectionStrings__TestManager` on BackOffice + FrontOffice |
| SQL Server | `builder.AddSqlServer("sql").AddDatabase("user-session1")` | `ConnectionStrings__UserSession` on BackOffice |
| Redis | `builder.AddRedis("redis")` | (TestRunner distributed-cache slice — slice 5) |
| Service Bus Emulator | `builder.AddContainer("sb", "microsoft/azure-messaging-servicebus-emulator")` + SQL backing | `ConnectionStrings__ServiceBus = ...;UseDevelopmentEmulator=true` on BackOffice |
| Dapr sidecars | Aspire Community Toolkit Dapr integration, `componentsPath: backend/.dapr/components` | Per-service sidecar |

### A.2 — What the developer pastes once (user-secrets on the AppHost project)

```powershell
cd backend\VietGeeks.TestPlatform.AppHost
dotnet user-secrets set "Auth0:ClientSecret" "<paste from Auth0 dev tenant>"           # required
dotnet user-secrets set "MailJet:PrivateKey" "<sandbox key>"                            # optional — SendTestAccessCode consumer
dotnet user-secrets set "MailJet:PublicKey"  "<sandbox key>"
```

The AppHost reads these and injects them via `WithEnvironment(...)` (e.g. `Authentication__Schemes__BackOffice__ClientSecret`).

**Auth0 stays real in dev** (decision: 2026-05-14). Devs hit the real Auth0 dev tenant; the multi-auth scheme behavior in `AspNetCore/ServiceCollectionExtensions.cs` runs the same code path as prod.

**OpenAI is not configured in dev** (decision: 2026-05-14). The question-import flow (`Management/TestDefinition/{testId}/Question:ReadFile`) is intentionally disabled locally. Slice 1.5 should:

- AppHost sets `OpenAi:Endpoint` to a placeholder like `https://disabled.invalid` so DI resolution of `AzureOpenAIClient` doesn't throw at boot, AND
- The controller already has a top-level `try/catch` that returns `400 BadRequest` on any failure — that's the existing degradation path. Anything more (a clean 503 + logged "import disabled in dev") belongs in **slice 7** (OpenAI productionization), not 1.5.

Minimum to boot the platform: **only `Auth0:ClientSecret`**. Mailjet stays optional — `SendTestAccessCode` consumer fails gracefully if keys aren't set.

### A.3 — Production KV inventory (unchanged, for reference)

Production still uses `AddTestPlatformKeyVault` against `TranchyKvTestPlatfDevSa`. Anything tagged `<KeyVault>` in `appsettings.json` must exist there with the prefix scheme below.

Naming convention (preserved per §3): `<app-prefix>-<config-key>` or `shared-<config-key>`, with `--` mapping to `:`. App prefixes: `test-manager-api-`, `test-runner-api-`. See `backend/VietGeeks.TestPlatform.AspNetCore/TestPlatformKeyVaultSecretManager.cs`.

Verification (run after `az login` against the prod tenant):

```powershell
az keyvault secret list --vault-name TranchyKvTestPlatfDevSa --query "[].name" -o tsv
az keyvault key    list --vault-name TranchyKvTestPlatfDevSa --query "[].name" -o tsv
```

The prod KV inventory itself is outside this charter's scope — owned by whoever provisions the environment via `deployment/base-infrastructure/key-vault-secrets.bicep`.

---

Update this doc as decisions land. When all sections 1–4 are filled in and slice 0 is done, agents can start picking up slices.
