using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NotesApplication.Data.Identity;

public class IdentityContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }
}