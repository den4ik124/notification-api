﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotesApplication.Core.Enums;
using NotesApplication.Data;
using NotesApplication.Data.Identity;

namespace NotesApplication.API;

public static class Extensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //Добавляем сервисы Entity Framework и указываем строку подключения к базе данных
        services.AddDbContext<NotesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NotesDatabase")));

        services.AddDbContext<IdentityContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NotesDatabaseInitial")));
        return services;

        //services.AddIdentity<IdentityUser, IdentityRole>()
        //.AddEntityFrameworkStores<NotesDbContext>();
    }

    public static async Task Migrate(this WebApplication app)
    {
        var contextTypes = typeof(NotesDbContext).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(DbContext)));

        foreach (var derivedType in contextTypes)
        {
            using DbContext context = (DbContext)app.Services.CreateScope().ServiceProvider.GetRequiredService(derivedType);

            //await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
        }
    }

    public static async Task Seed(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();

        var services = serviceScope.ServiceProvider;

        var roles = Enum
          .GetValues<Roles>();
        try
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role.ToString()));
            }
            // Проверяем, есть ли пользователь с таким имейлом уже в базе
            var existingUser = await userManager.FindByEmailAsync("string");

            if (existingUser == null)
            {
                // Если пользователя нет, создаем его
                var newUser = new IdentityUser
                {
                    UserName = "test@example",
                    Email = "string"
                };

                var result = await userManager.CreateAsync(newUser, "String!123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, nameof(Roles.Manager));
                    Console.WriteLine("Test user created successfully.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }
            else
            {
                Console.WriteLine("Test user already exists.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while adding the test user: " + ex.Message);
        }
    }

    //public static IEndpointConventionBuilder RequirePermissions<TBuilder>(
    //   this TBuilder builder, params Permission[] permissions)
    //       where TBuilder : IEndpointConventionBuilder
    //{
    //    return builder
    //        .RequireAuthorization(pb =>
    //            pb.AddRequirements(new PermissionRequirement(permissions)));
    //}
}