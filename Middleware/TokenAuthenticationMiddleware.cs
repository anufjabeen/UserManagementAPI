using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace UserManagementAPI.Middleware;

/// <summary>
/// Requires "Authorization: Bearer &lt;token&gt;" on every request except the public root endpoint.
/// The expected token comes from configuration (Auth:Token).
/// </summary>
public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _expectedToken;
    private readonly ILogger<TokenAuthenticationMiddleware> _logger;

    public TokenAuthenticationMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<TokenAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _expectedToken = configuration["Auth:Token"]
            ?? throw new InvalidOperationException("Auth:Token is not configured.");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // The root endpoint is a public health check.
        if (context.Request.Path == "/")
        {
            await _next(context);
            return;
        }

        var header = context.Request.Headers.Authorization.ToString();
        const string prefix = "Bearer ";

        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            await RejectAsync(context, "Missing or malformed Authorization header.");
            return;
        }

        var token = header[prefix.Length..].Trim();
        if (!TokensMatch(token, _expectedToken))
        {
            await RejectAsync(context, "Invalid token.");
            return;
        }

        await _next(context);
    }

    private static bool TokensMatch(string provided, string expected)
    {
        var a = Encoding.UTF8.GetBytes(provided);
        var b = Encoding.UTF8.GetBytes(expected);
        return CryptographicOperations.FixedTimeEquals(a, b);
    }

    private async Task RejectAsync(HttpContext context, string reason)
    {
        _logger.LogWarning("Unauthorized request to {Path}: {Reason}", context.Request.Path, reason);
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Unauthorized", detail = reason }));
    }
}
