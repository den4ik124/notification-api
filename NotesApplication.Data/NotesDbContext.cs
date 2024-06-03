using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NotesApplication.Core;
using System.Reflection;

namespace NotesApplication.Data;

public class NotesDbContext : DbContext
{
    private IDbContextTransaction _currentTransaction;

    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {
    }

    public NotesDbContext()
    {
    }

    public virtual DbSet<Note> Notes { get; set; }

    //  Для использования функционала Fluent API переопределяется метод OnModelCreating():
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);

        //modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        //modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            _currentTransaction?.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken);
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> func)
    {
        return await Database.CreateExecutionStrategy().ExecuteAsync(func);
    }
}