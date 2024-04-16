using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotesApplication.API.Middlewares;
using NotesApplication.Business.Behavior;
using NotesApplication.Business.GetAllNotes;
using NotesApplication.Data;

namespace NotesApplication.API;

public partial class Program
{
    public static async Task Main(string[] args)
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
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        ConfigureServices(builder.Services, builder.Configuration);

        //тут работаем с сервисами

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        var app = builder.Build();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<NotesDbContext>();

        // dbContext.Database.EnsureDeleted();   //  удаление БД MigrateAsync  EnsureCreated
        await dbContext.Database.MigrateAsync();
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