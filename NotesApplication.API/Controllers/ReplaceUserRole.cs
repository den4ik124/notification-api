using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NotesApplication.Core.Enums;
using NotesApplication.Core.Models;

namespace NotesApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReplaceUserRole : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ConnectionStrings _connectionStrings;

    public ReplaceUserRole(IOptions<ConnectionStrings> connectionStrings, UserManager<IdentityUser> userManager)
    {
        _connectionStrings = connectionStrings.Value;
        _userManager = userManager;
    }

    [Authorize(Policy = "AdminOrManagerPolicy")]
    [HttpPost("ReplaceRole")]
    public async Task<IActionResult> ReplaceUserRoles([FromBody] RoleUpdateRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!Enum.TryParse(typeof(Roles), model.NewRole, true, out var newRoleEnum))
        {
            return BadRequest(new { message = "Указана неверная роль" });
        }

        if (model.UserId == Guid.Empty)
        {
            return BadRequest(new { message = "Неверный GUID" });
        }

        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
        {
            return NotFound(new { message = "Пользователь не найден" });
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Contains(model.NewRole))
        {
            return BadRequest(new { message = "Пользователь уже имеет данную роль" });
        }

        string notesDbInitialConnection = _connectionStrings.NotesDatabaseInitial;

        using SqlConnection connection = new SqlConnection(notesDbInitialConnection);

        await connection.OpenAsync();
        using SqlTransaction transaction = connection.BeginTransaction();

        try
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.NewRole);

            await transaction.CommitAsync();
            return Ok(new { message = $"Роль пользователя успешно изменена на {model.NewRole}" });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { message = "Ошибка при изменении роли пользователя", details = ex.Message });
        }
    }

    [Authorize(Policy = "ManagerPolicy")]
    [HttpGet("GetAllUsersWithRoles")]
    public async Task<IActionResult> GetAllUsersWithRoles()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new List<UserWithRolesModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new UserWithRolesModel
            {
                UserId = Guid.Parse(user.Id),
                Email = user.Email,
                Roles = roles.ToList()
            });
        }

        return Ok(userRoles);
    }
}