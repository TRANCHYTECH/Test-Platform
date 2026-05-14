---
description: Boot local infra + BackOffice API and verify the host starts clean. Useful after touching Program.cs, auth, MassTransit, or module registration.
---

You are smoke-testing the BackOffice API end-to-end on the local machine. Goal: prove the host boots, all module registrations resolve, Swagger renders, and the BFF auth endpoints are reachable. You are NOT testing business logic — `/verify-slice` already covered that.

## Prerequisites the user should have set up once

- Local dev cert trusted (`dotnet dev-certs https --trust`)
- Azure CLI logged in to a tenant whose managed identity can reach the dev Key Vault (`az login`) — `TestPlatformKeyVaultSecretManager` will fail boot if Key Vault values cannot resolve
- `KeyVaultName` set locally (either in user secrets or env var) — `appsettings.json` ships it empty

If any of these are missing, stop and ask the user before proceeding.

## Steps

### 1. Bring up local infra

```powershell
docker compose -f backend\docker-compose.yml up -d
docker compose -f backend\docker-compose.yml ps
```

Confirm `mongo` (27017), `sql` (1434), and `redis` (6379) are healthy. If a container is already running, leave it.

### 2. Build

```powershell
dotnet build backend\VietGeeks.TestPlatform.BackOffice\VietGeeks.TestPlatform.BackOffice.csproj
```

Stop on build failure.

### 3. Run BackOffice in the background

```powershell
dotnet run --project backend\VietGeeks.TestPlatform.BackOffice
```

Launch with `run_in_background: true`. Watch the log until you see either:

- a line containing `Now listening on: https://localhost:7200` → continue
- an exception during host startup → stop and report

Common failure modes to call out explicitly if you see them:

- `KeyVaultName` empty or 401 from Key Vault → user needs to set it / re-`az login`
- Mongo connection refused → docker compose not up
- EF migration error on `SessionDbContext` → SQL container not up or `ApplyMigrationsOnStartup` flipped off

### 4. Probe

In parallel:

```powershell
curl.exe -k -s -o NUL -w "%{http_code}\n" https://localhost:7200/swagger/index.html
curl.exe -k -s -o NUL -w "%{http_code}\n" https://localhost:7200/swagger/v1/swagger.json
curl.exe -k -s -o NUL -w "%{http_code}\n" https://localhost:7200/bff/user
curl.exe -k -s -o NUL -w "%{http_code}\n" https://localhost:7200/test-portal
```

Expected:

- swagger UI → 200
- swagger.json → 200; pipe it through `jq '.paths | keys | length'` to count endpoints and report the number (sanity check that controllers registered)
- `/bff/user` → 401 unauthenticated, but 401 is "healthy" here (BFF is responding)
- `/test-portal` → 301/302 redirect to `PortalUrl`

### 5. Tear down

```powershell
# stop the background dotnet run (use the bg shell id from step 3)
docker compose -f backend\docker-compose.yml down   # only if you started it in step 1
```

## Report format

```
BackOffice smoke
  Infra (mongo/sql/redis) ......... up / down
  Build ........................... PASS / FAIL
  Host boot ....................... PASS / FAIL  (elapsed Xs)
  Swagger UI ...................... 200 / <code>
  Swagger JSON .................... 200 / <code>  (n endpoints)
  /bff/user ....................... 401 (expected) / <code>
  /test-portal redirect ........... 30x to <PortalUrl> / <code>
  Notable startup logs:
    - ...
```

Do not commit, push, or change Azure resources. Verification only.
