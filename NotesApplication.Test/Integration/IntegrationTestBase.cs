using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotesApplication.Data;

namespace NotesApplication.Test.Integration;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _webAppFactory;

    public HttpClient Client { get; private set; }

    private bool _disposedValue;

    public IntegrationTestBase(CustomWebApplicationFactory webAppFactory)
    {
        _webAppFactory = webAppFactory;
        Client = webAppFactory.CreateClient();
    }

    public NotesDbContext GetNotesDbContext()
        => _webAppFactory.Services.CreateScope().ServiceProvider.GetRequiredService<NotesDbContext>();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~IntegrationTestBase()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        CleanDb();
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void CleanDb()
    {
        var context = GetNotesDbContext();
        var deleteCommand = "delete from Notes;";
        context.Database.ExecuteSqlRaw(deleteCommand);
    }
}