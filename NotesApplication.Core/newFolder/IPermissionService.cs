using NotesApplication.Core.Enums;

namespace NotesApplication.Core.newFolder;

public interface IPermissionService
{
    Task<HashSet<Permission>> GetPermissionsAsync(Guid userId);
}