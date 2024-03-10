using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotesApplication.API;
using NotesApplication.Data;

namespace NotesApplication.Test.Integration;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    protected readonly NotesDbContext Context;

    public IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        Context = factory.Services.CreateScope().ServiceProvider.GetRequiredService<NotesDbContext>();
    }

    public void Setup()
    {
        Context.Database.EnsureDeleted();   //  удаление БД
        Context.Database.EnsureCreated();
    }
}