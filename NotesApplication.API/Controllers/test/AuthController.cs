using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesApplication.API.Models;

namespace NotesApplication.API.Controllers.test
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
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

            var token = _tokenService.GenerateToken(user);

            Response.Cookies.Append("secretCookie", token, new CookieOptions
            {
                HttpOnly = true, // Защита от XSS
                Secure = true,   // Использовать только через HTTPS
                SameSite = SameSiteMode.Strict, // Защита от CSRF
            });

            return Ok(new { token });
        }
    }

    //public class LoginModel
    //{
    //    public string Email { get; set; }
    //    public string Password { get; set; }
    //}
}