---
description: Run the canonical green-check sequence for a refactor slice (build + test, backend + frontend).
---

You are verifying a refactor slice is safe to merge. Run the checks below in order and report a concise table with status per step. Do not "fix" anything — your job is to report. If a step fails, stop and show the failure output.

## 1. Backend build

```powershell
dotnet build backend\VietGeeksTestPlatform.sln
```

Report: warning count, error count, elapsed time. Flag any new warnings vs. base if you can tell from the diff.

## 2. Backend tests

```powershell
dotnet test backend\VietGeeks.TestPlatform.TestManager.UnitTest\VietGeeks.TestPlatform.TestManager.UnitTest.csproj
```

If the slice adds another test project (e.g. `AccountManager.UnitTest`, `TestRunner.UnitTest`, `BackOffice.UnitTest`), run those too — check `git diff --name-only origin/master...HEAD` for new `*.UnitTest.csproj` files.

Report: passed / failed / skipped per project.

## 3. Frontend — only if `frontend/` was touched

```powershell
cd frontend
npx nx affected:build --base=origin/master
npx nx affected:test  --base=origin/master
npx nx affected:lint  --base=origin/master
```

Report: which projects were affected and the result per project.

## 4. Convention check

Compare the diff against the convention list in `CLAUDE.md` §Conventions. Specifically flag:

- new `.csproj` not registered in `VietGeeksTestPlatform.sln`
- new `PackageReference` whose version is **not** in `backend/Directory.Packages.props`
- new public service without a `RegisterXxxModule` registration
- new `appsettings.json` setting that should be a `<KeyVault>` placeholder but is not
- new MassTransit message added without a consumer
- new Akita store added outside an `_state/` folder

## 5. Final summary

Output a single block like:

```
Slice verification — <slice number / branch>
  Backend build .................. PASS / FAIL
  Backend tests .................. PASS / FAIL  (n passed, n failed, n skipped)
  Frontend affected build/test ... PASS / FAIL / N/A
  Convention check ............... PASS / FAIL  (list violations)
  Ready to open PR? .............. yes / no
```

Do not run `git push`, `git commit`, or any deploy/publish commands. This is verification only.
