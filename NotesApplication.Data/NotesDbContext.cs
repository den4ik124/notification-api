using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using NotesApplication.Core;
using NotesApplication.Core.Configurations;
using NotesApplication.Core.Entities;
using NotesApplication.Core.newFolder;
using System.Reflection;

namespace NotesApplication.Data;

public class NotesDbContext : DbContext
{
    private IDbContextTransaction _currentTransaction;
    private readonly IOptions<AuthorizationOptions> _authOptions;

    public NotesDbContext(
        DbContextOptions<NotesDbContext> options,
        IOptions<AuthorizationOptions> authOptions) : base(options)
    {
        _authOptions = authOptions;
    }

    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Note> Notes { get; set; }
    public DbSet<UserEntity> Users { get; set; }

    public DbSet<RoleEntity> Roles { get; set; }

    //  Для использования функционала Fluent API переопределяется метод OnModelCreating():
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);

        //modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        //modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration(_authOptions.Value));
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