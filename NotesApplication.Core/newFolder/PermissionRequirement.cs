using Microsoft.AspNetCore.Authorization;
using NotesApplication.Core.Enums;

namespace NotesApplication.Core.newFolder;

public class PermissionRequirement(Permission[] permissions)
    : IAuthorizationRequirement
{
    public Permission[] Permissions { get; set; } = permissions;
}