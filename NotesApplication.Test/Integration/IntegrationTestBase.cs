using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotesApplication.Core.CreateNote;
using NotesApplication.Core.GetAllNotes;
using NotesApplication.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace NotesApplication.Test.Integration;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _webAppFactory;

    public HttpClient Client { get; }

    private bool _disposedValue;

    public IntegrationTestBase(CustomWebApplicationFactory webAppFactory)
    {
        _webAppFactory = webAppFactory;
        Client = webAppFactory.CreateClient();
        Client.BaseAddress = new Uri("http://localhost:5000/");
    }

    public NotesDbContext GetNotesDbContext()
        => _webAppFactory.Services.CreateScope().ServiceProvider.GetRequiredService<NotesDbContext>();

    public async Task<TResult?> ConvertTo<TResult>(HttpResponseMessage httpMessage)
    {
        var test = await httpMessage.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<TResult>(test); //TODO бажина с Json
        return res;
    }

    public async Task<HttpResponseMessage>? SendGetRequest(string url)
    {
        return await Client.GetAsync(url);
    }

    public async Task<HttpResponseMessage>? SendPostRequest<TBody>(string url, TBody request)
    {
        using StringContent jsonContent = new(
        JsonSerializer.Serialize(request),
        Encoding.UTF8,
        "application/json");

        return await Client.PostAsync(url, jsonContent);
    }

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