# Debugging with GitHub Copilot

This file documents the bugs found in the first draft of the API and how Copilot Chat helped fix them.
**Replace the example prompts below with your own real Copilot sessions** so this reflects what you actually did.

## Bug 1: Middleware order let unauthenticated requests through / hid 401s from logs
- **Symptom:** requests with no token were not logged, and an exception thrown inside an endpoint returned an HTML error page.
- **Copilot prompt:** "Why aren't 401 responses showing up in my logs, and how should I order error handling, logging and authentication middleware in ASP.NET Core?"
- **Fix:** Order is now error handling -> logging -> authentication in `Program.cs`.

## Bug 2: Client could choose the user Id / invalid users were saved
- **Symptom:** POST accepted empty names, bad emails and ages like 12.
- **Copilot prompt:** "How do I validate a minimal API request body with data annotations?"
- **Fix:** Added `CreateUserRequest`/`UpdateUserRequest` DTOs with data annotations; `[ApiController]` on `UsersController` returns HTTP 400 automatically.

## Bug 3: Duplicate emails and PUT on a missing user
- **Symptom:** two users could share an email; PUT on an unknown id threw a `NullReferenceException` (500).
- **Copilot prompt:** "PUT /users/999 throws NullReferenceException, how do I handle a missing user?"
- **Fix:** `UserService.Update` checks for a missing user (404) and a duplicate email via `EmailExists(email, excludeId)` (409), returning a `ServiceResult` the controller maps to a status code.

## Bug 4: Concurrency
- **Symptom:** parallel POSTs could produce duplicate ids.
- **Copilot prompt:** "Is Dictionary<int, User> safe for concurrent requests in a singleton service?"
- **Fix:** Wrapped repository operations in a lock.
