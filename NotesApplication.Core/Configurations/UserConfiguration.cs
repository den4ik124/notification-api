//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using NotesApplication.Core.Entities;

//namespace NotesApplication.Core.Configurations;

//public partial class UserConfiguration : IEntityTypeConfiguration<UserEntity>
//{
//    public void Configure(EntityTypeBuilder<UserEntity> builder)
//    {
//        builder.HasKey(x => x.Id);

//        builder.HasMany(x => x.Roles)
//            .WithMany(x => x.Users)
//            .UsingEntity<UserRoleEntity>(
//            l => l.HasOne<RoleEntity>().WithMany().HasForeignKey(r => r.RoleId),
//            r => r.HasOne<UserEntity>().WithMany().HasForeignKey(u => u.UserId));
//    }
//}