using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace NotesApplication.Business.Behavior;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response = default;

        var requestName = request.GetType().Name;
        _logger.LogInformation("Handling {Name}", typeof(TRequest).Name);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            try
            {
                _logger.LogInformation(message: "[Properties] {requestName} {request}.", requestName, JsonSerializer.Serialize(request));
            }
            catch (NotSupportedException)
            {
                _logger.LogInformation(message: "[Serialization ERROR] {requestName} Could not serialize the request.", requestName);
            }
            response = await next();
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
            message: " Handled {requestName}; Execution time = {ElapsedMilliseconds}ms.", requestName, stopwatch.ElapsedMilliseconds);
        }
        return response;
    }
}