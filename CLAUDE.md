# Test-Platform (VietGeeks)

Online assessment platform: authors build tests in a back-office SPA, candidates take proctored exams in a runner SPA. Two .NET 8 APIs plus an Nx/Angular 16 monorepo.

## Repo layout

```
backend/                      .NET 8 solution (VietGeeksTestPlatform.sln)
  VietGeeks.TestPlatform.SharedKernel        Cross-cutting types (DatabaseOptions, IClock, exceptions, auth scheme/policy names, events)
  VietGeeks.TestPlatform.AspNetCore          Host/Service extensions: KeyVault config, DataProtection, OIDC + multi-auth, BFF wiring, tenant resolution
  VietGeeks.TestPlatform.Integration.Contract Messages shared across services (UserCreateRequest, SendTestAccessCodeRequest, TestInvitiationEventData)
  VietGeeks.TestPlatform.AccountManager      User/account aggregate; registered as a module inside BackOffice
  VietGeeks.TestPlatform.BackOffice          API host #1 — admin/author backend (appName "test-manager-api"); BFF for the author SPA
  VietGeeks.TestPlatform.TestManager         Test/question domain (controllers, services, validators, mapper profiles, MassTransit consumers)
  VietGeeks.TestPlatform.TestManager.Data    Domain entities + scoring mixers/calculators (GradingCalculator, SingleChoice/MultipleChoice, TestSuiteMixer)
  VietGeeks.TestPlatform.TestManager.UnitTest xUnit tests (only this module has tests)
  VietGeeks.TestPlatform.TestRunner          Test runner domain (Dapr ProctorActor, ExamController, proctor services)
  VietGeeks.TestPlatform.FrontOffice         API host #2 — exam runner (appName "test-runner-api")
frontend/                     Nx 16 workspace
  apps/test-manager           Angular 16 SPA for authors (Akita state in _state folders)
  apps/test-runner            Angular 16 SPA for candidates
  apps/test-manager-e2e       Cypress
  libs/core, libs/shared      Shared Angular libs
.github/workflows/            backoffice-api-dev, testrunner-api-dev (build + Azure Container App deploy), two SWA workflows
deployment/                   container-app-dapr.bicep (referenced by workflows)
```

## Architecture quick map

- **Two API hosts**: `BackOffice` (author/admin, port via BFF + cookies) and `FrontOffice` (exam runner, Dapr actors). They share `SharedKernel` + `AspNetCore` + `Integration.Contract`.
- **Auth**:
  - BackOffice uses Duende BFF + OIDC against Auth0 (`Authentication:Schemes:BackOffice`); SPA gets HttpOnly cookies, APIs are called via BFF proxy.
  - A second JWT scheme `UserSynchronization` handles Auth0 user-sync webhooks/tokens. A policy scheme (`AuthenticationSchemaNames.MultiAuthSchemes`) routes per-token-audience.
  - TestRunner uses cookie auth (HttpOnly + SameSite=Strict).
