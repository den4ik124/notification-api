namespace NotesApplication.API.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Логирование запроса
        Console.WriteLine("Request:");
        Console.WriteLine($"{context.Request.Method} {context.Request.Path}");
        Console.WriteLine($"Headers: {context.Request.Headers}");

        // Вызов следующего middleware в конвейере
        await _next(context);
    }
}