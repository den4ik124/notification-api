//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using NotesApplication.Core.Entities;
//using NotesApplication.Core.Enums;
//using NotesApplication.Core.newFolder;

//namespace NotesApplication.Core.Configurations;

//public partial class RolePermissionConfiguration
//    : IEntityTypeConfiguration<RolePermissionEntity>
//{
//    private readonly AuthorizationOptions _authorization;

//    public RolePermissionConfiguration(AuthorizationOptions authorization)
//    {
//        _authorization = authorization;
//    }

//    public void Configure(EntityTypeBuilder<RolePermissionEntity> builder)
//    {
//        builder.HasKey(r => new { r.RoleId, r.PermissionId });

//        builder.HasData(ParseRolePermissions());
//    }

//    private RolePermissionEntity[] ParseRolePermissions()
//    {
//        return _authorization.RolePermissions
//            .SelectMany(rp => rp.Permissions
//                .Select(p => new RolePermissionEntity
//                {
//                    RoleId = (int)Enum.Parse<Roles>(rp.Role),
//                    PermissionId = (int)Enum.Parse<Permission>(p)
//                }))
//                .ToArray();
//    }
//}