- **Persistence**:
  - Domain data in **MongoDB** via `MongoDB.Entities` (DB.InitAsync in each module's `ServiceCollectionExtensions`).
  - User sessions in **SQL Server** via EF Core (`SessionDbContext`, migrations in `BackOffice/Migrations/UserSessions`). `ApplyMigrationsOnStartup` flag in appsettings.
  - Redis available in `docker-compose.yml` (dev memory cache is `IMemoryCache` for now — note the `//todo: use distributed cache` in TestRunner).
- **Messaging**: MassTransit on Azure Service Bus with **Mongo outbox** (`AddMongoDbOutbox`, 30s duplicate detection). Endpoints kebab-cased; dev prefix is the OS username. Consumers in `TestManager.Infrastructure.EventConsumers`.
- **Dapr**: `AddDaprClient()` in both hosts; `AddActors(RegisterActor<ProctorActor>())` in TestRunner. Component config in `backend/.dapr/components/statestore.yaml`.
- **Secrets**: `<KeyVault>` placeholders in `appsettings.json` are resolved by `AddTestPlatformKeyVault` using a managed identity (`GetTokenCredential`) and `TestPlatformKeyVaultSecretManager` which scopes by appName prefix (`test-manager-api-*` / `test-runner-api-*`).
- **Data Protection**: keys persisted to Azure Blob, protected by Key Vault key (`AddTestPlatformDataProtection`).
- **OpenAI**: `AzureOpenAIClient` registered in BackOffice. Used by the question-import flow (`Api/Controllers/ImportQuestionsController.cs`). Deployment is `gpt-4o` per appsettings.

## Build / run / test

Backend (run from repo root):
```powershell
dotnet build backend\VietGeeksTestPlatform.sln
dotnet test  backend\VietGeeks.TestPlatform.TestManager.UnitTest\VietGeeks.TestPlatform.TestManager.UnitTest.csproj
dotnet run   --project backend\VietGeeks.TestPlatform.BackOffice    # BackOffice API
dotnet run   --project backend\VietGeeks.TestPlatform.FrontOffice   # TestRunner API
```

Local infra:
```powershell
docker compose -f backend\docker-compose.yml up -d   # mongo (27017), azure-sql-edge (1434), redis (6379)
```

Frontend (run from `frontend/`):
```powershell
npm install
npm run start:test-manager    # nx serve test-manager --port 4200 --ssl
npm run start:test-runner     # nx serve test-runner  --port 4300 --ssl
npm run test                  # nx test
```

CI: `.github/workflows/backoffice-api-dev.yml` and `testrunner-api-dev.yml` run the `TestManager.UnitTest` project, build the Docker image with the matching `final-backoffice` / `final-testrunner` target, push to ACR, then deploy via `deployment/container-app-dapr.bicep` to Azure Container Apps.

## Conventions

- **Namespace + folder**: `VietGeeks.TestPlatform.<Module>`. Modules expose `RegisterXxxModule(this IServiceCollection, XxxModuleOptions)` from a `ServiceCollectionExtensions` class.
- **Central package management**: all versions pinned in `backend/Directory.Packages.props`. Add a `<PackageVersion>` there before referencing in a `.csproj`.
- **Common props**: `backend/Directory.Build.props` enables nullable, implicit usings, and analyzers (Roslynator, Meziantou, BannedApiAnalyzers). Warnings are not errors today.
- **Tests**: xUnit + Moq + Faker.Net (auto-included when `IsTestProject == true`).
- **Frontend state**: each feature has a `_state/` folder with Akita `*.store.ts`, `*.query.ts`, `*.service.ts`, `*.model.ts`. Note Akita is deprecated upstream — relevant if a refactor touches state.
- **Frontend layouts**: `_layouts/` and `_base/` for shared shells/base classes.
- **Validators**: FluentValidation under `Infrastructure/Validators/<Aggregate>/`.
- **Mapping**: AutoMapper profiles under `Infrastructure/MapperProfiles/` (or `MapperProfiles/` for AccountManager).
- **Tenant**: `ITenant`/`Tenant` in AspNetCore — resolves the current tenant from the HTTP context; inject rather than reading claims directly.
- **Exceptions**: throw `TestPlatformException` for business errors; the global handler in `UseVietGeeksEssentialFeatures` converts them to 400 + `ErrorDetails`.

## Things to know before changing code

- Touching `BackOffice/Program.cs` affects BFF + multi-auth + MassTransit + Mongo init — there are no integration tests covering this. Smoke-test locally against `docker-compose` infra before merging.
- Adding a new persisted entity needs a `MongoDB.Entities` registration; for SQL Server, add an EF migration under `BackOffice/Migrations/UserSessions` and verify `ApplyMigrationsOnStartup` behavior.
- Adding a new message: declare in `Integration.Contract`, add a consumer in `TestManager.Infrastructure.EventConsumers`, and ensure `AddConsumersFromNamespaceContaining<...>` still picks it up.
- Key Vault: any new setting that should come from the vault must be declared in `appsettings.json` with value `"<KeyVault>"` so `AddTestPlatformKeyVault` discovers it.
- Dapr actors run on the sidecar — `ProctorActor` changes need the runner host + the Dapr sidecar (locally via `dapr run` or compose) to test end-to-end.

## Refactor

See `REFACTOR.md` for the in-progress charter (scope, goals, slicing, guardrails). Update it before kicking off agent-driven work.
