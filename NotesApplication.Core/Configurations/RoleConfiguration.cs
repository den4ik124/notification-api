//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using NotesApplication.Core.Entities;
//using NotesApplication.Core.Enums;

//namespace NotesApplication.Core.Configurations;

//public partial class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
//{
//    public void Configure(EntityTypeBuilder<RoleEntity> builder)
//    {
//        builder.HasKey(x => x.Id);

//        builder.HasMany(x => x.Permissions)
//            .WithMany(p => p.Roles)
//            .UsingEntity<RolePermissionEntity>(
//                l => l.HasOne<PermissionEntity>().WithMany().HasForeignKey(e => e.PermissionId),
//                r => r.HasOne<RoleEntity>().WithMany().HasForeignKey(e => e.RoleId));

//        var roles = Enum
//           .GetValues<Roles>()
//           .Select(r => new RoleEntity
//           {
//               Id = (int)r,
//               Name = r.ToString()
//           });

//        builder.HasData(roles);
//    }
//}