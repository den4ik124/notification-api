using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NotesApplication.Data.Identity;

internal class ApplicationDbContextFactory : IDesignTimeDbContextFactory<NotesDbContext>
{
    NotesDbContext IDesignTimeDbContextFactory<NotesDbContext>.CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<NotesDbContext>();
        var connectionString = configuration.GetConnectionString("ConnectionStrings");

        builder.UseSqlServer(connectionString);

        return new NotesDbContext(builder.Options);
    }
}