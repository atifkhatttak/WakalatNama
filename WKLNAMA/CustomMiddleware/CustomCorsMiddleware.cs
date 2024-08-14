using System.Net;

public class CustomCorsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HashSet<string> _allowedOrigins;

    public CustomCorsMiddleware(RequestDelegate next, IEnumerable<string> allowedOrigins)
    {
        _next = next;
        _allowedOrigins = new HashSet<string>(allowedOrigins);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var origin = context.Request.Headers["Origin"].ToString();

        if (_allowedOrigins.Contains(origin) || _allowedOrigins.Contains("*"))
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With, Authorization");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");

            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }
        }

        await _next(context);
    }
}
