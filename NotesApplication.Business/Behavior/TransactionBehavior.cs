using MediatR;
using Microsoft.Extensions.Logging;
using NotesApplication.Data;

namespace NotesApplication.Business.Behavior;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ITransactional
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly NotesDbContext _context;

    public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger, NotesDbContext dbContext)
    {
        _logger = logger;
        _context = dbContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await _context.RetryOnExceptionAsync(async () =>
            {
                _logger.LogInformation(message: "Begin transaction: {RequestName}.", request);
                await _context.BeginTransactionAsync(cancellationToken);
                var response = await next();
                await _context.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation(message: "End transaction: {RequestName}.", request);
                return response;
            });
        }
        catch (Exception e)
        {
            _logger.LogInformation(message: "Rollback transaction executed {RequestName}.", request);
            await _context.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(e.Message, e.StackTrace);
            throw;
        }
    }
}