using Microsoft.EntityFrameworkCore;
using NotesApplication.Data;

namespace NotesApplication.API;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        ConfigureServices(builder.Services, builder.Configuration);

        //��� �������� � ���������

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        var app = builder.Build();

        var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<NotesDbContext>();

        // dbContext.Database.EnsureDeleted();   //  �������� ��
        dbContext.Database.EnsureCreated();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // ��������� ������� Entity Framework � ��������� ������ ����������� � ���� ������
        services.AddDbContext<NotesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NotesDatabase")));
    }
}

public partial class Program
{ }