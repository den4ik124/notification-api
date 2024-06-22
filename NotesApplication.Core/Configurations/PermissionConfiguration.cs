//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using NotesApplication.Core.Entities;
//using NotesApplication.Core.Enums;

//namespace NotesApplication.Core.Configurations;

//public partial class PermissionConfiguration : IEntityTypeConfiguration<PermissionEntity>
//{
//    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
//    {
//        builder.HasKey(x => x.Id);

//        var permissions = Enum
//           .GetValues<Permission>()
//           .Select(p => new PermissionEntity
//           {
//               Id = (int)p,
//               Name = p.ToString()
//           });

//        builder.HasData(permissions);
//    }
//}