<!--
  This PR template is required for every refactor-pass change. See REFACTOR.md.
  Delete the "Refactor metadata" block only if this PR is unrelated to the refactor.
-->

## Refactor metadata

- **Slice:** <!-- e.g. "Slice 1 — remove PocProgram", from REFACTOR.md §4 -->
- **Why (links REFACTOR.md §1):**
- **Out of scope for this PR:**

## Summary

<!-- 1-3 bullets. What changed and why. The "why" is more useful than the "what" — the diff shows the what. -->

-

## CLAUDE.md conventions touched

<!-- Tick anything this PR added to / changed / relied on. Leave blank if N/A. -->

- [ ] Module registration (`RegisterXxxModule` / `ServiceCollectionExtensions`)
- [ ] `Directory.Packages.props` (central package version)
- [ ] MongoDB.Entities schema or registration
- [ ] EF migration (`BackOffice/Migrations/UserSessions`)
- [ ] MassTransit message contract / consumer
- [ ] Dapr actor / pub-sub component
- [ ] Auth scheme / policy
- [ ] Key Vault `<KeyVault>` placeholder in `appsettings.json`
- [ ] Frontend Akita store (`_state/*`)
- [ ] Controller / public API surface
- [ ] Other:

## Test plan

<!-- The /verify-slice command runs this sequence; paste the result. Replace TBD with the actual commands. -->

- [ ] `dotnet build backend\VietGeeksTestPlatform.sln` — green
- [ ] `dotnet test backend\VietGeeks.TestPlatform.TestManager.UnitTest` — green
- [ ] New/changed test project(s): TBD — green
- [ ] `npx nx affected:test` — green (if frontend touched)
- [ ] Manual smoke: TBD (link `/smoke-backoffice` output, hit endpoints, etc.)

## Risks & rollback

- **Risk:** <!-- e.g. "Mongo collection rename — old documents will not be readable until migration script runs" -->
- **Rollback:** <!-- revert this commit; any data cleanup needed? -->

## Reviewer notes

<!-- Anything the reviewer should look at first; areas where you want a second opinion; things deliberately left for a follow-up. -->

-

---

<sub>Tracking: REFACTOR.md · Guardrails: `.claude/settings.json`</sub>
