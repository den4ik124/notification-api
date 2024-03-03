using Microsoft.EntityFrameworkCore;
using NotesApplication.Core;
using System.Reflection;

namespace NotesApplication.Data;

public class NotesDbContext : DbContext
{
    public NotesDbContext(DbContextOptions options) : base(options)
    {
    }

    public NotesDbContext()
    {
    }

    public virtual DbSet<Note> Notes { get; set; }

    //  Для использования функционала Fluent API переопределяется метод OnModelCreating():
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
}