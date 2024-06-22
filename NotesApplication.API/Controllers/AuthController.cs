using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesApplication.Business.Auth;
using NotesApplication.Core.Models;
using NotesApplication.Data;
using NotesApplication.Data.Identity;

namespace NotesApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly NotesDbContext _notesDbContext;
    private readonly IdentityContext _identityContext;

    public AuthController(UserManager<IdentityUser> userManager,
                         SignInManager<IdentityUser> signInManager,
                         TokenService tokenService,
                         NotesDbContext notesDbContext,
                         IdentityContext identityContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _notesDbContext = notesDbContext;
        _identityContext = identityContext;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized();
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        var token = await _tokenService.GenerateToken(user);

        Response.Cookies.Append("secretCookie", token, new CookieOptions
        {
            HttpOnly = true, // Защита от XSS
            Secure = true,   // Использовать только через HTTPS
            SameSite = SameSiteMode.Strict, // Защита от CSRF
        });

        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (model.Password != model.PasswordConfirm)
        {
            return BadRequest(new { message = "Пароли не совпадают" });
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Пользователь с такой почтой уже существует" });
        }

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        // добавляем пользователя
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");

            //var newUser = new User(user.Id, user.Email);
            //_notesDbContext.Users.Add(newUser);

            await _notesDbContext.SaveChangesAsync();
            // установка куки
            //var token = await _tokenService.GenerateToken(user);

            //Response.Cookies.Append("secretCookie", token, new CookieOptions
            //{
            //    HttpOnly = true, // Защита от XSS
            //    Secure = true,   // Использовать только через HTTPS
            //    SameSite = SameSiteMode.Strict, // Защита от CSRF
            //});

            return Ok();// (new { token });
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }
}