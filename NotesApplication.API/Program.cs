using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NotesApplication.API.Middlewares;
using NotesApplication.Business.Behavior;
using NotesApplication.Business.GetAllNotes;
using NotesApplication.Data;
using NotesApplication.Data.Identity;
using System.Reflection;
using System.Text;

namespace NotesApplication.API;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var assembly = typeof(GetAllNotesQuery).Assembly;
        //builder.Configuration.AddJsonFile("appsettings.json");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(swagger =>
        {
            //This is to generate the Default UI of Swagger Documentation
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "JWT Token Authentication API",
                Description = ".NET 8 Web API"
            });
            // To Enable authorization using Swagger (JWT)
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            });
            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
        });

        builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://localhost:7037",
                ValidAudience = "https://localhost:7037",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345!!@##$%%^qwertyuiopsuperSecretKey@345!!@##$%%^qwertyuiopsuperSecretKey@345!!@##$%%^qwertyuiop"))
            };
        });

        builder.Services.AddIdentityApiEndpoints<IdentityUser>(x =>
        x.Tokens.AuthenticatorTokenProvider = JwtBearerDefaults.AuthenticationScheme)
        .AddEntityFrameworkStores<DataContext>();  ///

        builder.Services.AddValidatorsFromAssembly(assembly);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        ConfigureServices(builder.Services, builder.Configuration);

        //builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        //{
        //    options.Password.RequireDigit = true;
        //    options.Password.RequiredLength = 8;
        //});

        //тут работаем с сервисами

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        var notesDbContextType = typeof(DbContext);
        //using IServiceScope scopedServiceProvider = app.Services.CreateScope();

        var dbContextTypes = Assembly.GetExecutingAssembly().GetTypes()
                           .Where(t => t != notesDbContextType && notesDbContextType.IsAssignableFrom(t));

        foreach (var derivedType in dbContextTypes)
        {
            using (DbContext context = (DbContext)app.Services.CreateScope().ServiceProvider.GetRequiredService(derivedType))
            {
                await context.Database.MigrateAsync();
            }
        }

        //var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<NotesDbContext>(); // TODO рефлексия. как зарегать несколько контекстов
        //var dataContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();

        //// dbContext.Database.EnsureDeleted();   //  удаление БД MigrateAsync  EnsureCreated
        //await dbContext.Database.MigrateAsync();
        //await dataContext.Database.MigrateAsync();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapIdentityApi<IdentityUser>(); ///

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseAuthentication();

        app.MapControllers();

        app.Run();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //Добавляем сервисы Entity Framework и указываем строку подключения к базе данных
        services.AddDbContext<NotesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NotesDatabase")));

        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NotesDatabaseInitial")));

        //services.AddIdentity<IdentityUser, IdentityRole>()
        //.AddEntityFrameworkStores<NotesDbContext>();
    }
}