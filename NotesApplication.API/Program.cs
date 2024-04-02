using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotesApplication.Business.GetAllNotes;
using NotesApplication.Business.Validation;
using NotesApplication.Data;

namespace NotesApplication.API;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var assembly = typeof(GetAllNotesQuery).Assembly;
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddValidatorsFromAssembly(assembly);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        ConfigureServices(builder.Services, builder.Configuration);

        //тут работаем с сервисами

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        var app = builder.Build();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<NotesDbContext>();

        // dbContext.Database.EnsureDeleted();   //  удаление БД
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
        // Добавляем сервисы Entity Framework и указываем строку подключения к базе данных
        services.AddDbContext<NotesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NotesDatabase")));
    }
}

public partial class Program
{ }