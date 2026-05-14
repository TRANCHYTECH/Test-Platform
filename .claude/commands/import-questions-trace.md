---
description: Exercise the OpenAI question-import path end-to-end for slice 7. Verifies the controller, schema generation, Azure OpenAI call, and response shape.
argument-hint: [path-to-questions.txt] [testId]
---

You are tracing the OpenAI-backed question import flow. Endpoint under test:

```
POST https://localhost:7200/Management/TestDefinition/{testId}/Question:ReadFile
```

Implementation: `backend\VietGeeks.TestPlatform.TestManager\Api\Controllers\ImportQuestionsController.cs`

Behaviour to verify:

1. File size cap (1 MB) enforced before the OpenAI call.
2. Schema is generated from `QuestionResponse` and sent as `ChatResponseFormat.CreateJsonSchemaFormat`.
3. 15-second timeout on the OpenAI call (linked CTS).
4. Auth: endpoint is `[Authorize]` — unauthenticated request should fail.
5. `OpenAi:Deployment` resolves from config (default `gpt-4o`).

## Arguments

- `$1` — path to a sample questions text file. If not provided, create a small fixture at `backend\..\tmp\sample-questions.txt` with the format described in `ImportQuestionsController.cs` lines 22-30 (numbered questions, lettered answers, `[a]` marking correct answers). Pick 3 questions.
- `$2` — a `testId`. If the user has not provided one, ask before proceeding. Do not invent one — the controller routes on it and downstream consumers may persist against it.

## Prerequisites

Run `/smoke-backoffice` first or confirm BackOffice is already running on `https://localhost:7200`. If not, stop and tell the user to run smoke first.

Confirm the user has:

- `OpenAi:Endpoint` resolvable from Key Vault (boot would have failed otherwise — sanity check)
- A bearer token for the BFF or a cookie session. If neither, stop and ask the user to authenticate via the SPA (`/test-portal`) and either paste the cookie or set a local dev-only bypass; do NOT add an `[AllowAnonymous]` bypass to source.

## Steps

### 1. Happy path

```powershell
curl.exe -k -i `
  -H "Authorization: Bearer <token>" `
  -F "file=@<path-to-questions.txt>" `
  "https://localhost:7200/Management/TestDefinition/<testId>/Question:ReadFile"
```

Expected: `200 OK` with a JSON body matching `QuestionResponse.Questions` shape. Report:

- HTTP status
- response time (curl `-w "%{time_total}"`)
- number of questions parsed
- whether marked-correct answers (`[a]`) round-trip into the response

### 2. Oversize file

Generate a 2 MB text file, POST it. Expected: `400 Bad Request`, no OpenAI call (verify in BackOffice logs that the call did not happen).

### 3. Unauthenticated

Same as step 1 with no `Authorization` header. Expected: `401`.

### 4. Cancellation / timeout

Optional: temporarily lower the deployment to a model with high latency, or use the existing 15s linked CTS. Report whether the controller surfaces a clean `400` (the `catch` block) instead of a 500.

### 5. Logs to capture

While running steps 1-4, capture from the BackOffice console:

- any `ImportQuestionsController` log entries (none today — note this as a gap for slice 7)
- Azure OpenAI client errors / retries
- request duration

## Report format

```
ImportQuestions trace
  Happy path ............ 200 (n questions, Xms)
  Oversize file ......... 400 (no OpenAI call: true/false)
  Unauthenticated ....... 401
  Timeout behaviour ..... clean 400 / 500 / not tested
  Observability gaps:
    - ...
  Slice 7 followups:
    - max file size + token cap (currently TODO in source)
    - structured logging around the OpenAI call
    - cost / token-usage metric
    - retry policy
```

Do not commit any tmp fixture files. Do not check in tokens. Do not modify `appsettings.json` or `Directory.Packages.props` during this command.
