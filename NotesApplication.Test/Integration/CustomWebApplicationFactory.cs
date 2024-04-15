using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotesApplication.API;
using NotesApplication.Data;

namespace NotesApplication.Test.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IClassFixture<CustomWebApplicationFactory>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<NotesDbContext>));

            services.Remove(dbContextDescriptor);
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                 path: "appsettings.Testing.json",
                 optional: false,
                 reloadOnChange: true)
           .Build();
            services.AddSingleton<IConfiguration>(configuration);

            builder.UseConfiguration(configuration);

            services.AddDbContext<NotesDbContext>(container =>
            {
                var connectionString = configuration.GetConnectionString("NotesTestDatabase");
                container.UseSqlServer(connectionString);
                //container.UseInMemoryDatabase("NotesTestDatabase");
            });
        });

        builder.UseEnvironment("Testing");
    }

    public override ValueTask DisposeAsync()
    {
        var context = Services.GetRequiredService<NotesDbContext>();
        context.Database.EnsureDeleted();
        return base.DisposeAsync();
    }
}