using UserManagementAPI.Middleware;
using UserManagementAPI.Repositories;
using UserManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>(); // singleton: it holds the in-memory data
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Middleware pipeline. Order matters:
// 1. Error handling first so it can catch exceptions from everything below.
// 2. Logging next so every request is logged, including rejected (401) ones.
// 3. Authentication last, right before the endpoints.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TokenAuthenticationMiddleware>();

// Public health check (skipped by the authentication middleware)
app.MapGet("/", () => Results.Ok(new { service = "User Management API", status = "running" }));

app.MapControllers();

app.Run();
