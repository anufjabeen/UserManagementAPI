# User Management API

A small ASP.NET Core (.NET 8) minimal API for managing users, built for the Coursera
*Back-End Development with .NET* final project. Built and debugged with GitHub Copilot.

## Features

- **CRUD endpoints** for users (`GET`, `POST`, `PUT`, `DELETE`)
- **Validation**: required fields, name/department length, email format, age range 18-120, duplicate email check (409)
- **Middleware**
  - `ErrorHandlingMiddleware`: consistent JSON errors, no stack traces leaked
  - `RequestLoggingMiddleware`: logs method, path, status code and duration
  - `TokenAuthenticationMiddleware`: Bearer token required on all `/users` routes (401 otherwise)
- Layered architecture: Controller -> Service -> Repository, with DTOs separate from the entity
- In-memory, thread-safe storage (seeded with two users). **No database, migrations or SQL scripts are needed.**

## Run it

```bash
dotnet run
```

The API listens on `http://localhost:5000`. The token is set in `appsettings.json` (`Auth:Token`,
default `secret-token-123`). Change it before using this for anything real.

## Endpoints

| Method | Route         | Description              | Success | Errors        |
|--------|---------------|--------------------------|---------|---------------|
| GET    | `/`           | Public health check      | 200     |               |
| GET    | `/users`      | List all users           | 200     | 401           |
| GET    | `/users/{id}` | Get one user             | 200     | 401, 404      |
| POST   | `/users`      | Create a user            | 201     | 400, 401, 409 |
| PUT    | `/users/{id}` | Update a user            | 200     | 400, 401, 404, 409 |
| DELETE | `/users/{id}` | Delete a user            | 204     | 401, 404      |

Request body for POST/PUT:

```json
{ "name": "Carol White", "email": "carol@example.com", "age": 28, "department": "Finance" }
```

Example:

```bash
curl -H "Authorization: Bearer secret-token-123" http://localhost:5000/users
```

Ready-made requests (valid, invalid, 401, 404, 409) are in `UserManagementAPI.http`
(VS Code REST Client extension or Visual Studio).

## Project structure

```
Program.cs                       DI registration, middleware pipeline, health endpoint
Controllers/UsersController.cs   HTTP layer: routes, status codes (validation via [ApiController])
Services/                        IUserService + UserService: business rules, entity <-> DTO mapping
Repositories/                    IUserRepository + InMemoryUserRepository: data access
Models/User.cs                   Entity
DTOs/                            CreateUserRequest, UpdateUserRequest (validation attributes), UserResponse
Common/ServiceResult.cs          Result type the service returns to the controller
Middleware/                      Error handling, logging, authentication
```

Request flow: `Middleware -> Controller -> Service -> Repository`.

## Using SQL Server later (optional)

The repository sits behind `IUserRepository`, so you could add an EF Core implementation and register it in
`Program.cs` instead of `InMemoryUserRepository`. Only then would you need migrations
(`dotnet ef migrations add Init` and `dotnet ef database update`). This is not required for the project.

See `DEBUGGING.md` for how Copilot was used to debug the code.
